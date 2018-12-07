namespace Robot
{
    partial class frmSampler
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
            this.status = new System.Windows.Forms.Label();
            this.graph = new System.Windows.Forms.PictureBox();
            this.btnCW = new System.Windows.Forms.Button();
            this.btnCCW = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.graph)).BeginInit();
            this.SuspendLayout();
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Location = new System.Drawing.Point(13, 13);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(37, 13);
            this.status.TabIndex = 0;
            this.status.Text = "Status";
            // 
            // graph
            // 
            this.graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.graph.Location = new System.Drawing.Point(2, 83);
            this.graph.Name = "graph";
            this.graph.Size = new System.Drawing.Size(583, 280);
            this.graph.TabIndex = 1;
            this.graph.TabStop = false;
            this.graph.Paint += new System.Windows.Forms.PaintEventHandler(this.graph_Paint);
            // 
            // btnCW
            // 
            this.btnCW.Location = new System.Drawing.Point(62, 44);
            this.btnCW.Name = "btnCW";
            this.btnCW.Size = new System.Drawing.Size(75, 23);
            this.btnCW.TabIndex = 2;
            this.btnCW.Text = "CW";
            this.btnCW.UseVisualStyleBackColor = true;
            this.btnCW.Click += new System.EventHandler(this.btnCW_Click);
            // 
            // btnCCW
            // 
            this.btnCCW.Location = new System.Drawing.Point(170, 44);
            this.btnCCW.Name = "btnCCW";
            this.btnCCW.Size = new System.Drawing.Size(75, 23);
            this.btnCCW.TabIndex = 3;
            this.btnCCW.Text = "CCW";
            this.btnCCW.UseVisualStyleBackColor = true;
            this.btnCCW.Click += new System.EventHandler(this.btnCCW_Click);
            // 
            // frmSampler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 364);
            this.Controls.Add(this.btnCCW);
            this.Controls.Add(this.btnCW);
            this.Controls.Add(this.graph);
            this.Controls.Add(this.status);
            this.DoubleBuffered = true;
            this.Name = "frmSampler";
            this.Text = "frmSampler";
            ((System.ComponentModel.ISupportInitialize)(this.graph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label status;
        public System.Windows.Forms.PictureBox graph;
        private System.Windows.Forms.Button btnCW;
        private System.Windows.Forms.Button btnCCW;
    }
}