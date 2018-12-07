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
    public class iRobotCreateAdapter : IRobot
    {
        private double[] robotSonars = { 0, 60 };
        private double maxSonarDistance = 0xFFFF;
        
        private Queue<byte> receiveQueue = new Queue<byte>();

        private static bool stopThreads = false;
        private Thread pulseThread = null;
        private Thread infoThread = null;
        
        private Thread simReceiveThread = null;
        private bool simulatePort = false;
        private string simPortFilename;
        private StreamReader simPort = null;

        private int[] sonars = new int[16];

        private SerialPort sp;
        private long lastToRobotTime = 0;
        private bool moving = false;

        private string _name = "unknown";
        private string _type = "iRobot";
        private string _subtype = "unknown";

        public iRobotCreateAdapter(string commSettings)
        {
            initialize(commSettings);
        }

        public iRobotCreateAdapter()
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
                sp = new SerialPort(commSettings, 57600, Parity.None, 8, StopBits.One);
                sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            }
        }

        void sp_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            Utils.debugOut("pin changed " + e.ToString());
        }

        public double[] getSonarAngles()
        {
            return robotSonars;
        }

        public double getSonarMaxDistance()
        {
            return maxSonarDistance;
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

            // put iRobot in safe mode
            submit(new byte[] { 128, 133 }); //, 128, 131 });

            // set full control
            //submit(new byte[] { 128, 132 });

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
                if (simPort != null)
                {
                    simPort.Close();
                }
                else if (sp.IsOpen)
                {
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
            //if (arg)
            //    submit(new byte[] { 4, 0x3B, 1, 0 });
            //else
            //    submit(new byte[] { 4, 0x3B, 0, 0 });
            //submit(new byte[] { 152, 13, 137, 1, 44, 128, 0, 156, 1, 144, 137, 0, 0, 0, 0 } );
        }

        public void enableSonar(bool arg)
        {
            if (arg)
            {
                //submit(new byte[] { 28, 0x3B, 1, 0 });
            }
            else
            {
                //submit(new byte[] { 28, 0x3B, 0, 0 });
            }
        }

        public void resetDeadReckoning()
        {
            // reset local position to 0
            //submit(new byte[] { 7, 0x3B, 0, 0 });
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
            return 0;
        }

        public double getBattery()
        {
            return 0;
        }

        public double getLeftVelocity()
        {
            if (!this.isMoving())
            {
                return 0;
            }
            return 0;
        }

        public double getRightVelocity()
        {
            if (!this.isMoving())
            {
                return 0;
            }
            return 0;
        }

        public PointF getIntegratedPosition()
        {
            PointF p;
            p = new Point(0, 0);
            return p;
        }

        public double getIntegratedTheta()
        {
            return 0;
        }

        public int getSonarValue(int sonar)
        {
            return sonars[sonar];
        }

        public void sound(short arg)
        {
            //submit(new byte[] { 90, 0x3B, Utils.lowByte(arg), Utils.highByte(arg) });
        }

        public void stop()
        {
            //submit(new byte[] { 29 });
        }
        
        public void emergencyStop()
        {
            //submit(new byte[] { 55 });
        }

        public void setVelocity(byte left, byte right)
        {
            //submit(new byte[] { 128, 131, 145, 0, right, 0, left });
        }

        public void setHeadingAbsolute(short arg)
        {
            //
        }

        public void setHeadingRelative(short arg)
        {
            //
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
            // poll for all sensor states
            submit(new byte[] { 128, 142, 6 });
        }

        // TCM2 module commands; see P2 TCM2 manual for details
        private void tcm2(short arg)
        {
            //
        }

        // send two wheel velocity command to robot
        private void vel2(short arg)
        {
            //Utils.debugOut("Vel2: " + Utils.lowByte(arg) + ", " + Utils.highByte(arg));
            //submit(new byte[] { 32, 0x3B, Utils.lowByte(arg), Utils.highByte(arg) });
        }


        // After Sync0, 1, and 2 are send; the robot returns some info.  Get that info.
        private void getSync2Info()
        {
            // after sync2, robot returns information
            // three null terminated strings:
            //   name, type, and subtype
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

        class iRobotInfoPacket
        {
            public bool wheelDropCaster;
            public bool wheelDropLeft;
            public bool wheelDropRight;
            public bool bumpLeft;
            public bool bumpRight;

            public bool wall;
            public bool cliffLeft;
            public bool cliffFrontLeft;
            public bool cliffFrontRight;
            public bool cliffRight;
            public bool virtualWall;
            public bool LowSideDrive_OverCurrent_Left;
            public bool LowSideDrive_OverCurrent_Right;
            public bool LowSideDrive_OverCurrent_LD2;
            public bool LowSideDrive_OverCurrent_LD1;
            public bool LowSideDrive_OverCurrent_LD0;

            public byte infrared;

            public bool buttonAdvance;
            public bool buttonPlay;

            public int distanceTraveledInMillimeters;
            public int angle;

            public enum ChargeStatusType {
                NOT_CHARGING,
                RECONDITIONING_CHARGING,
                FULL_CHARGING,
                TRICKLE_CHARGING,
                WAITING,
                CHARGING_FAULT
            }
            public ChargeStatusType chargeStatus;

            public double voltage;
            public double current;

            public byte temperature;

            public int batteryRemaining;

            public int batterCapacity;

            public int wallSignal;

            public int cliffLeftSignal;
            public int cliffFrontLeftSignal;
            public int cliffFrontRightSignal;
            public int cliffRightSignal;
            public bool cargoDIO_0;
            public bool cargoDIO_1;
            public bool cargoDIO_2;
            public bool cargoDIO_3;

            public int cargoAnalogSignal;

            public bool homeBase;
            public bool internalCharger;

            public enum ModeType { OFF, PASSIVE, SAFE, FULL };
            public ModeType mode;
        }

        // thread to process serial data buffer
        private void infoLoop(object args)
        {
            while (!stopThreads)
            {
                try
                {
                    // get data from the robot
                    int[] gotMe = getRobotData(100);


                    // TODO parse info into iRobotInfoPacket
                    //

                    // update internal state variables (e.g. "sonar")
                }
                catch (Exception)
                {
                }
                Thread.Sleep(100);
            }
        }

        // get next string from serial port
        private int[] getRobotData(long timeout_ms)
        {
            int[] getme = new int[52];
            long timeOutTicks = DateTime.Now.Ticks + (timeout_ms * 10000);

            bool found = false;

            while (!found)
            {
                if (DateTime.Now.Ticks >= timeOutTicks)
                {
                    // timed out!
                    return getme;
                }

                if (receiveQueue.Count >= 52)
                {
                    lock (receiveQueue)
                    {
                        // first byte is correct, and queue is long enough, start checking
                        int i;
                        for (i = 0; i < 52; i++)
                        {
                            // typical info is 52 characters
                            byte b = receiveQueue.Dequeue();
                            getme[i] = (int)b;
                        }
                        return getme;
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
            Utils.debugOut(output); 
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
            {
                byte[] readBuffer = new byte[4096];

                int numBytes;
                try
                {
                    do
                    {
                        numBytes = sp.Read(readBuffer, 0, readBuffer.Length);
                        receivedData(readBuffer, numBytes);
                    } while (false);
                }
                catch (Exception)
                {
                }
            }
        }

        // send data to robot
        private bool submit(byte[] data)
        {
            try
            {
                // write out the data
                if (simPort != null)
                {
                }
                else if (sp.IsOpen)
                {
                    lock (sp)
                    {
                        sp.Write(data, 0, data.Length);
                        sp.WriteLine("");
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
    }
}
