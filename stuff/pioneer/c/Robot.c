#include<malloc.h>
#include<stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <string.h>
#include <unistd.h>
#include <pthread.h>
#include <time.h>

/** 
  General class for controlling Activmedia (MobileRobots) Pioneer and Amigobot robots.
  <br>Copyright 2007 by Sean Luke, Liviu Panait, and George Mason University.
  <br>Released under the Academic Free License, version 3.0, which is at the end of 
  this file.

  <p> This is a C-port of the Robot.java file

  <p> This implements much of the basic API used in Pioneer robots.  Consult 
  the Pioneer manual for more details.  You connect to the robot via this class
  in the following way:

  <p><ol>
    initialize();
    connectToBot("localhost", 3500);
    setupBotConnection();  
  <li>Start the serialdaemon
  <li>Call initialize in Robot.c
  <li>Call connecttoBot with the serialdaemon connection information.
  <li>Call setupBotConnection to open the connection to the Pioneer
  </ol>

  <p>Once you have followed the steps above,   
  you  can start sending commands to the robot, and are (soon) able to receive 
  sensor data from the robot.  The class is generally silent unless you turn on 
  verbose mode, at which point you get a bunch  of sensor packet data back, as well
  as connection and error messages.  You disconnect the robot  with destroy(). 
  

  <p>We're finding a bug in Linux boxes using the Prolific USB->Serial device;
  often packets are  off by 3 in their sensor data.  As a (horrific) workaround, 
  you an make the checksum lenient by that much.

  <p> NOTE:  Currently this code is only partially working.  It is not able to 
  enable the motors.  The connection is established successfully, and the sonars
  are able to be controlled (on and off, etc), but the motors cannot be enabled
  for some reason.
  */


typedef enum _boolean { false, true } boolean;

const double YPOS_CONVERSION = 0.5083;
const double XPOS_CONVERSION = 0.5083;
const double THPOS_CONVERSION = 0.001534;
const double LVEL_CONVERSION = 0.6154;
const double RVEL_CONVERSION = 0.6154;
const double CONTROL_CONVERSION = 0.001534;
const double SONAR_CONVERSION = 0.555;

// VERBOSITY.  Is the driver chatty about what's going on?
boolean verbose = false;

// data from robot
boolean validData = false;
boolean motorEngaged;
double xpos, ypos; // in millimeters
double orientation; // in radians
double leftWheelVelocity; // in mm/sec
double rightWheelVelocity; // in mm/sec
int battery;
boolean leftWheelStallIndicator;
boolean rightWheelStallIndicator;
double control; // server's angular position servo
double* sonars;
int sonarsSize;
double* _sonars;  // temporary usage
int _sonarsSize;
int* lastSonarTime;
int lastSonarTimeSize;
int* _lastSonarTime; // temporary usage
int _lastSonarTimeSize;
int currentSonarTime = 0;
int sockfd = -1;
boolean working = true;

pthread_t readThread, beatThread;


void initialize()
{
    sonars = malloc(32 * sizeof(double)); // in mm
    sonarsSize = 32;
    if (sonars == NULL) 
    {
        /* Memory could not be allocated, so print an error and exit. */
        fprintf(stderr, "Couldn't allocate memory\n");
        exit(EXIT_FAILURE);
    }    

    _sonars = malloc(32 * sizeof(double)); // in mm
    _sonarsSize = 32;
    if (_sonars == NULL) 
    {
        /* Memory could not be allocated, so print an error and exit. */
        fprintf(stderr, "Couldn't allocate memory\n");
        exit(EXIT_FAILURE);
    }  

    lastSonarTime = malloc(32 * sizeof(int)); // in mm
    lastSonarTimeSize = 32;
    if (lastSonarTime == NULL) 
    {
        /* Memory could not be allocated, so print an error and exit. */
        fprintf(stderr, "Couldn't allocate memory\n");
        exit(EXIT_FAILURE);
    }  

    _lastSonarTime = malloc(32 * sizeof(int)); // in mm
    _lastSonarTimeSize = 32;
    if (_lastSonarTime == NULL) 
    {
        /* Memory could not be allocated, so print an error and exit. */
        fprintf(stderr, "Couldn't allocate memory\n");
        exit(EXIT_FAILURE);
    }      
}




/** Set the driver to verbosely print out state sensor results and motor requests. */
void setVerbose(boolean val) 
{ 
    verbose = val;
}

/** Get whether or not the driver is verbosely printing out state sensor results and motor requests. */
boolean isVerbose() 
{ 
    return verbose;
}

// checksum lenience -- helps deal with problems in linux serial usb drivers
boolean checksumLenient = true;

