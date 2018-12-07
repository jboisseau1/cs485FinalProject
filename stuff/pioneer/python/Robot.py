#!/usr/bin/env python
# encoding: utf-8
#
#Robot.py
#Created by Ryan Garrett on 2008-09-28.
#An attempted re-implementation of Robot.java in Python
#Requires PySerial to be installed


#Issues to be resolved:
#--Check out how readPacket will handle (it won't) if the data turns out to not be there after the header. Euge!
#--> It crashes, that's how it handles it. This'll happen when you hot unplug the serial.
#
#--Defined that above as readInfoThread and am going to use self.readInfo for it I might want to change those around
#--Same thing for self.inputStream and inputStreamThread
#
#--Actually check to see if we disconnected the serial somewhere. isOpen()? Anyone? Yes? Perfect.
#--Reimplement printPacket, to work with readPacket at the end.
#--readInfoThread never sleeps, I don't believe. I think this is the same in the Java version as well, check it out?
#----could be quite the processor hog.

from __future__ import with_statement 
import threading
import serial
import time
from array import array
import math

class Robot:
	#Conversion Factors
	#YPOS_CONVERSION = 0.5083
	#XPOS_CONVERSION = 0.5083
	YPOS_CONVERSION = 1.0
	XPOS_CONVERSION = 1.0
	# OLD -- THPOS_CONVERSION = 0.001534
	THPOS_CONVERSION=(math.pi/2048.0)
	LVEL_CONVERSION = 0.6154
	RVEL_CONVERSION = 0.6154
	CONTROL_CONVERSION = 0.001534
	#SONAR_CONVERSION = 0.555
	SONAR_CONVERSION = 1.0
	def __init__(self):
		for  i in xrange(32):
			self.sonars.append(0)
			self.lastSonarTime.append(0)
		pass
	
	#Defines the thread to read information from the serial stream -- non-blocking
	class serialInputThread(threading.Thread):
		stream=None
		buf=array('B')
		threadLock=threading.Lock()
		lastData=long()
		working=1

		def __init__(self, stream):
			threading.Thread.__init__(self)
			self.stream=stream

		def read(self,length):
			outList=[]
			with self.threadLock:
				outList=self.buf.tolist()[:length]
				self.buf=self.buf[length:]
			return outList
			
		def timeoutRead(self,length,timeout):
			outList=[]
			startTime=int(time.time()*1000)
			while 1:
				#A tad bit messy -- Clean up later, perhaps.
				#Probably could alter this to do it a different way
				with self.threadLock:
					if (len(self.buf)>=len):
						outList=self.buf.tolist()[:length]
						self.buf=self.buf[length:]
						break
				if ((int(time.time()*1000)-startTime)>timeout):
					return []
				time.sleep(.02)
			return outList
		
		def run(self):
			ch=''
			self.working=1
			while (self.working):
				try:
					ch=self.stream.read(1)
					if (ch!=''):
						#We got a byte, add it up
						with self.threadLock:
							self.buf.append(ord(ch))
							self.lastData=time.time()
					else:
						pass
					#Give it a moment, and then check again
					#time.sleep(.02)
				except (serial.SerialException), e:
					print e
		
		def close(self):
			with self.threadLock:
				self.working=0
		
	class readInfoThread(threading.Thread):
		#You need to pass in the handle to the readPacket, displayInfo, and isVerbose functions when you start this thread
		DISPLAY_INTERVAL=1000
		readPacket=None
		displayInfo=None
		isVerbose=None
		
		working=1
		lastDisplayTime=0
		displayCount=0
		#I don't think this does anything here, ah well.
		threadLock=threading.Lock()
		def __init__(self,readPacket,displayInfo,isVerbose):
			threading.Thread.__init__(self)
			self.readPacket=readPacket
			self.displayInfo=displayInfo
			self.isVerbose=isVerbose
		def run(self):
			#displayCount
			self.working=1
			while (self.working):
				if (self.readPacket()): self.displayCount+=1
				if (self.isVerbose()):
					curtime=int(time.time()*1000)
					if (curtime - self.lastDisplayTime >= self.DISPLAY_INTERVAL):
						self.displayInfo(self.displayCount);
						self.lastDisplayTime = curtime
		def close(self):
			with self.threadLock:
				self.working=0
		
	class heartbeatThread(threading.Thread):
		pulse=None
		threadLock=threading.Lock()
		working=1
		def __init__(self,pulse):
			threading.Thread.__init__(self)
			self.pulse=pulse
		
		def run(self):
			self.working=1
			while (self.working):
				self.pulse()
				time.sleep(1.5)
		def close(self):
			with self.threadLock:
				self.working=0
		
	#The three thread names
	inputStream=None
	readInfo=None
	beatThread=None
	
	#PySerial Object
	#Different naming scheme from Java, since we only have one object
	stream = serial.Serial()
	
	#Verbosity
	#Initialize the verbosity variable, and declare a corresponding lock
	verbose=False
	verboseLock=threading.Lock()
	
	def setVerbose(self,val):
		"""Sets the driver to verbosely print out state sensor results and motor requests.
		
		Keyword Arguments:
		val -- boolean value to turn verbose on or off
		
		"""
		with self.verboseLock:
			self.verbose=val
	
	def isVerbose(self):
		"""Returns whether or not the driver is printing verbosely."""
		with self.verboseLock:
			val=self.verbose
		return val
	
	#Checksum Lenience -- "helps deal with problems in linux serial usb drivers"
	checksumLenient=False
	checksumLock=threading.Lock()
	
	def setChecksumLenient(self,val):
		"""Sets the driver to be lenient in permitting checksums off by 3 bytes.
		
		Keyword Arguments:
		val -- boolean value to turn checksum lenience on or off
		
		"""
		with self.checksumLock:
			self.checksumLenient=val
	
	def isChecksumLenient(self):
		"""Returns whether or not the driver is permitting checksums off by 3 bytes."""
		with self.checksumLock:
			val=self.ChecksumLenient
		return val
	
	#Communication Packet Header -- "All Pioneer robot packets being with this"
	#Hopefully it'll work as a simple list.
	#Move this later into the communication function? Seems abit odd to just be sitting out here.
	HEADER=[0xFA,0xFB]
	
	#Variable for independent threads (readInfo and beat)?
	working=True
	
	#Data from Robot -- Just going to declare ones that don't have a default value as the -- type thing
	#Ugly, but we can clean it up later?
	readLock=threading.Lock()
	validData=False
	motorEngaged=bool()
	xpos=float()
	ypos=float()
	orientation=float()
	leftWheelVelocity=float()
	rightWheelVelocity=float()
	battery=int()
	leftWheelStallIndicator=bool()
	rightWheelStallIndicator=bool()
	control=float()
	sonars=[]
	lastSonarTime=[]
	currentSonarTime=0
	
	lastToRobotTime=0
	
	robotName=""
	robotClass=""
	robotSubClass=""
	
	#Define the sync timeout
	SYNC_STREAM_WAIT = 300
	#Define the sync packets
	SYNC0_RESPONSE = [250, 251, 3, 0, 0, 0]
	SYNC1_RESPONSE = [250, 251, 3, 1, 0, 1]
	SYNC2_RESPONSE_START =  [250, 251]
	
	
	def isValidData(self):
		"""Returns true if we've received at least one valid sensor packet from the robot so far.
		
		Otherwise, all the sensor functions return garbage.
		"""
		with self.readLock:
			val=self.validData
		return val
	
	def isMotorEngaged(self):
		"""Returns true if the motor is engaged.  
		
		This information will not be valid until validData is true.
		"""
		with self.readLock:
			val=self.motorEngaged
		return val
	
	def getXPos(self):
		"""Returns the X coordinate of the center of the robot, as far as the robot believes.
		
		This information will not be valid until validData is true.  
		The ARCOS documentation claims the raw value provided by the robot is in millimeters.
		However, this appears to be incorrect.  We are multiplying by XPOS_CONVERSION (0.5083)
		to get to millimeters -- it may have to do with the wheel diameter.  Help here would be welcome.
		"""
		with self.readLock:
			val=self.xpos
		return val
	
	def getYPos(self):
		"""Returns the Y coordinate of the center of the robot, as far as the robot believes.
		
		This information will not be valid until validData is true.  
		The ARCOS documentation claims the raw value provided by the robot is in millimeters.
		However, this appears to be incorrect.  We are multiplying by YPOS_CONVERSION (0.5083)
		to get to millimeters -- it may have to do with the wheel diameter.  Help here would be welcome.
		"""
		with self.readLock:
			val=self.ypos
		return val
	
	def getOrientation(self):
		""""Returns the THETA (rotation) of the center of the robot, as far as the robot believes.
		
		This information will not be valid until validData is true.  
		The ARCOS documentation claims the raw value provided by the robot is 'angular units',
		with 0.001534 radians per 'angular unit'.  So we are multiplying by THPOS_CONVERSION (0.001534)
		to get to radians.  Help here would be welcome if this appears to be incorrect.
		"""
		with self.readLock:
			val=self.orientation
		return val
	
	def getLeftWheelVelocity(self):
		"""Returns the left wheel velocity in millimeters per second.
		
	    This information will not be valid until validData is true.  
		The ARCOS documentation claims the raw value provided by the robot is in mm/sec.
		However, this appears to be incorrect.  We are multiplying by LVEL_CONVERSION (0.6154)
		to get to mm/sec -- it may have to do with the wheel diameter.  Help here would be welcome.
		"""
		with self.readLock:
			val=self.leftWheelVelocity
		return val
	
	def getRightWheelVelocity(self):
		"""Returns the right wheel velocity in millimeters per second.
		
	    This information will not be valid until validData is true.  
		The ARCOS documentation claims the raw value provided by the robot is in mm/sec.
		However, this appears to be incorrect.  We are multiplying by RVEL_CONVERSION (0.6154)
		to get to mm/sec -- it may have to do with the wheel diameter.  Help here would be welcome.
		"""
		with self.readLock:
			val=self.rightWheelVelocity
		return val
	
	def getBatteryStatus(self):
		"""Returns the battery charge in tenths of volts."""
		with self.readLock:
			val=self.battery
		return val
	
	def isLeftWheelStalled(self):
		"""Returns true if the left wheel is being blocked and thus unable to move."""
		with self.readLock:
			val=self.leftWheelStallIndicator
		return val
	
	def isRightWheelStalled(self):
		"""Returns true if the right wheel is being blocked and thus unable to move."""
		with self.readLock:
			val=self.rightWheelStallIndicator
		return val
	
	def getControlServo(self):
		"""Returns the current value of the server's angular position servo, in degrees. 
		
		(unused on our robots)
		"""
		with self.readLock:
			val=self.control
		return val
		
	def getSonar(self,index):
		"""Returns the last value of sonar #index.
		
		Keyword Arguments:
		index -- index of sonar to retrieve
		
		The ARCOS documentation claims that the value is in millimeters away from the sonar.  However, this appears to be 
		incorrect.  We are multiplying by SONAR_CONVERSION (0.555) to get to millimeters. Note that sonars come in different 
		batches, so you may not receive all of them in one time tick.  Looking at the front of the robot, sonar 0 is the 
		far-right front servo and sonar 7 is the far-left front servo.  Looking at the back of the robot, sonar 8 is the
		far-right rear servo and sonar 15 is the far-left rear servo.
		"""
		with self.readLock:
			val=self.sonars[index]
		return val
		
	def getLastSonarTime(self,index):
		"""Returns the last time tick that information for sonar #index arrived.
		
		Keyword Arguments:
		index -- index of sonar to retrieve
		
		Note that sonars come in different batches, so you may not receive all of them in one time tick.  Each time any sonar batch
		arrives, the time tick is increased; this can thus give you an idea as whether your sonar data is really old (if it's older
		than 8, you've got old data -- older than 32 say, you've got REALLY old data). Looking at the front of the robot, sonar 0 
		is the far-right front servo and sonar 7 is the far-left front servo.  Looking at the back of the robot, sonar 8 is the 
		far-right rear servo and sonar 15 is the far-left rear servo.
		"""
		with self.readLock:
			val=self.lastSonarTime[index]
		return val
	
	def getCurrentSonarTime(self):
		"""Returns the current time tick to compare with past time ticks for sonar. """
		with self.readLock:
			val=self.currentSonarTime
		return val
		
	#The double[] maybeReuse is pointless to reproduce, because it only helps in Java to not have to declare another array
	#And in Python, it appears that what is passed to a function is merely a copy of the list, not a reference.
	def getSonars(self):
		"""Returns all the sonars values.  
		
		The ARCOS documentation claims that the values are in millimeters away from the sonar.  However, this appears to be incorrect.
		We are multiplying by SONAR_CONVERSION (0.555) to get to millimeters.  If you pass in NULL or an array differently sized, you'll
		get back a new array; else the array passed in will be filled in and returned, allowing you to save allocations. Looking at the
		front of the robot, sonar 0 is the far-right front servo and sonar 7 is the far-left front servo.  Looking at the back of the 
		robot, sonar 8 is the far-right rear servo and sonar 15 is the far-left rear servo.
		"""
		with self.readLock:
			#Using a colon allows you to make a copy rather than passing the reference. -- Need to look more into this.
			val=self.sonars[:]
		return val
	
	#Again, pointless to reproduce the argument
	def getLastSonarTimes(self):
		"""Returns all the time ticks, for each sonar, when the sonar data last arrived.
		
		Note that sonars come in different batches, so you may not receive all of them in one time tick.  Each time any sonar batch arrives,
		the time tick is increased; this can thus give you an idea as whether your sonar data is really old (if it's older than 8, you've got
		old data -- older than 32 say, you've got REALLY old data). Looking at the front of the robot, sonar 0 is the far-right front servo 
		and sonar 7 is the far-left front servo.  Looking at the back of the robot, sonar 8 is the far-right rear servo and sonar 15 is the
		far-left rear servo.
		"""
		with self.readLock:
			#Using a colon allows you to make a copy rather than passing the reference. -- Need to look more into this.
			val=self.lastSonarTime[:]
		return val
	
	def getRobotName(self):
		"""Returns the robot name string the robot had indicated upon connection."""
		with self.readLock:
			val=self.robotName
		return val
	
	def getRobotClass(self):
		"""Returns the robot class string the robot had indicated upon connection."""
		with self.readLock:
			val=self.robotClass
		return val
	
	def getRobotSubclass(self):
		"""Returns the robot subclass string the robot had indicated upon connection."""
		with self.readLock:
			val=self.robotSubclass
		return val
	
	#Checksum function nearly directly copied from Java
	def checksum(self,data):
		"""Calculates the checksum of the data. 
		
		Keyword Arguments:
		data -- list of bytes to checksum
		"""
		c,data1,data2=0,0,0
		
		for x in xrange(0,(len(data)-1),2):
			#Can probably clean up this some
			data1=data[x]
			data2=data[x+1]
			c+=((data1<<8) | data2)
			c=c & 0xffff
		if ((len(data) & 0x1) == 0x1):
			data1=data[len(data)-1]
			c = c^data1
		return c
	
	def submit(self,data):
		"""Send the data to the robot.
		
		Keyword Arguments:
		data -- list of bytes to process and send
		
		The data contains only the instruction number and parameters. the header if automatically added, the length of the
		packet is calculated and the checksum is appended at the end of the packet, so that the user's job is easier.
		"""
		try:
			#Intialize a new Byte array, first appending it with the header.
			packet=array('B',self.HEADER)
			checksum = self.checksum(data);
			#Add the length to the packet
			packet.append(len(data)+2)
			packet.extend(data)
			packet.append(checksum>>8)
			packet.append(checksum & 0x00ff)
			
			#Write out the data here
			if (self.stream.isOpen()):
				self.stream.write(packet.tostring())
			
			current = int(time.time()*1000)
			#The entire verbose print is commented out in Java, so I'm just going to leave it out.
			self.lastToRobotTime = current
		except (serial.SerialException):
			return False
		return True
	
	# Stopped before private void printPacket(byte[] header, byte[] data, PrintStream err)
	# Asking Sean if the length and checksum should have been included in that function
	# Assuming that they should be:
	
	#Yeah, I did this one wrong. It happens.
	def printPacket(self, header, data):
		"""Prints out the packet that submit would generate from the data.
		
		Keyword Arguments:
		header -- list of bytes to process
		data -- list of bytes to process
		"""
		packet=array('B',header)
		checksum = self.checksum(data);
		#Add the length to the packet
		packet.append(len(data)+2)
		packet.extend(data)
		packet.append(checksum>>8)
		packet.append(checksum & 0x00ff)
		
		packetstr=""
		for byte in packet.tolist():
			 packetstr+="%s "%(byte)
			
		print packetstr

	def readString(self):
		string=""
		ch=0
		try:
			while(1):
				try:
					ch=self.inputStream.read(1)[0]
					if (ch==0):
						break
					string+=chr(ch)
				except (IndexError):
					#There isn't a character yet, try again.
					pass
		except (serial.SerialException):
			#This can't be good.
			print e
			return None
		return string
	
	def lookFor(self,expected,timeout):
		startTime=int(time.time()*1000)
		count=0
		char=0
		while (count<len(expected)):
			try:
				char=self.inputStream.read(1)[0]
				if (char==expected[count]):
					count+=1
				else:
					count=0
			except IndexError:
				#There isn't a character yet, try again.
				pass
			if ((int(time.time()*1000)-startTime)>timeout):
				print "Timed out."
				return False
		return True
	
	THREAD_SLEEP=.003
	
	def sleep(self):
		time.sleep(self.THREAD_SLEEP)
		
		
	def readPacket(self):
		if (self.lookFor(self.SYNC2_RESPONSE_START, 1000)):
			#This puts us at the start of the Data packet
			#We might have to make this loop, if the other data isn't coming in fast enough, or just timeout
			data=[]
			length=0
			expectedChecksum=0
			gotChecksum=0
			
			while(1):
				try:
					length=self.inputStream.read(1)[0]
					break
				except IndexError:
					pass
				#time.sleep(.002)
			
			#Read the rest of the packet in
			#If the packet starts to turn out to not be there, this could get ugly. Might want to take a look at that, but since we've got the
			#Header in, we should be fine.
			#length-=1
			tl=length
			while (1):
				#print "Stuck Here -- 2"
				try:
					#print "Looking for length %d, have length %d"%(length,len(data))
					
					data.extend(self.inputStream.read(tl))
					if (len(data)==length):
						break
					else:
						tl=length-len(data)
						
				except (IndexError),e:
					print e
				#time.sleep(.002)
				
			#print "Packet is: %s"%data
			#This should be right.
			
			expectedChecksum=self.checksum(data[:len(data)-2])
			#Ugly? Yes.
			gotChecksum=((((data[len(data)-2])*256)+data[len(data)-1]) & 0xFFFF)
			
			#NOTE: On Linux boxen running with Prolific/Manhattan usb serial drivers, we're
			#seeing checksums that are ALWAYS off by 3! But the data seems to be correct, so 
			#we're allowing a weird checksum of this form.
			if ( expectedChecksum == gotChecksum or (self.checksumLenient and (expectedChecksum == gotChecksum + 3 or expectedChecksum == gotChecksum - 3))):
				#Temporary variables to store robot information, transferred to the real ones after we finish.
				_motorEngaged = False
				_xpos = 0.0
				_ypos = 0.0
				_orientation = 0.0
				_leftWheelVelocity = 0.0
				_rightWheelVelocity = 0.0
				_battery = 0
				_leftWheelStallIndicator = False
				_rightWheelStallIndicator = False
				_control = 0
				_sonars=[]
				_lastSonarTime=[]
				#Removed string "ss" -- it isn't used anywhere. Probably old verbosity function.
				
				if ( data[0] == 0x32 ):
				    _motorEngaged = False;
				elif ( data[0] == 0x33 ):
				    _motorEngaged = True;
				else:
				    print "Invalid motor status: %d"%(data[0])
				
				alpha=0.0
				
				#Get XPos
				alpha=data[2]
				alpha=alpha & 0x7F
				alpha = alpha * 256;
				alpha += data[1]
				if (alpha >= 16384): alpha -= 32768 
				_xpos = self.XPOS_CONVERSION * alpha
				
				#Get YPos
				alpha=data[4]
				alpha=alpha & 0x7F
				alpha = alpha * 256;
				alpha += data[3]
				if (alpha >= 16384): alpha -= 32768 
				_ypos = self.YPOS_CONVERSION * alpha
				
				#Read Orientation
				alpha = data[6]
				alpha = alpha * 256
				alpha += data[5]
				if (alpha >= 2048): alpha -= 4096;
				_orientation = self.THPOS_CONVERSION * float(alpha)
				
				#Read velocity of left wheel
				alpha = data[8]
				alpha = alpha * 256
				alpha += data[7]
				if (alpha >= 32768): alpha -= 65536;
				_leftWheelVelocity = self.LVEL_CONVERSION * alpha
				
				#Read velocity of right wheel
				alpha = data[10]
				alpha = alpha * 256
				alpha += data[9]
				if (alpha >= 32768): alpha -= 65536;
				_rightWheelVelocity = self.RVEL_CONVERSION * alpha
				
				#Read battery status
				_battery = data[11]
				
				#Read bumpers
				#Could be a bug, we'll see.
				#I think that this should do it, since we're going from 256 instead of 128?
				_leftWheelStallIndicator = (data[12]&0x100)!=0
				_rightWheelStallIndicator = (data[13]&0x100)!=0
				
				#Read control
				alpha = data[15]
				alpha = alpha*256
				alpha += data[14]
				_control = self.CONTROL_CONVERSION * alpha
				
				#Read PTU -- Not implemented
				
				#Read Compass -- Not implemented
				
				#Read Sonar
				nr=0
				nr = data[19]
				self.currentSonarTime+=1
				
				with self.readLock:
					_sonars=self.sonars[:]
					_lastSonarTime=self.lastSonarTime[:]
				
				for nn in xrange(0,nr):
					sonarNumber=data[20+3*nn]
					if (sonarNumber < 0 or sonarNumber > 31):
						#We've got a "crazy sonar"
						print "Bad Sonar Number %d, only 0 - 31 are valid"%(sonarNumber)
					elif ((20+3*nn+2) >= len(data)):
						print "Invalid sonar index, got %d"%(nn)
					else:
						alpha = data[20+3*nn+2]
						alpha = alpha * 256
						alpha += data[20+3*nn+1]
						_sonars[sonarNumber] = self.SONAR_CONVERSION * alpha
						_lastSonarTime[sonarNumber] = self.currentSonarTime
				
				#Now we lock, and dump the data.
				with self.readLock:
					self.validData = True
					self.motorEngaged = _motorEngaged
					self.xpos = _xpos
					self.ypos = _ypos
					self.orientation = _orientation
					self.leftWheelVelocity = _leftWheelVelocity
					self.rightWheelVelocity = _rightWheelVelocity
					self.battery = _battery
					self.leftWheelStallIndicator = _leftWheelStallIndicator
					self.rightWheelStallIndicator = _rightWheelStallIndicator
					# Be careful not to lose your self.control
					self.control = _control
					self.sonars=_sonars[:]
					self.lastSonarTime=_lastSonarTime[:]
				return True
			else:
				if (self.isVerbose()):
					print "Bad checksum, got %d , expected  %d"%(gotChecksum, expectedChecksum)
					#Should print the packet here.
			
		return False
	
	def connect(self, port, rate=9600):
		"""Opens the serial connection with the given parameters.
		
		Keyword Arguments:
		port -- name or number of serial port to open
		rate -- baud rate of serial connection to open
		
		Returns True if the stream opens, false if it does not.
		This also doesn't check if it's already open, it just goes for it.
		"""

		self.stream.port=port
		self.stream.baudrate=rate
		self.stream.timeout=0
		
		try:
			self.stream.open()
			self.stream.flushInput()
			self.stream.flushOutput()
			self.inputStream=self.serialInputThread(self.stream)
			self.inputStream.setDaemon(True)
			self.inputStream.start()
		except (serial.SerialException), e:
			return False
			#I think that we'd just kill the process in Java, here.
		#If we get to here, then the serial port should be open, ready for data.
		
		#I'm going to go ahead and combine both connect functions.self.sleep
		#return submit(new byte[ ] { 2 }, out);
		
		# -- Copied from Java, barely changed. Is there a better way to do this?
		#do doSync0, and if that succeeds, do doSync1, and if that succeeds, do doSync2, and
		# if that succeeds, we're done.  Else fall back and do them again.
		a = True;
		while (1):
			self.close()
			if (a):
				a = False
			else:
				self.sleep()
			b = True;
			while (1):
				if (b):
					b=False
				else:
					self.sleep()
				c = True
				while (1):
					if (c):
						c = False
					else:
						self.sleep()
					if (self.doSync0()): break
				if (self.doSync1()): break
			if (self.doSync2()): break
		print "We're out of the loop"
		#Read the autoconfiguration strings
		#String s = "Name: " + (robotName = readString()) + " / " + 
		#    (robotClass = readString()) + " / " + 
		#    (robotSubclass = readString());
		with self.readLock:
			self.robotName=self.readString()
			self.robotClass=self.readString()
			self.robotSubclass=self.readString()
			if (self.isVerbose()): print "Name: %s / %s / %s"%(self.robotName,self.robotClass,self.robotSubclass)
		
		#Pull down the two checksum bytes, timeout as needed.
		self.inputStream.read(2)
		#If we need to wait here, honestly, then we'll uncomment this.
		### POSSIBLE PROBLEM ###
		#self.inputStream.timeoutread(2,300)
		#Could also use lookFor, I think?
		
		self.open()
		
		#We start the two threads -- Hopefully passing the function will work.
		#We'll find out quickly if this works or not. -- i.e function not correctly called error, or such.
		self.readInfo=self.readInfoThread(self.readPacket,self.displayInfo,self.isVerbose)
		self.beatThread=self.heartbeatThread(self.pulse)
		
		#Make them daemon threads, so that Python kills them when our main thread dies.
		self.readInfo.setDaemon(True)
		self.beatThread.setDaemon(True)
		
		#Let's make 'em go!
		self.readInfo.start()
		self.beatThread.start()
		
		return True
	def displayInfo(self, count):
		with self.readLock:
			#Horribly long, but it gets the job done.
			print "In(%d) Loc(%.4f,%.4f,%.4f) Vel(L: %.4f %s R: %.4f %s C: %d) %s"%(count,self.xpos,self.ypos,self.orientation,self.leftWheelVelocity, ("S" if self.leftWheelStallIndicator else ""),self.rightWheelVelocity, ("S" if self.rightWheelStallIndicator else ""),self.control,"Go" if self.motorEngaged else "Stop")
			st=""
			count=0
			if (self.sonars!=[]):
				for sonar in self.sonars:
					st+="%d "%(sonar)
					if (count%8==7):
						st+='\n'
					count+=1
				print st
		
	def doSync0(self):
		if (self.sync0() == False):
			return False
		if (self.isVerbose()):
			print "Waiting for SYNC0...."
		return self.lookFor(self.SYNC0_RESPONSE, self.SYNC_STREAM_WAIT)
	
	def doSync1(self):
		if (self.sync1() == False):
			return False
		if (self.isVerbose()):
			print "Waiting for SYNC1...."
		return self.lookFor(self.SYNC1_RESPONSE, self.SYNC_STREAM_WAIT)
	
	def doSync2(self):
		if (self.sync2() == False):
			return False
		if (self.isVerbose()):
			print "Waiting for SYNC2...."
		
		time.sleep(self.SYNC_STREAM_WAIT/1000)
		
		if (self.lookFor(self.SYNC2_RESPONSE_START, self.SYNC_STREAM_WAIT)==False):
			print "Failed in the lookfor"
			return False	
		#if (self.inputStream.timeoutRead(2,self.SYNC_STREAM_WAIT)==[]):
		#	print "Failed in the timeoutread"
		#	return False
		self.inputStream.read(2)
		return True
	
	def lowByte(self, arg):
		return arg & 0xff
		
	def highByte(self,arg):
		return (arg >> 8) & 0xff
		
	def sync0(self):
		"""Sends sync0 message (for connection to robot)"""
		return self.submit([0])
	
	def sync1(self):
		"""Sends sync1 message (for connection to robot)"""
		return self.submit([1])
		
	def sync2(self):
		"""Sends sync2 message (for connection to robot)"""
		return self.submit([2])
	
	def pulse(self):
		"""Client pulse resets watchdog and prevents the robot from disconnecting."""
		return self.submit([0])
		
	def open(self):
		"""Starts the controller."""
		return self.submit([1])
		
	def close(self):
		"""Closes the client-server connection."""
		return self.submit([2])
	
	def polling(self,arg):
		"""Sets the sonar polling sequence.
		
		Keyword Arguments:
		arg -- string of the polling sequence
		"""
		temp=[]
		temp.append(3) # Command number
		temp.append(0x2B) # Parameter is a string
		temp.append(len(arg))
		temp.extend(arg)
		self.submit(temp)
	
	def enable(self, arg):
		"""Enable/disable the motors
		
		Keyword Arguments:
		arg -- boolean of requested motor state
		"""
		if (arg):
			return self.submit([4, 0x3B, 1, 0])
		else:
			return self.submit([4, 0x3B, 0, 0])
	def seta(self,arg):
		"""Set maximum possible translation acc/deceleration; in mm/sec
		
		Keyword Arguments:
		arg -- acceleration in mm/sec
		"""
		if( arg >= 0 ):
			return self.submit( [5, 0x3B, self.lowByte(arg), self.highByte(arg)])
		else:
			return self.submit( [5, 0x1B, self.lowByte(-arg), self.highByte(-arg)])
	
	def setv(self,arg):
		"""Sets the maximum possible translation velocity; in mm/sec
		
		Keyword Arguments:
		arg -- velocity in mm/sec
		"""
		return self.submit([6, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def seto(self):
		"""Resets the robot's believed position to be x=0, y=0, theta=0"""
		return self.submit([7])
	
	def setrv(self, arg):
		"""Sets maximum rotational velocity; in degrees/sec
		
		Keyword Arguments:
		arg -- velocity in degrees/sec
		"""
		return self.submit([10, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def vel(self, arg):
		"""Move forward (+) or reverse (-); in mm/sec
		
		Keyword Arguments:
		arg -- velocityin mm/sec
		"""
		return self.submit([11, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def head(self,arg):
		"""Turn to absolute heading (in the robot's belief); 0-359 degress
		
		Keyword Arguments:
		arg -- heading in degrees
		"""
		return self.submit([12, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def dhead(self, arg):
		"""Turn relative to current heading, that is, turn BY some number of degrees.
		
		Keyword Arguments:
		arg -- number of degrees to turn
		"""
		return self.submit([13, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def say(self, arg):
		"""Play some tones. 
		
		Keyword Arguments:
		arg -- list of tones to play
		Sound duration (20 ms increments)/tone (half-cycle) pairs.
		"""
		#Yes, we could make this a bit smaller by only using 2 extends instead of 3 appends and 1 extend
		
		temp=[]
		temp.append(15) #Command number
		temp.append(0x2B) # Parameter is a string
		temp.append(len(arg)) # Length of string
		temp.extend(arg) #Append all of the tones
		return self.submit(temp)
	
	#Request Configuration SIP -- Not implemented
	
	#Request Continuous (>0) or stop sending (=0) encoder SIPs -- Not implemented
	
	def rvel(self, arg):
		"""rotate at +/- degrees/sec
		
		Keyword Arguments:
		arg -- rate in degrees to rotate
		"""
		if( arg >= 0 ):
			return self.submit([21, 0x3B, self.lowByte(arg), self.highByte(arg)])
		else:
			return self.submit([21, 0x1B, self.lowByte(-arg), self.highByte(-arg)])
	
	
	def setra(self, arg):
		"""Sets rotational (+/-)de/acceleration; in mm/sec^2.
		
		Keyword Arguments:
		arg -- rate of acceleration in mm/sec^2
		"""
		return self.submit([23, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def sonar(self, arg):
		"""Enable/disable the sonars.
		
		Keyword Arguments:
		arg -- (True or False) to turn them all on or off -- (Int) to turn on or off a single one
		"""
		#Assuming that they send us either an int, or a bool
		if (type(arg)==bool):
			if (arg):
				return self.submit([28, 0x3B, 1, 0])
			else:
				return self.submit([28, 0x3B, 0, 0])
		else:
			return self.submit([28, 0x3B, (arg % 256), (arg / 256)])
	
	def stop(self):
		"""Stops the robot (motors remain enabled)."""
		return self.submit([29])
	
	def digout(self,arg):
		"""Msbits is a byte mask that selects output port(s) for changes; lsbits set (1) or reset (0) the selected port.
		
		Keyword Arguments:
		arg -- bytemask? 
		"""	
		#Make sure that the doc-string is correct.
		return self.submit([30, 0x3B, self.lowByte(arg), self.highByte(arg)])
		
	def vel2(self, arg1, arg2=None):
		"""Set independent wheel velocities, left and right.
		
		Key Arguments:
		arg1 -- If arg2 is not present, arg1 represents the combined left and right values. Otherwise, it is the left.
		arg2 -- If present, arg2 represents the right value.
		"""
		if (arg2!=None):
			#We've got two inputs. First is left, second is right.
			#Technically recursive:
			self.vel2((arg1<<8 | arg2))
		else:
			#We've only got one, it's the combination. Send it.
			return self.submit([32, 0x3B, self.lowByte(arg1), self.highByte(arg1)])
	
	#Pioneer Gripper server command -- Not implemented
	
	#Select the A/D port number for analog value in SIP -- Not implemented
	
	#Pioneer Gripper server value -- Not implemented
	
	#Msb is the port number (1-4) and lsb is the pulse width in 100 microsec units PSOS or 10 microsec units P2OS -- not implemented
	
	#Send string argument to serial device connected to AUX port on microcontroller -- not implemented
	
	#Request to retrieve 1-200 bytes from the aux serial channel; 0 flusshes the AUX serial input buffer -- not implemented
	
	def bumpstall(self, arg):
		"""Stop and register a stall.
		
		Keyword Arguments:
		arg -- In front (1), rear (2) or either (3) bump-ring contacted. Off (default) is 0
		"""
		return self.submit([44, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def e_stop(self):
		"""Emergency stop, overrides deceleration."""
		return self.submit([55])
		
	def sound(self, arg):
		"""Play stored sound -- Amigobot only.  Pioneer's don't have such a thing.
		
		Keyword Arguments:
		arg -- short representing tone
		"""
		return self.submit([90, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def playlist(self, arg):
		"""Request playlist packet for sound number or 0 for all user sounds -- Amigobot only.
		
		Keyword Arguments:
		arg -- short representing sound number, or 0
		"""
		return self.submit([91, 0x3B, self.lowByte(arg), self.highByte(arg)])
	
	def soundtog(self,arg):
		"""Mute (0) or Enable (1) sounds -- Amigobot only
		
		Keyword Arguments:
		arg -- 0 to mute, 1 to enable
		"""
		return self.submit([92, 0x3B, lowByte(arg), highByte(arg)])
	
	def disconnect(self):
		"""Shuts down the communication to the robot"""
		
		#Should the main process hard-quit, then the Daemon status should take care of killing the children.
		# -- The daemons are killing the children. This has to raise a few questions.
		#Kill and join the threads we've made.
		try:
			self.readInfo.close()
			self.readInfo.join()
		except Exception:
			pass
		try:
			self.beatThread.close()
			self.beatThread.join()
		except Exception:
			pass

		#Send the close message to the robot
		self.close()
		#Send the kill to inputStream, and join it.
		try:
			self.inputStream.close()
			self.inputStream.join()
		except Exception:
			pass
		
		#Flush the serial connection and then close it.
		try:
			self.stream.flushInput()
			self.stream.flushOutput()
			self.stream.close()
		except (serial.SerialException), e:
			print "Error Closing Connection: %s"%(e)
			pass
		
		#Grab a new Serial instance
		self.stream=serial.Serial()