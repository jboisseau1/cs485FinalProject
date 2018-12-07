# Robot.py
# Pythonized by Andrew Pennebaker

__license__="BSD"

import time, threading

# General class for controlling Activmedia (MobileRobots) Pioneer and Amigobot robots.
# Synchronization on the class occurs when data comes in, so don't sync on this class in general.

class Robot:
	# All Pioneer robot packets begin with this header.
	HEADER=[0xfa, 0xfb]

	THREAD_SLEEP=300 # milliseconds

	SYNC_STREAM_WAIT=300 # milliseconds
	SYNC0_RESPONSE=[0xfa, 0xfb, 0x03, 0x00, 0x00, 0x00]
	SYNC1_RESPONSE=[0xfa, 0xfb, 0x03, 0x01, 0x00, 0x01]
        
	DISPLAY_INTERVAL=1000

	def __init__(self):
		self.verbose=False

		self.outstream=None
		self.instream=None
		self.sock=None

		self.working=True
		self.beatThread=None
		self.readInfoThread=None

		self.motorMoving=False

		self.xpos, self.ypos=0.0, 0.0	# millimeters
		self.orientation=0.0			# radians
		self.leftWheelVelocity=0.0		# mm/sec
		self.rightWheelVelocity=0.0		# mm/sec

		self.battery=0
		self.leftWheelStallIndicator=False
		self.rightWheelStallIndicator=False
		self.control=0.0 # Server's angular position servo
		self.sonars=[0.0]*256 # millimeters

		self.robotName=""
		self.robotClass=""
		self.robotSubclass=""

		self.lastToRobotTime=0

		self.lastDisplayTime=0
		self.displayCount=0

		self.lock=threading.Lock()

	# Set the driver to verbosely print out state sensor results and motor requests.
	def setVerbose(self, value):
		self.verbose=value
	def isVerbose(self):
		return self.verbose

	def isMotorEngaged(self):
		return self.motorMoving

	def getXPos(self):
		return self.xpos

	def getYPos(self):
		return self.ypos

	def getOrientation(self):
		return self.orientation

	def getLeftWheelVelocity(self):
		return self.leftWheelVelocity

	def getRightWheelVelocity(self):
		return self.rightWheelVelocity

	def getBatteryStatus(self):
		return self.battery

	def isLeftWheelStalled(self):
		return self.leftWheelStallIndicator

	def isRightWheelStalled(self):
		return self.rightWheelStallIndicator

	def getControlServo(self):
		return self.control

	def getSonar(index):
		return self.sonars[index]

	def getSonars(maybeReuse=[]):
		return [e for e in self.sonars]

	def getRobotName(self):
		return self.robotName

	def getRobotClass(self):
		return self.robotClass

	def getRobotSubclass(self):
		return self.robotSubclass

	def checksum(self, data):
		c, data1, data2=0, 0, 0

		i=0
		while i<len(data)-1:
			data1=data[i]
			if data1<0:
				data1+=256

			data2=data[i+1]
			if data2<0:
				data2+=256

			c+=(data1<<8) | data2
			c&=0xffff
			i+=2

		if (len(data)&0x1)==0x01: # odd
			data1=data[-1]
			if data1<0:
				data1+=256

			c^=data1

		return c

	# Send the data to the robot. The data contains only the instruction number and parameters.
	# The header if automatically added, the length of the packet is calculated and the checksum is
	# appended at the end of the packet, so that the user's job is easier.
        
	def submit(self, data):
		try:
			packet=[0x00]*3+[e for e in data]+[0x00]*5

			packet[0]=self.HEADER[0]
			packet[1]=self.HEADER[1]
			packet[2]=len(data)+2 # This cannot exceed 200!

			sum=self.checksum(data)

			packet[-2]=sum>>8
			packet[-1]=sum&0xff

			# Write out the data
			self.lock.acquire()
			if self.outstream!=None:
				self.outstream.write(packet)
			self.lock.release()

			current=time.time()
			if self.isVerbose():
				print (
					"Packet Sent ("+
					(current-self.lastToRobotTime)+
					"):"
				)

				self.printPacket(packet)

			self.lastToRobotTime=current

		except IOError, e:
			return False

		return True

	# Prints out the two arrays to err in order.
	# Bytes are printed 0...256, not -128...127
	# Either array or array2 can be None.
	def printPacket(self, header, data=[]):
		if header!=[]:
			print " ".join([(e+256)%256 for e in header])
		if data!=[]:
			print " ".join([(e+256)%256 for e in data])

		print ""

	def connect(host, port):
		self.sock=socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.sock.connect((host, port))
		self.outstream=# ...
		self.instream=# ...

	def readBytes(self, buf, offset, size):
		ptr=0
		while ptr<size:
			ptr+=self.instream.read(buf, ptr+offset, size-ptr)
			if ptr<size:
				try:
					Thread.currentThread().sleep(20) # milliseconds
				except:
					pass

	def sleep(self):
		try:
			time.sleep(THREAD_SLEEP)
		except:
			pass

	# Waits for the byte string in 'expected' to show up in the incoming stream.
	# This is a simpler algorithm than your typical string-matching algorithm, because
	# it assumes that the FIRST byte in 'expected' is different from all the others. 
	# Times out after the given timeout in milliseconds.

	def lookFor(self, expected, timeout):
		t=time.time()
		count=0
		buf=[0]

		while count<len(expected):
			try:
				self.readBytes(buf, 0, 1)
				if buf[0]==expected[count]:
					count+=1
				else:
					count=0
			except IOError, e:
				pass

			if time.time()-timeout>=t:
				return False

		return True

	def doSync0(self, buf):
		if self.sync0()==False:
			return False
		if self.isVerbose():
			print "Waiting for SYNC0...."

		return self.lookFor(self.SYNC0_RESPONSE, self.SYNC_STREAM_WAIT)

	def doSync1(self, buf):
		if self.sync1()==False:
			return False
		if self.isVerbose():
			print "Waiting for SYNC1...."

		return self.lookFor(self.SYNC1_RESPONSE, self.SYNC_STREAM_WAIT)

	def doSync2(self, buf):
		if self.sync2()==False:
			return False
		if self.isVerbose():
			print "Waiting for SYNC2...."

		try:
			self.readBytes(buf, 0, 4)
		except IOError, e:
			return False # 4 junk bytes sent, not in documentation

		return True
        
	def readString(self):
		try:
			buf=[0]
			sbuf=""
			while True:
				self.readBytes(buf, 0, 1)
				if buf[0]==0:
					break
				else:
					sbuf+=chr(buf[0])
			return sbuf
		except IOError, e:
			return None

	def connect(self, instream, outstream):
		# set up a nonblocking input stream so syncs work more reliably
		syncInputStream=TimeoutInputStream(NonClosingInputStream(inputStream), 256, 100, 0)

		buf=[0]*7

		# if count is exceeded in SYNCing, it's assume that the robot's stream is already open
		MAX_COUNT=5
		count=0

		self.instream=syncInputStream
		self.outstream=outstream

		# Do doSync0, and if that succeeds, do doSync1, and if that succeeds, do doSync2, and
		# if that succeeds, we're done. Else fall back and do them again.
		a=True
		do:
			# close the controller if it's already open
			self.close()

			if a:
				a=False
			else:
				self.sleep()
			b=True

			do:
				if b:
					b=False
				else:
					sleep()
				c=True

				do:
					if c:
						c=False
					else:
						sleep()
				while not self.doSync0(buf)
			while not self.doSync1(buf)
		while not self.doSync2(buf)

		# read autoconfiguration strings (3)
		self.robotName=self.readString()
		self.robotClass=self.readString()
		self.robotSubclass=self.readString()

		if self.isVerbose():
			print "Name: "+(self.robotName)+" / "+(self.robotClass)+" / "+(self.robotSubclass)

		# start the controller
		self.open()

		# We've synced, so get rid of the troublesome TimeoutInputStream
		syncInputStream.close()
		syncInputStream=None
		self.instream=instream

		# start the read thread
		self.readInfoThread=Thread(
			Runnable():
				def run():
					while self.working:
						try:
							if self.readPacket():
								self.displayCount++
						except IOError, e:
							print e
					if self.isVerbose():
						cur=time.time() # not millis
						if cur-self.lastDisplayTime>=self.DISPLAY_INTERVAL:
							self.displayInfo()
							self.lastDisplayTime=cur
		)

		self.readInfoThread.start()

		# Start the beat thread
		self.beatThread=Thread(
			Runnable():
				def run():
					while self.working:
						self.pulse()
						try:
							time.sleep(1500) # milliseconds
						except SleepInterruptedException, e: # Must be under 2 seconds
							pass
		)
		self.beatThread.start()

	def synchronized displayInfo(self):
		print (
			"In("+self.displayCount+") Loc("+self.xpos+", "+self.ypos+", "+self.orientation+") Vel(L: "+self.leftWheelVelocity+(self.leftWheelStallIndicator ? "S":"")+" R: "+self.rightWheelVelocity+(self.rightWheelStallIndicator ? "S":"")+" C: "+self.control+(self.motorMoving ? ") Go":") Stop"
		)

	def lowByte(self, arg):
		return arg&0xff

	def highByte(self, arg):
		return (arg>>8)&0xff

	def sync0(self):
		return self.submit([0x00])

	def sync1(self):
		return self.submit([0x01])

	def sync2(self):
		return self.submit([0x02])

	# Client pulse resets watchdog and prevents the robot from disconnecting
	def pulse(self):
		return self.submit([0x00])

	# Starts the controller
	def open(self):
		return self.submit([0x01])

	# Close client-server connection
	def close(self):
		return self.submit([0x02])

	# Set sonar polling sequence
	def polling(self, arg):
		return self.submit([0x03, 0x2b, len(arg)]+arg)

	# Enable/disable the motors
	def enable(self, arg):
		if arg:
			return self.submit([0x04, 0x3b, 0x01, 0x00])

		return self.submit([0x04, 0x3b, 0x00, 0x00])

	# Sets translation acc/deceleration; in mm/sec^2
	def seta(self, arg):
		if arg>=0:
			return self.submit([0x05, 0x3b, self.lowByte(arg), self.highByte(arg)])

		return self.submit([0x05, 0x1b, self.lowByte(-arg), self.highByte(-arg)])

	# Set maximum translation velocity; in mm/sec
	def setv(self, arg):
		return self.submit([0x06, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Resets server to 0,0,0 origin
	def seto(self):
		return self.submit([0x07])

	# Sets maximum rotational velocity; in degrees/sec
	def setrv(self, arg):
		return submit([0x0a, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Move forward (+) or reverse (-); in mm/sec
	def vel(self, arg):
		return self.submit(0x0b, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Turn to absolute heading; 0-359 degress
	def head(self, arg):
		return self.submit(0x0c, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Turn relative to current heading
	def dhead(self, arg):
		return self.submit([0x0d, 0x3b, self.lowByte(arg), self.highByte(arg)])

	def hostbaud(self, arg):
		return self.submit([0x32, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Sound duration (20 ms increments)/tone (half-cycle) pairs.
	def say(self, tones):
		return self.submit([0x0f, 0x2b, len(tones)]+tones)

	# Request configuration SIP
	def config(self, arg):
		return self.submit([0x13, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Request continuous (>0) or stop sending (=0) encoder SIPs
	def encoder(self, arg):
		return self.submit([0x14, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Rotate at +/- degrees/sec
	def rvel(self, arg):
		if arg>=0:
			return self.submit([0x15, 0x3b, self.lowByte(arg), self.highByte(arg)])

		return self.submit([0x15, 0x1b, self.lowByte(-arg), self.highByte(-arg)])

	# Colbert relative heading setpoint; +/- degrees
	def dchead(self, arg)
		return self.submit([0x16, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Sets rotational (+/-)de/acceleration; in mm/sec^2
	def setra(self, arg):
		return submit([0x17, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Enable/disable the sonars
	def sonar(self, arg):
		if arg:
			return self.submit([0x1c, 0x3b, 0x01, 0x00])

		return self.submit([0x1c, 0x3b, 0x00, 0x00])

	# Stops the robot (motors remain enabled)
	def stop(self):
		return self.submit([0x1d])

	# msbits is a byte mask that selects output port(s) for changes; lsbits set (1) or reset (0) the selected port
	def digout(self, arg):
		return self.submit([0x1e, 0x3b, self.lowByte(arg), self.highByte(arg)])

	def vel2(self, left, right):
		l=left
		r=right
		if l<0:
			l=256+l
		if r<0:
			r=256+r

		return self.vel2(l<<8|r)

	# Independent wheel velocities; lsb=right wheel; msb=left wheel; PSOS is in +/- 4mm/sec; POS/AmigOS is in 2 cm/sec increments
	def vel2(self, arg):
		return self.submit([0x20, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Pioneer Gripper server command.  see the Pioneer Gripper manuals for details
	def gripper(self, arg)
		return self.submit([0x21, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Select the A/D port number for analog value in SIP.  selected port reported in SIP timer value
	def adsel(self, arg):
		return self.submit([0x23, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Pioneer Gripper server value.  see P2 Gripper manual for details
	def gripperval(self, arg):
		return self.submit([0x24, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# msb is the port number (1-4) and lsb is the pulse width in 100 microsec units PSOS or 10 microsec units P2OS
	def ptupos(self, arg):
		return self.submit([0x29, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Send string argument to serial device connected to AUX port on microcontroller
	def tty2(self, arg):
		return self.submit([0x2a, 0x2b, len(arg)]+arg)

	# Request to retrieve 1-200 bytes from the aux serial channel; 0 flusshes the AUX serial input buffer
	def getaux(self, arg):
		return self.submit([0x2b, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Stop and register a stall in front (1), rear (2) or either (3) bump-ring contacted. Off (default) is 0
	def bumpstall(self, arg):
		return self.submit([0x2c, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# TCM2 module commands; see P2 TCM2 manual for details
	def tcm2(self, arg):
		return self.submit([0x2d, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Emergency stop, overrides deceleration
	def e_stop(self):
		return self.submit([0x37])

	# Single-step mode (simulator only)
	def step(self):
		return self.submit(0x40])

	# Play stored sound
	def sound(self, arg):
		return self.submit([0x5a, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Request playlist packet for sound number or 0 for all user sounds
	def playlist(self, arg):
		return self.submit([0x5b, 0x3b, self.lowByte(arg), self.highByte(arg)])

	# Mute (0) or enable (1) sounds
	def soundtog(self, arg):
		return self.submit(0x5c, 0x3b, self.lowByte(arg), self.highByte(arg)])

	def readPacket(self):
		temp=[0x00]*3

		self.lock.acquire()

		if self.instream.available()>3:
			self.readBytes(temp, 0, 3)
			self.lock.release()

			if temp[0]==0xfa and temp[1]==0xfb: # If header is ok
				length=0
				if temp[2]<0:
					length=temp[2]+256
				else:
					length=temp[2]

				data=[0x00]*length
				self.readBytes(data, 0, length) # Read rest of packet

				# Is checksum correct?
				forchecksum=data+[0x00, 0x00]

				packetChecksum=data[-2]
				if packetChecksum<0:
					packetChecksum+=256

				packetChecksum*=256;
				packetChecksum+=data[-1]
				if packetChecksum<0:
					packetChecksum+=256

				# If checksum is correct, interpret data
				if self.checksum(forchecksum)==packetChecksum&0xffff:
					ss=""

					for i in range(len(data)):
						if data[i]>=0
							ss+=" "+data[i]
						else:
							ss+=" "+(data[i]+256)

					# Read motor status
					if data[0]==0x32:
						self.motorMoving=False
					elif data[0]==0x33:
						self.motorMoving=True
					else:
						print "Invalid motor status: "+data[0]

					alpha=0

					# Read xpos
					if data[2]>=0:
						alpha=data[2]
					else:
						alpha=data[2]+256

					alpha&=0x7f
					alpha*=256
					if data[1]>=0:
						alpha+=data[1]
					else:
						alpha+=data[1]+256

					if alpha>=16384:
						alpha-=32768

					self.xpos=alpha*0.5083                         

					# Read ypos
					if data[4]>=0:
						alpha=data[4]
					else:
						alpha=data[4]+256

					alpha&=0x7f
					alpha*=256

					if data[3]>=0:
						alpha+=data[3]
					else:
						alpha+=data[3]+256

					if alpha>=16384:
						alpha-=32768

					self.ypos=alpha*0.5083

					# Read orientation
					if data[6]>=0:
						alpha=data[6]
					else:
						alpha=data[6]+256

					alpha*=256
					if data[5]>=0:
						alpha+=data[5]
					else:
						alpha+=data[5]+256

					self.orientation=alpha*0.001534

					# Read velocity of left wheel
					if data[8]>=0:
						alpha=data[8]
					else:
						alpha=data[8]+256

					alpha*=256
					if data[7]>=0:
						alpha+=data[7]
					else:
						alpha+=data[7]+256

					self.leftWheelVelocity=alpha*0.6154

					# Read velocity of right wheel
					if data[10]>=0:
						alpha=data[10]
					else:
						alpha=data[10]+256

					alpha*=256
					if data[9]>=0:
						alpha+=data[9]
					else:
						alpha+=data[9]+256

					self.rightWheelVelocity=alpha*0.6154

					# Read battery status
					if data[11]>=0:
						battery=data[11]
					else:
						battery=data[11]+256

					# Read bumpers
					self.leftWheelStallIndicator=(data[12]&0x80)!=0
					self.rightWheelStallIndicator=(data[13]&0x80)!=0

					# Read control
					if data[15]>=0:
						alpha=data[15]
					else:
						alpha=data[15]+256

					alpha*=256
					if data[14]>=0:
						alpha+=data[14]
					else:
						alpha+=data[14]+256

					self.control=alpha*0.001534

					# Read PTU
					# Not implemented

					# Read Compass
					# Not implemented

					# Read Sonar readings
					nr=data[19]

					for i in range(nr):
						n_sonar=data[20+3*i]
						if data[20+3*i+2]>=0:
							alpha=data[20+3*i+2]
						else:
							alpha=data[20+3*i+2]+256

						alpha*=256

						if data[20+3*i+1]>=0:
							alpha+=data[20+3*i+1]
						else:
							alpha+=data[20+3*i+1]+256

						sonars[n_sonar]=alpha*0.555 # Calculate sonar reading

					return True

				elif self.isVerbose():
					print "Bad checksum, got "+(packetChecksum&0xffff)+", expected "+self.checksum(forchecksum)+"."
					print "Bad packet was: "
					self.printPacket(temp, data)
			else if self.isVerbose()):
				print "Bad header, got "
				self.printPacket(temp, None)

		return False

	# Just in case
	def finalize(self):
		self.lock.acquire()
		self.disconnect()
		self.lock.release()

	# Shuts down the communication to the robot
	def disconnect(self):
		self.working=False

		try:
			if self.beatThread!=None:
				self.beatThread.join()
		except InterruptedException, e:
			pass

		try:
			if self.readInfoThread!=None:
				self.readInfoThread.join()
		except InterruptedException, e:
			pass

		self.working=True # For next time

		# Stop sonars
		self.sonar(False)
		# Stop motor
		self.enable(False)

		if self.isVerbose():
			print "Disconecting"

		self.close()

		# Important: Disconnect the socket first if we're using IBM's timeout
		# input stream -- else it hangs forever on close.

		try:
			if self.sock!=None:
				self.sock.close()
		except IOError, e:
			pass

		self.sock=None

		try:
			if self.instream!=None:
				self.instream.close()
		except IOError, e:
			pass

		self.instream=None

		try:
			if self.outstream!=None:
				self.outstream.close()
		except IOError, e:
			pass

		self.outstream=None