/** Set the driver to be lenient in permitting checksums off by 3 bytes -- 
 * which permits *some* usage of broken Linux serial drivers.
 * True by default. */
void setChecksumLenient(boolean val) 
{ 
    checksumLenient = val;
}

/** Returns whether or not the code permits checksums off by 3 bytes -- 
 * permits *some* usage of broken Linux serial drivers.  True by default. */
boolean isChecksumLenient() 
{
    return checksumLenient;
}

/** The communication packet header.  All Pioneer robot packets begin with 
 * this header. */
char HEADER[2] = { 0xFA, 0xFB };


/** Returns true if we've received at least one valid sensor packet from the 
 * robot so far.  Otherwise, all the sensor functions return garbage. */
boolean isValidData()
{
    return validData;
}

/** Returns true if the motor is engaged.  This information will not be valid 
 * until validData is true.  */
boolean isMotorEngaged()
{
    return motorEngaged;
}

/** Returns the X coordinate of the center of the robot, as far as the robot believes.
  This information will not be valid until validData is true.  
  The ARCOS documentation claims the raw value provided by the robot is in millimeters.
  However, this appears to be incorrect.  We are multiplying by XPOS_CONVERSION (0.5083)
  to get to millimeters -- it may have to do with the wheel diameter.  Help here 
  would be welcome.*/
double getXPos()
{
    return xpos;
}

/** Returns the Y coordinate of the center of the robot, as far as the robot believes.
  This information will not be valid until validData is true.  
  The ARCOS documentation claims the raw value provided by the robot is in millimeters.
  However, this appears to be incorrect.  We are multiplying by YPOS_CONVERSION (0.5083)
  to get to millimeters -- it may have to do with the wheel diameter.  Help here 
  would be welcome.*/
double getYPos()
{
    return ypos;
}

/** Returns the THETA (rotation) of the center of the robot, as far as the robot 
 * believes.  This information will not be valid until validData is true.  
 The ARCOS documentation claims the raw value provided by the robot is 'angular units',
 with 0.001534 radians per 'angular unit'.  So we are multiplying by 
 THPOS_CONVERSION (0.001534)
 to get to radians.  Help here would be welcome if this appears to be incorrect. */
double getOrientation()
{
    return orientation;
}

/** Returns the left wheel velocity in millimeters per second.
  This information will not be valid until validData is true.  
  The ARCOS documentation claims the raw value provided by the robot is in mm/sec.
  However, this appears to be incorrect.  We are multiplying by LVEL_CONVERSION (0.6154)
  to get to mm/sec -- it may have to do with the wheel diameter.  Help here would 
  be welcome.*/
double getLeftWheelVelocity()
{
    return leftWheelVelocity;
}

/** Returns the right wheel velocity in millimeters per second.
  This information will not be valid until validData is true.  
  The ARCOS documentation claims the raw value provided by the robot is in mm/sec.
  However, this appears to be incorrect.  We are multiplying by RVEL_CONVERSION (0.6154)
  to get to mm/sec -- it may have to do with the wheel diameter.  Help here would 
  be welcome.*/
double getRightWheelVelocity()
{
    return rightWheelVelocity;
}

/** Returns the battery charge in tenths of volts.  */
int getBatteryStatus()
{
    return battery;
}

/** Returns true if the left wheel is being blocked and thus unable to move.  */
boolean isLeftWheelStalled()
{
    return leftWheelStallIndicator;
}

/** Returns true if the right wheel is being blocked and thus unable to move.  */
boolean isRightWheelStalled()
{
    return rightWheelStallIndicator;
}

/** Returns the current value of the server's angular position servo, in degrees 
 * (unused on our robots).  */
double getControlServo()
{
    return control;
}

/** Returns the last value of sonar #index.  The ARCOS documentation claims that 
 * the value is in millimeters away from the sonar.  However, this appears to 
 * be incorrect.  We are multiplying by SONAR_CONVERSION (0.555) to get to millimeters.
 Note that sonars come in different batches, so you may not receive all of them
 in one time tick.  Looking at the front of the robot, sonar 0 is the far-right 
 front servo and sonar 7 is the far-left front servo.  Looking at the back of
 the robot, sonar 8 is the far-right rear servo and sonar 15 is the far-left 
 rear servo. */
double getSonar(int index)
{
    return sonars[index];
}

