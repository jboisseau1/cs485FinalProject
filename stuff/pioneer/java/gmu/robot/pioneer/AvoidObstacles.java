/*
 * Written by:
 *
 *    Jacob Boisseau
 *    Cody Kidwell
 *    Jimmy Prohaska
 *    Pablo Turriago-Lopez
 *    Lorenzo Zamora
 *
 *    Bug 0 for the Pioneer 3 Robot. Uses the robot's sonar sensors to detect obstacles.
 *
 */

package gmu.robot.pioneer;
//javac ./AvoidObstacles.java ./PioneerRobot.java ./TimeoutInputStream.java ./NonClosingInputStream.java
public class AvoidObstacles{

  //goal destination
  static double goalX = 2500;
  static double goalY = 0;

  //robot position and orientation
  static double posX = 0;
  static double posY = 0;
  static double posH = 0;

  //prints port
  public static void usage(){
          System.out.println("Usage: AvoidObstacles <port>");
          System.exit(0);
  }

  //used when the robot detects an obsticle
  public static boolean goAway(double[] sonars, PioneerRobot pioneer){
          //get orientation
          posH = pioneer.getOrientation();

          //move back 5 cm
          pioneer.move((short)-50);

          //rotate right
          //convert to deg
          double deg = 180/Math.PI * posH;

          //add to 90 deg
          deg += 90;

          //set heading
          pioneer.head((short)deg);

          //move forward alittle
          pioneer.move((short)50);

          //operation successful
          return true;
  }

  //used when the robot is free to move towards the goal
  public static boolean goToGoal(double[] sonars,  PioneerRobot pioneer){
          //get pos and orientation
          posX = pioneer.getXPos();
          posY = pioneer.getYPos();
          posH = pioneer.getOrientation();

          //get vectors
          double vectX = goalX - posX;
          double vectY = goalY - posY;

          //get unit vector
          double mag = Math.sqrt((vectX*vectX)+(vectY*vectY));
          double unitX = vectX / mag;
          double unitY = vectY / mag;

          //get degree to goal
          short deg = (short)Math.toDegrees(Math.atan((goalY-posY)/(goalX-posX)));

          //turn to goal
          pioneer.head((short)deg);

          //move forward
          pioneer.move((short)30);

          //return true if successful
          return true;
  }

  //main method, controls the logic of the continous loop
  public static void main( String[] args ) throws Exception{

          //checks arguments
          if (args.length<1)
                  usage();

          //create new PeioneerRobot
          PioneerRobot robot = new PioneerRobot();
          robot.setVerbose(true);
          robot.connect("localhost", Integer.parseInt(args[0]));

          //enable sonar
          robot.sonar( true );

          //enable the robot
          robot.enable( true );

          //tracks if the robot is still going to the goal
          boolean going = true;

          //loops continously until the robot has reached the goal
          while(going){

                  //get sonar sensors
                  double[] sonars = robot.getSonars();

                  //if there is a obsticle in the front
                  if (robot.getSonar(1) < 350 || robot.getSonar(2) < 350 || robot.getSonar(3) < 350 || robot.getSonar(4) < 350 || robot.getSonar(5) < 450 || robot.getSonar(6) < 350) {

                          //debug statements, shows robot state and x and y pos
                          System.out.println("*************************************************************************");
                          System.out.println("Go AWAY X:"+robot.getXPos()+ " Y:"+robot.getYPos());

                          //print the sensors
                          for(int i = 0; i<16; i++) {
                                  System.out.print(" "+i +": "+sonars[i]);
                          }
                          System.out.println("\n");

                          //call go away
                          goAway( sonars, robot);
                  }

                  //if reached the goal
                  else if(Math.abs(posX - goalX) < 80 && Math.abs(posY- goalY) < 80) {

                          //debug statements, shows robot state and x and y pos
                          System.out.println("DONE! X:"+robot.getXPos()+ " Y:"+robot.getYPos());

                          //reached goal and no longer needs to move
                          going = false;

                          //stop the robot
                          robot.stop();
                  }

                  //move forward
                  else{
                          //debug statements, shows robot state and x and y pos
                          System.out.println("*************************************************************************");
                          System.out.println("Moving Forward X:"+robot.getXPos()+ " Y:"+robot.getYPos());

                          //print the sensors
                          for(int i = 0; i<16; i++) {
                                  System.out.print(" "+i +": "+sonars[i]);
                          }
                          System.out.println("\n");

                          //call to the goToGoal function
                          goToGoal(sonars, robot);
                  }

                  //sleep, so the robot has a small delay between steps
                  Thread.sleep(50);

          }

  }
}
