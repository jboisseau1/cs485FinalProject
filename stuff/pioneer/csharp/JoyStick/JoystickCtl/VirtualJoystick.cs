using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using WinUI.MCS.Interfaces;

namespace WinUI.MCS
{
	/// <summary>
	/// Summary description for UserControl1.
	/// </summary>
	public class VirtualJoystick : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		
		private int guiDeadZone = 2;

		#region Properties
		private int _left = 0;
		public int left
		{
			get
			{
//				int width = this.Width - ( borderWidth * 2 );
//				//int x0 = ( handlePosition.X - ( this.Width / 2 ) ) * ( 100 / ( width / 2 ) );
//				double radius = ( (double)this.Height / 2.0f ) - borderWidth;
//				int x0 = (int) ( 100f * ( (double) handlePosition.X - ( (double) this.Width / 2.0f ) ) / radius );
//				if ( x0 < (-1 * guiDeadZone) )
//					return Math.Abs( x0 );
//				else
//					return 0;
				return _left;
			}
			set
			{
				_left = value;
				if ( _left > 100 ) _left = 100;
				if ( _left < 0 ) _left = 0;
				int width = this.Width - ( borderWidth * 2 );
				handlePosition.X = ( ( -1 * _left ) / ( 100 / ( width / 2 ) ) ) + ( this.Width / 2 );
				moveHandle( handlePosition.X, handlePosition.Y );
			}
		}

		private int _right = 0;
		public int right
		{
			get
			{
//				int width = this.Width - ( borderWidth * 2 );
//				//int x0 = ( handlePosition.X - ( this.Width / 2 ) ) * ( 100 / ( width / 2 ) );
//				double radius = ( (double)this.Height / 2.0f ) - borderWidth;
//				int x0 = (int) ( 100f * ( (double) handlePosition.X - ( (double) this.Width / 2.0f ) ) / radius );
//				if ( x0 < guiDeadZone )
//					return Math.Abs( x0 );
//				else
//					return 0;
				return _right;
			}
			set
			{
				_right = value;
				if ( _right > 100 ) _right = 100;
				if ( _right < 0 ) _right = 0;
				int width = this.Width - ( borderWidth * 2 );
				handlePosition.X = ( _right / ( 100 / ( width / 2 ) ) ) + ( this.Width / 2 );
				moveHandle( handlePosition.X, handlePosition.Y );
			}
		}
		private int _up = 0;
		public int up
		{
			get
			{
//				int height = this.Height - ( borderWidth * 2 );
//				//int y0 = ( handlePosition.Y - ( this.Height / 2 ) ) * ( 100 / ( height / 2 ) );
//				double radius = ( (double)this.Height / 2.0f ) - borderWidth;
//				int y0 = (int) ( 100f * ( (double) handlePosition.Y - ( (double) this.Height / 2.0f ) ) / radius );
//				if ( y0 < (-1 * guiDeadZone) )
//					return Math.Abs( y0 );
//				else
//					return 0;
				return _up;
			}
			set
			{
				_up = value;
				if ( _up > 100 ) _up = 100;
				if ( _up < 0 ) _up = 0;
				int height = this.Height - ( borderWidth * 2 );
				handlePosition.Y = ( ( -1 * _up ) / ( 100 / ( height / 2 ) ) ) + ( this.Height / 2 );
				moveHandle( handlePosition.X, handlePosition.Y );
			}
		}
		private int _down = 0;
		public int down
		{
			get
			{
//				int height = this.Height - ( borderWidth * 2 );
//				//int y0 = ( handlePosition.Y - ( this.Height / 2 ) ) * ( 100 / ( height / 2 ) );
//				double radius = ( (double)this.Height / 2.0f ) - borderWidth;
//				int y0 = (int) ( 100f * ( (double) handlePosition.Y - ( (double) this.Height / 2.0f ) ) / radius );
//				if ( y0 < guiDeadZone )
//					return Math.Abs( y0 );
//				else
//					return 0;
				return _down;
			}
			set
			{
				_down = value;
				if ( _down > 100 ) _down = 100;
				if ( _down < 0 ) _down = 0;
				int height = this.Height - ( borderWidth * 2 );
				handlePosition.Y = ( _down / ( 100 / ( height / 2 ) ) ) + ( this.Height / 2 );
				moveHandle( handlePosition.X, handlePosition.Y );
			}
		}
		
		#endregion

		private Image backgroundImage;
		private Image handleImage;
		private Point handlePosition = new Point( 0, 0 );

		private bool trackMouse = false;
    //private bool threadRunning = false;

		private Thread dropHandleThread;
		
		private int handleOffsetX = 0;
		private int handleOffsetY = 0;

		private int borderWidth;

		private System.ComponentModel.Container components = null;

		public class PositionChangedEventArgs : EventArgs
		{
			public MCSConsts.Direction verticalMove;
			public int verticalMoveValue;
			public MCSConsts.Direction horizontalMove;
			public int horizontalMoveValue;
		}