/** Returns the last time tick that information for sonar #index arrived.  Note that
 * sonars come in different batches, so you may not receive all of them
 * in one time tick.  Each time any sonar batch arrives, the time tick is increased; 
 * this can thus give you an idea as whether your sonar data is really old
 * (if it's older than 8, you've got old data -- older than 32 say, you've 
 * got REALLY old data).  Looking at the front of the robot, sonar 0 is the
 * far-right front servo and sonar 7 is the far-left front servo.  Looking 
 * at the back of the robot, sonar 8 is the far-right rear servo and sonar 15 is 
 * the far-left rear servo. */
int getLastSonarTime(int index)
{
    return lastSonarTime[index];
}

/** Returns the current time tick to compare with past time ticks for sonar. */
int getCurrentSonarTime()
{
    return currentSonarTime;
}

/** Returns all the sonars values.  The ARCOS documentation claims that the values 
 * are in millimeters  away from the sonar.  However, this appears to be incorrect.
 * We are multiplying by SONAR_CONVERSION (0.555) to get to millimeters.  
 * The pointer passed in will be set to reference the sonar data pointer, and the 
 * retArrSize will be the size of the sonar data array.
 *
 * NOTE:  BE CAREFUL, as any changes to the returned array will be changes to 
 * stored sonar data.
 *
 Looking at the front of the robot, sonar 0 is the far-right front servo and sonar
 7 is the far-left front servo.  Looking at the back of the robot, sonar 8 is the 
 far-right rear servo and sonar 15 is the far-left rear servo.
 *
 */
void getSonars(const double* retVal, const int* retArrSize)
{
    retVal = sonars;
    retArrSize = &sonarsSize;
}

/** Returns all the time ticks, for each sonar, when the sonar data last arrived.
  Note that sonars come in different batches, so you may not receive all of them
  in one time tick.  Each time any sonar batch arrives, the time tick is increased;
  this can thus give you an idea
  as whether your sonar data is really old (if it's older than 8, you've got old 
  data -- older than 32 say, you've got REALLY old data).
  Looking at the front of the robot, sonar 0 is the far-right front servo 
  and sonar 7 is the far-left front servo.  Looking at the back of the robot, sonar 
  8 is the far-right rear servo and sonar 15 is the far-left rear servo. 

 * The pointer passed in will be set to reference the lastSonarTimes data pointer, 
 * and the retArrSize will be the size of the lastSonarTimes data array.
 *
 * NOTE:  BE CAREFUL, as any changes to the returned array will be changes to 
 * stored data.
 *  
 */
void getLastSonarTimes(int* retVal, int* retArrSize)
{
    retVal = lastSonarTime;
    retArrSize = &lastSonarTimeSize;
}


/** Calculates the checksum of the data. */
short checksum(char* data, int dataLen)
{
    int c=0, data1=0, data2=0, x=0;
    for(x=0;x<dataLen-1;x+=2)
    {
        data1 = data[x];  
        if(data1 < 0) 
            data1 += 256;

        data2 = data[x+1];  
        if(data2 < 0) 
            data2 += 256;
        c += ( (data1 << 8) | data2);
        c = c & 0xffff;
    }
    if ((dataLen & 0x1) == 0x1)  // odd
    {
        data1 = data[dataLen-1];  
        if(data1 < 0) 
            data1 += 256;
        c = c ^ data1;
    }
    return (short)c;	
}



long lastToRobotTime = 0;
double THREAD_SLEEP = 0.3;

/**
  send the data to the robot.  The data contains only the instruction number and 
  parameters.  The header if automatically added, the length of the packet is 
  calculated and the checksum is appended at the end of the packet, so that the 
  user's job is easier.
  */

boolean submit(char* data, int dataLen)
{
    if(sockfd < 0)
    {
        printf("Socket has not been opened yet, cannot call submit!\n");
        return false;
    }
    char temp[dataLen + 5];
    int tmpLen = dataLen + 5;
    int n;

    short csum = checksum(data, dataLen);
    memcpy(&temp[3], data, dataLen);
    temp[0] = HEADER[0];
    temp[1] = HEADER[1];
    temp[2] = (char)(dataLen+2); // remember this cannot exceed 200!
    temp[tmpLen-2] = (char)(csum >> 8);
    temp[tmpLen-1] = (char)(csum & 0x00ff);

    // send data
    n = write(sockfd, temp, tmpLen);
    if(n < 0)
    {
        printf("Could not write the data over the socket!\n");
        return false;
    }

    return true;
}

