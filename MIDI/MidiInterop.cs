// Stephen Toub
// stoub@microsoft.com
//
// MidiInterop.cs
// Classes for interop with Win32 MCI and low-level MIDI API

#region Namespaces
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
#endregion

namespace Toub.Sound.Midi
{
	/// <summary>Provides access to the Media Control Interface and other MIDI functionality.</summary>
	internal sealed class MidiInterop
	{
		#region Constants
		/// <summary>The default output MIDI device id.</summary>
		private const int _MIDI_MAPPER = -1;
		#endregion

		#region MCI Commands
		/// <summary>Sends an MCI command.</summary>
		/// <param name="command">The command to be sent.</param>
		public static void MciSendString(string command)
		{
			// Make sure we got a command
			if (command == null) throw new ArgumentNullException("command");

			// Send the command.
			int rv = NativeMethods.MciSendString(command, null, 0, IntPtr.Zero);
			if (rv != 0) ThrowMCIError(rv, "Could not execute command '" + command + "'.");
		}

		/// <summary>Gets the description for the given MCI error code.</summary>
		/// <param name="errorCode">The error code for which we need an error description.</param>
		/// <returns>The error description (or null if none exists).</returns>
		private static string GetMciError(int errorCode) 
		{
			StringBuilder buffer = new StringBuilder(255); // max string should be 128, so 255 just to be safe
			if (NativeMethods.MciGetErrorString(errorCode, buffer, buffer.Capacity) == 0) return null;
			return buffer.ToString();
		}
		#endregion

		#region Low-Level Commands
		/// <summary>Opens the default MIDI output device.</summary>
		public static MidiDeviceHandle OpenMidiOut()
		{
			// Open the MIDI_MAPPER device (default).
			return OpenMidiOut(_MIDI_MAPPER);
		}

		/// <summary>Opens the specified MIDI output device.</summary>
		/// <param name="deviceID">The ID of the MIDI output device to be opened.</param>
		public static MidiDeviceHandle OpenMidiOut(int deviceID)
		{
			int handle = 0;
			int rv = NativeMethods.MidiOutOpen(ref handle, deviceID, IntPtr.Zero, 0, 0);
			if (rv != 0) ThrowMCIError(rv, "Could not open MIDI out.");
			return new MidiDeviceHandle(handle);
		}

		/// <summary>Sends the message to as a short MIDI message to the MIDI output device.</summary>
		/// <param name="handle">Handle to the MIDI output device.</param>
		/// <param name="message">The message to be sent.</param>
		public static void SendMidiMessage(MidiDeviceHandle handle, int message)
		{
			if (handle == null) throw new ArgumentNullException("handle", "The handle does not exist.  Make sure the MIDI device has been opened.");
			int rv = NativeMethods.MidiOutShortMessage(handle.Handle, message);
			if (rv != 0) ThrowMCIError(rv, "Could not execute message '" + message + "'.");
		}

		/// <summary>Throws an exception based on the MCI error number.</summary>
		/// <param name="rv">The MCI error number.</param>
		/// <param name="optionalMessage">The message to throw if an MCI message can't be retrieved.</param>
		private static void ThrowMCIError(int rv, string optionalMessage)
		{
			//If there is an error, throw an exception with the best error description we can get.
			string error = GetMciError(rv);
			if (error == null) error = "Could not close MIDI out.";
			throw new InvalidOperationException(error);
		}
		#endregion

		/// <summary>Represents a safe handle to a MIDI device.</summary>
		public sealed class MidiDeviceHandle : IDisposable
		{
			#region Member Variables
			/// <summary>Handle to the MIDI device.</summary>
			private int _handle = 0;
			/// <summary>Whether the handle has been disposed.</summary>
			private bool _isDisposed = false;
			#endregion

			#region Construction
			/// <summary>Initialize the safe handle.</summary>
			/// <param name="handle">The handle to the MIDI device.</param>
			public MidiDeviceHandle(int handle)
			{
				// Store the handle
				_handle = handle;
			}

			/// <summary>Dispose of the handle.</summary>
			~MidiDeviceHandle()
			{
				Dispose(false);
			}
			#endregion

			#region Closing the Handle
			/// <summary>Closes the handle.</summary>
			public void Close()
			{
				// If the handle is open, close it and mark it as such.
				if (IsOpen)
				{
					int rv = NativeMethods.MidiOutClose(_handle);
					_handle = 0;
					if (rv != 0) ThrowMCIError(rv, "Could not close MIDI out.");
				}
			}

			/// <summary>Gets whether the handle is active and open.</summary>
			public bool IsOpen { get { return _handle != 0; } }

