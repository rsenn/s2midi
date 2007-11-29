
#region Namespace Inclusions
using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using S2MIDI.Properties;
using Toub.Sound.Midi;
using System.Runtime.InteropServices;
#endregion

namespace SerialPortTerminal
{
  #region Public Enumerations
  public enum DataMode { Text, Hex }
  public enum LogMsgType { Incoming, Outgoing, Normal, Warning, Error };
  #endregion

  public partial class frmTerminal : Form
  {
    #region Local Variables
      static string Version = "1.01";

    // Not covered by the MIDIPlayer library
      [DllImport("winmm.dll")]
      protected static extern int midiOutGetNumDevs();

      [DllImport("winmm.dll")]
      protected static extern int midiOutGetDevCaps(int deviceID,
          ref MidiOutCaps caps, int sizeOfMidiOutCaps);

    // The main control for communicating through the RS-232 port
    private SerialPort comport = new SerialPort();

    // Various colors for logging info
    private Color[] LogMsgTypeColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
// MIDI stuff
    Queue<byte> midiQueue = new Queue<byte>(); // unprocessed data
    Queue<byte> midiMsg = new Queue<byte>(); // msg to be sent
      public struct MidiOutCaps
      {
          #region MidiOutCaps Members

          /// <summary>
          /// Manufacturer identifier of the device driver for the Midi output 
          /// device. 
          /// </summary>
          public short mid;

          /// <summary>
          /// Product identifier of the Midi output device. 
          /// </summary>
          public short pid;

          /// <summary>
          /// Version number of the device driver for the Midi output device. The 
          /// high-order byte is the major version number, and the low-order byte 
          /// is the minor version number. 
          /// </summary>
          public int driverVersion;

          /// <summary>
          /// Product name.
          /// </summary>
          [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
          public string name;

          /// <summary>
          /// Flags describing the type of the Midi output device. 
          /// </summary>
          public short technology;

          /// <summary>
          /// Number of voices supported by an internal synthesizer device. If 
          /// the device is a port, this member is not meaningful and is set 
          /// to 0. 
          /// </summary>
          public short voices;

          /// <summary>
          /// Maximum number of simultaneous notes that can be played by an 
          /// internal synthesizer device. If the device is a port, this member 
          /// is not meaningful and is set to 0. 
          /// </summary>
          public short notes;

          /// <summary>
          /// Channels that an internal synthesizer device responds to, where the 
          /// least significant bit refers to channel 0 and the most significant 
          /// bit to channel 15. Port devices that transmit on all channels set 
          /// this member to 0xFFFF. 
          /// </summary>
          public short channelMask;

          /// <summary>
          /// Optional functionality supported by the device. 
          /// </summary>
          public int support;

          #endregion
      }

#endregion

    #region Constructor
    public frmTerminal()
    {
      // Build the form
      InitializeComponent();

      // Restore the users settings
      InitializeControlValues();

      // Enable/disable controls based on the current state
      EnableControls();

      // When data is recieved through the port, call this method
      comport.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
    }
    #endregion

    #region Local Methods
    
    /// <summary> Save the user's settings. </summary>
    private void SaveSettings()
    {
      Settings.Default.BaudRate = int.Parse(cmbBaudRate.Text);
      Settings.Default.DataBits = int.Parse(cmbDataBits.Text);
      Settings.Default.DataMode = CurrentDataMode;
      Settings.Default.Parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text);
      Settings.Default.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBits.Text);
      Settings.Default.PortName = cmbPortName.Text;
      Settings.Default.midiPort = midiPortBox.Text;

      Settings.Default.Save();
    }

    /// <summary> Populate the form's controls with default settings. </summary>
    private void InitializeControlValues()
    {
        // Midi Setup
        if (midiOutGetNumDevs() == 0)
        {
            MessageBox.Show(this, "No MIDI output ports detected.", "No MIDI ports", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }

        MidiOutCaps caps = new MidiOutCaps();
        for (int c = 0; c < midiOutGetNumDevs(); c++)
        {
            int result = midiOutGetDevCaps(c, ref caps, Marshal.SizeOf(caps));
            midiPortBox.Items.Add(caps.name);
        }
        if (midiPortBox.Items.Contains(Settings.Default.midiPort))
            midiPortBox.Text = Settings.Default.midiPort;
        else
            midiPortBox.SelectedIndex = 0;

// Serial Setup
      cmbParity.Items.Clear(); cmbParity.Items.AddRange(Enum.GetNames(typeof(Parity)));
      cmbStopBits.Items.Clear(); cmbStopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));

      cmbParity.Text = Settings.Default.Parity.ToString();
      cmbStopBits.Text = Settings.Default.StopBits.ToString();
      cmbDataBits.Text = Settings.Default.DataBits.ToString();
      cmbParity.Text = Settings.Default.Parity.ToString();
      cmbBaudRate.Text = Settings.Default.BaudRate.ToString();
      CurrentDataMode = Settings.Default.DataMode;

      cmbPortName.Items.Clear();
      foreach (string s in SerialPort.GetPortNames())
        cmbPortName.Items.Add(s);

      if (cmbPortName.Items.Contains(Settings.Default.PortName)) cmbPortName.Text = Settings.Default.PortName;
      else if (cmbPortName.Items.Count > 0) cmbPortName.SelectedIndex = 0;
      else
      {
        MessageBox.Show(this, "No COM ports detected.", "No COM ports", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.Close();
      }
    }