/*

// prints out the two arrays to err in order.  Bytes are printed 0...256, not -128...127
// either array or array2 can be null
private void printPacket(byte[] header, byte[] data, PrintStream err)
{
boolean first = false;
if (header!=null)
for( byte i = 0 ; i < header.length ; i++ ) 
{
if (!first) first = true;
else err.print(" "); 
err.print((((int)header[i])+256)%256);
}
if (data!=null)
for( byte i = 0 ; i < data.length ; i++ ) 
{
if (!first) first = true;
else err.print(" "); 
err.print((((int)data[i])+256)%256);
}
err.println();
}
*/

/**
  Connects to the given IP Address and port number (namely the 
  serialdaemon).

  Note, I am no wiz at socket programming so much of the following 
  is modified from code found at:
http://www.cs.rpi.edu/courses/sysprog/sockets/client.c
*/
boolean connectToBot(const char* hostName, int port)
{
    struct sockaddr_in serv_addr;
    struct hostent *server;

    if(hostName==NULL)
    {
        printf("hostName is NULL, cannot connect!\n");
        return false;
    }

    //create the socket
    sockfd = socket(AF_INET, SOCK_STREAM, 0);

    if (sockfd < 0) 
    {
        printf("Cannot open socket!\n");
        return false;
    }

    server = gethostbyname(hostName);
    if (server == NULL) {
        printf("No such host\n");
        return false; 
    }    

    bzero((char *) &serv_addr, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    bcopy((char *)server->h_addr, 
            (char *)&serv_addr.sin_addr.s_addr,
            server->h_length);
    serv_addr.sin_port = htons(port);
    if (connect(sockfd,(struct sockaddr *)&serv_addr,sizeof(serv_addr)) < 0) 
    {
        printf("Could not connect!\n");
        return false;
    }

    return true;
}

struct ReadWDStruct
{
    int n;
    char* buf;
    int offset;
    int size;
};

void* readWD(void* param)
{
    struct ReadWDStruct* wdStruct = (struct ReadWDStruct*)param;
    wdStruct->n = 0;
    do
    {
        wdStruct->n += read(sockfd, wdStruct->buf + wdStruct->n + wdStruct->offset,(wdStruct->size)-(wdStruct->n));
        if ((wdStruct->n) < (wdStruct->size))
        {
            sleep(0.02);
        }
    } while((wdStruct->n)<(wdStruct->size));

    return NULL;
}

int readByteswithTimeout(char* buf, int offset, int size, double timeout )
{
    int n;
    struct ReadWDStruct wdStruct;
    wdStruct.n = n;
    wdStruct.buf = buf;
    wdStruct.offset = offset;
    wdStruct.size = size;

    pthread_t wdThread;
    if (pthread_create(&wdThread, NULL, readWD, (void*)&wdStruct) != 0)
    {
        fprintf( stderr, "pthread_create failed!\n");
        exit( 1 );
    }

    sleep(timeout);
    return n;    
}

int readBytes(char* buf, int offset, int size )
{
    int n = 0;
    do
    {
        n += read(sockfd, buf+offset+n,size-n);
        if (n < size)
        {
            sleep(0.02);
        }
    } while (n<size);

    return n;
}

/** Waits for the byte string in 'expected' to show up in the incoming stream.
  This is a simpler algorithm than your typical string-matching algorithm, because
  it assumes that the FIRST byte in 'expected' is different from all the others. */
boolean lookFor(char* expected, int expectedLen)
{
    int bytesSearched = 0;
    int count = 0;
    char buf[1];
    while(count < expectedLen)
    {
        readBytes(buf, 0, 1);
        if (buf[0] == expected[count]) 
        {
            count++;
        }
        else 
        { 
            count = 0; 
            bytesSearched++; 
        }
    }
    if (isVerbose() && bytesSearched > 0) 
        printf("Searched %d\n",bytesSearched);
    return true;
}

char lowByte( short arg )
{
    return (char)( arg & 0xff );
}

char highByte( short arg )
{
    return (char)( (arg >> 8) & 0xff );
}

/** send sync0 message (for connection to robot) */
boolean sync0()
{
    char byte[1] = {0};
    return submit(byte, 1);
}

/** send sync1 message (for connection to robot) */
boolean sync1()
{
    char byte[1] = {1};
    return submit(byte, 1);
}

/** send sync2 message (for connection to robot) */
boolean sync2()
{
    char byte[1] = {2};
    return submit(byte, 1);
}

double SYNC_STREAM_WAIT = 0.3;
char SYNC0_RESPONSE[16] = { (char)250, (char)251, 3, 0, 0, 0 };
boolean doSync0(char* buf, int bufLen)
{
    if (sync0() == false) 
        return false;
    if (isVerbose()) 
        printf("Waiting for SYNC-0....\n");
    return lookFor(SYNC0_RESPONSE, bufLen);
}

char SYNC1_RESPONSE[16] = { (char)250, (char)251, 3, 1, 0, 1 };
boolean doSync1(char* buf, int bufLen)
{
    if (sync1() == false) 
        return false;
    if (isVerbose()) 
        printf( "Waiting for SYNC1....\n" );
    return lookFor(SYNC1_RESPONSE, bufLen);
}

char SYNC2_RESPONSE_START[16] = { (char)250, (char)251 };
boolean doSync2(char* buf, int bufLen)
{
    if (sync2() == false) 
        return false;
    if (isVerbose()) 
        printf( "Waiting for SYNC2....\n" );
    sleep(SYNC_STREAM_WAIT);
    if (!lookFor(SYNC2_RESPONSE_START, 2)) 
        return false;  // 250, 251
    char buffer[128];
    if (!readByteswithTimeout(buffer, 0, 2, SYNC_STREAM_WAIT)) 
    {
        printf("dosync2 failed!\n");
//        return false;  // num bytes for string and Sync2 response
    }
    return true;
}


/** client pulse resets watchdog and prevents the robot from disconnecting */
boolean pulse()
{
    char byte[1] = {0};
    return submit(byte, 1);
}

/** starts the controller */
boolean openController()
{
    char byte[1] = {1};
    return submit(byte, 1);
}

/** close client-server connection */
boolean closeController()
{
    char byte[1] = {2};
    return submit(byte, 1);
}

/** set sonar polling sequence */
boolean polling(char* arg, int argLen )
{
    char temp[argLen + 3];
    int i;
    temp[0] = 3; // command number
    temp[1] = 0x2B; // the parameter is a string
    temp[2] = (char)argLen; // length of string
    for(i = 0 ; i < argLen ; i++ )
        temp[3+i] = arg[i];
    return submit( temp, argLen+3 );
}

/** enable/disable the motors */
boolean enableMotors( boolean arg )
{
    if( arg )
    {
        char byte[4] = { 4, 0x3B, 1, 0 }; 
        return submit(byte, 4);
    }
    else
    {
        char byte[4] = { 4, 0x3B, 0, 0 }; 
        return submit(byte, 4);
    }
}

/** sets translation acc/deceleration; in mm/sec^2 */
boolean seta( short arg )
{
    if( arg >= 0 )
    {
        char byte[4] = { 5, 0x3B, lowByte(arg), highByte(arg) }; 
        return submit(byte, 4);
    }
    else
    {
        char byte[4] = { 5, 0x1B, lowByte((short)(-arg)), highByte((short)(-arg))}; 
        return submit(byte, 4);
    }
}

/** set maximum possible translation velocity; in mm/sec */
boolean setv( short arg )
{
    char byte[4] = { 6, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** Resets the robot's believed position to be x=0, y=0, theta=0 */
boolean seto()
{
    char byte[1] = {7};
    return submit(byte, 1);
}

/** sets maximum rotational velocity; in degrees/sec */
boolean setrv( short arg )
{
    char byte[4] = { 10, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** move forward (+) or reverse (-); in mm/sec */
boolean vel( short arg )
{
    char byte[4] = { 11, 0x3B, lowByte(arg), highByte(arg) };
    return submit(byte, 4);
}

/** turn to absolute heading (in the robot's belief); 0-359 degress */
boolean head( short arg )
{
    char byte[4] = { 12, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** turn relative to current heading, that is, turn BY some number of degrees. */
boolean dhead( short arg )
{
    char byte[4] = { 13, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** rotate at +/- degrees/sec */
boolean rvel( short arg )
{
    if( arg >= 0 )
    {
        char byte[4] = { 21, 0x3B, lowByte(arg), highByte(arg) }; 
        return submit(byte, 4);
    }
    else
    {
        char byte[4] = { 21, 0x1B, lowByte((short)(-arg)), highByte((short)(-arg)) }; 
        return submit(byte, 4);
    }
}

/** sets rotational (+/-)de/acceleration; in mm/sec^2 */
boolean setra( short arg )
{
    char byte[4] = { 23, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** enable/disable the sonars */
boolean enableSonars( boolean arg )
{
    if( arg )
    {
        char byte[4] = { 28, 0x3B, 1, 0 }; 
        return submit(byte, 4);
    }
    else
    {
        char byte[4] = { 28, 0x3B, 0, 0}; 
        return submit(byte, 4);
    }
}

/** enable/disable a given sonar. */
boolean enableSingleSonar( short arg )
{
    char byte[4] = { 28, 0x3B, (char)(arg % 256), (char)(arg / 256) }; 
    return submit(byte, 4);
}

/** stops the robot (motors remain enabled) */
boolean stopRobot()
{
    char byte[1] = {29};
    return submit(byte, 1);
}

/** msbits is a byte mask that selects output port(s) for changes; lsbits set (1) or reset (0) the selected port */
boolean digout( short arg )
{
    char byte[4] = { 30, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** independent wheel velocities; lsb=right wheel; msb=left wheel; PSOS is in +/- 4mm/sec; POS/AmigOS is in 2 cm/sec increments */
boolean vel2( short arg )
{
    char byte[4] = { 32, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** independent wheel velocities, left and right. PSOS is in +/- 4mm/sec; POS/AmigOS is in 2 cm/sec increments */
boolean vel2IndepWheels(char left, char right)
{
    int l = left;
    int r = right;
    if (l < 0) l = 256 + l;
    if (r < 0) r = 256 + r;
    return vel2((short)(l << 8 | r));
}

boolean bumpstall( short arg )
{
    char byte[4] = { 44, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}


/** emergency stop, overrides deceleration */
boolean e_stop()
{
    char byte[1] = {55};
    return submit(byte, 1);
}

/** play stored sound -- Amigobot only.  Pioneer's don't have such a thing. */
boolean sound( short arg )
{
    char byte[4] = { 90, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** request playlist packet for sound number or 0 for all user sounds -- Amigobot only.  */
boolean playlist( short arg )
{
    char byte[4] = { 91, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

/** mute (0) or enable (1) sounds -- Amigobot only. */
boolean soundtog( short arg )
{
    char byte[4] = { 92, 0x3B, lowByte(arg), highByte(arg) }; 
    return submit(byte, 4);
}

boolean readPacket()
{
    if (lookFor(SYNC2_RESPONSE_START, 2))
    {
        char temp[1];
        readBytes(temp, 0, 1);
        int length;
        if (temp[0] < 0)
            length = temp[0]+256;
        else
            length = temp[0];

        char data[length];
        readBytes( data, 0, length ); // read rest of packet

        // check if checksum is correct
        char forchecksum[length-2];
        memcpy(forchecksum, data, length-2); 
        int packetCheckSum;
        packetCheckSum = data[length-2];
        if (data[length-2] < 0) 
            packetCheckSum += 256;
        packetCheckSum *=256;
        packetCheckSum += data[length-1];
        if (data[length-1] < 0) 
            packetCheckSum += 256;

        short expectedCheckSum = checksum(forchecksum, length-2);
        short gotCheckSum = (short)(packetCheckSum & 0xFFFF);

        // if checksum is correct, interpret data

        // NOTE: On Linux boxes running with Prolific/Manhattan usb serial drivers, 
        // we're seeing checksums that are ALWAYS off by 3! But the data seems to 
        // be correct, so we're allowing a weird checksum of this form.
        //
        if( expectedCheckSum == gotCheckSum || (checksumLenient && 
                    ((expectedCheckSum == gotCheckSum + 3) || 
                     (expectedCheckSum == gotCheckSum - 3))))
        {
            // temp variables -- we'll move these into the real variables after the 
            // read process
            boolean _motorEngaged = false;
            double _xpos = 0;
            double _ypos = 0;
            double _orientation = 0;
            double _leftWheelVelocity = 0;
            double _rightWheelVelocity = 0;
            int _battery = 0;
            boolean _leftWheelStallIndicator = false;
            boolean _rightWheelStallIndicator = false;
            double _control = 0;
            boolean changedSonarArr[32];
            int j=0;
            for(j=0;j<32;++j)
                changedSonarArr[j] = false;


            char ss[128] = {};
            int currCounter = 0;
            short i;
            for(i = 0 ; i < length ; ++i)
                if( data[i] >= 0 )
                {
                    ss[currCounter] = ' ';
                    ++currCounter;
                    ss[currCounter] = data[i];
                    ++currCounter;
                }
                else
                {
                    ss[currCounter] = ' ';
                    ++currCounter;
                    ss[currCounter] = data[i] + 256;
                    ++currCounter;
                }                    
            // read motor status
            if( data[0] == 0x32 )
                _motorEngaged = false;
            else if( data[0] == 0x33 )
                _motorEngaged = true;
            else
            {
                if(isVerbose())
                    printf("Invalid motor status: %d\n" + data[0]);
            }

            int alpha;
            // read xpos
            if( data[2] >= 0 )
                alpha = data[2];
            else
                alpha = data[2] + 256;
            alpha = alpha & 0x7F;
            alpha = alpha * 256;
            if( data[1] >= 0 )
                alpha += data[1];
            else
                alpha += data[1] + 256;
            if (alpha >= 16384) alpha -= 32768;
            _xpos = XPOS_CONVERSION * alpha; //0.5083*alpha;


            // read ypos
            if( data[4] >= 0 )
                alpha = data[4];
            else
                alpha = data[4] + 256;
            alpha = alpha & 0x7F;
            alpha = alpha * 256;
            if( data[3] >= 0 )
                alpha += data[3];
            else
                alpha += data[3] + 256;
            if (alpha >= 16384) 
                alpha -= 32768;
            _ypos = YPOS_CONVERSION * alpha; // 0.5083*alpha;


            // read orientation
            if( data[6] >= 0 )
                alpha = data[6];
            else
                alpha = data[6] + 256;
            alpha = alpha * 256;
            if( data[5] >= 0 )
                alpha += data[5];
            else
                alpha += data[5] + 256;
            _orientation = THPOS_CONVERSION * alpha; // 0.001534 * ((short)alpha);


            // read velocity of left wheel
            if( data[8] >= 0 )
                alpha = data[8];
            else
                alpha = data[8] + 256;
            alpha = alpha * 256;
            if( data[7] >= 0 )
                alpha += data[7];
            else
                alpha += data[7] + 256;
            _leftWheelVelocity = LVEL_CONVERSION * alpha; // 0.6154 * ((short)alpha);


            // read velocity of right wheel
            if( data[10] >= 0 )
                alpha = data[10];
            else
                alpha = data[10] + 256;
            alpha = alpha * 256;
            if( data[9] >= 0 )
                alpha += data[9];
            else
                alpha += data[9] + 256;
            _rightWheelVelocity = RVEL_CONVERSION * alpha; // 0.6154 * ((short)alpha);


            // read battery status
            if( data[11] >= 0 )
                _battery = data[11];
            else
                _battery = data[11] + 256;


            // read bumpers
            _leftWheelStallIndicator = (data[12]&0x80)!=0;
            _rightWheelStallIndicator = (data[13]&0x80)!=0;


            // read control
            if( data[15] >= 0 )
                alpha = data[15];
            else
                alpha = data[15] + 256;
            alpha = alpha * 256;
            if( data[14] >= 0 )
                alpha += data[14];
            else
                alpha += data[14] + 256;
            _control = CONTROL_CONVERSION * alpha; // 0.001534 * ((short)alpha);

            // read PTU
            // not implemented


            // read Compass
            // not implemented

            // read Sonar readings
            int nr = data[19];
            int nn;
            currentSonarTime++;
            for(nn = 0 ; nn < nr ; nn++)
            {
                int n_sonar = data[20+3*nn];
                if (n_sonar < 0 || n_sonar > 31)  // crazy sonar
                { 
                    if (isVerbose()) 
                    { 
                        printf("Bad Sonar number, got %d, expect a value between "
                                "0 and 31\n", n_sonar); 
                    }
                }
                else if (20+3*nn+2 >= length)  // crazy index
                {
                    if (isVerbose()) 
                    { 
                        printf("Invalid sonar index, got %d\n", nn); 
                    }
                }
                else
                {
                    if( data[20+3*nn+2] >= 0 )
                        alpha = data[20+3*nn+2];
                    else
                        alpha = data[20+3*nn+2] + 256;
                    alpha = alpha * 256;
                    if( data[20+3*nn+1] >= 0 )
                        alpha += data[20+3*nn+1];
                    else
                        alpha += data[20+3*nn+1] + 256;
                    _sonars[n_sonar] = SONAR_CONVERSION * alpha; // 0.555 * alpha;
                    changedSonarArr[n_sonar] = true;
                    _lastSonarTime[n_sonar] = currentSonarTime;
                }
            }



            validData = true;
            motorEngaged = _motorEngaged;
            xpos = _xpos;
            ypos = _ypos;
            orientation = _orientation;
            leftWheelVelocity = _leftWheelVelocity;
            rightWheelVelocity = _rightWheelVelocity;
            battery = _battery;
            leftWheelStallIndicator = _leftWheelStallIndicator;
            rightWheelStallIndicator = _rightWheelStallIndicator;
            control = _control;
            for(j=0;j<32;++j)
            {
                if(changedSonarArr[j] == true)
                {
                    sonars[j] = _sonars[j];
                    lastSonarTime[j] = _lastSonarTime[j];
                }
            }

            return true;
        }
        else if (isVerbose())
        {
            printf("Bad checksum, got %d, expected %d.\n", 
                    (short)(packetCheckSum & 0xFFFF), checksum(forchecksum, length-2));

                        
                          printf("Bad packet was: ");
                          int i;
                          for(i=0; i<length; ++i)
                          {
                          printf("  %d", data[i]);
                          }
                          printf("\n");
                          
        }
    }
    return false;
}


int readString(char* rtnBuf, int rtnBufSize)
{
    char buf[5];
    int currIndex=0;
    while(true)
    {
        readBytes(buf,0,1);
        if( (buf[0] == 0 ) || (currIndex == rtnBufSize))
            break;
        else
        {
            rtnBuf[currIndex] = buf[0];
            ++currIndex;
        }
    }
    return currIndex;
}

void* heartBeat()
{
    while(working)
    {
        pulse();
        sleep(1.5);
    }
    return NULL;
}

long DISPLAY_INTERVAL = 1;
long lastDisplayTime = 0;
long displayCount = 0;

void* reading()
{
    while(working)
    {
        if (readPacket()) 
        { 
            displayCount++; 
        }
    }
    return NULL;
}    

void setupBotConnection()
{
    char buf[7];
    // do doSync0, and if that succeeds, do doSync1, and if that succeeds, do doSync2, and
    // if that succeeds, we're done.  Else fall back and do them again.
    boolean a = true;
    do
    {
        // close the controller if it's already open
        //
        // NOTE, for some reason, this closes when there wasn't a connection,
        // so the bot won't respond to the syncs
        //closeController();
        if (a) 
            a = false; 
        else   
        {
            sleep(THREAD_SLEEP);
        }
        boolean b = true;
        do
        {
            if (b)
                b = false; 
            else 
            {
                sleep(THREAD_SLEEP);
            }
            boolean c = true;
            do 
            {
                if (c) 
                    c = false; 
                else 
                {
                    sleep(THREAD_SLEEP);
                }
            } while (!doSync0(buf, 6));
        } while (!doSync1(buf, 6));
    } while (!doSync2(buf, 6));

    // read autoconfiguration strings (3)

    char robotName[64];
    int robotNameLen = readString(robotName,64);
    robotName[robotNameLen] = 0;
    char robotClass[64];
    int robotClassLen = readString(robotClass,64);
    robotClass[robotClassLen] = 0;
    char robotSubClass[64];
    int robotSubClassLen = readString(robotSubClass,64);    
    robotSubClass[robotSubClassLen] = 0;

    if (isVerbose()) 
    {
        printf("Name: %s / %s / %s\n", robotName, robotClass, robotSubClass);
    }

    char readBuf[10];
    readByteswithTimeout(readBuf, 0, 2, SYNC_STREAM_WAIT);  // checksum of string

    // start the controller
    openController();

    //The working var will allow the threads to run
    working = true;    
    if (pthread_create(&readThread, NULL, reading, NULL) != 0)
    {
        fprintf( stderr, "pthread_create failed for reading thread!\n");
        exit( 1 );
    }


    if (pthread_create(&beatThread, NULL, heartBeat, NULL) != 0)
    {
        fprintf( stderr, "pthread_create failed for heartbeat thread!\n");
        exit( 1 );
    }
}

void destroy()
{
    if(isVerbose())
        printf("Stoping, closing, and disconnecting\n");
    //stop the motors and sonars
    enableMotors(false);
    enableSonars(false);

    working = false;

    //it will take the beat thread up to 1.5 seconds
    //to find out that it should be stopped.
    sleep(2);

    //don't need to join, as the threads will end when they notice that
    //working is false
    //pthread_join(beatThread, NULL);
    //pthread_join(readThread, NULL);
    
    closeController();
    close(sockfd);

/*
    if(sonars != NULL)
        free(sonars);
    if(_sonars != NULL)
        free(_sonars);
    if(lastSonarTime != NULL)
        free(lastSonarTime);
    if(_lastSonarTime != NULL)
        free(_lastSonarTime);
*/
}

/* This is sample code, it should be removed for operational code */
int main()
{
    setVerbose(true);
    initialize();
    connectToBot("localhost", 3500);
    setupBotConnection();
    sleep(1);
    printf("enabling the sonars\n");
    enableSonars(true);

    enableMotors(true);

    printf("setting velocity to 20\n");
    setv((short)20);


    while(!isValidData())
    {
        sleep(0.5);
    }

    int i=0;
    for(i=0;i<10;++i)
    {
        int i;
        printf("\n\n New Data:\n");
        for(i=0; i<16; ++i)
        {
            printf("sonars[%d] = %f\n", i, sonars[i]);
        }
        sleep(10);
    }


    enableSonars(false);


    sleep(30);
    destroy();
    return 0;
}
