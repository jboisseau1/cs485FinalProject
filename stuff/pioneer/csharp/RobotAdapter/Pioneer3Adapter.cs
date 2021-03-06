using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Robot
{
    public class Pioneer3Adapter : IRobot
    {
        // standard Pioneer3 16 sonar configuration angles
        private double[] robotSonars = { -80, -50, -30, -10, 10, 30, 50, 80, 100, 130, 150, 170, -170, -150, -130, -100 };
        private double maxSonarDistance = 5000;

        private Queue<byte> receiveQueue = new Queue<byte>();

        private static bool stopThreads = false;
        private Thread pulseThread = null;
        private Thread infoThread = null;
        
        private Thread simReceiveThread = null;
        private bool simulatePort = false;
        private string simPortFilename;
        private StreamReader simPort = null;

        private int[] sonars = new int[16];
        private ARCOSSIP lastARCOSSIP;

        private SerialPort sp;
        private long lastToRobotTime = 0;
        private bool moving = false;

        private string _name = "unknown";
        private string _type = "unknown";
        private string _subtype = "unknown";

        private byte[] HEADER = { (byte)(0xFA), (byte)(0xFB) };

        private class ARCOSSIP
        {
            public bool motorsMoving;   // motors status (0x3s; s=2 when motor stopped, s=3 when motors on)
            public int xPos;            // wheel encoder position in mm
            public int yPos;            // wheel encoder position in mm
            public int thetaPos;        // angular orientation, (AngleConvFactor‡ = 0.001534 radians per angular unit = 2π/4096).
            public int leftVel;         // Wheel velocities in millimeters per second (VelConvFactor‡ = 1.0)
            public int rightVel;
            public byte battery;        // Battery charge in tenths of volts (e.g. 101 = 10.1 volts)
            public int stallAndBump;    //Motor stall and bumper indicators. Bit 0 is the left
                                        // wheel stall indicator, set to 1 if stalled. Bits 1-7
                                        // correspond to the first bumper I/O digital input states
                                        // (accessory dependent). Bit 8 is the right wheel stall,
                                        // and bits 9-15 correspond the second bumper I/O states,
                                        // also accessory and application dependent.
            public int servoSetPoint;   // set point of servo's angualr position in degrees
            public int flags;           // Bit 0 motors status; bits 1-4 sonar array status; bits
                                        // 5,6 STOP; bits 7,8 ledge-sense IRs; bit 9 joystick fire
                                        // button; bit 10 auto—charger power-good.
            public int compass;         // heading in 2 degree units.
            public byte sonarCount;     // number of sonar readings
            public byte[] sonarNumber;  // if sonar count > 0, this is sonar disk number
            public int[] sonarRange;    // sonar range in mm


            public ARCOSSIP(byte[] data)
            {
                try
                {
                    int index = 0;
                    motorsMoving = (data[index++] == 0x33);
                    xPos = data[index++] + data[index++] * 0x100;
                    yPos = data[index++] + data[index++] * 0x100;
                    thetaPos = data[index++] + data[index++] * 0x100;
                    leftVel = data[index++] + data[index++] * 0x100;
                    rightVel = data[index++] + data[index++] * 0x100;
                    battery = data[index++];
                    stallAndBump = data[index++] + data[index++] * 0x100;
                    servoSetPoint = data[index++] + data[index++] * 0x100;
                    flags = data[index++] + data[index++] * 0x100;
                    compass = data[index++];
                    sonarCount = data[index++];
                    sonarNumber = new byte[sonarCount];
                    sonarRange = new int[sonarCount];
                    for (int i = 0; i < sonarCount; i++)
                    {
                        sonarNumber[i] = data[index++];
                        sonarRange[i] = data[index++] + data[index++] * 0x100;
                    }
                }
                catch (Exception)
                {
                    //errorOut("Error parsing ARCOSIP data");
                }
            }
        }

        public Pioneer3Adapter(string commSettings)
        {
            initialize(commSettings);
        }

        public Pioneer3Adapter()
            //: this("COM10")
        {
        }

        #region IRobot Members

        public void initialize(string commSettings)
        {
            if (commSettings.ToLower().StartsWith("file:"))
            {
                simPortFilename = commSettings.Substring(commSettings.IndexOf(':') + 1);
                if (File.Exists(simPortFilename))
                {
                    simPort = File.OpenText(simPortFilename);
                    simulatePort = true;
                    simReceiveThread = new Thread(new ParameterizedThreadStart(simReceiveData));
                }
            }
            else // serialPort == "COM1" or "COM2"...
            {
                simulatePort = false;
                sp = new SerialPort(commSettings, 9600, Parity.None, 8, StopBits.One);
                sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            }
        }

        public double[] getSonarAngles()
        {
            return robotSonars;
        }

        public double getSonarMaxDistance()
        {
            return maxSonarDistance;
        }

        void sp_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Utils.debugOut("pin changed " + e.ToString());
        }

        public bool open()
        {
            if (isOpenned())
            {
                Utils.debugOut("Port is already open, close first before attempting to reopen");
                return true;
            }

            stopThreads = false;

            Utils.debugOut("Trying to open robot port");
            // clear input queue of any old commands
            receiveQueue.Clear();

            // if simulating connection, start sim thread
            if ((simulatePort) && (simPort != null))
            {
                simReceiveThread.Start();
            }
            else
            {
                try
                {
                    sp.Open();
                }
                catch (Exception)
                {
                    Utils.debugOut("Cannot open port " + sp.PortName);
                    System.Windows.Forms.MessageBox.Show("Cannot open port " + sp.PortName);
                    return false;
                }
            }

            // close first, just in case                
            if (( sp != null ) && (sp.IsOpen))
            {
                byte[] close = { 250, 251, 3, 2, 0, 2 };
                //sp.Write(close, 0, close.Length);
                Thread.Sleep(100);
            }

            // send sync0
            submit(new byte[] { 0 });
            Utils.debugOut("waiting for sync 0");
            if (!waitForData(new byte[] { 250, 251, 3, 0, 0, 0 }, 5000))
            {
                if (!simulatePort)
                {
                    submit(new byte[] { 0 });
                    Utils.debugOut("waiting for sync 0 (2)");
                    if (!waitForData(new byte[] { 250, 251, 3, 0, 0, 0 }, 5000))
                    {
                        submit(new byte[] { 0 });
                        Utils.debugOut("waiting for sync 0 (3)");
                        if (!waitForData(new byte[] { 250, 251, 3, 0, 0, 0 }, 5000))
                        {
                            return false;
                        }
                    }
                }
            }
            // send sync1
            submit(new byte[] { 1 });
            Utils.debugOut("waiting for sync 1");
            if (!waitForData(new byte[] { 250, 251, 3, 1, 0, 1 }, 5000))
            {
                if (!simulatePort)
                    return false;
            }

            // send sync2
            submit(new byte[] { 2 });
            getSync2Info();

            // send open
            submit(new byte[] { 1 });

            Utils.debugOut("Robot port open");

            stopThreads = false;

            pulseThread = new Thread(new ParameterizedThreadStart(pulseLoop));
            pulseThread.Start();

            infoThread = new Thread(new ParameterizedThreadStart(infoLoop));
            infoThread.Start();

            return true;
        }

        public void close()
        {
            if (!isOpenned())
            {
                Utils.debugOut("cannot close, port not open");
                return;
            }
            //submit(new byte[] { 1 });
            try
            {
                stopThreads = true;
                byte[] close = { 250, 251, 3, 2, 0, 2 };
                if (simPort != null)
                {
                    simPort.Close();
                }
                else if (sp.IsOpen)
                {
                    sp.Write(close, 0, close.Length);
                    System.Threading.Thread.Sleep(250);
                    sp.Close();
                }
                Utils.debugOut("Robot port closed");
            }
            catch (Exception)
            {
                Utils.debugOut("Error closing robot port!");
            }
        }

        public void enableMotors(bool arg)
        {
            if (arg)
                submit(new byte[] { 4, 0x3B, 1, 0 });
            else
                submit(new byte[] { 4, 0x3B, 0, 0 });
        }

        public void enableSonar(bool arg)
        {
            if (arg)
            {
                submit(new byte[] { 28, 0x3B, 1, 0 });
            }
            else
            {
                submit(new byte[] { 28, 0x3B, 0, 0 });
            }
        }

        public void resetDeadReckoning()
        {
            // reset local position to 0
            submit(new byte[] { 7, 0x3B, 0, 0 });
        }

        public bool isOpenned()
        {
            if (
                    ((sp != null) && sp.IsOpen) ||
                    ((simReceiveThread != null) &&
                    ((simReceiveThread.ThreadState != ThreadState.Unstarted) && 
                    (simReceiveThread.ThreadState != ThreadState.Stopped))))
            {
                return true;
            }
            return false;
        }

        public bool isMoving()
        {
            return moving;
        }

        public string getName()
        {
            return _name;
        }

        public string getType()
        {
            return _type;
        }

        public string getSubType()
        {
            return _subtype;
        }


        public double getCompass()
        {
            if (lastARCOSSIP != null)
            {
                return (double)lastARCOSSIP.battery / 10.0;
            }
            return 0;
        }

        public double getBattery()
        {
            if (lastARCOSSIP != null)
            {
                return (double)lastARCOSSIP.battery / 10.0;
            }
            return 0;
        }

        public double getLeftVelocity()
        {
            if (!this.isMoving())
            {
                return 0;
            }
            if (lastARCOSSIP != null)
            {
                int v = lastARCOSSIP.leftVel;
                if ((v & 0x8000) > 0)
                    v = (v - 0xFFFF) - 1;
                return (double)v / 1000.0;
            }
            return 0;
        }

        public double getRightVelocity()
        {
            if (!this.isMoving())
            {
                return 0;
            }
            if (lastARCOSSIP != null)
            {
                int v = lastARCOSSIP.rightVel;
                if ((v & 0x8000) > 0)
                    v = (v - 0xFFFF) - 1;
                return (double)v / 1000.0;
            }
            return 0;
        }

        public PointF getIntegratedPosition()
        {
            PointF p;
            if (lastARCOSSIP != null)
            {
                int x = lastARCOSSIP.xPos;
                int y = lastARCOSSIP.yPos;
                if ((x & 0x8000) > 0 )
                    x = (x - 0xFFFF) - 1;
                if ((y & 0x8000) > 0 )
                    y = (y - 0xFFFF) - 1;
                p = new PointF((float)y / (float)1000.0, (float)x / (float)1000.0);
            }
            else
            {
                p = new Point(0, 0);
            }
            return p;
        }

        public double getIntegratedTheta()
        {
            if (lastARCOSSIP != null)
            {
                double rawAngle = (double)lastARCOSSIP.thetaPos;                
                double angle = 0.001534 * rawAngle;
                
                // convert to degrees
                angle = angle * 180 / Math.PI;
                
                angle = angle - 360;
                angle *= -1;

                if (angle == 360)
                    angle = 0;

                return (double)angle;
            }
            return 0;
        }

        public int getSonarValue(int sonar)
        {
            return sonars[sonar];
        }

        public void sound(short arg)
        {
            submit(new byte[] { 90, 0x3B, Utils.lowByte(arg), Utils.highByte(arg) });
        }

        public void stop()
        {
            submit(new byte[] { 29 });
        }
        
        public void emergencyStop()
        {
            submit(new byte[] { 55 });
        }

        public void setVelocity(byte left, byte right)
        {
            int l = left;
            int r = right;
            if (l < 0) l = 256 + l;
            if (r < 0) r = 256 + r;
            vel2((short)(l << 8 | r));
        }

        public void setHeadingRelative(short angle)
        {
            angle = (short)(angle - 360);
            angle *= -1;

            if (angle >= 360)
                angle = (short)(angle - 360);

            submit(new byte[] { 13, 0x3B, Utils.lowByte(angle), Utils.highByte(angle) });
        }

        public void setHeadingAbsolute(short angle)
        {
            angle = (short)(angle - 360);
            angle *= -1;

            if (angle == 360)
                angle = 0;

            submit(new byte[] { 12, 0x3B, Utils.lowByte(angle), Utils.highByte(angle) });
        }

        #endregion


        // set robot baud (does not seem to work on Pioneer 3s
        private void hostbaud( short arg )
        {
            // 0=9600, 1=19200, 2=38400, 3=57600, or 4=115200
            //submit(new byte[] { 50, 0x3B, Utils.lowByte(arg), Utils.highByte(arg) });
        }

        // send pulse (aka Hearbeat) data packet to robot
        private void pulse()
        {
            submit(new byte[ ] { 0 });
        }

        // TCM2 module commands; see P2 TCM2 manual for details
        private void tcm2(short arg)
        {
            submit(new byte[] { 45, 0x3B, Utils.lowByte(arg), Utils.highByte(arg) });
        }

        // send two wheel velocity command to robot
        private void vel2(short arg)
        {
            Utils.debugOut("Vel2: " + Utils.lowByte(arg) + ", " + Utils.highByte(arg));
            submit(new byte[] { 32, 0x3B, Utils.lowByte(arg), Utils.highByte(arg) });
        }

        // Get standard robot information data
        private ARCOSSIP getNextARCOSSIP(long timeout_ms)
        {
            {
                long timeOutTicks = DateTime.Now.Ticks + (timeout_ms * 10000);


                bool found = false;
                bool foundHeader = false;
                byte byteCount = 0;

                while (!found)
                {
                    if (DateTime.Now.Ticks >= timeOutTicks)
                    {
                        // timed out!
                        return null;
                    }

                    if (receiveQueue.Count > 0)
                    {
                        // first find the header (250, 251)
                        while ( ( !foundHeader ) && ( receiveQueue.Count >= 2 ) )
                        {
                            if ((receiveQueue.Peek() == 250) && (receiveQueue.Count >= 2))
                            {
                                lock (receiveQueue)
                                {
                                    byte gotMe1 = receiveQueue.Dequeue();
                                    byte gotMe2 = receiveQueue.Dequeue();
                                    if ((gotMe1 == 250) && (gotMe2 == 251))
                                    {
                                        foundHeader = true;
                                    }
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Ticks >= timeOutTicks)
                                {
                                    // timed out!
                                    Utils.debugOut("Timed out looking for ARCOSSIP header!");
                                    return null;
                                }
                                lock (receiveQueue)
                                {
                                    receiveQueue.Dequeue();
                                }
                            }
                        }
                        // then get the byte count
                        if ((foundHeader) && (byteCount == 0))
                        {
                            if (receiveQueue.Count > 0)
                            {
                                lock (receiveQueue)
                                {
                                    byteCount = receiveQueue.Dequeue();
                                }
                            }
                        }
                        // then get the data
                        else if ((foundHeader) && (byteCount > 0))
                        {
                            // get all bytes first
                            byte[] infoPacket = new byte[byteCount/* + 3*/];
                            byte[] checkSumBytes = new byte[2];
                            
                            int i = 0;

                            while (i < byteCount)
                            {
                                if (receiveQueue.Count > 0)
                                {
                                    lock (receiveQueue)
                                    {
                                        if ( i >= (byteCount - 2) )
                                        {
                                            byte gotByte = receiveQueue.Dequeue();
                                            checkSumBytes[byteCount - i - 1] = gotByte;
                                        }
                                        else
                                        {
                                            infoPacket[i /*+ 3*/] = receiveQueue.Dequeue();                                            
                                        }
                                        i++;
                                    }                                    
                                }
                                else
                                {
                                    Thread.Sleep(5);
                                }
                            }

                            // confirm checksum
                            uint checkSumCalc = (uint)calcChecksum(infoPacket);
                            uint checkSumPacket = (uint)(checkSumBytes[1] * 0x100 + checkSumBytes[0]);

                            if ((checkSumCalc == checkSumPacket) || ((0x0000FFFF & checkSumCalc) == (0x0000FFFF & checkSumPacket)))
                            {
                                // decode the bytes
                                ARCOSSIP arcossip = new ARCOSSIP(infoPacket);
                                //Utils.debugOut("got info packet");
                                return arcossip;
                            }
                            else
                            {
                                Utils.debugOut("Bad checksum on info packet: was " + string.Format("{0:x}", checkSumPacket) + " should have been " + string.Format("{0:x}", checkSumCalc));
                                
                                // let's clear the queue here
                                lock (receiveQueue)
                                {
                                    receiveQueue.Clear();
                                }
                                return null;
                            }

                        }
                    }
                    Thread.Sleep(10);
                }
            }
            return null;
        }

        // After Sync0, 1, and 2 are send; the robot returns some info.  Get that info.
        private void getSync2Info()
        {
            // after sync2, robot returns information
            // three null terminated strings:
            //   name, type, and subtype
            _name = getStringFromData(5000);
            _type = getStringFromData(5000);
            _subtype = getStringFromData(5000);
        }
        
        // thread to send pulse every 1.5s to robot
        private void pulseLoop(object param)
        {
            Utils.debugOut("pulse thread started");
            while (!stopThreads)
            {
                Thread.Sleep(1500);
                this.pulse();
            }
        }

        // thread to process serial data buffer
        private void infoLoop(object args)
        {
            while (!stopThreads)
            {
                ARCOSSIP info = getNextARCOSSIP(5000);
                try
                {
                    if (info != null)
                    {
                        // update sonar info
                        for (int i = 0; i < info.sonarCount; i++)
                        {
                            int index = info.sonarNumber[i];
                            sonars[index] = info.sonarRange[i];
                        }
                        moving = info.motorsMoving;

                        lastARCOSSIP = info;
                    }
                }
                catch (Exception)
                {
                }
                Thread.Sleep(10);
            }
        }

        // get next string from serial port
        private string getStringFromData(long timeout_ms)
        {
            string getme = "";
            long timeOutTicks = DateTime.Now.Ticks + (timeout_ms * 10000);

            bool found = false;

            while (!found)
            {
                if (DateTime.Now.Ticks >= timeOutTicks)
                {
                    // timed out!
                    return getme;
                }

                if (receiveQueue.Count > 0)
                {
                    lock (receiveQueue)
                    {
                        // first byte is correct, and queue is long enough, start checking
                        int i;
                        for (i = 0; i < receiveQueue.Count; i++)
                        {
                            byte b = receiveQueue.Dequeue();
                            if (b == 0)
                            {
                                //found null terminator
                                found = true;
                                return getme;
                            }
                            if (!((b == 250) || (b == 251) || (b == 35) || (b == 2)))
                            {
                                getme += (char)b;
                            }
                        }
                    }
                }
            }
            return getme;
        }

        // wait for data to be received on serial port
        private bool waitForData(byte[] data, long timeout_ms)
        {
            long timeOutTicks = DateTime.Now.Ticks + (timeout_ms * 10000);

            if (data.Length > 0)
            {
                bool found = false;

                while (!found)
                {
                    if (DateTime.Now.Ticks >= timeOutTicks)
                    {
                        // timed out!
                        Utils.errorOut("Time out waiting for data");
                        return false;
                    }

                    if (receiveQueue.Count>0)
                    {
                        lock (receiveQueue)
                        {
                            if ((receiveQueue.Peek() == data[0]) && (receiveQueue.Count >= data.Length))
                            {
                                // first byte is correct, and queue is long enough, start checking
                                int i;
                                for (i = 0; i < data.Length; i++)
                                {
                                    if (receiveQueue.Dequeue() != data[i])
                                    {
                                        break;
                                    }
                                }
                                if (i == data.Length)
                                {
                                    found = true;
                                    return true;
                                }
                            }
                        }
                    }
                    Thread.Sleep(50);
                }
            }
            return false;
        }

        // received data handler
        private void receivedData(byte[] data, int length)
        {
            string output = "Received: ";

            lock (receiveQueue)
            {
                for (int i = 0; i < length; i++)
                {
                    output += (data[i].ToString() + " ");
                    receiveQueue.Enqueue(data[i]);
                }
            }
            //Utils.debugOut(output); 
        }

        // when reading a file, pretend to receive data
        private void simReceiveData( object param )
        {
            string nextLine;
            Thread.Sleep(500); // simulate initial handshake delay
            try
            {
                while (!stopThreads)
                {
                    nextLine = simPort.ReadLine();
                    if (simPort.EndOfStream)
                    {
                        Utils.debugOut("Restarting Sim File----------");
                        simPort.Close();
                        simPort = File.OpenText(simPortFilename);
                        nextLine = simPort.ReadLine();
                    }
                    if (nextLine.ToLower().StartsWith("received:"))
                    {
                        string newData = nextLine.Substring(10).Trim();
                        string[] newDataSplit = newData.Split(new char[] { ' ' });
                        int length = newDataSplit.Length;
                        byte[] bytes = new byte[length];
                        for (int i = 0; i < length; i++)
                        {
                            bytes[i] = byte.Parse(newDataSplit[i]);
                        }
                        receivedData(bytes, length);
                    }
                    else
                    {
                        continue;
                    }
                    Thread.Sleep(25);
                }
            }
            catch (Exception)
            {
                //
            }
            int me = 0;
        }

        // Received data from serial port
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //lock(sp)
            {
                byte[] readBuffer0 = new byte[4096];
                byte[] readBuffer = new byte[4096];

                int count = 0;                
                int numBytes;
                try
                {
                    do
                    {
                        //char[] test = sp.ReadExisting().ToCharArray();
                        numBytes = sp.Read(readBuffer, 0, readBuffer.Length);
                        receivedData(readBuffer, numBytes);
                    } while (false); // hack for other usb/serial devices: //numBytes != 0);
                }
                catch (Exception)
                {
                }
                int i = 0;
                //sp.DtrEnable = true;                
            }
        }

        // send data to robot
        private bool submit(byte[] data)
        {
            try
            {
                byte[] temp = new byte[data.Length + 5];

                short checksum = calcChecksum(data);
                Array.Copy(data, 0, temp, 3, data.Length);
                temp[0] = HEADER[0];
                temp[1] = HEADER[1];
                temp[2] = (byte)(data.Length + 2); // remember this cannot exceed 200!
                temp[temp.Length - 2] = (byte)(checksum >> 8);
                temp[temp.Length - 1] = (byte)(checksum & 0x00ff);

                // write out the data
                if (simPort != null)
                {
                }
                else if (sp.IsOpen)
                {
                    lock (sp)
                    {
                        sp.Write(temp, 0, temp.Length);
                    }
                }
                else
                {
                    Utils.errorOut("Cannot write to port");
                }

                long current = DateTime.Now.Ticks;
                lastToRobotTime = current;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        // calculate checksum for Pioneer 3
        private static short calcChecksum(byte[] data)
        {
            int c = 0, data1, data2;
            for (int x = 0; x < data.Length - 1; x += 2)
            {
                data1 = data[x]; if (data1 < 0) data1 += 256;
                data2 = data[x + 1]; if (data2 < 0) data2 += 256;
                c += ((data1 << 8) | data2);
                c = c & 0xffff;
            }
            if ((data.Length & 0x1) == 0x1)  // odd
            {
                data1 = data[data.Length - 1]; if (data1 < 0) data1 += 256;
                c = c ^ data1;
            }
            return (short)c;
        }
    }
}
