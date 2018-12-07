using System;

namespace WinUI.MCS.Interfaces
{
	public class MCSFeedBackEventArgs
	{
		// TODO events do not work with Sentinel plug-in architecture.  need to find work around.
	}

	public delegate void MCSFeedBackEventHandler(object sender, MCSFeedBackEventArgs e);  

	public abstract class MCSConsts
	{
        public enum Direction
        {
            LEFT = 0, RIGHT,
            UP, DOWN,
            IN, OUT,
            OPEN, CLOSE,
            NEAR, FAR,
            PRESET, SET_PRESET,
            AUTO_IRIS, AUTO_FOCUS,
            AUX_ON, AUX_OFF,
            UNDEFINED
        };
        public static string[] directionString = { 
													 "PAN_LEFT", "PAN_RIGHT", 
													 "TILT_UP", "TILT_DOWN", 
													 "ZOOM_IN", "ZOOM_OUT", 
													 "IRIS_OPEN", "IRIS_CLOSE", 
													 "FOCUS_NEAR", "FOCUS_FAR",
													 "PRESET", "SET_PRESET",
													 "AUTO_IRIS", "AUTO_FOCUS",
													 "AUX_ON", "AUX_OFF",
													 "UNDEFINED"
												 };
    }

	/// <summary>
	/// Summary description for IMCSController.
	/// </summary>
	public interface IMCSController //: IDisposable
	{
		//[Action( "Get string of last MCS error.", "D10B7DF2-BD44-4e3a-B7BE-7447AFC655B4") ]
		string GetLastMCSError( );
		// Parameters
		//  none
		// Return
		//  last error string

		// Connection Methods    
		//[Action( "Connect to MCS controller (if matrix switch, host device must already be connected.)",
			 //"68B6349C-1CCA-4d71-BC93-6626FABE60F1" ) ]
        void ConnectMCS(string camera);
		// If the device is a matrix switcher, it must already be connected for this command to succeed.  It basically selected the camera to be controlled.
		// Parameters
		//  camera – camera number/name
		// Return
		//  true if connected is valid

		//[Action( "Disconnect MCS", "E150D542-4C6B-4bb5-AEAE-993BF3968E8C" ) ]
        void DisconnectMCS();

		//[Action( "Get CurrentCamera", "41FDE849-8D99-43a3-ABB5-C0B992E0362F" ) ]
		string CurrentMCSCamera(  );
		// Parameters
		//  none
		// Return
		//  Number of camera controlling
    
		//[Action( "Check if connected", "595C0F55-A140-4dc2-B2A2-62A15448A948" ) ]
		bool IsMCSConnected(  );
		// Parameters
		//  none
		// Return
		//  true if connected


		//MCS Methods
		//[Action( "Move MCS Camera, check to see if in use first", "828D4C97-34FD-4397-8326-A99B06A3BDAE" ) ]
		bool SafeMove( int idir, long speed );
		//void SafeMove( MCSConsts.Direction dir, long speed );
		// Parameters
		//  direction: RIGHT, LEFT, UP...
		//  speed: ( 0 – 100 )
		// Return
		//  true if successful

		//[Action( "Move MCS Camera", "868BF403-ECE3-4a91-A6C2-A88C2E0C075E" ) ]
        void Move(int idir, long speed);
		//void Move( MCSConsts.Direction dir, long speed );
		// Parameters
		//  direction: RIGHT, LEFT, UP...
		//  speed: ( 0 – 100 )
		// Return
		//  none

		//[Action( "Stop All MCS Movement", "153F9EAA-3B28-4af0-ABFA-AEBE0D8A4C70" ) ]
        void StopAll();
		// Stop all MCS movements for this camera
		// Parameters
		//  none
		// Return
		//  true if successful
    
		//[Action( "Goto MCS Preset", "D37FDBBB-F8A2-478b-8525-48A07D00CE5F" ) ]
        void Preset(long number);
		// Parameters
		//  number: preset number
		// Return
		//  true if successful
        
		//[Action( "Set MCS Preset", "0A674A10-9AE0-4eaf-9F6E-104328CD1378" ) ]
        void SetPreset(long number);
		// Parameters
		//  number: preset number
		// Return
		//  true if successful
       
		//[Action( "Goto absolute position (may not be supported)", "EFEDE13B-557C-4af6-ABC6-4F0505C0AAE5" ) ]
        void SetPosition(float pan, float tilt, float zoom);
		// If selected MCS camera supports absolute positioning, this can be used to command it.
		// Parameters
		//  pan: degrees
		//  tilt: degrees
		//  zoom: X units
		// Return
		//  true if successful
    	    
		//[Action( "Get current position (may not be supported)", "5A44B63D-72CE-46e6-B643-2D06227160AD" ) ]    
		string GetPosition();
		// Parameters
		//  pan: degrees (0.00 – 359.99)
		//  tilt: degrees (0.00 – 359.99)
		//  zoom: units (0.00 – 100.00)
		// Return
		//  Position in string form: “122.0, -10.5, 2.2”    

        void DisposeController();
		// Events
		
	}


    //public interface IMCSControllerEvent //: IDisposable
    //{
    //    event MCSFeedBackEventHandler MCSFeedBack;
    //}
}
