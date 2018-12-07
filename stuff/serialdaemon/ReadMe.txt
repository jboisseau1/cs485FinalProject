/* ***********************************************************************************
	Windows Console (Win32) version of the serialdaemon (George Mason University - Feb 27, 2008)

	The main file is Serial32.cpp in the Serial32 directory.
	The other directories (Exceptions, SerialPortLib, and SocketLib contain projects Serial32 depends on.

	I do not have a solution for only these proejects but creating one is easy.  Just put all the porjects in.
		Remember that SerialPortLIb and SocketLib should compile as static libraries (it easier that way) and
		Serial32 needs to have access to the resulting libraries.

	Serial32 has another readme.txt file....

	The exe file included (seraildaemon.exe) should run in any Windows machine, but I tested only in Vista.

	Bye,

		Guillermo


   ************************************* */