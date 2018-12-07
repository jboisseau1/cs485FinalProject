using System;

namespace WinUI.MCS.Interfaces
{

    public class PositionChangedEventArgs : EventArgs
    {
        public MCSConsts.Direction move;
        public int moveValue;
    }


	/// <summary>
	/// Summary description for IMCSClient.
	/// </summary>
	public interface IMCSClient
	{
		// Connection Methods    
		//[Action( "Message sent by IMCSController if device in use.", "9C2D6D11-AE0E-454c-A301-C30C07237D1F" ) ]
		void MCSInUse( string message );

		//[Action( "Message sent by IMCSController if device has error.", "31961D8E-5F4D-4f42-94FF-920D95B75126" ) ]
        void MCSError(string message);
	}

	public interface IMCSClientGui
	{
		//[Action( "Move MCS Camera, check to see if in use first", "98BADFF4-88B3-42de-B932-0E88BD37103E" ) ]
		bool OnSafeMove( int idir, long speed );

		//[Action( "Move MCS Camera", "75133822-733E-4ebd-B662-F474C690395C" ) ]
        void OnMove(int idir, long speed);

		//[Action( "Stop All MCS Movement", "8F7BEA4E-3C6E-45a3-BD4D-62EE9537CDB5" ) ]
        void OnStopAll();

		//[Action( "Goto MCS Preset", "07015A99-3A0C-44dd-9A52-F3A2919EF085" ) ]
        void OnPreset(long number);

		//[Action( "Set MCS Preset", "BEE3D32F-3A64-4484-94F5-F94BF944C20F" ) ]
        void OnSetPreset(long number);

		//[Action( "Goto absolute position (may not be supported)", "AE304C13-072E-4045-9165-09C15B3DAFD9" ) ]
        void OnSetPosition(float pan, float tilt, float zoom);
	}
}
