using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Robot
{

    public partial class SonarViewer : UserControl
    {
        public class SonarElement
        {
            public SonarElement(int id, double angle, double distance)
            {
                this.id = id;
                this.angle = angle;
                this.distance = distance;
            }
            
            //public minDistance;

            public int id;
            public double angle;
            public double distance;
        }

        private ArrayList elements = new ArrayList();
        
        public double maxDistance = 5000.0;

        public SonarViewer()
        {
            InitializeComponent();            
        }

        public SonarElement getSonarElement( int id )
        {
            foreach( object o in elements )
            {
                SonarElement se = o as SonarElement;
                if ( se.id == id )
                {
                    return se;
                }
            }
            return null;
        }

        public void addSonarElement( SonarElement element )
        {
            elements.Add(element);
        }

        public bool updateSonarElement( int id, double distance )
        {
            SonarElement se = getSonarElement( id );
            if ( se == null )
                return false;

            se.distance = distance;
            this.Invalidate();
            return true;
        }

        private bool checkIfReallyInDesignMode()
        {
            return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
        }

        void SonarViewer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {            
            Graphics g = e.Graphics;

            if (checkIfReallyInDesignMode())
            {
                g.FillEllipse(new SolidBrush(Color.Blue), new Rectangle(0, 0, this.Width, this.Height));
                
                Font drawFont = new Font("Arial", 16);
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Center;

                g.DrawString("SonarViewer", drawFont,
                    new SolidBrush(Color.Yellow),
                    new RectangleF(0, this.Height / 2 - (drawFont.Height / 2), this.Width, this.Height),
                    drawFormat);
                return;
            }

                        
            PointF center = new PointF( this.Width / 2, this.Height / 2 );            
            PointF[] points = new PointF[ elements.Count ];
            Byte[] types = new Byte[elements.Count];
            int i = 0;
            foreach (object o in elements)
            {
                SonarElement se = o as SonarElement;
                double angleRad = se.angle * (Math.PI / 180.0);
                double scaledWidth = se.distance * this.Width / ( maxDistance * 2 );
                double scaledHeight = se.distance * this.Height / ( maxDistance * 2 );
                double x = scaledWidth * Math.Sin(angleRad);
                double y = scaledHeight * Math.Cos(angleRad);

                x += center.X;
                y = (this.Height / 2) - y;

                types[i] = (byte)(255.0 * se.distance / maxDistance);
                points[i++] = new PointF((float)x, (float)y);
            }

            try
            {
                //Brush brush = new PathGradientBrush( new GraphicsPath( points, types ) );
                Brush brush = new PathGradientBrush(points);

                //Brush brush = new SolidBrush(Color.Red);
                g.FillPolygon(brush, points);
                
                //Pen p = new Pen(Color.Blue, 5);
                //g.DrawPolygon(p, points);
            }
            catch
            {
                //System.Console.WriteLine("Error drawing sonar view");
            }
        }

    }
}