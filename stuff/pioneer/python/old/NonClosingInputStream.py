# NonClosingInputStream.py
# Pythonized by Andrew Pennebaker

__license__="BSD"

class NonClosingInputStream(FilterInputStream):
	def __init__(self, instream):
		self.instream=instream

	# Do not close the underlying stream
	def close(self):
		self.instream=None
