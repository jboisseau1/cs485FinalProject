using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Robot
{
    public partial class frmSampler : Form
    {
        public frmMain mainForm;

        public frmSampler()
        {
            InitializeComponent();
        }

        private void btnCW_Click(object sender, EventArgs e)
        {
            mainForm._movementCommand = 1;
        }

        private void btnCCW_Click(object sender, EventArgs e)
        {
            mainForm._movementCommand = 2;
        }

        private bool checkIfReallyInDesignMode()
        {
            return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
        }

        private void graph_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (checkIfReallyInDesignMode())
            {
                g.FillRectangle(new SolidBrush(Color.Crimson), new Rectangle(0, 0, this.Width, this.Height));
                return;
            }

            g.FillRectangle(new SolidBrush(Color.DarkBlue), new Rectangle(0, 0, this.Width, this.Height));

            int count = 360 / frmMain._sampleDegreeSize;
            int margin = 10;
            int spacing = (graph.Width - (2 * margin)) / count;

            for (int i = 0; i < count; i++)
            {
                Font drawFont = new Font("Arial", 12);
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Center;

                int degrees = i * frmMain._sampleDegreeSize;
                g.DrawString(degrees.ToString(), drawFont,
                    new SolidBrush(Color.Yellow),
                    ((spacing * i) + margin), graph.Height - 24,
                    drawFormat);
            }

            foreach (frmMain.particle p in mainForm._particles)
            {
                int h0 = graph.Height - 24 - margin;

                int w = Math.Max(spacing - 2, 2);
                int h = (int)((double)h0 * p.weight);

                int x = ((spacing * p.value) + margin);
                int y = h0 - h;

                Rectangle rect = new Rectangle(
                    x, y, w, h );
                g.FillRectangle(new SolidBrush(Color.Yellow), rect);
            }
        }
    }
}