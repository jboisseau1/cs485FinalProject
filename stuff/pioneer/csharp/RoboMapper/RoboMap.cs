using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Robot
{
    public partial class RoboMap : UserControl
    {
        private GraphicsPath _robot = null;
        private GraphicsPath _sonar = null;
        private List<PointF> _sonarPoints = new List<PointF>();

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern int PostMessageA(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int SB_HORZ = 0x0;
        private const int SB_VERT = 0x1;
        private const int WM_HSCROLL = 0x114; // Horizontal scroll
        private const int WM_VSCROLL = 0x115; // Vertical scroll
        private const int SB_THUMBPOSITION = 0x4;

        // all units assume meters
        public class Position
        {
            public double velocityLeft = 0.0;
            public double velocityRight = 0.0;
            public double x = 0.0;
            public double y = 0.0;
            public double theta = 0.0;
            public DateTime time = DateTime.Now;
            public bool timeSet = false;

            public Position()
            {
                timeSet = false;
            }

            public Position(double x, double y, double theta, double velocityLeft, double velocityRight)
            {
                this.x = x;
                this.y = y;
                this.theta = theta;
                this.velocityLeft = velocityLeft;
                this.velocityRight = velocityRight;
                timeSet = false;
            }

            public void copy(Position source)
            {
                this.x = source.x;
                this.y = source.y;
                this.theta = source.theta;
                this.velocityLeft = source.velocityLeft;
                this.velocityRight = source.velocityRight;
                this.time = source.time;
            }
        }

        private Position _currentPosition = new Position();
        private Position _previousPosition = new Position();

        
        public RoboMap()
        {
            InitializeComponent();
            pictureBox1.Size = new System.Drawing.Size(1000, 1000);
        }

        private double l = 0.35;

        public void redraw()
        {
            pictureBox1.Invalidate();
        }

	    public double axleLength
	    {
		    get { return l;}
		    set { l = value;}
	    }

        ////////////////////
        // sonarAngle : degrees
        // sonarDistance : meters
        private Hashtable _sonarValues = new Hashtable();
        private double _maxDistance = 5000.0;
        public void updateRobotSonarInfo( double sonarAngle, double sonarDistance)
        {
            if (_sonarValues.ContainsKey(sonarAngle))
            {
                _sonarValues[sonarAngle] = sonarDistance;
            }
            else
            {
                _sonarValues.Add(sonarAngle, sonarDistance);
            }
        }
        
        ////////////////////
        // Perform position integration to determin theta, x and y
        public void updateRobotVelocity(double leftMetersPerSec, double rightMetersPerSec)
        {
            if (!_currentPosition.timeSet)
            {
                _currentPosition.time = DateTime.Now;
                _currentPosition.velocityLeft = leftMetersPerSec;
                _currentPosition.velocityRight = rightMetersPerSec;
                _currentPosition.timeSet = true;
            }
            else
            {
                TimeSpan ts = DateTime.Now - _currentPosition.time;
                double timeDelta = ts.Milliseconds / 1000.0;
                if (timeDelta > 1)
                {
                    // must be stepping through code
                    timeDelta = 0.125;
                }
                double theta = _currentPosition.theta + timeDelta * (_currentPosition.velocityRight - _currentPosition.velocityLeft) / l;
                double x = _currentPosition.x + (timeDelta / 2) * Math.Cos(_currentPosition.theta) * (_currentPosition.velocityRight + _currentPosition.velocityLeft);
                double y = _currentPosition.y + (timeDelta / 2) * Math.Sin(_currentPosition.theta) * (_currentPosition.velocityRight + _currentPosition.velocityLeft);

                _currentPosition.velocityLeft = leftMetersPerSec;
                _currentPosition.velocityRight = rightMetersPerSec;
                _currentPosition.x = x;
                _currentPosition.y = y;
                _currentPosition.theta = theta;
                _currentPosition.time = DateTime.Now;

                System.Console.WriteLine("x = " + _currentPosition.x + ", y = " + _currentPosition.y + ", theta = " + ( _currentPosition.theta * 180.0 / Math.PI));
            }            
        }

        ////////////////////
        // Robot may have built in position integration for dead reckoning
        public void updateRobotPosition(Position position)
        {
            _currentPosition.copy( position );
        }

        private bool checkIfReallyInDesignMode()
        {
            return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
        }

        //private Matrix _localMatrix = new Matrix();
        private GraphicsPath _drawSonar()
        {
            GraphicsPath sonar = new GraphicsPath();

            foreach (double sonarAngle in _sonarValues.Keys)
            {
                double sonarDistance = (double)_sonarValues[sonarAngle] / 100.0;
                double angleRad = sonarAngle * (Math.PI / 180.0);
                double x = sonarDistance * Math.Sin(angleRad);
                double y = sonarDistance * Math.Cos(angleRad);
                y *= -1;

                sonar.AddLine(new PointF(0, 0), new PointF((float)x, (float)y));
            }

            return sonar;
        }

        private GraphicsPath _drawRobot()
        {
            GraphicsPath robot = new GraphicsPath();
            robot.AddPolygon(new Point[] { new Point(0, -8), new Point(-6, -2), new Point(6, -2) });
            robot.AddLine(new Point(-16, 0), new Point(16, 0));
            robot.AddRectangle(new Rectangle(-20, -6, 4, 12));
            robot.AddRectangle(new Rectangle(16, -6, 4, 12));

            return robot;
        }

        private void RoboMap_Paint(object sender, PaintEventArgs e)
        {            
            Graphics g = e.Graphics;

            if (checkIfReallyInDesignMode())
            {
                g.FillRectangle(new SolidBrush(Color.DarkBlue), new Rectangle(0, 0, this.Width, this.Height));

                Font drawFont = new Font("Arial", 16);
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Center;

                g.DrawString("RoboMap", drawFont, 
                    new SolidBrush(Color.Yellow),
                    new RectangleF(0, this.Height/2 - (drawFont.Height/2), this.Width, this.Height), 
                    drawFormat);
                
                return;
            }
            pictureBox1.Invalidate();
        }

        private Bitmap _sonarMap = null;

        // should this start at rgb 255,255,255 and work it's way to 0,0,0???

        private void _setSonarPixel( int x, int y )
        {
            List<Point> nearPoints = new List<Point>();
            int checkDistance = 2;
            int colorIncrement = 5;
            //int colorStartValue = 10;

            // are there points already near this one?
            int x0 = x - checkDistance;
            int y0 = y - checkDistance;
            int x1 = x + checkDistance;
            int y1 = y + checkDistance;

            _sonarMap.SetPixel(x,y,_incrementPixelColor(_sonarMap.GetPixel(x,y),colorIncrement));

            try
            {
                for (int i = x0; i <= x1; i++)
                {
                    for( int j = y0; j <= y1; j++)
                    {
                        if (( i == x ) && ( j == y ))
                        {
                            continue;
                        }

                        Color pc = _sonarMap.GetPixel(i,j);
                        
                        if (( pc.R ) < 255)
                        {
                            pc = _incrementPixelColor(pc, colorIncrement);
                        }
                        _sonarMap.SetPixel(i,j,pc);
                        nearPoints.Add( new Point(i,j) );
                    }
                }
            }
            catch(Exception)
            {
            }
        }

        private Color _incrementPixelColor(Color pc, int colorIncrement)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            //if ((pc.R + colorIncrement) > 255)
            //{
            //    r = 255;
            //}
            //else if ((pc.G + colorIncrement) > 255)
            //{
            //    r = colorIncrement + pc.R;
            //}
            //else if ((pc.B + colorIncrement) > 255)
            //{
            //    g = colorIncrement + pc.G;
            //}
            //else
            //{
            //    b = colorIncrement + pc.B;
            //}
            r = pc.R - colorIncrement;
            g = pc.G - colorIncrement;
            b = pc.B - colorIncrement;

            pc = System.Drawing.Color.FromArgb(((int)(((byte)(r)))), ((int)(((byte)(g)))), ((int)(((byte)(b)))));
            return pc;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (checkIfReallyInDesignMode())
            {
                return;
            }

            if (_sonarMap == null)
            {
                _sonarMap = new Bitmap((sender as Control).Width, (sender as Control).Height);
                Graphics gMap = Graphics.FromImage(_sonarMap);
                gMap.Clear(Color.White);
            }

            Graphics g = e.Graphics;

            if (_autoscroll)
            {
                int horzScrollPos = (pictureBox1.Width / 2) - (picturePanel.Width / 2);
                int vertScrollPos = (pictureBox1.Height / 2) - (picturePanel.Height / 2);
                horzScrollPos -= (int)_currentPosition.x;
                vertScrollPos -= (int)_currentPosition.y;

                SetScrollPos((IntPtr)picturePanel.Handle, SB_HORZ, horzScrollPos, true);
                SendMessage((IntPtr)picturePanel.Handle, WM_HSCROLL, SB_THUMBPOSITION + 0x10000 * horzScrollPos, 0);

                SetScrollPos((IntPtr)picturePanel.Handle, SB_VERT, vertScrollPos, true);
                SendMessage((IntPtr)picturePanel.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * vertScrollPos, 0);
            }

            Matrix roboMatrix = new Matrix();
            Matrix localMatrix = new Matrix();

            // control center
            double x0 = this.pictureBox1.Height / 2;
            double y0 = this.pictureBox1.Width / 2;

            // Draw robot and sonar sweep
            _robot = _drawRobot();
            _sonar = _drawSonar();

            double theta1 = _currentPosition.theta;

            roboMatrix.Translate((float)(x0 - _currentPosition.x), (float)(y0 - _currentPosition.y)); ;
            roboMatrix.Rotate((float)theta1);

            // Draw sonar points                
            localMatrix.Translate((float)(x0 - _currentPosition.x) * -1, (float)(y0 - _currentPosition.y) * -1);
            PointF center = new PointF((float)(x0 - _currentPosition.x), (float)(y0 - _currentPosition.y));
            localMatrix.RotateAt(360 - (float)theta1, center);

            localMatrix.Invert();

            foreach (double sonarAngle in _sonarValues.Keys)
            {
                if ((double)_sonarValues[sonarAngle] == _maxDistance)
                    continue;

                double sonarDistance = (double)_sonarValues[sonarAngle] / 100.0;
                double angleRad = sonarAngle * (Math.PI / 180.0);
                double x = sonarDistance * Math.Sin(angleRad);
                double y = sonarDistance * Math.Cos(angleRad);
                y *= -1;

                PointF[] p = new PointF[1];
                p[0].X = (float)x;
                p[0].Y = (float)y;

                localMatrix.TransformPoints(p);

                _setSonarPixel((int)p[0].X, (int)p[0].Y);
            }

            _robot.Transform(roboMatrix);
            _sonar.Transform(roboMatrix);
            _previousPosition.copy(_currentPosition);

            g.DrawImage(_sonarMap, 0, 0);
            g.DrawPath(new Pen(Color.Yellow, 1), _sonar);
            g.DrawPath(new Pen(Color.Green, 1), _robot);   
        }

        private bool _autoscroll = true;
        private void btnAutoScroll_CheckedChanged(object sender, EventArgs e)
        {
            _autoscroll = btnAutoScroll.Checked;
        }

        private void btnCenterRobot_Click(object sender, EventArgs e)
        {
            int horzScrollPos = (pictureBox1.Width / 2) - (picturePanel.Width / 2);
            int vertScrollPos = (pictureBox1.Height / 2) - (picturePanel.Height / 2);

            horzScrollPos -= (int)_currentPosition.x;
            vertScrollPos -= (int)_currentPosition.y;

            SetScrollPos((IntPtr)picturePanel.Handle, SB_HORZ, horzScrollPos, true);
            SendMessage((IntPtr)picturePanel.Handle, WM_HSCROLL, SB_THUMBPOSITION + 0x10000 * horzScrollPos, 0);

            SetScrollPos((IntPtr)picturePanel.Handle, SB_VERT, vertScrollPos, true);
            SendMessage((IntPtr)picturePanel.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * vertScrollPos, 0);
        }

        private void btnSaveMap_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "JPEG(*.JPG)|*.JPG|PNG(*.PNG)|*.PNG|BMP(*.BMP)|*.BMP";
            saveDialog.FilterIndex = 1;
            saveDialog.RestoreDirectory = true;

            DialogResult result = saveDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                if (saveDialog.FilterIndex == 1)
                {
                    _sonarMap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);                    
                }
                else if (saveDialog.FilterIndex == 2)
                {
                    _sonarMap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                else if (saveDialog.FilterIndex == 3)
                {
                    _sonarMap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }

            }
        }

        private void RoboMap_Load(object sender, EventArgs e)
        {

        }

        private bool _localization = false;
        private void cbPerformLocalization_CheckedChanged(object sender, EventArgs e)
        {
            _localization = cbPerformLocalization.Checked;
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Filter = "JPEG(*.JPG)|*.JPG|PNG(*.PNG)|*.PNG|BMP(*.BMP)|*.BMP";
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;

            DialogResult result = openDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                _sonarMap = new Bitmap(openDialog.FileName);
            }
        }
    }
}