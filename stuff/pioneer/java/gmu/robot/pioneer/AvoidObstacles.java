/*
 * Example.java
 */

package gmu.robot.pioneer;

public class AvoidObstacles
    {
    
	
		double goalX = 0;
		double goalY = 1000;
		double posX = 0;
		double posY = 0;
		double posH = 0;
	
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
        
    public static boolean goAway(double[] sonars, int numSonars, PioneerRobot robot)
        {
			//get position and orientation
			posX = pioneer.getXPos();
			posY = pioneer.getYPos();
			posH = pioneer.getOrientation();
			
			//move back 5 cm
			pioneer.move(-50);
			
			//rotate right
			//convert to deg
			double deg = 180/Math.PI * posH;
			//add to 90 deg
			deg += 90;
			pioneer.head(deg);
			
			//move forward alittle
			pioneer.move(20);
			
        }

    public static boolean goToGoal(double[] sonars, int numSonars, PioneerRobot robot)
        {
			posX = pioneer.getXPos();
			posY = pioneer.getYPos();
			posH = pioneer.getOrientation();
			
			double vectX = goalX - robotX;
			double vectY = goalY - robotY;
			
			double mag = Math.sqrt((vectX*vectX)+(vectY*vectY));
			double unitX = vectX / mag;
			double unitY = vectY / mag;
			
			short deg = Math.toDegrees(Math.atan((goalY-posY)/(goalX-posX)));
			
			//turn to goal
			pioneer.head(deg);
			
			//move forward
			pioneer.move(20);
			
			
			return true;
        }

    public static final double[] angles = {-90,-50,-30,-10,10,30,50,90,90,130,150,170,-170,-150,-130,-90};
        
    public static void main( String[] args ) throws Exception
        {  
	
		
        if (args.length<1)
            usage();

        for(int x=0;x<angles.length;x++)
            angles[x] = (angles[x] / 180) * Math.PI;

        PioneerRobot robot = new PioneerRobot();
        robot.setVerbose(true);
        robot.connect("localhost", Integer.parseInt(args[0]));
        robot.sonar( true );
        robot.enable( true );
        int lastDirection = 0;
        double scale = 1.5;

        while(true)
            {
            double[] sonars = robot.getSonars();
            int numSonars = (sonars.length < 16 ? sonars.length: 16);
            int sum = 0;
            double min = 10000;
            for(int i = 0; i < numSonars; i++) 
            { 
				sum += sonars[i];
				min = Math.min(min, sonars[i]);
			}
                                
            if (sum > 0)
            {
				//if there is a obsticle in the front
                if (sonars[3] < 80 || sonars[4] < 80) {
					System.err.println("Go AWAY");
					goAway( sonars, numSonars, robot);
				}
				//if reached the goal
				else if(Math.abs(posX - goalX) < 80 && Math.abs(posY- goalY) < 80){
					break;
				}
                else // forward
                {
					System.err.println("Go AWAY");
                    goToGoal(sonars, numSonars, pioneer);
                }
            }
            Thread.sleep(10);
            }
        }
    }