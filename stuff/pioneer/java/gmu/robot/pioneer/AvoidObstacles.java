/*
 * Example.java
 */
package gmu.robot.pioneer;
//javac ./AvoidObstacles.java ./PioneerRobot.java ./TimeoutInputStream.java ./NonClosingInputStream.java
public class AvoidObstacles
    {
    
	
		static double goalX = 8000;
		static double goalY = 0;
		static double posX = 0;
		static double posY = 0;
		static double posH = 0;
	
    static long every_time = 0;
    public static boolean every(long millis)
        {
        long cur = System.currentTimeMillis();
        if (every_time + millis < cur)
            {
            every_time = cur;
            return true;
            }
        else return false;
        }

    public static void usage()
        {
			System.out.println("Usage: AvoidObstacles <port>");
			System.exit(0);
        }
        
    public static double min(double a, double b, double c)
        {
			if (a < b && a < c) return a;
			if (b < c) return b; 
			return c;
        }
        
    public static boolean goAway(double[] sonars, PioneerRobot pioneer)
        {
			//get position and orientation
			posX = pioneer.getXPos();
			posY = pioneer.getYPos();
			posH = pioneer.getOrientation();
			
			//move back 5 cm
			pioneer.move((short)-50);
			
			//rotate right
			//convert to deg
			double deg = 180/Math.PI * posH;
			//add to 90 deg
			deg += 90;
			pioneer.head((short)deg);
			
			//move forward alittle
			pioneer.move((short)50);
			
			return true;
			
        }

    public static boolean goToGoal(double[] sonars,  PioneerRobot pioneer)
        {
			posX = pioneer.getXPos();
			posY = pioneer.getYPos();
			posH = pioneer.getOrientation();
			
			double vectX = goalX - posX;
			double vectY = goalY - posY;
			
			double mag = Math.sqrt((vectX*vectX)+(vectY*vectY));
			double unitX = vectX / mag;
			double unitY = vectY / mag;
			
			short deg = (short)Math.toDegrees(Math.atan((goalY-posY)/(goalX-posX)));
			
			//turn to goal
			pioneer.head((short)deg);
			
			//move forward
			pioneer.move((short)20);
			
			
			return true;
        }

   
        
    public static void main( String[] args ) throws Exception
        {  
	
		
        if (args.length<1)
            usage();

        PioneerRobot robot = new PioneerRobot();
        robot.setVerbose(true);
        robot.connect("localhost", Integer.parseInt(args[0]));
        robot.sonar( true );
        robot.enable( true );

		boolean going = true;
		
        while(going)
        {
            double[] sonars = robot.getSonars();
            
			//if there is a obsticle in the front
            if (robot.getSonar(1) < 350 || robot.getSonar(2) < 350 || robot.getSonar(3) < 350 || robot.getSonar(4) < 350 || robot.getSonar(5) < 450 || robot.getSonar(6) < 350) {
				System.out.println("*************************************************************************");
				System.out.println("Go AWAY X:"+robot.getXPos()+ " Y:"+robot.getYPos());
				for(int i = 0; i<16; i++){
					System.out.print(" "+i +": "+sonars[i]);
				}
				System.out.println("\n");
				goAway( sonars, robot);
			}
			//if reached the goal
			else if(Math.abs(posX - goalX) < 80 && Math.abs(posY- goalY) < 80){
				System.out.println("DONE! X:"+robot.getXPos()+ " Y:"+robot.getYPos());
				going = false;
				robot.vel2((byte)-100,(byte)100);
				robot.stop();
			}
            else // forward
            {
				System.out.println("Moving Forward X:"+robot.getXPos()+ " Y:"+robot.getYPos());
				for(int i = 0; i<16; i++){
					System.out.print(" "+i +": "+sonars[i]);
				}
				System.out.println("\n");
                goToGoal(sonars, robot);
            }
			
			Thread.sleep(200);
			
        }
        
        }
    }