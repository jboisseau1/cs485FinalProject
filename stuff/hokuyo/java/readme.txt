See the API specification in the api/ folder.

Run the serialdaemon like this:

$ ./serialdaemon -serial /dev/ttyACM0 -port 1701 -baud 115200 -debug

Then, run the demo like this:

$ java -cp jars/SCIP.jar:. com.brianziman.robotics.LaserVisualizer 1701

