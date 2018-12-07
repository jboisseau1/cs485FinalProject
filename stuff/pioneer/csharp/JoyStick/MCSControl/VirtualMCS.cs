using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using WinUI.MCS;
using WinUI.MCS.Interfaces;

namespace WinUI.MCS
{
    /// <summary>
    /// Summary description for UserControl1.
    /// </summary>
    public class VirtualMCS : System.Windows.Forms.UserControl
    {

        public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);
        public event PositionChangedEventHandler PositionChanged;

        private DirectionalInput MCSInput;

        private WinUI.MCS.VirtualJoystick virtualJoystick1;

        private bool keyDown = false;

        private Thread pollInputThread;
        private bool stopThreads = false;
        private bool threadsRunning = false;

        private bool initialized = false;

        //private System.Windows.Forms.RichTextBox richTextBox1;

        private int _guiPollingRate = 250;

        public int guiPollingRate
        {
            get
            {
                return _guiPollingRate;
            }
            set
            {
                _guiPollingRate = value;
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public VirtualMCS()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

        }

        public void init()
        {
            if (initialized)
                return;

            // todo: add any initialization after the initcomponent call
            this.MCSInput = new DirectionalInput();
            this.MCSInput.Movement += new MovementEventHandler(this.MCSInput_Movement);

            // start polling thread            
            pollInputThread = new Thread(new ThreadStart(pollInput));

            pollInputThread.Start();

            initialized = true;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            stopThreads = true;
            int killCount = 0;
            try
            {
                while ((threadsRunning) && (killCount < 3))
                {
                    Thread.Sleep(100);
                    stopThreads = true;
                    killCount++;
                }
                if (killCount >= 3)
                {
                    pollInputThread.Abort();
                }
            }
            catch (Exception)
            {
            }


            if (MCSInput != null)
            {
                MCSInput.Dispose();
            }
            MCSInput = null;

            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.virtualJoystick1 = new WinUI.MCS.VirtualJoystick();
            this.SuspendLayout();
            // 
            // virtualJoystick1
            // 
            this.virtualJoystick1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(179)))), ((int)(((byte)(179)))));
            this.virtualJoystick1.down = 0;
            this.virtualJoystick1.left = 0;
            this.virtualJoystick1.Location = new System.Drawing.Point(8, 8);
            this.virtualJoystick1.Name = "virtualJoystick1";
            this.virtualJoystick1.right = 0;
            this.virtualJoystick1.Size = new System.Drawing.Size(160, 160);
            this.virtualJoystick1.TabIndex = 12;
            this.virtualJoystick1.up = 0;
            this.virtualJoystick1.PositionChanged += new WinUI.MCS.VirtualJoystick.PositionChangedEventHandler(this.virtualJoystick1_PositionChanged);
            // 
            // VirtualMCS
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(179)))), ((int)(((byte)(179)))));
            this.Controls.Add(this.virtualJoystick1);
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Name = "VirtualMCS";
            this.Size = new System.Drawing.Size(180, 179);
            this.Load += new System.EventHandler(this.VirtualMCS_Load);
            this.ResumeLayout(false);

        }
        #endregion

        private void virtualJoystick1_PositionChanged(object sender, WinUI.MCS.VirtualJoystick.PositionChangedEventArgs e)
        {
            if (e.horizontalMove != MCSConsts.Direction.UNDEFINED)
            {
                sendMCSCommand(e.horizontalMove, e.horizontalMoveValue);
            }

            if (e.verticalMove != MCSConsts.Direction.UNDEFINED)
            {
                sendMCSCommand(e.verticalMove, e.verticalMoveValue);
            }
        }

        private void MCSInput_Movement(object sender, MovementEventArgs e)
        {
            string whatsHappening = MCSConsts.directionString[(int)e.moveDirection];
            whatsHappening += " " + e.moveValue;
            int absValue = Math.Abs(e.moveValue);
            switch (e.moveDirection)
            {
                case MCSConsts.Direction.LEFT:
                    {
                        this.virtualJoystick1.Focus();
                        this.virtualJoystick1.left = absValue;
                        break;
                    }
                case MCSConsts.Direction.RIGHT:
                    {
                        this.virtualJoystick1.Focus();
                        this.virtualJoystick1.right = absValue;
                        break;
                    }
                case MCSConsts.Direction.UP:
                    {
                        this.virtualJoystick1.Focus();
                        this.virtualJoystick1.up = absValue;
                        break;
                    }
                case MCSConsts.Direction.DOWN:
                    {
                        this.virtualJoystick1.Focus();
                        this.virtualJoystick1.down = absValue;
                        break;
                    }
            }
        }


        private void sendMCSCommand(MCSConsts.Direction moveDirection, int moveValue)
        {
            string whatsHappening = MCSConsts.directionString[(int)moveDirection];
            whatsHappening += " " + moveValue;
            //richTextBox1.Text = whatsHappening + "\n" + richTextBox1.Text;
            PositionChangedEventArgs e = new PositionChangedEventArgs();
            e.move = moveDirection;
            e.moveValue = moveValue;
            OnPositionChanged(e);
            //richTextBox1.Select( richTextBox1.Text.Length, 0 );	
        }


        private delegate void SetTextPropertyDelegate(object control, string text);
        private void SetTextProperty(object control, string text)
        {
            //if (control is TextBoxBase)
            {
                (control as TextBoxBase).Text = text;
            }
        }

        private delegate string GetTextPropertyDelegate(object control);
        private string GetTextProperty(object control)
        {
            //if (control is RichTextBox)
            {
                return (control as TextBoxBase).Text;
            }
        }

        private delegate int GetControlValueDelegate(object control);
        private int GetControlValue(object control)
        {
            return (control as TrackBar).Value;
        }

        private void pollInput()
        {
            threadsRunning = true;
            int[] lastValues = new int[MCSConsts.directionString.Length];
            int[] currValues = new int[MCSConsts.directionString.Length];

            while (!stopThreads)
            {
                try
                {
                    // poll for input

                    // from Virual Joystick
                    currValues[(int)MCSConsts.Direction.LEFT] = virtualJoystick1.left;
                    currValues[(int)MCSConsts.Direction.RIGHT] = virtualJoystick1.right;
                    currValues[(int)MCSConsts.Direction.UP] = virtualJoystick1.up;
                    currValues[(int)MCSConsts.Direction.DOWN] = virtualJoystick1.down;

                    for (int i = 0; i < MCSConsts.directionString.Length; i++)
                    {
                        // only if changed
                        if (lastValues[i] != currValues[i])
                        {
                            lastValues[i] = currValues[i];
                            sendMCSCommand((MCSConsts.Direction)i, currValues[i]);
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }

                Thread.Sleep(_guiPollingRate);


            }
            threadsRunning = false;
        }

        private void RaiseEvent(Delegate eventDelegate, object[] args)
        {
            if (eventDelegate != null)
            {
                try
                {
                    if (((Control)eventDelegate.Target).InvokeRequired)
                    {
                        ((Control)eventDelegate.Target).BeginInvoke(eventDelegate, args);
                        return;
                    }
                }
                catch
                {
                }
                eventDelegate.DynamicInvoke(args);
            }
        } 

        protected virtual void OnPositionChanged(PositionChangedEventArgs ea)
        {
            RaiseEvent(PositionChanged, new object[] { this, ea });
        }

        private void VirtualMCS_Load(object sender, EventArgs e)
        {
            //pollInputThread.Start();
        }
    }
}
