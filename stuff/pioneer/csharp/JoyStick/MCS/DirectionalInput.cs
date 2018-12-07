using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using WinUI.MCS.Interfaces;
using log4net;
using System.Xml;

namespace WinUI.MCS
{
    public class MovementEventArgs : EventArgs
    {
        public MCSConsts.Direction moveDirection;
        public int moveValue;
    }

    public delegate void MovementEventHandler(object sender, MovementEventArgs e);

    /// <summary>
    ///		DirectionalInput class
    /// </summary>
    public class DirectionalInput : IDisposable
    {
        protected static DIRECTIONALINPUTLib.DirectionalInput dinput;
        private Thread InputPollThread;
        private bool stopInputThread = false;
        private bool inputThreadStopped = false;

        public const int MAX_PRESETS = 16;
        public const int MAX_AUX = 4;

        public static ILog Logger = LogManager.GetLogger(typeof(DirectionalInput));

        public event MovementEventHandler Movement;

        public const string MCSConfigFileName = "MCS_Config.xml";
        public string MCSConfigPath = @"c:\MCS";
        public string MCSXmlNodeName = "Config/MCS/DeviceToMCSMapping";

        public DirectionalInput()
        {
            // create instance of DirectionalInput control
            dinput = new DIRECTIONALINPUTLib.DirectionalInput();

            // setup DirectionalInput control
            dinput.ConfigFileNodePath = MCSXmlNodeName;

            string MCSFile = MCSConfigPath + "\\" + MCSConfigFileName;

            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(MCSFile))
            {
                dinput.ConfigFilePath = MCSFile;
                doc.Load(dinput.ConfigFilePath);
            }
            else
            {
                MCSFile = System.IO.Directory.GetCurrentDirectory() + "\\" + MCSConfigFileName;
                if (System.IO.File.Exists(MCSFile))
                {
                    dinput.ConfigFilePath = MCSFile;
                    doc.Load(dinput.ConfigFilePath);
                }
                else
                {
                    // uhoh, cannot find config file
                    //System.Windows.Forms.MessageBox.Show("Cannot locate MCS config file: " + MCSConfigFileName);
                    // okay, just use the embedded one
                    Stream configStream = null;
                    
                    Assembly a = Assembly.GetExecutingAssembly();

                    // get a list of resource names from the manifest
                    string[] resNames = a.GetManifestResourceNames();

                    foreach (string s in resNames)
                    {
                        if (s.ToLower() == "directinput.mcs_config.xml")
                        {
                            // attach to stream to the resource in the manifest
                            configStream = a.GetManifestResourceStream(s);
                            dinput.ConfigFilePath = "MCS_config_tmp.xml";
                            System.IO.FileStream file = new System.IO.FileStream(dinput.ConfigFilePath, FileMode.Create, FileAccess.ReadWrite);                            
                            byte[] buffer = new byte[ configStream.Length ];
                            configStream.Read( buffer, 0, (int)configStream.Length );
                            file.Write(buffer, 0, (int)configStream.Length);
                            file.Close();
                            configStream.Seek(0, SeekOrigin.Begin);
                            doc.Load(configStream);
                            break;
                        }
                    }

                    if (configStream == null)
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot locate MCS config file: " + MCSConfigFileName);
                    }
                }
            }

            int deadZone = 15;
            int functionMaxValue = 1;
            int stateMaxValue = 100;

            XmlNodeList settingsNodeList = doc.SelectNodes("/Root/Config/MCS/Settings");

