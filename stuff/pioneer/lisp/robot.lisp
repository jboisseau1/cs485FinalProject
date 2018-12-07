;;;Jason P. Thomas
;;;CS685
;;;Prof. Luke
;;;08 December 2007
;;;functions remaining to be translated from Robot.java:
;;;readBytes( byte[] buf, int offset, int size )
;;;lookFor(byte[] expected, long timeout)
;;;readBytesWithTimeout(int numBytes, long timeout)
;;;doSync0(byte[] buf)
;;;doSync1(byte[] buf)
;;;doSync2(byte[] buf)
;;;readString()
;;;connect(InputStream inputStream, OutputStream outputStream)
;;;displayInfo()
;;;readPacket()
;;;finalize()
;;;disconnect()
;;;The socket code still needs some work and testing to get things
;;;squared away.
(require 'sb-bsd-sockets)
(defpackage :pioneer-robot (:use "CL" "SB-THREAD" "SB-BSD-SOCKETS"))

(in-package :pioneer-robot)





;;;conversion variables, possibly unnecessary
(defconstant +ypos_conversion+ 0.5083)
(defconstant +xpos_conversion+ 0.5083)
(defconstant +thpos_conversion+ 0.001534)
(defconstant +lvel_conversion+ 0.6154)
(defconstant +rvel_conversion+ 0.6154)
(defconstant +control_conversion+ 0.001534)
(defconstant +sonar_conversion+ 0.555)
(defconstant +packet-header+ #(#xfa #xfb))

(defmacro synced-defun (name args mutex &body body)
  "Defines a function that uses the specified mutex for mutual
   exclusion purposes.  Syncronizes entire function body, so
   it is intended for use with the get and set functions for
   the robot."
  `(defun ,name ,args 
    (with-mutex (,mutex)
      ,@body)))

(let ((verbose t) ;Controls the amount of feedback from the robot.
      (verbose-lock (make-mutex :name "verbosity lock"))
      (read-lock (make-mutex :name "data read lock"))
      (checksum-lock (make-mutex :name "checksum lock"))
      (address #(127 0 0 1)) ;Socket connect address.
      (port 5000) ;Socket connect port
      (valid-data nil) ;True after the first sensor packet is received
                       ;Values returned by sensor related functions 
                       ;should be ignored until valid data returns true.
      (motor-engaged nil) ;Motor status.
      (xpos 0) ;Robot's believed x position in mm of its center.
      (ypos 0) ;Robot's believed y position in mm of its center.
      (orientation 0) ;Robot's believed theta position in radians.
      (left-wheel-velocity 0) ;Left wheel velocity in mm/sec
      (right-wheel-velocity 0) ;Right wheel velocity in mm/sec
      (angular-position-servo 0) ;Not used on our robots
      (left-wheel-stall nil) ;Left wheel stall indicator
      (right-wheel-stall nil) ;Right wheel stall indicator
      (battery 0) ;Battery charge in tenths of volts
      (sonars (make-array 32)) ;Stores all sonar values.  Sonar 0 is the far
			       ;right sonar in the front as you look at the 
			       ;robot.  Sonars are then numbered clockwise.
      (last-sonar-times (make-array 32)) ;Stores the last time tick that 
                                         ;information for sonar #index arrived.
                                         ;Note that sonars come in different 
                                         ;batches, so you may not receive all of
                                         ;them in one time tick.  Each time any
                                         ;sonar batch arrives, the time tick is
                                         ;increased; this can thus give you an 
                                         ;idea whether your sonar data is really
                                         ;old (if it's older than 8, you've got 
                                         ;old data -- older than 32 say, you've 
                                         ;got REALLY old data).
      (current-sonar-time 0)) ;Stores current sonar time tick

  (synced-defun set-verbose (arg) verbose-lock
    (setf verbose arg))

  (synced-defun verbose-p () verbose-lock
    (if (null verbose)
	nil
	t))

  (synced-defun valid-data-p () read-lock
    (if (null valid-data)
	nil
	t))

  (synced-defun motor-engaged-p () read-lock
    motor-engaged)

  (synced-defun get-xpos () read-lock
    xpos)

  (synced-defun get-ypos () read-lock
    ypos)

  (synced-defun get-orientation () read-lock
    orientation)

  (synced-defun get-left-wheel-velocity () read-lock
    left-wheel-vel)

  (synced-defun get-right-wheel-velocity () read-lock
    right-wheel-vel)

  (synced-defun get-battery-status () read-lock
    battery)

  (synced-defun left-wheel-stalled-p () read-lock
    (if (null left-wheel-stall)
	nil
	t))

  (synced-defun right-wheel-stalled-p () read-lock
    (if (null right-wheel-stall)
	nil
	t))

  (synced-defun get-control-servo () read-lock
    angular-position-servo)

  (synced-defun get-sonar (index) read-lock
    (svref sonars index))

  (synced-defun get-last-sonar-time (index) read-lock
    (svref last-sonar-times index))

  (synced-defun get-current-sonar-time () read-lock
    current-sonar-time)

  (synced-defun get-sonars () read-lock
    (copy-seq sonars));prevent write to vector by returning copy

  (synced-defun get-last-sonar-times () read-lock
    (copy-seq last-sonar-times));prevent write to vector by returning copy
  
  (defun checksum (data)
    "Generates checksum for the vector of bytes passed in."
    (let (data1
	  data2
	  (c 0))
      (do ((x 0 (+ x 2)))
	  ((>= x (length data)))
	(setf data1 (svref data x))
	(when (< data1 0) (incf data1 256))
	(setf data2 (svref data (+ x 1)))
	(when (< 0 data1 0) (incf data2 256))
	(print data1)
	(print data2)
	(incf c (boole boole-ior (ash data1 8) data2))
	(setf c (boole boole-and c #xffff))
	(print c))
      (when (= (boole boole-and (length data) #x1) #x1)
	(setf data1 (svref data (- (length data) 1)))
	(when (< data1 0)
	  (incf data1 256))
	(setf c (expt c data1)))
      c))

  (defun submit (data)
    (let ((check-sum (checksum data))
	  (packet (make-array (+ 5 (length data)) :element-type '(unsigned-byte 8)
			      :initial-element 0)))
      (setf (aref packet 0) (svref +packet-header+ 0))
      (setf (aref packet 1) (svref +packet-header+ 1))
      (setf (aref packet 2) (+ 2 (length data)))
      (dotimes (x (length data))
	(setf (aref packet (+ 3 x)) (aref data x)))
      (setf (aref packet (- (length packet) 2)) (ash check-sum -8))
      (setf (aref packet (1- (length packet))) (logand check-sum #x00ff))
      (socket-send socket packet nil)))

  (defun print-packet (header data stream)
    (let ((first-packet nil))
	  (unless (null header)
	    (dotimes (x (length header))
	      (if (null first-packet)
		  (setf first-packet t)
		  (format stream " "))
	      (format stream "~A" (mod (+ (svref header x) 256) 256))))
	  (unless (null data)
	    (dotimes (x (length data))
	      (if (null first-packet)
		  (setf first-packet t)
		  (format stream " "))
	      (format stream "~A" (mod (+ (aref data x) 256) 256))))
	  (format stream "~%")))

;  (defun read-bytes (buf offset size)
;    (let ((ptr 0)
  (defun sync0 ()
    (submit (make-array 1 :element-type '(unsigned-byte 8)
			:initial-element 0)))

  (defun sync1 ()
    (submit (make-array 1 :element-type '(unsigned-byte 8)
			:initial-element 1)))

  (defun sync2 ()
    (submit (make-array 1 :element-type '(unsigned-byte 8)
			:initial-element 2)))

  (defun pulse ()
    (submit (make-array 1 :element-type '(unsigned-byte 8)
			:initial-element 0)))

  (defun open-bot ()
    (submit (make-array 1 :element-type '(unsigned-byte 8)
			:initial-element 1)))

  (defun close-bot ()
    (submit (make-array 1 :element-type '(unsigned-byte 8)
			:initial-element 2)))

  (defun polling (poll-sequence)
    (let* ((dele (string-to-octets poll-sequence)) 
	  (temp (make-array (+ 3 (length dele)) :element-type '(unsigned-byte 8)
			    :initial-element 0)))
      (setf (aref temp 0) 3);command number
      (setf (aref temp 1) #x2b);parameter is a string
      (setf (aref temp 2) (length dele))
      (dotimes (x (length dele))
	(setf (aref temp (+ x 3)) (aref dele x))))
    (submit temp))

  (defun enable (motor-status)
    (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 0)))
      (setf (aref packet 0) 4)
      (setf (aref packet 1) #x3b)
    (unless (null motor-status)
	(setf (aref packet 2) 1))
    (submit packet)))

  (defun seta (max-trans-accel)
    (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 5)));command number
      (cond ((>= max-trans-accel 0)
	     (setf (aref packet 1) #x3b)
	     (setf (aref packet 2) (low-byte max-trans-accel))
	     (setf (aref packet 3) (hight-byte max-trans-accel)))
	    (t
	     (setf (aref packet 1) #x1b)
	     (setf (aref packet 2) (low-byte (* -1 max-trans-accel)))
	     (setf (aref packet 3) (hight-byte (* -1 max-trans-accel)))))
      (submit packet)))

  (defun setv (max-trans-vel)
    (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 6)));command number
      (setf (aref packet 1) #x3b)
      (setf (aref packet 2) (low-byte max-trans-vel))
      (setf (aref packet 3) (hight-byte max-trans-vel))
      (submit packet)))

  (defun seto ()
    (submit (make-array 1 :element-type '(unsigned-byte 8)
			:initial-element 7)));command number

  (defun setrv (max-rot-vel)
    (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 10)));command number
      (setf (aref packet 1) #x3b)
      (setf (aref packet 2) (low-byte max-rot-vel))
      (setf (aref packet 3) (hight-byte max-rot-vel))
      (submit packet)))

  (defun vel (velocity)
    (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 11)));command number
      (setf (aref packet 1) #x3b)
      (setf (aref packet 2) (low-byte velocity))
      (setf (aref packet 3) (hight-byte velocity))
      (submit packet)))

  (defun head (abs-heading)
    (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 12)));command number
      (setf (aref packet 1) #x3b)
      (setf (aref packet 2) (low-byte abs-heading))
      (setf (aref packet 3) (hight-byte abs-heading))
      (submit packet)))

  (defun dhead (rel-heading)
    (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 13)));command number
      (setf (aref packet 1) #x3b)
      (setf (aref packet 2) (low-byte rel-heading))
      (setf (aref packet 3) (hight-byte rel-heading))
      (submit packet)))
  
  (defun say (tones)
    (let ((temp (make-array (+ 3 (length tones)) :element-type '(unsigned-byte 8)
			    :initial-element 15)));command number
      (setf (aref temp 1) #x2b);parameter is a string
      (setf (aref temp 2) (length tones))
      (dotimes (x (length tones))
	(setf (aref temp (+ x 3)) (aref tones x))))
    (submit temp))
  
  (defun rvel (spin-vel)
    (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 21)));command number
      (cond ((>= spin-vel 0)
	     (setf (aref packet 1) #x3b)
	     (setf (aref packet 2) (low-byte spin-vel))
	     (setf (aref packet 3) (hight-byte spin-vel)))
	    (t
	     (setf (aref packet 1) #x1b)
	     (setf (aref packet 2) (low-byte (* -1 spin-vel)))
	     (setf (aref packet 3) (hight-byte (* -1 spin-vel)))))
      (submit packet)))
  
  (defun setra (ra)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 23)));command number
      (setf (aref packet 1) #x3b)
      (setf (aref packet 2) (low-byte ra))
      (setf (aref packet 3) (hight-byte ra))
      (submit packet)))

    (defun sonar-p (on)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
				  :initial-element 0)))
	(setf (aref packet 0) 28)
	(setf (aref packet 1) #x3b)
	(unless (null index)
	  (setf (aref packet 2) 1))
	(submit packet)))
    
    (defun sonar (index)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 28)));command number
      (setf (aref packet 1) #x3b)
      (setf (aref packet 2) (mod index 256))
      (setf (aref packet 3) (/ index 256))
      (submit packet)))

    (defun stop ()
      (submit (make-array 1 :element-type '(unsigned-byte 8)
			  :initial-element 29)))

    (defun digout (dunno)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
			      :initial-element 30)));command number
      (setf (aref packet 1) #x3b)
      (setf (aref packet 2) (low-byte dunno))
      (setf (aref packet 3) (hight-byte dunno))
      (submit packet)))

    (defun vel2lr (left right)
      (let (result)
	(setf result (logior (ash (if (< left 0)
				      (+ left 256)
				      left)
				  8)
			     (if (< right 0)
				 (+ right 256)
				 right)))))
    (defun vel2 (arg)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
				:initial-element 32)));command number
	(setf (aref packet 1) #x3b)
	(setf (aref packet 2) (low-byte arg))
	(setf (aref packet 3) (hight-byte arg))
	(submit packet)))

    (defun bumpstall (arg)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
				:initial-element 44)));command number
	(setf (aref packet 1) #x3b)
	(setf (aref packet 2) (low-byte arg))
	(setf (aref packet 3) (hight-byte arg))
	(submit packet)))

    (defun e-stop ()
      (submit (make-array 1 :element-type '(unsigned-byte 8)
			  :initial-element 55)));command number

    (defun sound (arg)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
				:initial-element 90)));command number
	(setf (aref packet 1) #x3b)
	(setf (aref packet 2) (low-byte arg))
	(setf (aref packet 3) (hight-byte arg))
	(submit packet)))

    (defun playlist (arg)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
				:initial-element 32)));command number
	(setf (aref packet 1) #x3b)
	(setf (aref packet 2) (low-byte arg))
	(setf (aref packet 3) (hight-byte arg))
	(submit packet)))

    (defun soundtog (arg)
      (let ((packet (make-array 4 :element-type '(unsigned-byte 8)
				:initial-element 32)));command number
	(setf (aref packet 1) #x3b)
	(setf (aref packet 2) (low-byte arg))
	(setf (aref packet 3) (hight-byte arg))
	(submit packet)))



  (defun connect (&key addr prt)
    "Connects to the robot with the optionally specified address and port.
     If no address and/or port is supplied, default values of 127.0.0.1
     and 5000 are used."
    (unless (null addr)
      (setf address addr))
    (unless (null prt)
      (setf port prt))
    (tcp-connect))
  
  (defun tcp-connect ()
    "Connects to an inet address and port specified in the enclosing let"
    (setf socket (make-instance 'inet-socket :type :stream :protocol :tcp))
    (socket-connect socket address port))


;;;The following code was found as an example of sb-bsd-sockets.
;;;It was used to learn and test socket communication.
(defvar *port* 7000)

(defun make-echoer (stream id disconnector)
  (lambda (_)
    (declare (ignore _))
    (handler-case
        (let ((packet (make-array 10 :element-type '(unsigned-byte 8) :initial-element 0)))
	  (sb-sys:read-n-bytes stream packet 0 10 nil)
;	  (sb-sys:read-n-bytes stream packet 0 35)
	  (format t "~%~a~%" packet)
	  (print (type-of packet))	  
	  (force-output stream)))))

(defun make-disconnector (socket id)
  (lambda ()
    (let ((fd (socket-file-descriptor socket)))
      (format t "~a: closing~%" id)
      (sb-impl::invalidate-descriptor fd)
      (socket-close socket))))

(defun serve (socket id)
  (let ((stream (socket-make-stream socket :output t :input t :element-type '(unsigned-byte 8)))
        (fd (socket-file-descriptor socket)))
    (sb-impl::add-fd-handler fd
                             :input
                             (make-echoer stream
                                          id
                                          (make-disconnector socket id)))))

(defun echo-server (&optional (port *port*))
  (let ((socket (make-instance 'inet-socket :type :stream :protocol :tcp))
        (counter 0))
    (socket-bind socket #(127 0 0 1) port)
    (socket-listen socket 5)
    (sb-impl::add-fd-handler (socket-file-descriptor socket)
                             :input
                             (lambda (_)
                               (declare (ignore _))
                               (incf counter)
                               (format t "Accepted client ~A~%" counter)
                               (serve (socket-accept socket) counter)))))

;;;read thread
(sb-thread:make-thread (lambda ()
                         (echo-server)
                         (loop
                            (sb-impl::serve-all-events)))))
  
;;;End of borrowed code  








#|
Just some testing code while learning sockets.
(require 'sb-bsd-sockets)
(use-package 'sb-bsd-sockets)
(defvar socket (make-instance 'sb-bsd-sockets:inet-socket :type :stream 
			      :protocol :tcp))
(socket-connect socket #(127 0 0 1) 7000)
(defvar buf (make-array 10 :element-type '(unsigned-byte 8)
			:initial-element 1))
(dotimes (x 10)
  (setf (aref buf x) (1+ x)))
(setf (aref buf 0) #xfa)
(setf (aref buf 1) #xfb)
(setf (aref buf 2) 10)
(socket-send socket buf nil)
(socket-close socket)
|#


#|
Academic Free License ("AFL") v. 3.0

This Academic Free License (the "License") applies to any original work
of authorship (the "Original Work") whose owner (the "Licensor") has
placed the following licensing notice adjacent to the copyright notice
for the Original Work:

Licensed under the Academic Free License version 3.0

1) Grant of Copyright License. Licensor grants You a worldwide,
royalty-free, non-exclusive, sublicensable license, for the duration of
the copyright, to do the following:

        a) to reproduce the Original Work in copies, either alone or as
           part of a collective work;

        b) to translate, adapt, alter, transform, modify, or arrange the
           Original Work, thereby creating derivative works ("Derivative
           Works") based upon the Original Work;

        c) to distribute or communicate copies of the Original Work and
           Derivative Works to the public, UNDER ANY LICENSE OF YOUR
           CHOICE THAT DOES NOT CONTRADICT THE TERMS AND CONDITIONS,
           INCLUDING LICENSOR'S RESERVED RIGHTS AND REMEDIES, IN THIS
           ACADEMIC FREE LICENSE;

        d) to perform the Original Work publicly; and

        e) to display the Original Work publicly.

2) Grant of Patent License. Licensor grants You a worldwide,
royalty-free, non-exclusive, sublicensable license, under patent claims
owned or controlled by the Licensor that are embodied in the Original
Work as furnished by the Licensor, for the duration of the patents, to
make, use, sell, offer for sale, have made, and import the Original Work
and Derivative Works.

3) Grant of Source Code License. The term "Source Code" means the
preferred form of the Original Work for making modifications to it and
all available documentation describing how to modify the Original Work.
Licensor agrees to provide a machine-readable copy of the Source Code of
the Original Work along with each copy of the Original Work that
Licensor distributes. Licensor reserves the right to satisfy this
obligation by placing a machine-readable copy of the Source Code in an
information repository reasonably calculated to permit inexpensive and
convenient access by You for as long as Licensor continues to distribute
the Original Work.

4) Exclusions From License Grant. Neither the names of Licensor, nor the
names of any contributors to the Original Work, nor any of their
trademarks or service marks, may be used to endorse or promote products
derived from this Original Work without express prior permission of the
Licensor. Except as expressly stated herein, nothing in this License
grants any license to Licensor's trademarks, copyrights, patents, trade
secrets or any other intellectual property. No patent license is granted
to make, use, sell, offer for sale, have made, or import embodiments of
any patent claims other than the licensed claims defined in Section 2.
No license is granted to the trademarks of Licensor even if such marks
are included in the Original Work. Nothing in this License shall be
interpreted to prohibit Licensor from licensing under terms different
from this License any Original Work that Licensor otherwise would have a
right to license.

5) External Deployment. The term "External Deployment" means the use,
distribution, or communication of the Original Work or Derivative Works
in any way such that the Original Work or Derivative Works may be used
by anyone other than You, whether those works are distributed or
communicated to those persons or made available as an application
intended for use over a network. As an express condition for the grants
of license hereunder, You must treat any External Deployment by You of
the Original Work or a Derivative Work as a distribution under section
1(c).

6) Attribution Rights. You must retain, in the Source Code of any
Derivative Works that You create, all copyright, patent, or trademark
notices from the Source Code of the Original Work, as well as any
notices of licensing and any descriptive text identified therein as an
"Attribution Notice." You must cause the Source Code for any Derivative
Works that You create to carry a prominent Attribution Notice reasonably
calculated to inform recipients that You have modified the Original
Work.

7) Warranty of Provenance and Disclaimer of Warranty. Licensor warrants
that the copyright in and to the Original Work and the patent rights
granted herein by Licensor are owned by the Licensor or are sublicensed
to You under the terms of this License with the permission of the
contributor(s) of those copyrights and patent rights. Except as
expressly stated in the immediately preceding sentence, the Original
Work is provided under this License on an "AS IS" BASIS and WITHOUT
WARRANTY, either express or implied, including, without limitation, the
warranties of non-infringement, merchantability or fitness for a
particular purpose. THE ENTIRE RISK AS TO THE QUALITY OF THE ORIGINAL
WORK IS WITH YOU. This DISCLAIMER OF WARRANTY constitutes an essential
part of this License. No license to the Original Work is granted by this
License except under this disclaimer.

8) Limitation of Liability. Under no circumstances and under no legal
theory, whether in tort (including negligence), contract, or otherwise,
shall the Licensor be liable to anyone for any indirect, special,
incidental, or consequential damages of any character arising as a
result of this License or the use of the Original Work including,
without limitation, damages for loss of goodwill, work stoppage,
computer failure or malfunction, or any and all other commercial damages
or losses. This limitation of liability shall not apply to the extent
applicable law prohibits such limitation.

9) Acceptance and Termination. If, at any time, You expressly assented
to this License, that assent indicates your clear and irrevocable
acceptance of this License and all of its terms and conditions. If You
distribute or communicate copies of the Original Work or a Derivative
Work, You must make a reasonable effort under the circumstances to
obtain the express assent of recipients to the terms of this License.
This License conditions your rights to undertake the activities listed
in Section 1, including your right to create Derivative Works based upon
the Original Work, and doing so without honoring these terms and
conditions is prohibited by copyright law and international treaty.
Nothing in this License is intended to affect copyright exceptions and
limitations (including "fair use" or "fair dealing"). This License shall
terminate immediately and You may no longer exercise any of the rights
granted to You by this License upon your failure to honor the conditions
in Section 1(c).

10) Termination for Patent Action. This License shall terminate
automatically and You may no longer exercise any of the rights granted
to You by this License as of the date You commence an action, including
a cross-claim or counterclaim, against Licensor or any licensee alleging
that the Original Work infringes a patent. This termination provision
shall not apply for an action alleging patent infringement by
combinations of the Original Work with other software or hardware.

11) Jurisdiction, Venue and Governing Law. Any action or suit relating
to this License may be brought only in the courts of a jurisdiction
wherein the Licensor resides or in which Licensor conducts its primary
business, and under the laws of that jurisdiction excluding its
conflict-of-law provisions. The application of the United Nations
Convention on Contracts for the International Sale of Goods is expressly
excluded. Any use of the Original Work outside the scope of this License
or after its termination shall be subject to the requirements and
penalties of copyright or patent law in the appropriate jurisdiction.
This section shall survive the termination of this License.

12) Attorneys' Fees. In any action to enforce the terms of this License
or seeking damages relating thereto, the prevailing party shall be
entitled to recover its costs and expenses, including, without
limitation, reasonable attorneys' fees and costs incurred in connection
with such action, including any appeal of such action. This section
shall survive the termination of this License.

13) Miscellaneous. If any provision of this License is held to be
unenforceable, such provision shall be reformed only to the extent
necessary to make it enforceable.

14) Definition of "You" in This License. "You" throughout this License,
whether in upper or lower case, means an individual or a legal entity
exercising rights under, and complying with all of the terms of, this
License. For legal entities, "You" includes any entity that controls, is
controlled by, or is under common control with you. For purposes of this
definition, "control" means (i) the power, direct or indirect, to cause
the direction or management of such entity, whether by contract or
otherwise, or (ii) ownership of fifty percent (50%) or more of the
outstanding shares, or (iii) beneficial ownership of such entity.

15) Right to Use. You may use the Original Work in all ways not
otherwise restricted or conditioned by this License or by law, and
Licensor promises not to interfere with or be responsible for such uses
by You.

16) Modification of This License. This License is Copyright (c) 2005
Lawrence Rosen. Permission is granted to copy, distribute, or
communicate this License without modification. Nothing in this License
permits You to modify this License as applied to the Original Work or to
Derivative Works. However, You may modify the text of this License and
copy, distribute or communicate your modified version (the "Modified
License") and apply it to other original works of authorship subject to
the following conditions: (i) You may not indicate in any way that your
Modified License is the "Academic Free License" or "AFL" and you may not
use those names in the name of your Modified License; (ii) You must
replace the notice specified in the first paragraph above with the
notice "Licensed under <insert your license name here>" or with a notice
of your own that is not confusingly similar to the notice in this
License; and (iii) You may not claim that your original works are open
source software unless your Modified License has been approved by Open
Source Initiative (OSI) and You comply with its license review and
certification process.
|#