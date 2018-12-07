namespace Robot
{
    partial class RoboMap
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picturePanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnSaveMap = new System.Windows.Forms.Button();
            this.btnAutoScroll = new System.Windows.Forms.CheckBox();
            this.btnCenterRobot = new System.Windows.Forms.Button();
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.cbPerformLocalization = new System.Windows.Forms.CheckBox();
            this.picturePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // picturePanel
            // 
            this.picturePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picturePanel.AutoScroll = true;
            this.picturePanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picturePanel.Controls.Add(this.pictureBox1);
            this.picturePanel.Location = new System.Drawing.Point(0, 0);
            this.picturePanel.Name = "picturePanel";
            this.picturePanel.Size = new System.Drawing.Size(445, 260);
            this.picturePanel.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(5000, 5000);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // btnSaveMap
            // 
            this.btnSaveMap.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSaveMap.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSaveMap.Location = new System.Drawing.Point(12, 267);
            this.btnSaveMap.Name = "btnSaveMap";
            this.btnSaveMap.Size = new System.Drawing.Size(75, 23);
            this.btnSaveMap.TabIndex = 2;
            this.btnSaveMap.Text = "save map";
            this.btnSaveMap.UseVisualStyleBackColor = true;
            this.btnSaveMap.Click += new System.EventHandler(this.btnSaveMap_Click);
            // 
            // btnAutoScroll
            // 
            this.btnAutoScroll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAutoScroll.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnAutoScroll.Checked = true;
            this.btnAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnAutoScroll.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAutoScroll.Location = new System.Drawing.Point(360, 267);
            this.btnAutoScroll.Name = "btnAutoScroll";
            this.btnAutoScroll.Size = new System.Drawing.Size(75, 23);
            this.btnAutoScroll.TabIndex = 3;
            this.btnAutoScroll.Text = "auto scroll";
            this.btnAutoScroll.UseVisualStyleBackColor = true;
            this.btnAutoScroll.CheckedChanged += new System.EventHandler(this.btnAutoScroll_CheckedChanged);
            // 
            // btnCenterRobot
            // 
            this.btnCenterRobot.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCenterRobot.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCenterRobot.Location = new System.Drawing.Point(186, 267);
            this.btnCenterRobot.Name = "btnCenterRobot";
            this.btnCenterRobot.Size = new System.Drawing.Size(75, 23);
            this.btnCenterRobot.TabIndex = 4;
            this.btnCenterRobot.Text = "center robot";
            this.btnCenterRobot.UseVisualStyleBackColor = true;
            this.btnCenterRobot.Click += new System.EventHandler(this.btnCenterRobot_Click);
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnLoadMap.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLoadMap.Location = new System.Drawing.Point(99, 267);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(75, 23);
            this.btnLoadMap.TabIndex = 5;
            this.btnLoadMap.Text = "load map";
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
            // 
            // cbPerformLocalization
            // 
            this.cbPerformLocalization.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cbPerformLocalization.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbPerformLocalization.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbPerformLocalization.Location = new System.Drawing.Point(273, 267);
            this.cbPerformLocalization.Name = "cbPerformLocalization";
            this.cbPerformLocalization.Size = new System.Drawing.Size(75, 23);
            this.cbPerformLocalization.TabIndex = 6;
            this.cbPerformLocalization.Text = "localization";
            this.cbPerformLocalization.UseVisualStyleBackColor = true;
            this.cbPerformLocalization.CheckedChanged += new System.EventHandler(this.cbPerformLocalization_CheckedChanged);
            // 
            // RoboMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.cbPerformLocalization);
            this.Controls.Add(this.btnLoadMap);
            this.Controls.Add(this.btnCenterRobot);
            this.Controls.Add(this.btnAutoScroll);
            this.Controls.Add(this.btnSaveMap);
            this.Controls.Add(this.picturePanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.Name = "RoboMap";
            this.Size = new System.Drawing.Size(445, 306);
            this.Load += new System.EventHandler(this.RoboMap_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RoboMap_Paint);
            this.picturePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel picturePanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnSaveMap;
        private System.Windows.Forms.CheckBox btnAutoScroll;
        private System.Windows.Forms.Button btnCenterRobot;
        private System.Windows.Forms.Button btnLoadMap;
        private System.Windows.Forms.CheckBox cbPerformLocalization;

    }
}