            if (settingsNodeList.Count > 0)
            {
                XmlNodeList nodeList = settingsNodeList.Item(0).ChildNodes;
                if (nodeList.Count > 0)
                {
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        if (nodeList.Item(i).Name.ToLower().CompareTo("deadzone") == 0)
                        {
                            deadZone = int.Parse(nodeList.Item(i).InnerText);
                        }
                        else if (nodeList.Item(i).Name.ToLower().CompareTo("functionmaxvalue") == 0)
                        {
                            functionMaxValue = int.Parse(nodeList.Item(i).InnerText);
                        }
                        else if (nodeList.Item(i).Name.ToLower().CompareTo("statemaxvalue") == 0)
                        {
                            stateMaxValue = int.Parse(nodeList.Item(i).InnerText);
                        }
                    }
                }
            }

            dinput.FunctionMaxValue = functionMaxValue;
            dinput.StateMaxValue = stateMaxValue;
            dinput.JoyStickDeadZone = deadZone;

            // initialize DirectionalInput control
            if (dinput.Initialize())
            {
                InputPollThread = new Thread(new ThreadStart(InputPoll));
                InputPollThread.Start();
            }
            else
            {
                // throw an exception
            }
        }

        public void Dispose()
        {
            stopInputThread = true;
            // wait for input thread to stop
            DateTime waitStartTime = DateTime.Now;
            while (!inputThreadStopped)
            {
                TimeSpan ts = DateTime.Now - waitStartTime;
                if (ts > TimeSpan.FromSeconds(5.0))
                {
                    // shouldn't take this long, let's just get outta here and hope for the best
                    return;
                }
                Application.DoEvents();
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



        protected virtual void OnMovement(MovementEventArgs ea)
        {
            RaiseEvent(Movement, new object[] { this, ea });
            //if (Movement != null)
            //{
            //    //Invokes the delegates.
            //    Movement(this, e);
            //}
        }

        public void InputPoll()
        {
            int inputValue;
            string preset;

            int[] prevState = new int[MCSConsts.directionString.Length];
            int[] prevPresetState = new int[MAX_PRESETS];
            int[] prevSetPresetState = new int[MAX_PRESETS];

            try
            {
                while (!stopInputThread)
                {
                    // update direct input
                    dinput.PollInput();

                    // Check positional
                    for (int i = 0; i < MCSConsts.directionString.Length; i++)
                    {
                        inputValue = dinput.GetInputState(MCSConsts.directionString[i]);
                        if (inputValue != prevState[i])
                        {
                            MovementEventArgs e = new MovementEventArgs();
                            e.moveDirection = (MCSConsts.Direction)i;
                            e.moveValue = inputValue;
                            prevState[i] = inputValue;
                            OnMovement(e);
                        }
                    }

                    // Check presets
                    for (int i = 0; i < MAX_PRESETS; i++)
                    {
                        preset = "PRESET_" + i;
                        inputValue = dinput.GetInputState(preset);
                        if (inputValue != (prevPresetState[i]))
                        {
                            prevPresetState[i] = inputValue;
                            if (inputValue != 0) // only send command when state is non-zero, thus command only on button down
                            {
                                MovementEventArgs e = new MovementEventArgs();
                                e.moveDirection = MCSConsts.Direction.PRESET;
                                e.moveValue = i;
                                OnMovement(e);
                            }
                        }

                        preset = "SET_PRESET_" + i;
                        inputValue = dinput.GetInputState(preset);
                        if (inputValue != (prevSetPresetState[i]))
                        {
                            prevSetPresetState[i] = inputValue;
                            if (inputValue != 0) // only send command when state is non-zero, thus command only on button down
                            {
                                MovementEventArgs e = new MovementEventArgs();
                                e.moveDirection = MCSConsts.Direction.SET_PRESET;
                                e.moveValue = i;
                                OnMovement(e);
                            }
                        }
                    }

                    // Check auxillary commands
                    for (int i = 0; i < MAX_AUX; i++)
                    {
                        preset = "AUX_ON_" + i;
                        inputValue = dinput.GetInputState(preset);
                        if (inputValue != (prevPresetState[i]))
                        {
                            prevPresetState[i] = inputValue;
                            if (inputValue != 0) // only send command when state is non-zero, thus command only on button down
                            {
                                MovementEventArgs e = new MovementEventArgs();
                                e.moveDirection = MCSConsts.Direction.AUX_ON;
                                e.moveValue = i;
                                OnMovement(e);
                            }
                        }

                        preset = "AUX_OFF_" + i;
                        inputValue = dinput.GetInputState(preset);
                        if (inputValue != (prevSetPresetState[i]))
                        {
                            prevSetPresetState[i] = inputValue;
                            if (inputValue != 0) // only send command when state is non-zero, thus command only on button down
                            {
                                MovementEventArgs e = new MovementEventArgs();
                                e.moveDirection = MCSConsts.Direction.AUX_OFF;
                                e.moveValue = i;
                                OnMovement(e);
                            }
                        }
                    }


                    // Check set presets

                    Thread.Sleep(50);
                }
                dinput = null;
            }
            catch (ThreadAbortException e)
            {
                inputThreadStopped = true;
                throw (e);
            }
            catch (Exception)
            {
                // likely a direct input error
            }
            inputThreadStopped = true;
        }
    }
}