			/// <summary>Dispose of the handle.</summary>
			/// <param name="disposing">Whether we're being called from Dispose.</param>
			private void Dispose(bool disposing)
			{
				// If not yet disposed
				if (!_isDisposed)
				{
					// Close the handle and mark us as having been disposed
					Close();
					_isDisposed = true;
					if (disposing) GC.SuppressFinalize(this);
				}
			}

			/// <summary>Dispose of the handle.</summary>
			void IDisposable.Dispose() { Dispose(true); }
			#endregion

			#region Properties
			/// <summary>Gets the underlying handle.</summary>
			public int Handle { get { return _handle; } }
			#endregion
		}

		/// <summary>Imports for native Win32 functions.</summary>
		private sealed class NativeMethods
		{
			#region DllImports for MCI
			/// <summary>
			/// The mciSendString function sends a command string to an MCI device. The device that the 
			/// command is sent to is specified in the command string.
			/// </summary>
			/// <param name="lpszCommand">Pointer to a null-terminated string that specifies an MCI command string.</param>
			/// <param name="lpszReturnString">Pointer to a buffer that receives return information. If no return information is needed, this parameter can be null.</param>
			/// <param name="cchReturn">Size, in characters, of the return buffer specified by the lpszReturnString parameter.</param>
			/// <param name="hwndCallback">Handle to a callback window if the "notify" flag was specified in the command string.</param>
			/// <returns>
			/// Returns zero if successful or an error otherwise. The low-order word of the returned 
			/// DWORD value contains the error return value. If the error is device-specific, the 
			/// high-order word of the return value is the driver identifier; otherwise, the high-order 
			/// word is zero.
			/// </returns>
			[DllImport( "winmm.dll", EntryPoint="mciSendStringA", CharSet=CharSet.Ansi)]
			public static extern int MciSendString(string lpszCommand, StringBuilder lpszReturnString, int cchReturn, IntPtr hwndCallback);
			
			/// <summary>The mciGetErrorString function retrieves a string that describes the specified MCI error code.</summary>
			/// <param name="fdwError">Error code returned by the mciSendCommand or mciSendString function.</param>
			/// <param name="lpszErrorText">Pointer to a buffer that receives a null-terminated string describing the specified error.</param>
			/// <param name="cchErrorText">Length of the buffer, in characters, pointed to by the lpszErrorText parameter.</param>
			/// <returns>Returns non-zero if successful or 0 if the error code is not known.</returns>
			/// <remarks>Each string that MCI returns, whether data or an error description, can be a maximum of 128 characters.</remarks>
			[DllImport("winmm.dll", EntryPoint="mciGetErrorStringA", CharSet=CharSet.Ansi)]
			public static extern int MciGetErrorString(int fdwError, StringBuilder lpszErrorText, int cchErrorText);
			#endregion
		
			#region Low-Level MIDI API
			/// <summary>The midiOutOpen function opens a MIDI output device for playback.</summary>
			/// <param name="lphMidiOut">Pointer to an HMIDIOUT handle.</param>
			/// <param name="uDeviceID">Identifier of the MIDI output device that is to be opened.</param>
			/// <param name="dwCallback">Pointer to a callback function, an event handle, a thread identifier, or a handle of a window or thread called during MIDI playback to process messages related to the progress of the playback.</param>
			/// <param name="dwInstance">User instance data passed to the callback.</param>
			/// <param name="dwFlags">Callback flag for opening the device.</param>
			/// <returns>Returns MMSYSERR_NOERROR if successful or an error otherwise.</returns>
			[DllImport("winmm.dll", EntryPoint="midiOutOpen", CharSet=CharSet.Ansi)]
			public static extern int MidiOutOpen(ref int lphMidiOut, int uDeviceID, IntPtr dwCallback, int dwInstance, int dwFlags);

			/// <summary>The midiOutClose function closes the specified MIDI output device.</summary>
			/// <param name="hMidiOut">Handle to the MIDI output device.</param>
			/// <returns>Returns MMSYSERR_NOERROR if successful or an error otherwise.</returns>
			[DllImport("winmm.dll", EntryPoint="midiOutClose", CharSet=CharSet.Ansi)]
			public static extern int MidiOutClose(int hMidiOut);

			/// <summary>The midiOutShortMsg function sends a short MIDI message to the specified MIDI output device.</summary>
			/// <param name="hMidiOut">Handle to the MIDI output device.</param>
			/// <param name="dwMsg">MIDI message.</param>
			/// <returns>Returns MMSYSERR_NOERROR if successful or an error otherwise.</returns>
			[DllImport("winmm.dll", EntryPoint="midiOutShortMsg", CharSet=CharSet.Ansi)]
			public static extern int MidiOutShortMessage(int hMidiOut, int dwMsg);
			#endregion
		}
	}
}
