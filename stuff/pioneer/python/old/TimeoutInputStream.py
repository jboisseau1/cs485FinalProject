# Pythonized by Andrew Pennebaker

__license__="BSD"

import threading

# Wraps an input stream that blocks indefinitely to simulate timeouts on read(),
# skip(), and close().  The resulting input stream is buffered and supports
# retrying operations that failed due to an InterruptedIOException.

# Supports resuming partially completed operations after an InterruptedIOException
# REGARDLESS of whether the underlying stream does unless the underlying stream itself
# generates InterruptedIOExceptions in which case it must also support resuming.
# Check the bytesTransferred field to determine how much of the operation completed;
# conversely, at what point to resume.

# Note: This class/interface is part of an interim API that is still under 
# development and expected to change significantly before reaching stability. 
# It is being made available at this early stage to solicit feedback from pioneering 
# adopters on the understanding that any code that uses this API will almost 
# certainly be broken (repeatedly) as the API evolves.

class TimeoutInputStream:
	# Unsynchronized variables
	readTimeout=0 # read() timeout in millis
	closeTimeout=0 # close() timeout in millis, or -1

	# Requests for the thread (synchronized)
	closeRequested=False # If True, close requested

	# Responses from the thread (synchronized)
	thread=None # If None, thread has terminated
	iobuffer=[] # Circular buffer
	head=0 # Points to first unread byte
	length=0 # Number of remaining unread bytes
	ioe=None # If not None, contains a pending exception
	waitingForClose=False # If True, thread is waiting for close()
	growWhenFull=False # If true, buffer will grow when it is full

	# Creates a timeout wrapper for an input stream.
	# @param in the underlying input stream
	# @param bufferSize the buffer size in bytes; should be large enough to mitigate
	#	Thread synchronization and context switching overhead
	# @param readTimeout the number of milliseconds to block for a read() or skip() before
	#	throwing an InterruptedIOException; 0 blocks indefinitely
	# @param closeTimeout the number of milliseconds to block for a close() before throwing
	#	an InterruptedIOException; 0 blocks indefinitely, -1 closes the stream in the background

	def __init__(self, instream, bufferSize, readTimeout, closeTimeout, growWhenFull=False):		self.readTimeout=readTimeout
		self.closeTimeout=closeTimeout
		self.iobuffer=[0x00]*bufferSize
		self.thread=Thread(
			Runnable():
				def run():
					self.runThread()
		)

		self.thread.setDaemon(True)
		self.thread.start()

		self.growWhenFull=growWhenFull

		self.lock=threading.Lock()

	# Wraps the underlying stream's method.
	# It may be important to wait for a stream to actually be closed because it
	# holds an implicit lock on a system resoure (such as a file) while it is
	# open.  Closing a stream may take time if the underlying stream is still
	# servicing a previous request.
	# @throws InterruptedIOException if the timeout expired
	# @throws IOException if an i/o error occurs

	def close(self):
		oldThread=None

		self.lock.acquire()

		if self.thread==None:
			return
		else:
			oldThread=self.thread
			closeRequested=True
			self.thread.interrupt()
			self.checkError()

		if self.closeTimeout==-1:
			return

		try:
			oldThread.join(self.closeTimeout)
		except InterruptedException, e:
			pass # We were not expecting to be interrupted

		self.checkError()
		threadIsNone=self.thread==None

		self.lock.release()

		if threadIsNone:
			raise InterruptedIOException()

	# Returns the number of unread bytes in the buffer.
	# @throws IOError if an i/o error occurs

	def available(self):
		self.lock.acquire()

		if self.length==0:
			self.checkError()

		m=max(self.length, 0)
		self.lock.release()

		return m
        
	# Reads a byte from the stream.
	# @throws InterruptedIOException if the timeout expired and no data was received,
	#	bytesTransferred will be zero
	# @throws IOError if an i/o error occurs

	def sread(self):
		self.lock.acquire()

		if not self.syncFill():
			return -1 # EOF reached

		b=self.iobuffer[self.head]&0xff
		self.head+=1

		if self.head==len(self.iobuffer):
			self.head=0

		self.length-=1

		self.lock.release()

		return b

	# Reads multiple bytes from the stream.
	# @throws InterruptedIOException if the timeout expired and no data was received,
	#	bytesTransferred will be zero
	# @throws IOError if an i/o error occurs

	def read(self, buf, offset, length):
		self.lock.acquire()

		if not self.syncFill():
			return -1 # EOF reached

		pos=offset
		length=min(length, self.length)

		while length>0:
			buf[pos]=self.iobuffer[head]
			pos+=1
			self.head+=1

			if self.head==len(self.iobuffer):
				self.head=0

			self.length-=1

		self.lock.release()

		return pos-offset

	# Skips multiple bytes in the stream.
	# @throws InterruptedIOException if the timeout expired before all of the
	#	bytes specified have been skipped, bytesTransferred may be non-zero
	# @throws IOError if an i/o error occurs

	def skip(self, count):
		self.lock.acquire()

		amount=0
		try:
			while amount<count:
				if not self.syncFill():
					break # EOF reached

				skip=min(count-amount, self.length)
				self.head=(self.head+skip)%len(self.iobuffer)
				self.length-=skip
				amount+=skip

			self.lock.release()
		except InterruptedIOException, e:
			self.lock.release()
			e.bytesTransferred=amount # assumes amount < Integer.MAX_INT
			raise e

		return amount

	# Mark is not supported by the wrapper even if the underlying stream does, returns false.

	def markSupported():
		return False

	# Waits for the buffer to fill if it is empty and the stream has not reached EOF.
	# @return true if bytes are available, false if EOF has been reached
	# @throws InterruptedIOException if EOF not reached but no bytes are available

	def syncFill(self):
		if self.length!=0:
			return True

		self.checkError() # Check errors only after we have read all remaining bytes

		if self.waitingForClose:
			return False

		try:
			self.wait(self.readTimeout)
		except InterruptedException, e:
			Thread.currentThread().interrupt() # We were not expecting to be interrupted

		if self.length!=0:
			return True

		self.checkError() # Check errors only after we have read all remaining bytes

		if self.waitingForClose:
			return False

		raise InterruptedIOException()

	# If an exception is pending, throws it.
	def checkError(self):
		if self.ioe!=None:
			e=self.ioe
			self.ioe=None
            raise e

	# Runs the thread in the background.
	def runThread(self):
		try:
			self.readUntilDone()
		except IOError, e:
			self.lock.acquire()
			self.ioe=e
			self.lock.release()
		finally:
			self.waitUntilClosed()
			try:
				self.instream.close()
			except IOError, e:
				self.lock.acquire()
				self.ioe=e
				self.lock.release()
			finally:
				self.lock.acquire()
				self.thread=None
				self.lock.release()

	# Waits until we have been requested to close the stream.

	def waitUntilClosed(self):
		self.lock.acquire()

		self.waitingForClose=True

		while not self.closeRequested:
			try:
				self.wait()
			except InterruptedException, e:
				self.closeRequested=True # Alternate quit signal

		self.lock.release()

	# Reads bytes into the buffer until EOF, closed, or error.
	def readUntilDone(self):
		while True:
			offset, length=0, 0

			self.lock.acquire()

			while self.isBufferFull():
				if self.closeRequested:
					return # Quit signal
				self.waitForRead()
			offset=(self.head+self.length)%len(self.iobuffer)

			length=self.head
			if self.head<=offset:
				length=len(self.iobuffer)-offset

			self.lock.release()

			count=0

			try:
				# The I/O operation might block without releasing the lock,
				# so we do this outside of the synchronized block.
				count=self.instream.read(self.iobuffer, offset, length)
				if count==-1:
					return # EOF encountered
			except InterruptedIOException, e:
				count=e.bytesTransferred # Keep partial transfer

			self.lock.acquire()
			self.length+=count
			self.lock.release()

	# Wait for a read when the buffer is full (with the implication that
	# space will become available in the buffer after the read takes place).

	def waitForRead(self):
		self.lock.acquire()

		try:
			if self.growWhenFull:
				# Wait a second before growing to let reads catch up
				self.wait(self.readTimeout)
			else:
				self.wait()
		except InterruptedException, e:
			self.closeRequested=True # Alternate quit signal

		# If the buffer is still full, give it a chance to grow
		if self.growWhenFull and self.isBufferFull():
			self.growBuffer()

		self.lock.release()

	def growBuffer(self):
		self.lock.acquire()

		newSize=2*len(self.iobuffer)
		if newSize>len(self.iobuffer):
#			if Policy.DEBUG_STREAMS:
#				print "InputStream growing to "+newSize+" bytes" # $NON-NLS-1$ # $NON-NLS-2$

			newBuffer=[0x00]*newSize
			pos=0
			length=self.length
			while length>0:
				newBuffer[pos]=self.iobuffer[head]
				pos+=1
				self.head+=1

				if self.head==len(self.iobuffer):
					self.head=0

			self.iobuffer=newBuffer
			self.head=0
			# length instance variable was not changed by this method
			self.length-=1

		self.lock.release()

	def isBufferFull(self):
		return self.length==len(self.iobuffer)
