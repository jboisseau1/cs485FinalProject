using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
//using Boeing.NetEffects.DataModel;
//using Boeing.NetEffects.Nodes;
//using Boeing.NetEffects.WinUi.Control;

namespace Boeing.VirtualMCS
{
	/// <summary>
	/// Summary description for VirtualMCSUi.
	/// </summary>
    public class VirtualMCSUi : System.Windows.Forms.UserControl //: ControlBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private WinUI.MCS.VirtualMCS virtualMCS1;
	
		public VirtualMCSUi()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
            //_virtualMCSNode = new VirtualMCSNode(new NodeInfo(), this);
            //ClientNode = _virtualMCSNode;

			virtualMCS1.guiPollingRate = 250;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
			this.virtualMCS1 = new WinUI.MCS.VirtualMCS();
			this.SuspendLayout();
			// 
			// virtualMCS1
			// 
			this.virtualMCS1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(179)), ((System.Byte)(179)), ((System.Byte)(179)));
			this.virtualMCS1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.virtualMCS1.guiPollingRate = 50;
			this.virtualMCS1.Location = new System.Drawing.Point(0, 0);
			this.virtualMCS1.Name = "virtualMCS1";
			this.virtualMCS1.Size = new System.Drawing.Size(352, 232);
			this.virtualMCS1.TabIndex = 0;
			this.virtualMCS1.PositionChanged += new WinUI.MCS.VirtualMCS.PositionChangedEventHandler(this.virtualMCS1_PositionChanged);
			// 
			// VirtualMCSUi
			// 
			this.Controls.Add(this.virtualMCS1);
			this.Name = "VirtualMCSUi";
			this.Size = new System.Drawing.Size(352, 232);
			this.ResumeLayout(false);

		}
		#endregion

        private void virtualMCS1_PositionChanged(object sender, WinUI.MCS.Interfaces.PositionChangedEventArgs e)
		{
			//_virtualMCSNode.Move((int)e.move, e.moveValue);
		}
	}
}
