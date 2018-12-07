namespace Robot
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.sonar0 = new System.Windows.Forms.Label();
            this.sonar1 = new System.Windows.Forms.Label();
            this.sonar2 = new System.Windows.Forms.Label();
            this.sonar3 = new System.Windows.Forms.Label();
            this.sonar4 = new System.Windows.Forms.Label();
            this.sonar5 = new System.Windows.Forms.Label();
            this.sonar6 = new System.Windows.Forms.Label();
            this.sonar7 = new System.Windows.Forms.Label();
            this.sonar15 = new System.Windows.Forms.Label();
            this.sonar14 = new System.Windows.Forms.Label();
            this.sonar13 = new System.Windows.Forms.Label();
            this.sonar12 = new System.Windows.Forms.Label();
            this.sonar11 = new System.Windows.Forms.Label();
            this.sonar10 = new System.Windows.Forms.Label();
            this.sonar9 = new System.Windows.Forms.Label();
            this.sonar8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.compass = new System.Windows.Forms.Label();
            this.battery = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.leftVel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.moving = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.rightVel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Joystick = new WinUI.MCS.VirtualMCS();
            this.explorerTimer = new System.Windows.Forms.Timer(this.components);
            this.lblFront = new System.Windows.Forms.Label();
            this.lblBack = new System.Windows.Forms.Label();
            this.lblLeft = new System.Windows.Forms.Label();
            this.lblRight = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblHeadingGuess = new System.Windows.Forms.Label();
            this.lblHallWidth = new System.Windows.Forms.Label();
            this.txtMinSpeed = new System.Windows.Forms.TextBox();
            this.txtMidSpeed = new System.Windows.Forms.TextBox();
            this.txtMaxSpeed = new System.Windows.Forms.TextBox();
            this.txtTurnInc = new System.Windows.Forms.TextBox();
            this.lblMoveDir = new System.Windows.Forms.Label();
            this.sonarViewer = new Robot.SonarViewer();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblTheta = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.robotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectCOMPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.emergancyStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customFunctionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exploreHallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.waitAndSpinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roboMap = new Robot.RoboMap();
            this.samplingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runRotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.pulseTimer_Tick);
            // 
            // sonar0
            // 
            this.sonar0.AutoSize = true;
            this.sonar0.Location = new System.Drawing.Point(331, 108);
            this.sonar0.Name = "sonar0";
            this.sonar0.Size = new System.Drawing.Size(39, 13);
            this.sonar0.TabIndex = 7;
            this.sonar0.Text = "sonar0";
            this.sonar0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar1
            // 
            this.sonar1.AutoSize = true;
            this.sonar1.Location = new System.Drawing.Point(349, 85);
            this.sonar1.Name = "sonar1";
            this.sonar1.Size = new System.Drawing.Size(39, 13);
            this.sonar1.TabIndex = 8;
            this.sonar1.Text = "sonar1";
            this.sonar1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar2
            // 
            this.sonar2.AutoSize = true;
            this.sonar2.Location = new System.Drawing.Point(380, 72);
            this.sonar2.Name = "sonar2";
            this.sonar2.Size = new System.Drawing.Size(39, 13);
            this.sonar2.TabIndex = 9;
            this.sonar2.Text = "sonar2";
            this.sonar2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar3
            // 
            this.sonar3.AutoSize = true;
            this.sonar3.Location = new System.Drawing.Point(419, 59);
            this.sonar3.Name = "sonar3";
            this.sonar3.Size = new System.Drawing.Size(39, 13);
            this.sonar3.TabIndex = 10;
            this.sonar3.Text = "sonar3";
            this.sonar3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar4
            // 
            this.sonar4.AutoSize = true;
            this.sonar4.Location = new System.Drawing.Point(471, 59);
            this.sonar4.Name = "sonar4";
            this.sonar4.Size = new System.Drawing.Size(39, 13);
            this.sonar4.TabIndex = 11;
            this.sonar4.Text = "sonar4";
            this.sonar4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar5
            // 
            this.sonar5.AutoSize = true;
            this.sonar5.Location = new System.Drawing.Point(512, 72);
            this.sonar5.Name = "sonar5";
            this.sonar5.Size = new System.Drawing.Size(39, 13);
            this.sonar5.TabIndex = 12;
            this.sonar5.Text = "sonar5";
            this.sonar5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar6
            // 
            this.sonar6.AutoSize = true;
            this.sonar6.Location = new System.Drawing.Point(543, 84);
            this.sonar6.Name = "sonar6";
            this.sonar6.Size = new System.Drawing.Size(39, 13);
            this.sonar6.TabIndex = 13;
            this.sonar6.Text = "sonar6";
            this.sonar6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar7
            // 
            this.sonar7.AutoSize = true;
            this.sonar7.Location = new System.Drawing.Point(559, 108);
            this.sonar7.Name = "sonar7";
            this.sonar7.Size = new System.Drawing.Size(39, 13);
            this.sonar7.TabIndex = 14;
            this.sonar7.Text = "sonar7";
            this.sonar7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar15
            // 
            this.sonar15.AutoSize = true;
            this.sonar15.Location = new System.Drawing.Point(331, 215);
            this.sonar15.Name = "sonar15";
            this.sonar15.Size = new System.Drawing.Size(45, 13);
            this.sonar15.TabIndex = 22;
            this.sonar15.Text = "sonar15";
            this.sonar15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar14
            // 
            this.sonar14.AutoSize = true;
            this.sonar14.Location = new System.Drawing.Point(349, 237);
            this.sonar14.Name = "sonar14";
            this.sonar14.Size = new System.Drawing.Size(45, 13);
            this.sonar14.TabIndex = 21;
            this.sonar14.Text = "sonar14";
            this.sonar14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar13
            // 
            this.sonar13.AutoSize = true;
            this.sonar13.Location = new System.Drawing.Point(380, 250);
            this.sonar13.Name = "sonar13";
            this.sonar13.Size = new System.Drawing.Size(45, 13);
            this.sonar13.TabIndex = 20;
            this.sonar13.Text = "sonar13";
            this.sonar13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar12
            // 
            this.sonar12.AutoSize = true;
            this.sonar12.Location = new System.Drawing.Point(413, 263);
            this.sonar12.Name = "sonar12";
            this.sonar12.Size = new System.Drawing.Size(45, 13);
            this.sonar12.TabIndex = 19;
            this.sonar12.Text = "sonar12";
            this.sonar12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar11
            // 
            this.sonar11.AutoSize = true;
            this.sonar11.Location = new System.Drawing.Point(471, 263);
            this.sonar11.Name = "sonar11";
            this.sonar11.Size = new System.Drawing.Size(45, 13);
            this.sonar11.TabIndex = 18;
            this.sonar11.Text = "sonar11";
            this.sonar11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar10
            // 
            this.sonar10.AutoSize = true;
            this.sonar10.Location = new System.Drawing.Point(512, 250);
            this.sonar10.Name = "sonar10";
            this.sonar10.Size = new System.Drawing.Size(45, 13);
            this.sonar10.TabIndex = 17;
            this.sonar10.Text = "sonar10";
            this.sonar10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar9
            // 
            this.sonar9.AutoSize = true;
            this.sonar9.Location = new System.Drawing.Point(543, 237);
            this.sonar9.Name = "sonar9";
            this.sonar9.Size = new System.Drawing.Size(39, 13);
            this.sonar9.TabIndex = 16;
            this.sonar9.Text = "sonar9";
            this.sonar9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar8
            // 
            this.sonar8.AutoSize = true;
            this.sonar8.Location = new System.Drawing.Point(559, 215);
            this.sonar8.Name = "sonar8";
            this.sonar8.Size = new System.Drawing.Size(39, 13);
            this.sonar8.TabIndex = 15;
            this.sonar8.Text = "sonar8";
            this.sonar8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 349);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Compass";
            // 
            // compass
            // 
            this.compass.AutoSize = true;
            this.compass.Location = new System.Drawing.Point(77, 349);
            this.compass.Name = "compass";
            this.compass.Size = new System.Drawing.Size(50, 13);
            this.compass.TabIndex = 24;
            this.compass.Text = "Compass";
            // 
            // battery
            // 
            this.battery.AutoSize = true;
            this.battery.Location = new System.Drawing.Point(77, 371);
            this.battery.Name = "battery";
            this.battery.Size = new System.Drawing.Size(40, 13);
            this.battery.TabIndex = 26;
            this.battery.Text = "Battery";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 371);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Battery";
            // 
            // leftVel
            // 
            this.leftVel.AutoSize = true;
            this.leftVel.Location = new System.Drawing.Point(77, 415);
            this.leftVel.Name = "leftVel";
            this.leftVel.Size = new System.Drawing.Size(43, 13);
            this.leftVel.TabIndex = 30;
            this.leftVel.Text = "Left Vel";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 415);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "Left Vel";
            // 
            // moving
            // 
            this.moving.AutoSize = true;
            this.moving.Location = new System.Drawing.Point(77, 393);
            this.moving.Name = "moving";
            this.moving.Size = new System.Drawing.Size(42, 13);
            this.moving.TabIndex = 28;
            this.moving.Text = "Moving";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 393);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Moving";
            // 
            // rightVel
            // 
            this.rightVel.AutoSize = true;
            this.rightVel.Location = new System.Drawing.Point(77, 437);
            this.rightVel.Name = "rightVel";
            this.rightVel.Size = new System.Drawing.Size(50, 13);
            this.rightVel.TabIndex = 32;
            this.rightVel.Text = "Right Vel";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 437);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 31;
            this.label10.Text = "Right Vel";
            // 
            // Joystick
            // 
            this.Joystick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(179)))), ((int)(((byte)(179)))));
            this.Joystick.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Joystick.guiPollingRate = 250;
            this.Joystick.Location = new System.Drawing.Point(378, 75);
            this.Joystick.Name = "Joystick";
            this.Joystick.Size = new System.Drawing.Size(175, 176);
            this.Joystick.TabIndex = 33;
            this.Joystick.PositionChanged += new WinUI.MCS.VirtualMCS.PositionChangedEventHandler(this.Joystick_PositionChanged);
            // 
            // explorerTimer
            // 
            this.explorerTimer.Tick += new System.EventHandler(this.explorerTimer_Tick);
            // 
            // lblFront
            // 
            this.lblFront.AutoSize = true;
            this.lblFront.Location = new System.Drawing.Point(449, 39);
            this.lblFront.Name = "lblFront";
            this.lblFront.Size = new System.Drawing.Size(28, 13);
            this.lblFront.TabIndex = 38;
            this.lblFront.Text = "front";
            this.lblFront.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBack
            // 
            this.lblBack.AutoSize = true;
            this.lblBack.Location = new System.Drawing.Point(449, 285);
            this.lblBack.Name = "lblBack";
            this.lblBack.Size = new System.Drawing.Size(31, 13);
            this.lblBack.TabIndex = 39;
            this.lblBack.Text = "back";
            this.lblBack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(331, 161);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(21, 13);
            this.lblLeft.TabIndex = 40;
            this.lblLeft.Text = "left";
            this.lblLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(571, 161);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(27, 13);
            this.lblRight.TabIndex = 41;
            this.lblRight.Text = "right";
            this.lblRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 304);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Hall Width";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 304);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 43;
            this.label3.Text = "Heading Guess";
            // 
            // lblHeadingGuess
            // 
            this.lblHeadingGuess.AutoSize = true;
            this.lblHeadingGuess.Location = new System.Drawing.Point(145, 317);
            this.lblHeadingGuess.Name = "lblHeadingGuess";
            this.lblHeadingGuess.Size = new System.Drawing.Size(80, 13);
            this.lblHeadingGuess.TabIndex = 45;
            this.lblHeadingGuess.Text = "Heading Guess";
            // 
            // lblHallWidth
            // 
            this.lblHallWidth.AutoSize = true;
            this.lblHallWidth.Location = new System.Drawing.Point(12, 317);
            this.lblHallWidth.Name = "lblHallWidth";
            this.lblHallWidth.Size = new System.Drawing.Size(56, 13);
            this.lblHallWidth.TabIndex = 44;
            this.lblHallWidth.Text = "Hall Width";
            // 
            // txtMinSpeed
            // 
            this.txtMinSpeed.Location = new System.Drawing.Point(80, 531);
            this.txtMinSpeed.Name = "txtMinSpeed";
            this.txtMinSpeed.Size = new System.Drawing.Size(53, 20);
            this.txtMinSpeed.TabIndex = 46;
            this.txtMinSpeed.Text = "5";
            // 
            // txtMidSpeed
            // 
            this.txtMidSpeed.Location = new System.Drawing.Point(80, 557);
            this.txtMidSpeed.Name = "txtMidSpeed";
            this.txtMidSpeed.Size = new System.Drawing.Size(53, 20);
            this.txtMidSpeed.TabIndex = 47;
            this.txtMidSpeed.Text = "10";
            // 
            // txtMaxSpeed
            // 
            this.txtMaxSpeed.Location = new System.Drawing.Point(80, 582);
            this.txtMaxSpeed.Name = "txtMaxSpeed";
            this.txtMaxSpeed.Size = new System.Drawing.Size(53, 20);
            this.txtMaxSpeed.TabIndex = 48;
            this.txtMaxSpeed.Text = "20";
            // 
            // txtTurnInc
            // 
            this.txtTurnInc.Location = new System.Drawing.Point(80, 608);
            this.txtTurnInc.Name = "txtTurnInc";
            this.txtTurnInc.Size = new System.Drawing.Size(31, 20);
            this.txtTurnInc.TabIndex = 49;
            this.txtTurnInc.Text = "2";
            // 
            // lblMoveDir
            // 
            this.lblMoveDir.AutoSize = true;
            this.lblMoveDir.Location = new System.Drawing.Point(261, 304);
            this.lblMoveDir.Name = "lblMoveDir";
            this.lblMoveDir.Size = new System.Drawing.Size(64, 13);
            this.lblMoveDir.TabIndex = 50;
            this.lblMoveDir.Text = "Gross Move";
            // 
            // sonarViewer
            // 
            this.sonarViewer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(179)))), ((int)(((byte)(179)))));
            this.sonarViewer.Location = new System.Drawing.Point(38, 62);
            this.sonarViewer.Name = "sonarViewer";
            this.sonarViewer.Size = new System.Drawing.Size(168, 189);
            this.sonarViewer.TabIndex = 51;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 534);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 52;
            this.label5.Text = "Min Speed";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 560);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 53;
            this.label7.Text = "Mid Speed";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 585);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 54;
            this.label9.Text = "Max Speed";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 611);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 13);
            this.label11.TabIndex = 55;
            this.label11.Text = "Turn Delta";
            // 
            // lblTheta
            // 
            this.lblTheta.AutoSize = true;
            this.lblTheta.Location = new System.Drawing.Point(77, 503);
            this.lblTheta.Name = "lblTheta";
            this.lblTheta.Size = new System.Drawing.Size(35, 13);
            this.lblTheta.TabIndex = 60;
            this.lblTheta.Text = "Theta";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 503);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 13);
            this.label13.TabIndex = 59;
            this.label13.Text = "Theta Val";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(77, 481);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(14, 13);
            this.lblY.TabIndex = 58;
            this.lblY.Text = "Y";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 481);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 13);
            this.label15.TabIndex = 57;
            this.label15.Text = "Y Val";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(77, 459);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(14, 13);
            this.lblX.TabIndex = 62;
            this.lblX.Text = "X";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(18, 459);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 13);
            this.label17.TabIndex = 61;
            this.label17.Text = "X Val";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 697);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(735, 22);
            this.statusStrip1.TabIndex = 64;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.robotToolStripMenuItem,
            this.sessionToolStripMenuItem,
            this.customFunctionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(735, 24);
            this.menuStrip1.TabIndex = 65;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // robotToolStripMenuItem
            // 
            this.robotToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.configureToolStripMenuItem,
            this.toolStripMenuItem3,
            this.emergancyStopToolStripMenuItem});
            this.robotToolStripMenuItem.Name = "robotToolStripMenuItem";
            this.robotToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.robotToolStripMenuItem.Text = "&Robot";
            // 
            // initToolStripMenuItem
            // 
            this.initToolStripMenuItem.Name = "initToolStripMenuItem";
            this.initToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.initToolStripMenuItem.Text = "&Init";
            this.initToolStripMenuItem.Click += new System.EventHandler(this.initToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(179, 6);
            // 
            // configureToolStripMenuItem
            // 
            this.configureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectCOMPortToolStripMenuItem});
            this.configureToolStripMenuItem.Name = "configureToolStripMenuItem";
            this.configureToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.configureToolStripMenuItem.Text = "Configure";
            // 
            // selectCOMPortToolStripMenuItem
            // 
            this.selectCOMPortToolStripMenuItem.Name = "selectCOMPortToolStripMenuItem";
            this.selectCOMPortToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.selectCOMPortToolStripMenuItem.Text = "Select COM Port";
            this.selectCOMPortToolStripMenuItem.Click += new System.EventHandler(this.selectCOMPortToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(179, 6);
            // 
            // emergancyStopToolStripMenuItem
            // 
            this.emergancyStopToolStripMenuItem.Name = "emergancyStopToolStripMenuItem";
            this.emergancyStopToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.emergancyStopToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.emergancyStopToolStripMenuItem.Text = "Emergency Sto&p";
            this.emergancyStopToolStripMenuItem.Click += new System.EventHandler(this.emergancyStopToolStripMenuItem_Click);
            // 
            // sessionToolStripMenuItem
            // 
            this.sessionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.recordToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.sessionToolStripMenuItem.Name = "sessionToolStripMenuItem";
            this.sessionToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.sessionToolStripMenuItem.Text = "&Session";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.loadToolStripMenuItem.Text = "&Load";
            // 
            // recordToolStripMenuItem
            // 
            this.recordToolStripMenuItem.Name = "recordToolStripMenuItem";
            this.recordToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.recordToolStripMenuItem.Text = "&Record";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // customFunctionsToolStripMenuItem
            // 
            this.customFunctionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exploreHallToolStripMenuItem,
            this.waitAndSpinToolStripMenuItem,
            this.localizationToolStripMenuItem});
            this.customFunctionsToolStripMenuItem.Name = "customFunctionsToolStripMenuItem";
            this.customFunctionsToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
            this.customFunctionsToolStripMenuItem.Text = "Custom Functions";
            // 
            // exploreHallToolStripMenuItem
            // 
            this.exploreHallToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.exploreHallToolStripMenuItem.Name = "exploreHallToolStripMenuItem";
            this.exploreHallToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.exploreHallToolStripMenuItem.Text = "Explore Hall";
            this.exploreHallToolStripMenuItem.Click += new System.EventHandler(this.exploreHallToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F1)));
            this.startToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // waitAndSpinToolStripMenuItem
            // 
            this.waitAndSpinToolStripMenuItem.Name = "waitAndSpinToolStripMenuItem";
            this.waitAndSpinToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.waitAndSpinToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.waitAndSpinToolStripMenuItem.Text = "Wait and Spin";
            this.waitAndSpinToolStripMenuItem.Click += new System.EventHandler(this.waitAndSpinToolStripMenuItem_Click);
            // 
            // localizationToolStripMenuItem
            // 
            this.localizationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.samplingToolStripMenuItem,
            this.runRotationToolStripMenuItem});
            this.localizationToolStripMenuItem.Name = "localizationToolStripMenuItem";
            this.localizationToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.localizationToolStripMenuItem.Text = "Localization";
            // 
            // roboMap
            // 
            this.roboMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.roboMap.AutoScroll = true;
            this.roboMap.axleLength = 0.35;
            this.roboMap.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.roboMap.Location = new System.Drawing.Point(264, 344);
            this.roboMap.Name = "roboMap";
            this.roboMap.Size = new System.Drawing.Size(431, 332);
            this.roboMap.TabIndex = 56;
            // 
            // samplingToolStripMenuItem
            // 
            this.samplingToolStripMenuItem.Name = "samplingToolStripMenuItem";
            this.samplingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.samplingToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.samplingToolStripMenuItem.Text = "Sampling - rotation";
            this.samplingToolStripMenuItem.Click += new System.EventHandler(this.samplingToolStripMenuItem_Click);
            // 
            // runRotationToolStripMenuItem
            // 
            this.runRotationToolStripMenuItem.Name = "runRotationToolStripMenuItem";
            this.runRotationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.R)));
            this.runRotationToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.runRotationToolStripMenuItem.Text = "Run - rotation";
            this.runRotationToolStripMenuItem.Click += new System.EventHandler(this.runRotationToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(179)))), ((int)(((byte)(179)))));
            this.ClientSize = new System.Drawing.Size(735, 719);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lblX);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.lblTheta);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblY);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.roboMap);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sonarViewer);
            this.Controls.Add(this.lblMoveDir);
            this.Controls.Add(this.txtTurnInc);
            this.Controls.Add(this.txtMaxSpeed);
            this.Controls.Add(this.txtMidSpeed);
            this.Controls.Add(this.txtMinSpeed);
            this.Controls.Add(this.lblHeadingGuess);
            this.Controls.Add(this.lblHallWidth);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.lblBack);
            this.Controls.Add(this.lblFront);
            this.Controls.Add(this.rightVel);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.leftVel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.moving);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.battery);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.compass);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sonar15);
            this.Controls.Add(this.sonar14);
            this.Controls.Add(this.sonar13);
            this.Controls.Add(this.sonar12);
            this.Controls.Add(this.sonar11);
            this.Controls.Add(this.sonar10);
            this.Controls.Add(this.sonar9);
            this.Controls.Add(this.sonar8);
            this.Controls.Add(this.sonar7);
            this.Controls.Add(this.sonar6);
            this.Controls.Add(this.sonar5);
            this.Controls.Add(this.sonar4);
            this.Controls.Add(this.sonar3);
            this.Controls.Add(this.sonar2);
            this.Controls.Add(this.sonar1);
            this.Controls.Add(this.sonar0);
            this.Controls.Add(this.Joystick);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.btnStartExplore_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Label sonar0;
        private System.Windows.Forms.Label sonar1;
        private System.Windows.Forms.Label sonar2;
        private System.Windows.Forms.Label sonar3;
        private System.Windows.Forms.Label sonar4;
        private System.Windows.Forms.Label sonar5;
        private System.Windows.Forms.Label sonar6;
        private System.Windows.Forms.Label sonar7;
        private System.Windows.Forms.Label sonar15;
        private System.Windows.Forms.Label sonar14;
        private System.Windows.Forms.Label sonar13;
        private System.Windows.Forms.Label sonar12;
        private System.Windows.Forms.Label sonar11;
        private System.Windows.Forms.Label sonar10;
        private System.Windows.Forms.Label sonar9;
        private System.Windows.Forms.Label sonar8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label compass;
        private System.Windows.Forms.Label battery;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label leftVel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label moving;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label rightVel;
        private System.Windows.Forms.Label label10;
        private WinUI.MCS.VirtualMCS Joystick;
        private System.Windows.Forms.Timer explorerTimer;
        private System.Windows.Forms.Label lblFront;
        private System.Windows.Forms.Label lblBack;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblHeadingGuess;
        private System.Windows.Forms.Label lblHallWidth;
        private System.Windows.Forms.TextBox txtMinSpeed;
        private System.Windows.Forms.TextBox txtMidSpeed;
        private System.Windows.Forms.TextBox txtMaxSpeed;
        private System.Windows.Forms.TextBox txtTurnInc;
        private System.Windows.Forms.Label lblMoveDir;
        private Robot.SonarViewer sonarViewer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private RoboMap roboMap;
        private System.Windows.Forms.Label lblTheta;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem robotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customFunctionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exploreHallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem waitAndSpinToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem emergancyStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectCOMPortToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem localizationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem samplingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runRotationToolStripMenuItem;
    }
}

