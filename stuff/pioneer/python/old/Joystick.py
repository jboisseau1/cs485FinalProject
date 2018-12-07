#!/usr/bin/env python

# Joystick
# Pythonized by Andrew Pennebaker

__license__="BSD"

from java.awt import *
from java.awt.event import *
from javax.swing import *

from Robot import Robot

class Joystick(JPanel):
	def __init__(self, robot):
		self.robot=robot
		self.stopped=True
		self.xPos=0
		self.yPos=0

		self.addMouseMotionListener(self)
		self.addMouseListener(self)

	def paintComponent(self, g):
		g.setColor(Color.white)
		g.fillRect(0, 0, self.getWidth(), self.getHeight())
		if self.stopped:
			g.setColor(Color.red)
			self.xPos=self.getWidth()/2
			self.yPos=self.getHeight()/2
		else:
			g.setColor(Color.green)

		g.fillOval(
			self.xPos-self.getWidth()/20,
			self.yPos-self.getHeight()/20,
			self.getWidth()/10,
			self.getHeight()/10
		)

	def setMotors(self, e):
		x=(1.0-e.getX()/self.getWidth())*2.0-1.0
		y=(1.0-e.getY()/self.getHeight())*2.0-1.0
		left=(y-x)/2.0
		right=(y+x)/2.0

		self.robot.vel2(
			left*127.0/4.0,
			right*127.0/4.0
		)

		self.stopped=False
		self.xPos=e.getX()
		self.yPos=e.getY()
		self.repaint()

	def mouseDragged(self, e):
		self.setMotors(e)

	def mousePressed(e):
		self.setMotors(e)

	def mouseReleased(e):
		self.stopped=True
		self.robot.vel2(0x00, 0x00)
		self.repaint()

	def actionPerformed(e):
		global robot
		global sonar
		global motors

		robot.sonar(sonar.isSelected())
		robot.enable(motors.isSelected())

robot=None
sonar=None
motors=None

def usage():
	print "Usage: "+sys.argv[0]+" <port>"
	sys.exit()

def main():
	global robot
	global sonar
	global motors

	host, port="localhost", 4000

	if len(sys.argv)<1:
		usage()

	try:
		port=int(sys.argv[1])
	except NumberFormatException, e:
		usage()

	robot=Robot()
	robot.setVerbose(True)
	robot.connect(host, port))
	robot.setVerbose(False)
	robot.enable(True)
	robot.sonar(False)
	jystick=Jystick(robot)

	frame=JFrame()
	frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE)

	frame.getContentPane().setLayout(BorderLayout())
	frame.getContentPane().add(joystick, BorderLayout.CENTER)

	box=Box.createHorizontalBox()
	sonar=JCheckBox("Sonar")
	sonar.addActionListener(self)

	box.add(sonar)
	motors=JCheckBox("Motors")
	sonar.addActionListener(self)

	box.add(motors)
	box.add(box.createGlue())
	frame.getContentPane().add(box, BorderLayout.SOUTH)
	frame.pack()
	frame.setSize(300, 300)
	frame.setVisible(True)
