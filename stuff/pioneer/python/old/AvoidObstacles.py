# Pythonized by Andrew Pennebaker

__license__="BSD"

import time, math, sys

from Robot import Robot

# EVERY_TIME and every are not used in main().

EVERY_TIME=0

def every(sec):
	global EVERY_TIME

	cur=time.time()
	if EVERY_TIME+sec<cur:
		EVERY_TIME=cur
		return True

	return False

def usage():
	print "Usage: "+sys.argv[0]+" <port>"
	sys.exit()

def main():
	# Degrees to radians
	angles=[
		d*math.pi/180.0 for d in [
			-90.0, -50.0, -30.0, -10.0,
			10.0, 30.0, 50.0, 90.0,
			90.0, 130.0, 150.0, 170.0,
			-170.0, -150.0, -130.0, -90.0
		]
	]

	host, port="localhost", 4000

	if len(sys.argv)<2:
		usage()
	try:
		port=int(sys.argv[1])
	except NumberFormatException, e:
		usage()

	robot=Robot()
	robot.setVerbose(True)
	robot.connect(host, port))
	robot.sonar(True)
	robot.enable(True)

	fx=0.0
	fy=0.0
	strength=0.0

	while True:
		# gather angle
		# Equation is: += 1/Sqrt(dist) * unit in direction

		x, y=0.0, 0.0
		mag=100000.0

		sonars=robot.getSonars()

		for i in range(len(sonars))
			if sonars[i]<mag:
				mag=sonars[i]
				x=math.cos(angles[i])
				y=math.sin(angles[i])

		alpha=0.5

		if mag>2000.0:
			mag=2000.0 # we don't care about bigger
			x, y=fx, fy

		mag=2000.0-mag

		strength=(1.0-alpha)*strength+alpha*mag
		fx=(1.0-alpha)*fx+alpha*x
		fy=(1.0-alpha)*fy+alpha*y

		# Other vector
		tx=0.0

		# Now mix in some x
		FORWARD=2.0

		heading=math.atan2(fy, fx)*180.0/math.pi

		if heading<90.0 and heading>-90.0: # Back away
			tx=-strength/100.0
			tx*=(90.0-abs(heading))/90.0
		else: # Move forward
			heading+=180.0
			if heading>180.0
				heading-=360.0

			tx=strength/100.0
			tx*=(90.0-math.abs(heading))/90.0

		heading/=5.0 # Cut-down

		robot.vel2(heading+tx, -heading+tx)
		time.sleep(10) # Milliseconds

if __name__=="__main__":
	main()
