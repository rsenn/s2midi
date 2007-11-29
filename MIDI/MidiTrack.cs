// Stephen Toub
// stoub@microsoft.com
//
// MidiTrack.cs
// Class to represent an entire track in a MIDI file.

#region Namespaces
using System;
using System.IO;
using System.Text;
using System.Collections;
#endregion

namespace Toub.Sound.Midi
{
	/// <summary>Represents a single MIDI track in a MIDI file.</summary>
	[Serializable]
	public class MidiTrack
	{
		#region Member Variables
		/// <summary>Whether the track can be written without an end of track marker.</summary>
		private bool _requireEndOfTrack;
		/// <summary>Collection of MIDI event added to this track.</summary>
		private MidiEventCollection _events;
		#endregion

		#region Construction
		/// <summary>Initialize the track.</summary>
		public MidiTrack()
		{
			// Create the buffer to store all event information
			_events = new MidiEventCollection();

			// We don't yet have an end of track marker, but we want one eventually.
			_requireEndOfTrack = true;
		}
		#endregion

		#region Properties
		/// <summary>Gets whether an end of track event has been added.</summary>
		public bool HasEndOfTrack 
		{
			get
			{
				// Determine whether the last event is an end of track event
				if (_events.Count == 0) return false;
				return _events[_events.Count-1] is EndOfTrack;
			}
		}
		/// <summary>Gets or sets whether an end of track marker is required for writing out the entire track.</summary>
		/// <remarks>
		/// Note that MIDI files require an end of track marker at the end of every track.  
		/// Setting this to false could have negative consequences.
		/// </remarks>
		public bool RequireEndOfTrack { get { return _requireEndOfTrack; } set { _requireEndOfTrack = value; } }
		/// <summary>Gets the collection of MIDI events that are a part of this track.</summary>
		public MidiEventCollection Events { get { return _events; } }
		#endregion

		#region Saving the Track
		/// <summary>Write the track to the output stream.</summary>
		/// <param name="outputStream">The output stream to which the track should be written.</param>
		public void Write(Stream outputStream)
		{
			// Validate the stream
			if (outputStream == null) throw new ArgumentNullException("outputStream");
			if (!outputStream.CanWrite) throw new ArgumentException("Cannot write to stream.", "outputStream");

			// Make sure we have an end of track marker if we need one
			if (!HasEndOfTrack && _requireEndOfTrack) throw new InvalidOperationException("The track cannot be written until it has an end of track marker.");
			
			// Get the event data and write it out
			MemoryStream memStream = new MemoryStream();
			for(int i=0; i<_events.Count; i++) _events[i].Write(memStream);

			// Tack on the header and write the whole thing out to the main stream
			MTrkChunkHeader header = new MTrkChunkHeader(memStream.ToArray());
			header.Write(outputStream);
		}
		#endregion

		#region To String
		/// <summary>Writes the track to a string in human-readable form.</summary>
		/// <returns>A human-readable representation of the events in the track.</returns>
		public override string ToString()
		{
			// Create a writer, dump to it, return the string
			StringWriter writer = new StringWriter();
			ToString(writer);
			return writer.ToString();
		}

		/// <summary>Dumps the MIDI track to the writer in human-readable form.</summary>
		/// <param name="writer">The writer to which the track should be written.</param>
		public void ToString(TextWriter writer)
		{
			// Validate the writer
			if (writer == null) throw new ArgumentNullException("writer");
		
			// Print out each event
			foreach(MidiEvent ev in Events)
			{
				writer.WriteLine(ev.ToString());
			}
		}
		#endregion
	}
}