    /// <summary> Enable/disable controls based on the app's current state. </summary>
    private void EnableControls()
    {
      // Enable/disable controls based on whether the port is open or not
      gbPortSettings.Enabled = gbMidi.Enabled = !comport.IsOpen;

      if (comport.IsOpen)
      {
          btnOpenPort.Text = "&Stop";
          MidiPlayer.OpenMidi(midiPortBox.SelectedIndex);
      }
      else
      {
          MidiPlayer.CloseMidi();
          btnOpenPort.Text = "&Start";
      }
    }


    /// <summary> Log data to the terminal window. </summary>
    /// <param name="msgtype"> The type of message to be written. </param>
    /// <param name="msg"> The string containing the message to be shown. </param>
    private void Log(LogMsgType msgtype, string msg)
    {
      rtfTerminal.Invoke(new EventHandler(delegate
      {
        rtfTerminal.SelectedText = string.Empty;
        rtfTerminal.SelectionFont = new Font(rtfTerminal.SelectionFont, FontStyle.Bold);
        rtfTerminal.SelectionColor = LogMsgTypeColor[(int)msgtype];
        rtfTerminal.AppendText(msg);
        rtfTerminal.AppendText("\n");
        rtfTerminal.ScrollToCaret();
      }));
    }

    /// <summary> Convert a string of hex digits (ex: E4 CA B2) to a byte array. </summary>
    /// <param name="s"> The string containing the hex digits (with or without spaces). </param>
    /// <returns> Returns an array of bytes. </returns>
    private byte[] HexStringToByteArray(string s)
    {
      s = s.Replace(" ", "");
      byte[] buffer = new byte[s.Length / 2];
      for (int i = 0; i < s.Length; i += 2)
        buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
      return buffer;
    }

    /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
    /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
    /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
    private string ByteArrayToHexString(byte[] data)
    {
      StringBuilder sb = new StringBuilder(data.Length * 3);
      foreach (byte b in data)
        sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
      return sb.ToString().ToUpper();
    }
    #endregion

    #region Local Properties
    private DataMode CurrentDataMode
    {
      get
      {
        if (rbHex.Checked) return DataMode.Hex;
        else return DataMode.Text;
      }
      set
      {
        if (value == DataMode.Text) rbText.Checked = true;
        else rbHex.Checked = true;
      }
    }
    #endregion

    #region Event Handlers
    
    private void frmTerminal_Shown(object sender, EventArgs e)
    {
        Log(LogMsgType.Normal, String.Format("Version {0}\n", Version));
    }
    private void frmTerminal_FormClosing(object sender, FormClosingEventArgs e)
    {
      // The form is closing, save the user's preferences
      SaveSettings();
    }

    private void rbText_CheckedChanged(object sender, EventArgs e)
    { if (rbText.Checked) CurrentDataMode = DataMode.Text; }
    private void rbHex_CheckedChanged(object sender, EventArgs e)
    { if (rbHex.Checked) CurrentDataMode = DataMode.Hex; }

    private void cmbBaudRate_Validating(object sender, CancelEventArgs e)
    { int x; e.Cancel = !int.TryParse(cmbBaudRate.Text, out x); }
    private void cmbDataBits_Validating(object sender, CancelEventArgs e)
    { int x; e.Cancel = !int.TryParse(cmbDataBits.Text, out x); }

    private void btnOpenPort_Click(object sender, EventArgs e)
    {
      // If the port is open, close it.
      if (comport.IsOpen) comport.Close();
      else
      {
        // Set the port's settings
        comport.BaudRate = int.Parse(cmbBaudRate.Text);
        comport.DataBits = int.Parse(cmbDataBits.Text);
        comport.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBits.Text);
        comport.Parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text);
        comport.PortName = cmbPortName.Text;

        // Open the port
        comport.Open();
      }

      // Change the state of the form's controls
      EnableControls();

    }

    private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        // Obtain the number of bytes waiting in the port's buffer
        int bytes = comport.BytesToRead;
        // Create a byte array buffer to hold the incoming data
        byte[] buffer = new byte[bytes];
        comport.Read(buffer, 0, bytes);
        for (int c = 0; c < bytes; c++)
            midiQueue.Enqueue(buffer[c]);
        if (CurrentDataMode == DataMode.Hex)
        {
            Log(LogMsgType.Incoming, ByteArrayToHexString(buffer));
        }
        while (midiQueue.Count > 0)
            {
            midiMsg.Enqueue(midiQueue.Dequeue());
            if (midiMsg.Count == 3)
                {
                byte byteOne = midiMsg.Dequeue();
                byte byteTwo = midiMsg.Dequeue();
                byte byteThree = midiMsg.Dequeue();
                // sanatize everything for MidiPlayer
                if (byteTwo > 127)  byteTwo = 127;
                if (byteThree > 127) byteThree = 127;

                if (byteOne >= 0x90 && byteOne <= 0x9F)
                {
                    byteOne -= 0x90;
                    MidiPlayer.Play(new NoteOn(1, byteOne, byteTwo, byteThree));
                    if (CurrentDataMode == DataMode.Text)
                        Log(LogMsgType.Normal, String.Format("Channel: {1} Note: {0} Velocity: {2}", MidiEvent.GetNoteName(byteTwo), byteOne+1, byteThree));                   
                }
                else if (byteOne >= 0xB0 && byteOne <= 0xBF)
                {
                    byteOne -= 0xB0;
                    MidiPlayer.Play(new Controller(0, byteOne, byteTwo, byteThree));
                    if (CurrentDataMode == DataMode.Text)
                        Log(LogMsgType.Normal, String.Format("Channel: {0} CC: {1} Value: {2}", byteOne+1, byteTwo, byteThree));
                }
            }
        }   
    }
    #endregion
  }
}