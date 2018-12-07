# Example

# Pythonized by Andrew Pennebaker

__license__="BSD"

# This is the sample program for controlling an AmigoBot robot.
# Copyright (c) 2001 Evolutinary Computation Laboratory @ George Mason University
# author Liviu Panait
# version 1.0

import sys, getopt, time

from Robot import Robot

def usage():
	print "Usage: "+sys.argv[0]+" <port>"

def main():
	host, port="localhost", 4000
	if len(sys.argv)>1:
		try:
			port=int(sys.argv[2])
		except NumberFormatException, e:
			usage()

	robot=Robot()
	robot.connect(host, port)
	robot.sonar(True)
	robot.enable(True)

	for i in range(10):
		robot.sound(i)
		robot.dhead(
			(60*(2*(i%2)-1))
		)
		time.sleep(1)

	robot.disconnect()

if __name__=="__main__":
	main()