		public delegate void PositionChangedEventHandler( object sender, PositionChangedEventArgs e );
		public event PositionChangedEventHandler PositionChanged;

		public VirtualJoystick()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);

			Assembly a = Assembly.GetExecutingAssembly();

			Stream imageStream = a.GetManifestResourceStream( "Boeing.VirtualMCS.disk-1.jpg" );
			backgroundImage = Image.FromStream( imageStream );
			imageStream = a.GetManifestResourceStream( "Boeing.VirtualMCS.disk-handle.jpg" );
			handleImage = Image.FromStream( imageStream );

			//borderWidth = (int)( 18 + 0.09 * this.Width );
			borderWidth = (int)( ( (double)handleImage.PhysicalDimension.Width / 2.0f ) + ( (double)this.Width / 18f ) );

			handlePosition.X = this.Width / 2;
			handlePosition.Y = this.Height / 2;
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			// Calling the base class OnPaint
			base.OnPaint(pe);
			
			System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle( 0, 0, 300, 300 );
			System.Drawing.Rectangle destRect = new System.Drawing.Rectangle( 0, 0, this.Width, this.Height );
			
			//pe.Graphics.DrawImage( backgroundImage, destRect, srcRect, System.Drawing.GraphicsUnit.Pixel );
			TextureBrush backgroundBrush = new TextureBrush( backgroundImage, System.Drawing.Drawing2D.WrapMode.Clamp );
			backgroundBrush.ScaleTransform( (float)this.Width / 300f, (float)this.Height / 300f );
			pe.Graphics.FillEllipse( backgroundBrush, destRect );

			// draw the handle
			Size handleSize = handleImage.PhysicalDimension.ToSize();
			TextureBrush handleBrush = new TextureBrush( handleImage, System.Drawing.Drawing2D.WrapMode.Clamp );

			Point handleOffset = new Point();
			handleOffset.X = handlePosition.X;
			handleOffset.Y = handlePosition.Y;
			handleOffset.Offset( handleSize.Width / -2, handleSize.Height / -2 );

			destRect = new Rectangle( handleOffset.X, handleOffset.Y,
																					handleSize.Width, handleSize.Height );

			handleBrush.TranslateTransform( handlePosition.X - (handleSize.Width / 2),
																			handlePosition.Y - (handleSize.Height / 2) );

			pe.Graphics.FillEllipse( handleBrush, destRect );
			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// VirtualJoystick
			// 
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(179)), ((System.Byte)(179)), ((System.Byte)(179)));
			this.Name = "VirtualJoystick";
			this.Size = new System.Drawing.Size(152, 136);
			this.LocationChanged += new System.EventHandler(this.ResizeMe);
			this.SizeChanged += new System.EventHandler(this.ResizeMe);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
			this.DragLeave += new System.EventHandler(this.VirtualJoystick_DragLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseMove);
			this.MouseLeave += new System.EventHandler(this.VirtualJoystick_MouseLeave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);

		}
		#endregion

		private void ResizeMe(object sender, System.EventArgs e)
		{
			// we must be square so our image is circular
			if ( this.Width > this.Height )
				this.Height = this.Width;
			else
				this.Width = this.Height;

			handlePosition.X = this.Width / 2;
			handlePosition.Y = this.Height / 2;

			//borderWidth = (int) ( (double)this.Width * 0.18f );
			RepaintMe();
		}

		private void RepaintMe()
		{
			Invalidate();
		}

		private void moveHandle( int x, int y )
		{
			// is it within our circle? (assume circle, not ellipse)
			double radius = ( (double)this.Height / 2.0f ) - borderWidth;
			
			double x1 = (double)x - ( (double)this.Width / 2.0f );
			double y1 = (double)y - ( (double)this.Height / 2.0f );
			double r1 = Math.Sqrt( x1*x1 + y1*y1 );

			if ( r1 > radius )
			{
				double phi = Math.Atan( y1 / x1 );
				double x2 = (int)Math.Abs( radius * Math.Cos( phi ) );
				if ( Math.Sign( x1 ) != Math.Sign( x2 ) )
					x2 *= Math.Sign( x1 );

				double y2 = (int)Math.Abs( radius * Math.Sin( phi ) );
				int i = Math.Sign( y1 );
				if ( Math.Sign( y1 ) != Math.Sign( y2 ) )
					y2 *= Math.Sign( y1 );

				x = (int)( x2 + ( (double)this.Width / 2.0f ) );
				y = (int)( y2 + ( (double)this.Height / 2.0f ) );
			}

			handlePosition.X = x;
			handlePosition.Y = y;

			PositionChangedEventArgs e = new PositionChangedEventArgs();
			int width = this.Width - ( borderWidth * 2 );
			int height = this.Height - ( borderWidth * 2 );

			//int x0 = ( x - ( this.Width / 2 ) ) * ( 100 / ( width / 2 ) );
			int x0 = (int) ( 100f * ( (double) x - ( (double) this.Width / 2.0f ) ) / radius );
			//int y0 = ( y - ( this.Height / 2 ) ) * ( 100 / ( height / 2 ) );
			int y0 = (int) ( 100f * ( (double) y - ( (double) this.Height / 2.0f ) ) / radius );

			if ( x0 > guiDeadZone )
			{
				e.horizontalMove = MCSConsts.Direction.RIGHT;
				e.horizontalMoveValue = x0;
				_right = e.horizontalMoveValue;
				_left = 0;
			}
			else if ( x0 < (-1 * guiDeadZone) )
			{
				e.horizontalMove = MCSConsts.Direction.LEFT;
				e.horizontalMoveValue = Math.Abs( x0 );
				_right = 0;
				_left = e.horizontalMoveValue;
			}
			else
			{
				e.horizontalMove = MCSConsts.Direction.LEFT;
				e.horizontalMoveValue = 0;
				_right = 0;
				_left = 0;
			}

			if ( y0 > guiDeadZone )
			{
				e.verticalMove = MCSConsts.Direction.DOWN;
				e.verticalMoveValue = y0;
				_up = 0;
				_down = e.verticalMoveValue;
			}
			else if ( y0 < (-1 * guiDeadZone) )
			{
				e.verticalMove = MCSConsts.Direction.UP;
				e.verticalMoveValue = Math.Abs( y0 );
				_up = e.verticalMoveValue;
				_down = 0;
			}
			else
			{
				e.verticalMove = MCSConsts.Direction.DOWN;
				e.verticalMoveValue = 0;
				_up = 0;
				_down = 0;
			}

			//OnPositionChanged( e );

			RepaintMe();
		}

		private void droppingHandle()
		{
			//threadRunning = true;

			int x = handlePosition.X;
			int y = handlePosition.Y;
			
			double radius = ( (double)this.Height / 2.0f ) - borderWidth;
			
			double x1 = (double)x - ( (double)this.Width / 2.0f );
			double y1 = (double)y - ( (double)this.Height / 2.0f );
			double r0 = Math.Sqrt( x1*x1 + y1*y1 );

			for ( int count = 2; count < 5; count++ )
			{
				double r1 = r0 / count;
				double phi = Math.Atan( y1 / x1 );
				double x2 = (int)Math.Abs( r1 * Math.Cos( phi ) );
				if ( Math.Sign( x1 ) != Math.Sign( x2 ) )
					x2 *= Math.Sign( x1 );

				double y2 = (int)Math.Abs( r1 * Math.Sin( phi ) );
				int i = Math.Sign( y1 );
				if ( Math.Sign( y1 ) != Math.Sign( y2 ) )
					y2 *= Math.Sign( y1 );

				x = (int)( x2 + ( (double)this.Width / 2.0f ) );
				y = (int)( y2 + ( (double)this.Height / 2.0f ) );

				handlePosition.X = x;
				handlePosition.Y = y;
				RepaintMe();		
	
				Thread.Sleep( 30 );
			}

			moveHandle( this.Width / 2, this.Height / 2 );
			RepaintMe();		

			//threadRunning = false;
		}

		private void dropHandle()
		{
			dropHandleThread = new Thread( new ThreadStart (droppingHandle) );
			dropHandleThread.Start();
		}

		private void VirtualJoystick_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{			
			// check if mouse down in area of handle
			int centerX = this.Width / 2;
			int centerY = this.Height / 2;
			int handleRadius = handleImage.Width / 2;
			int selectRadius = (int) Math.Sqrt( Math.Pow( ( e.X - centerX ), 2 ) + Math.Pow( ( e.Y - centerY ), 2 ) );

			if ( selectRadius <= handleRadius )
			{
				// selection is on the handle, lets set an offset so the handle doesn't jump but instead remain where it is
				handleOffsetX = e.X - centerX;
				handleOffsetY = e.Y - centerY;
			}
			
			trackMouse = true;
			
			// lets adjust x and y for the selection offset (user slop)
			int x = e.X - handleOffsetX;
			int y = e.Y - handleOffsetY;
			moveHandle( x, y );
		}

		private void VirtualJoystick_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			trackMouse = false;
			dropHandle();
		}

		private void VirtualJoystick_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( trackMouse )
			{
				// lets adjust x and y for the selection offset (user slop)
				int x = e.X - handleOffsetX;
				int y = e.Y - handleOffsetY;
				moveHandle( x, y );
			}
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

		protected virtual void OnPositionChanged( PositionChangedEventArgs ea) 
		{
            RaiseEvent(PositionChanged, new object[] { this, ea });
		}


		private void VirtualJoystick_DragLeave(object sender, System.EventArgs e)
		{
			if ( trackMouse )
			{
				trackMouse = false;
				dropHandle();
			}
		}

		private void VirtualJoystick_MouseLeave(object sender, System.EventArgs e)
		{
			if ( trackMouse )
			{
				trackMouse = false;
				dropHandle();
			}
		}
	}
}
