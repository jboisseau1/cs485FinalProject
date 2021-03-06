using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Robot
{
    public partial class frmMain : Form
    {
        IRobot robot;

        private string _robotComPort = "COM10";  // KeySpan HS
        //private string _robotComPort = "COM6";  // Digi Port 1
        //private string _robotComPort = @"file:c:\develop\Robot\session5.txt";
        //private string _robotComPort = @"file:c:\develop\Robot\session6-spinning.txt";
        
        private Timer _samplingTimer = new Timer();
        private Timer _localizationTimer = new Timer();        

        public frmMain()
        {
            InitializeComponent();

            selectCOMPortToolStripMenuItem.Text = "Change port (" + _robotComPort + ")";

            robot = new Pioneer3Adapter();
            //robot = new iRobotCreateAdapter();

            Joystick.init();            

            int i = 0;
            double[] robotSonars = robot.getSonarAngles();
            double maxSonarDistance = robot.getSonarMaxDistance();
            foreach (double sonarAngle in robotSonars)
            {
                sonarViewer.addSonarElement(new Robot.SonarViewer.SonarElement(i++, sonarAngle, maxSonarDistance));
            }
            sonarViewer.maxDistance = (int)robot.getSonarMaxDistance();

            _samplingTimer.Tick += new EventHandler(_samplingTimer_Tick);
            _localizationTimer.Tick += new EventHandler(_localizationTimer_Tick);
        }

        private void updateSonarMap()
        {
            double[] robotSonars = robot.getSonarAngles();
            for ( int i = 0; i < robotSonars.Length; i++)
            {
                roboMap.updateRobotSonarInfo(robotSonars[i], robot.getSonarValue(i));
            }
        }

        private void _init()
        {
            robot.initialize(_robotComPort);
            bool openned = robot.open();
            if (openned)
            {
                robot.enableMotors(true);
                robot.enableSonar(true);
                robot.resetDeadReckoning();
                updateTimer.Start();
                explorerTimer.Start();
                this.Text = "Robot: " + robot.getName() + " (" + robot.GetType() + ", " + robot.getSubType() + ")";
            }
            initToolStripMenuItem.Checked = openned;
        }

        private string sonarToText(int range)
        {
            if (range == robot.getSonarMaxDistance())
            {
                return "?";
            }
            else
            {
                double meters = (double)range / 1000;
                return meters.ToString() + "m";
            }
        }

        private void pulseTimer_Tick(object sender, EventArgs e)
        {
            sonar0.Text = sonarToText(robot.getSonarValue(0));
            sonar1.Text = sonarToText(robot.getSonarValue(1));
            sonar2.Text = sonarToText(robot.getSonarValue(2));
            sonar3.Text = sonarToText(robot.getSonarValue(3));
            sonar4.Text = sonarToText(robot.getSonarValue(4));
            sonar5.Text = sonarToText(robot.getSonarValue(5));
            sonar6.Text = sonarToText(robot.getSonarValue(6));
            sonar7.Text = sonarToText(robot.getSonarValue(7));
            sonar8.Text = sonarToText(robot.getSonarValue(8));
            sonar9.Text = sonarToText(robot.getSonarValue(9));
            sonar10.Text = sonarToText(robot.getSonarValue(10));
            sonar11.Text = sonarToText(robot.getSonarValue(11));
            sonar12.Text = sonarToText(robot.getSonarValue(12));
            sonar13.Text = sonarToText(robot.getSonarValue(13));
            sonar14.Text = sonarToText(robot.getSonarValue(14));
            sonar15.Text = sonarToText(robot.getSonarValue(15));

            for (int i = 0; i < 16; i++)
            {
                sonarViewer.updateSonarElement(i, robot.getSonarValue(i));                
            }

            updateSonarMap();

            compass.Text = robot.getCompass().ToString();
            battery.Text = robot.getBattery().ToString() + "V";
            moving.Text = robot.isMoving().ToString();
            leftVel.Text = robot.getLeftVelocity().ToString();
            rightVel.Text = robot.getRightVelocity().ToString();
            lblX.Text = robot.getIntegratedPosition().X.ToString();
            lblY.Text = robot.getIntegratedPosition().Y.ToString();
            lblTheta.Text = robot.getIntegratedTheta().ToString();

            if (robot.isMoving())
            {
                roboMap.updateRobotPosition( new RoboMap.Position( 
                    robot.getIntegratedPosition().X * 10, robot.getIntegratedPosition().Y * 10,
                    robot.getIntegratedTheta(), robot.getLeftVelocity(), robot.getRightVelocity()));
                roboMap.redraw();
            }
        }

        private void _close()
        {
            updateTimer.Stop();
            robot.enableMotors(false);
            robot.enableSonar(false);
            robot.close();
            initToolStripMenuItem.Checked = false;
        }


        int y = 0;
        int x = 0;
        
        private void Joystick_PositionChanged(object sender, WinUI.MCS.Interfaces.PositionChangedEventArgs e)
        {
            if (e.move == WinUI.MCS.Interfaces.MCSConsts.Direction.UP)
            {
                y = e.moveValue; 
            }
            else if (e.move == WinUI.MCS.Interfaces.MCSConsts.Direction.DOWN)
            {
                y = -1 * e.moveValue; 
            }

            else if (e.move == WinUI.MCS.Interfaces.MCSConsts.Direction.RIGHT)
            {
                x = e.moveValue;
            }

            else if (e.move == WinUI.MCS.Interfaces.MCSConsts.Direction.LEFT)
            {
                x = -1 * e.moveValue;
            }
            
            int left = 0;
            int right = 0;

            System.Console.WriteLine("y = " + y + ", x = " + x);

            if ((x == 0) && (y != 0))
            {
                left = (int)((double)y * (127.0 / 100.0));
                right = (int)((double)y * (127.0 / 100.0));
            }
            else if ((y == 0) && (x != 0))
            {
                left = (int)((double)x * (127.0 / 100.0));
                right = (int)((double)x * (-127.0 / 100.0));
            }
            else
            {
                double x1 = -2.0 * (double)x / 100.0;
                double y1 = 2.0 * (double)y / 100.0;

                double left1 = (y1 - x1) / 2.0;
                double right1 = (y1 + x1) / 2.0;

                byte left2 = (byte)(((left1) * 127) / 4);
                byte right2 = (byte)(((right1) * 127) / 4);

                left = left2;
                right = right2;
            }

            if (left < 0)
            {
                left += 255;
            }

            if (right < 0)
            {
                right += 255;
            }


            if ((left == 0) && (right == 0))
            {
                robot.stop();
                System.Console.WriteLine("Stop");
            }
            else
            {
                robot.setVelocity((byte)left, (byte)right);
                System.Console.WriteLine("Left = " + left + ", Right = " + right);
            }
        }

        private bool _explore = false;


        private int averageSonar(int[] sonars)
        {
            int count = 0;
            int sum = 0;
            int min = 4999;
            for (int i = 0; i < sonars.Length; i++)
            {
                if ((sonars[i] != 0) && (sonars[i] != robot.getSonarMaxDistance()))
                {
                    if (sonars[i] < min)
                    {
                        min = sonars[i];
                    }
                    count++;
                    sum += sonars[i];
                }
            }
            int average = (int)robot.getSonarMaxDistance();
            if ( count != 0 )
                average = (int)((double)sum / (double)count);

            return average;
        }

        private int minSonar(int[] sonars)
        {
            int count = 0;
            int sum = 0;
            int min = 4999;
            for (int i = 0; i < sonars.Length; i++)
            {
                if (sonars[i] != 0)
                {
                    if (sonars[i] < min)
                    {
                        min = sonars[i];
                    }
                    count++;
                    sum += sonars[i];
                }
            }
            return min;
        }


        private bool near( int testValue, int idealValue )
        {
            if ( Math.Abs(testValue - idealValue) > 20 )
            {
                return true;
            }
            return false;
        }

        private int previousFrontSonar = 0;
        private int previousRightSonar = 0;
        private int previousLeftSonar = 0;
        private int previousBackSonar = 0;

        private bool pushKeyDown = false;
        private bool stopKeyDown = false;

        private int maxSpeed = 9;
        private int midSpeed = 4;
        private int minSpeed = 2;
        private int turnInc = 1;
        private int confidence = 0;
        private int hallWidth = 1200;
        private int forwardObstacle = 0;
        private int apparentHallWidth = 0;
        private Robot.Utils.DataTrend frontTrend = new Utils.DataTrend();
        private Robot.Utils.DataTrend backTrend = new Utils.DataTrend();
        private Robot.Utils.DataTrend leftTrend = new Utils.DataTrend();
        private Robot.Utils.DataTrend rightTrend = new Utils.DataTrend();

        private void explorerTimer_Tick(object sender, EventArgs e)
        {
            // Check for emergency stop
            if (stopKeyDown)
            {
                robot.emergencyStop();
                explorerTimer.Stop();
                return;
            }

            // get speed levels from GUI
            try
            {
                maxSpeed = int.Parse(txtMaxSpeed.Text);
                midSpeed = int.Parse(txtMidSpeed.Text);
                minSpeed = int.Parse(txtMinSpeed.Text);
                turnInc = int.Parse(txtTurnInc.Text);
            }
            catch (Exception)
            {
                // user may have made error editing... just ignore
            }

            ////////////////////
            // Gather sonar data

            // Front Distance
            int frontSonar = averageSonar(new int[] { robot.getSonarValue(3), robot.getSonarValue(4) });
            int frontSonarWide = minSonar(new int[] { robot.getSonarValue(2), robot.getSonarValue(5) });
            frontSonar = ( frontSonarWide < 300 ? Math.Min( frontSonarWide,frontSonar ) : frontSonar );
            frontTrend.addDataPoint(frontSonar);
            lblFront.Text = sonarToText(frontSonar);

            // Back Distance
            int backSonar = minSonar(new int[] { robot.getSonarValue(10), robot.getSonarValue(11), robot.getSonarValue(12), robot.getSonarValue(13) });
            backTrend.addDataPoint(backSonar);
            lblBack.Text = sonarToText(backSonar);
            
            // Left Distance
            int leftSonar = averageSonar(new int[] { robot.getSonarValue(0), robot.getSonarValue(15) });
            leftTrend.addDataPoint(leftSonar);
            lblLeft.Text = sonarToText(leftSonar);
            
            // Right Distance
            int rightSonar = averageSonar(new int[] { robot.getSonarValue(7), robot.getSonarValue(8) });
            rightTrend.addDataPoint(rightSonar);
            lblRight.Text = sonarToText(rightSonar);

            // Check Tolerances
            bool pushingMe = ((backSonar < 750) | pushKeyDown);
            if (forwardObstacle > 0)
            {
                pushingMe = true;
            }
            bool frontBlocked = frontSonar < 600;
            bool tooCloseOnFront = frontSonar < 1200;
            bool tooCloseOnLeft = (leftSonar < 400);
            if (Math.Min(robot.getSonarValue(1), robot.getSonarValue(2)) < 200)
            {
                tooCloseOnLeft = true;
            }

            bool tooCloseOnRight = (rightSonar < 400);
            if (Math.Min(robot.getSonarValue(6), robot.getSonarValue(5)) < 200)
            {
                tooCloseOnRight = true;
            }

            bool closeOnLeft = (leftSonar < 500);
            bool closeOnRight = (rightSonar < 500 );
            
            // Update GUI for tolernace
            if (pushingMe)
                lblBack.BackColor = Color.Red;
            else
                lblBack.BackColor = this.BackColor;

            if (frontBlocked)
                lblFront.BackColor = Color.Red;
            else if (tooCloseOnFront)
                lblFront.BackColor = Color.Yellow;
            else
                lblFront.BackColor = this.BackColor;


            if (tooCloseOnLeft)
                lblLeft.BackColor = Color.Red;
            else if (closeOnLeft)
                lblLeft.BackColor = Color.Yellow;
            else
                lblLeft.BackColor = this.BackColor;

            if (tooCloseOnRight)
                lblRight.BackColor = Color.Red;
            else if (closeOnRight)
                lblRight.BackColor = Color.Yellow;
            else
                lblRight.BackColor = this.BackColor;

            // Check last n left/right data points to see if we going left, right, or kinda straight
            double lTrend = leftTrend.getDataTrend();
            double rTrend = rightTrend.getDataTrend();
            
            // Ignore the wall we are furthest from
            double minLeftTrend = 0;            
            if ( leftSonar < rightSonar )
            {
                minLeftTrend = lTrend;
            }
            else
            {
                minLeftTrend = -1 * rTrend;
            }
            bool movingRight = (minLeftTrend > 0);
            bool movingLeft = !movingRight;

            // Filter out small changes so we are not always updating speed
            double filterLevel = 3;
            if (Math.Abs(minLeftTrend) < filterLevel)
            {
                minLeftTrend = 0;
                movingRight = false;
                movingLeft = false;
            }
            else
            {
                movingRight = (minLeftTrend > 0);
                movingLeft = !movingRight;
            }

            // Update GUI for data trend
            if (movingRight)
            {
                //System.Console.WriteLine("Looks like I am Moving Right " + minLeftTrend);
                lblMoveDir.Text = "Right " + minLeftTrend;
            }
            else if (movingLeft)
            {
                //System.Console.WriteLine("Looks like I am Moving Left " + minLeftTrend);
                lblMoveDir.Text = "Left " + ( -1 * minLeftTrend );
            }
            else
            {
                lblMoveDir.Text = "Straight-ish";
            }

            // Wheel settings
            int left = 0;
            int right = 0;
            int heading = 0;

            // Now process the info and determine what to do
            if (pushingMe)
            {
                apparentHallWidth = leftSonar + rightSonar;
                lblHallWidth.Text = apparentHallWidth.ToString();

                // First check for forward obsticles
                if (pushingMe && (frontBlocked /*|| tooCloseOnFront*/ || ( forwardObstacle > 0 ) ))
                {
                    confidence = 0;

                    if (frontBlocked || ( forwardObstacle == 1 ) )
                    {                        
                        forwardObstacle = 1;
                        int test = (((int)robot.getLeftVelocity() & (int)robot.getRightVelocity()));
                        if (((test != 0xFFFF) && (test != 0)) && ((test & 0xFF00) == 0))
                        {
                            // going forward!!!  Stop!!!
                            robot.emergencyStop();
                            //System.Console.WriteLine("STOP!!!");
                        }
                        else
                        {
                            left = (-1 * minSpeed);
                            right = (-1 * minSpeed);
                            //System.Console.WriteLine("Too close to front, backing up");
                        }
                        if (!frontBlocked && tooCloseOnFront)
                        {
                            forwardObstacle = 2;
                            left = 0;
                            right = 0;
                        }
                        else if (!frontBlocked && !tooCloseOnFront)
                        {
                            forwardObstacle = 0;
                            left = 0;
                            right = 0;
                        }
                    }
                    else if ( forwardObstacle == 2 )                    
                    {
                        if (robot.isMoving())
                        {
                            robot.stop();
                        }
                        else
                        {
                            forwardObstacle = 3;
                        }
                    }
                    else if ( forwardObstacle == 3 )
                    {
                        //System.Console.WriteLine("Blocked... rotating");

                        if (leftSonar < rightSonar)
                        {
                            heading = -180;
                        }
                        else
                        {
                            heading = 180;
                        }

                        forwardObstacle = 4;
                    }
                    else if (forwardObstacle == 4)
                    {
                        // need to wait for heading to start
                        if (robot.isMoving())
                        {
                            forwardObstacle = 5;
                        }
                    }
                    else if (forwardObstacle == 5)
                    {
                        // need to wait for heading to stop
                        if (!robot.isMoving())
                        {
                            forwardObstacle = 0;
                        }
                    }
                }
                // too close on all sides, be careful
                else if (closeOnLeft && closeOnRight)
                {
                    // tight fit
                    //System.Console.WriteLine("Tight fit on sides, moving carefully.");
                    confidence = 0;
                    left = minSpeed;
                    right = minSpeed;
                }
                // only too close on left
                else if (tooCloseOnLeft)
                {
                    //System.Console.WriteLine("Too close on the left.");
                    left = minSpeed + (2 * turnInc);
                    right = minSpeed - ( 2 * turnInc );
                }
                // only too close on right
                else if (tooCloseOnRight)
                {
                    left = minSpeed - (2 * turnInc);
                    right = minSpeed + (2 * turnInc);
                }
                // somewhere in the middle of where we are
                else
                {
                    // try to center ourselves, check if distance on left is less than half hall width
                    if (leftSonar < (hallWidth / 2))
                    {
                        //System.Console.WriteLine("we think we need to go closer to the right");
                        // we need to go right, are we going right?
                        if (movingRight)
                        {
                            //System.Console.WriteLine(" -Continuing to move to right");
                            confidence = 0;
                            left = midSpeed;
                            right = midSpeed;
                        }
                        else
                        {
                            //System.Console.WriteLine(" -Moving more to the right");
                            confidence = 0;
                            if (closeOnLeft)
                            {
                                left = midSpeed + turnInc;
                                right = minSpeed - turnInc;
                            }
                            else
                            {
                                left = midSpeed;
                                right = midSpeed - turnInc;
                            }
                        }
                    }
                    // try to center ourselves, check if distance on right is less than half hall width
                    else if (rightSonar < (hallWidth / 2))
                    {
                        // we need to go left, are we going left?
                        //System.Console.WriteLine("we think we need to go closer to the left");
                        if (movingLeft)
                        {
                            //System.Console.WriteLine(" -Continuing to move to the left");
                            confidence = 0;
                            left = midSpeed;
                            right = midSpeed;
                        }
                        else
                        {
                            //System.Console.WriteLine(" -Moving more to the left");
                            if (closeOnRight)
                            {
                                left = minSpeed;
                                right = midSpeed + turnInc;
                            }
                            else
                            {
                                confidence = 0;
                                left = midSpeed - turnInc;
                                right = midSpeed;
                            }
                        }
                    }
                    // in this case we seem to be in the middle of the hall...
                    else
                    {                        
                        confidence++;
                        // do we need to straighten the robot out?
                        if (movingRight)
                        {
                            //System.Console.WriteLine("We are in the center, but going a little to the right.  Straightening out.");
                            left = (Math.Abs(minLeftTrend) > 100) ? maxSpeed - 2 : midSpeed;
                            right = maxSpeed;
                        }
                        else if (movingLeft)
                        {
                            //System.Console.WriteLine("We are in the center, but going a little to the left.  Straightening out.");
                            left = maxSpeed;
                            right = maxSpeed - ((Math.Abs(minLeftTrend) > 100) ? 3 : 2); //((Math.Abs(minLeftTrend) / 20.0) > maxSpeed ? maxSpeed : (int)(Math.Abs(minLeftTrend) / 20.0));
                        }
                        else if (confidence > 20)
                        {
                            //System.Console.WriteLine("Centered, and confident, moving max speed");
                            left = maxSpeed;
                            right = maxSpeed;
                        }
                        else
                        {
                            //System.Console.WriteLine("Centered, moving mid speed");
                            left = midSpeed;
                            right = midSpeed;
                        }
                    }
                }
            } // end of if pushingMe
            else
            {
                // if not pushing us, then roll to a stop
                if ( _explore )
                    robot.stop();
            }

            // process movement commands
            if (heading != 0)
            {
                if (_explore)
                {
                    left = 0;
                    right = 0;
                    robot.setHeadingRelative((short)heading);
                }
            }
            else if ( ( left != 0 ) || ( right != 0 ) )
            {
                if (_explore)
                {
                    if (tooCloseOnFront)
                    {
                        left /= 2;
                        right /= 2;
                    }
                    robot.setVelocity((byte)left, (byte)right);
                }
            }

            // store distances so we can determine changes next time
            previousFrontSonar = frontSonar;
            previousRightSonar = rightSonar;
            previousLeftSonar = leftSonar;
            previousBackSonar = rightSonar;
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.ShiftKey )
            {
                pushKeyDown = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                stopKeyDown = true;
            }           
        }

        private void btnStartExplore_KeyUp(object sender, KeyEventArgs e)
        {
            pushKeyDown = false;
            stopKeyDown = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            _init();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _close();
        }

        private Timer _waitAndSpinTimer = new Timer();
        
        void _waitAndSpinTimer_Tick(object sender, EventArgs e)
        {
            if (robot.isMoving())
            {
                robot.setVelocity(0, 0);
            }
            else
            {
                // spin
                robot.setVelocity(6, 249);
            }
        }

        private void initToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _init();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _close();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            previousFrontSonar = 0;
            previousRightSonar = 0;
            previousLeftSonar = 0;
            previousBackSonar = 0;
            _explore = true;
            pushKeyDown = false;
            exploreHallToolStripMenuItem.Checked = true;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            robot.stop();
            _explore = false;
            pushKeyDown = false;
            exploreHallToolStripMenuItem.Checked = false;
        }

        private void waitAndSpinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_waitAndSpinTimer.Enabled == false)
            {
                _waitAndSpinTimer.Interval = 20000;
                _waitAndSpinTimer.Start();
                _waitAndSpinTimer.Tick += new EventHandler(_waitAndSpinTimer_Tick);
                waitAndSpinToolStripMenuItem.Checked = true;
            }
            else //running, let's stop it
            {
                _waitAndSpinTimer.Stop();
                robot.setVelocity(0, 0);
                waitAndSpinToolStripMenuItem.Checked = false;
            }
        }

        private void exploreHallToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void selectCOMPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _robotComPort = Microsoft.VisualBasic.Interaction.InputBox("COM Port, enter in form of \"COMi\"", "Robot COM Port", "COM12", this.Left, this.Top);
            selectCOMPortToolStripMenuItem.Text = "Change port (" + _robotComPort + ")";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void emergancyStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // stop all timers and threads
            _explore = false;
            _waitAndSpinTimer.Stop();
            robot.emergencyStop();
            robot.close();
        }

        #region Particle Filter

        public static int _sampleDegreeSize = 10;
        private static int _particleCount = 200;
        private static int _sampleCount = 10;
        //private static int[] _sampleSonars = new int[] { 2, 3, 4, 11, 12, 14 };
        private static int[] _sampleSonars = new int[] { 3, 4, 11, 12, 14 };
        //private static int[] _sampleSonars = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };


        private static int _sampleInc = 0;
        private static int[, ,] _sensorModelCapture = new int[360 / _sampleDegreeSize, _sampleSonars.Length, _sampleCount];
        private static int[,] _sensorModel = new int[360 / _sampleDegreeSize, _sampleSonars.Length];

        private static double[, ,] _actionModel = new double[2, 360 / _sampleDegreeSize, 360 / _sampleDegreeSize];

        private bool _sampleMoveToLocation = false;
        private bool _sampleMovingToLocation = false;
        private bool _sampleCollecting = false;
        private int _sampleLocation = 0;
        private frmSampler _sampleForm = new frmSampler();
        private DateTime _sampleStartTime;
        public List<particle> _particles = new List<particle>();
        private bool _particlesReady = false;
        
        private void samplingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_samplingTimer.Enabled)
            {
                samplingToolStripMenuItem.Checked = false;
                _samplingTimer.Stop();
            }
            else
            {
                samplingToolStripMenuItem.Checked = true;
                _fillInActionModel();
                _samplingTimer.Start();
            }
        }

        
        void _samplingTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!_sampleForm.Visible)
                {
                    _sampleForm.Show();
                    _sampleForm.mainForm = this;
                }
            }
            catch (Exception)
            {
                _sampleForm = new frmSampler();
            }

            if ( ( !_sampleMoveToLocation ) && ( !_sampleMovingToLocation) && ( !_sampleCollecting ) )
            // not collecting or moving, lets move to the next location and start
            {
                _sampleForm.status.Text = "Prepare to move to location " + _sampleLocation;
                //if (_sampleLocation == 0)
                //{
                //    int theta = 360 - (int)robot.getCompass();
                //    robot.setHeading((short)theta);
                //}
                //else
                {
                    robot.setHeadingAbsolute((short)_sampleLocation);
                }
                _sampleMoveToLocation = true;
            }

                // maybe should instead wait for compass...

            else if ((_sampleMoveToLocation) && (!_sampleMovingToLocation) && (!_sampleCollecting))
            // are we moving to location yet?
            {
                _sampleForm.status.Text = "Waiting to move to location " + _sampleLocation;
                if (robot.isMoving())
                {
                    _sampleMovingToLocation = true;
                }
            }
            else if ((_sampleMoveToLocation) && (_sampleMovingToLocation) && (!_sampleCollecting))
            // are we there yet?
            {
                _sampleForm.status.Text = "Moving to location " + _sampleLocation;
                if (!robot.isMoving())
                {
                    _sampleMoveToLocation = false;
                    _sampleMovingToLocation = false;
                    _sampleCollecting = true;
                    _sampleInc = 0;
                    _sampleStartTime = DateTime.Now;
                }
            }
            else if ((!_sampleMoveToLocation) && (!_sampleMovingToLocation) && (_sampleCollecting))
            // we made it to a new position, start collecting
            {                
                int i = _sampleLocation / _sampleDegreeSize;
                int currentTimeDeltaTicks = (int)(DateTime.Now.Ticks - _sampleStartTime.Ticks);

                _sampleForm.status.Text = "Collecting data for " + i + " location.  Sample Point: " + currentTimeDeltaTicks;

                for( int s = 0; s < _sampleSonars.Length; s++)                    
                {
                    _sensorModelCapture[i, s, _sampleInc] =
                        robot.getSonarValue(_sampleSonars[s]);
                }

                _sampleInc++;
                if (_sampleInc >= _sampleCount)
                {                
                    _sampleCollecting = false;
                    _sampleLocation += _sampleDegreeSize;
                    if (_sampleLocation >= 360)
                    {
                        _samplingTimer.Stop();
                        
                        // done sampling
                        _dumpSonarModel();

                        _sampleForm.status.Text = "Sensor model data point collection complete";

                        samplingToolStripMenuItem.Checked = false;
                    }
                }
            }
        }

        private void _fillInActionModel()
        {
            for (int i = 0; i < 360; i = i + _sampleDegreeSize)
            {
                // forward model
                int j = i / _sampleDegreeSize;
                _actionModel[0, j, j] = 0.25;
                if (i >= (360 - _sampleDegreeSize))
                    _actionModel[0, j, 0] = 0.75;
                else
                    _actionModel[0, j, j + 1] = 0.75;

                // backward model
                _actionModel[0, j, j] = 0.25;
                if (i == 0)
                    _actionModel[0, j, ((360 / _sampleDegreeSize) - 1)] = 0.75;
                else
                    _actionModel[0, j, 0] = 0.75;
            }
        }

        private void _dumpSonarModel()
        {
            StreamWriter fileOut = new StreamWriter(@"c:\sonar_model.txt", true);
            fileOut.WriteLine("Dump Sonar Model: " + DateTime.Now.ToString());

            fileOut.Write("Degree Increment, Sample Number");
            for (int s = 0; s < _sampleSonars.Length; s++)
            {
                fileOut.Write(", Sonar " + _sampleSonars[s]);
            }

            fileOut.WriteLine();

            for( int deg = 0; deg < 360; deg = deg + _sampleDegreeSize )
            {
                int[] sonarAvgs = new int[_sampleSonars.Length];
                for (int i = 0; i < _sampleCount; i++)
                {
                    fileOut.Write(deg + ", " + i);
                    for (int s = 0; s < _sampleSonars.Length; s++)
                    {
                        int j = deg / _sampleDegreeSize;
                        fileOut.Write(", " + _sensorModelCapture[j, s, i]);
                        sonarAvgs[s] += _sensorModelCapture[j, s, i];
                    }                    
                    fileOut.WriteLine();
                }
                fileOut.Write(deg + ", average");
                for (int i = 0; i < sonarAvgs.Length; i++)
                {
                    sonarAvgs[i] /= _sampleCount;
                    int j = deg / _sampleDegreeSize;
                    _sensorModel[j, i] = sonarAvgs[i];
                    fileOut.Write(", " + sonarAvgs[i]);
                }
                fileOut.WriteLine();
            }
            fileOut.Close();
        }

        private void runRotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_localizationTimer.Enabled)
            {
                localizationToolStripMenuItem.Checked = false;
                _localizationTimer.Stop();
            }
            else
            {
                _sampleMoveToLocation = false;
                _sampleMovingToLocation = false;
                _sampleCollecting = false;                
                _sampleLocation = 0;
                _particlesReady = false;

                localizationToolStripMenuItem.Checked = true;
                
                _localizationTimer.Interval = 750;
                _localizationTimer.Start();
            }
        }

        public int _movementCommand = 0;
        
        public class particle
        {
            public int value;
            public double weight;
            public particle( int value, double weight )
            {
                this.value = value;
                this.weight = weight;
            }
        }
        
        private void _checkParticleFitness( int action )
        {
            int[] sonars = new int[_sampleSonars.Length];
 
            for (int s = 0; s < _sampleSonars.Length; s++)
            {
                sonars[s] = 
                    robot.getSonarValue(_sampleSonars[s]);
            }

            double worstDif = robot.getSonarMaxDistance() * _sampleSonars.Length;

            if ( action == 0 )
            {
                particle max = null;
                // initialize
                foreach (particle p in _particles)
                {
                    double dif = 0;
                    for (int s = 0; s < _sampleSonars.Length; s++)
                    {
                        dif += Math.Abs(_sensorModel[p.value,s] - sonars[s]);
                    }
                    dif /= _sampleSonars.Length;
                    p.weight = dif;
                    p.weight /= worstDif;
                    p.weight = 1 - p.weight;
                    
                    if ((max == null) || (max.weight <= p.weight))
                    {
                        max = p;
                    }

                    //p.weight = 0.25;
                    Utils.debugOut("location: " + (p.value * _sampleDegreeSize) + ", dif: " + dif + ", weight: " + p.weight);
                }
                foreach (particle p in _particles)
                {
                    if (p != max)
                    {
                        p.weight *= 0.75;
                    }
                    //p.weight *= 0.25;
                }
            }
            else
            {
                particle max = null;

                foreach (particle p in _particles)
                {
                    int newValue;
                    if (action == 2) // CW
                    {

                        newValue = p.value - 1;
                        if (newValue < 0)
                            newValue = (360 / _sampleDegreeSize) - 1;
                    }
                    else
                    {
                        newValue = p.value + 1;
                        if (newValue >= (360 / _sampleDegreeSize))
                            newValue = 0;
                    }

                    double dif = 0;
                    double oldWeight = p.weight;

                    for (int s = 0; s < _sampleSonars.Length; s++)
                    {
                        dif += Math.Abs(_sensorModel[newValue, s] - sonars[s]);
                    }

                    dif /= _sampleSonars.Length;
                    double weight = dif;
                    weight /= worstDif;
                    weight = 1 - weight;

                    if (weight > oldWeight)
                    {                        
                        p.weight *= 1.25;
                        if (p.weight > 1.0)
                        {
                            p.weight = 0.99;
                        }
                        Utils.debugOut("location: " + (p.value * _sampleDegreeSize) + " more likely (" + p.weight + ")");
                    }
                    else if (weight == oldWeight)
                    {
                        p.weight = weight;
                    }
                    else
                    {
                        if (oldWeight > 0.95)
                        {
                            p.weight *= 0.9;
                        }
                        else if (oldWeight > 0.9)
                        {
                            p.weight *= 0.75;
                        }
                        else
                        {
                            p.weight *= 0.5;
                        }
                        Utils.debugOut("location: " + (p.value * _sampleDegreeSize) + " less likely (" + p.weight + ")");                        
                    }
                    p.value = newValue;

                    if ((max == null) || (max.weight <= p.weight))
                    {
                        max = p;
                    }
                }
                foreach (particle p in _particles)
                {
                    if (p != max)
                    {
                        p.weight *= 0.80;
                    }
                }
            }
        }

        private void _sampleFormUpdateGraph()
        {
            _sampleForm.graph.Invalidate();
        }

        private void _localizationTimer_Tick(object sender, EventArgs e)
        {
            _sampleFormUpdateGraph();
            if (!_particlesReady)
            {
                // create a buncha particles
                Random rand = new Random();
                for (int i = 0; i < _particleCount; i++)
                {
                    particle p = new particle(rand.Next(0, (360/_sampleDegreeSize)), 0);
                    _particles.Add(p);
                }
                
                // initial fitness check
                _checkParticleFitness( 0 );
                
                _particlesReady = true;
            }            

            // wait for movement command            
            if (_movementCommand < 0)
            {
                // robot was moved
                // check predicition - once robot stops moving
                if (!robot.isMoving())
                {
                    _checkParticleFitness(-1 * _movementCommand);
                    _movementCommand = 0;
                }                
            }
            else if (_movementCommand == 1)
            {
                _movementCommand *= -1;
                // move robot CW
                robot.setHeadingRelative((short)(_sampleDegreeSize));
            }
            else if (_movementCommand == 2)
            {
                _movementCommand *= -1;
                // move robot CCW
                robot.setHeadingRelative((short)(_sampleDegreeSize * -1));
            }
        }
        #endregion
    }
}