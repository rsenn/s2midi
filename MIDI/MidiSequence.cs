// Stephen Toub
// stoub@microsoft.com
//
// MidiSequence.cs
// Class to represent an entire MIDI sequence/file.

#region Namespaces
using System;
using System.IO;
using System.Collections;
#endregion

namespace Toub.Sound.Midi
{
	/// <summary>Represents a MIDI sequence containing tracks of MIDI data.</summary>
	[Serializable]
	public class MidiSequence : IEnumerable
	{
		#region Member Variables
		/// <summary>The format of the MIDI file (0, 1, or 2).</summary>
		private int _format;
		/// <summary>The meaning of delta-times.</summary>
		private int _division;
		/// <summary>The tracks in the MIDI sequence.</summary>
		private ArrayList _tracks;
		#endregion

		#region Construction
		/// <summary>Initialize the MIDI sequence.</summary>
		/// <param name="format">
		/// The format for the MIDI file (0, 1, or 2).
		/// 0 - a single multi-channel track
		/// 1 - one or more simultaneous tracks
		/// 2 - one or more sequentially independent single-track patterns
		/// </param>
		/// <param name="division">The meaning of the delta-times in the file.</param>
		public MidiSequence(int format, int division)
		{
			// Store values
			SetFormat(format);
			Division = division;
			_tracks = new ArrayList();
		}
		#endregion

		#region Sequence Information
		/// <summary>Gets the format of the sequence.</summary>
		public int Format { get { return _format; } }
		/// <summary>Gets or sets the division for the sequence.</summary>
		public int Division 
		{ 
			get { return _division; } 
			set 
			{ 
				if (value < 0) throw new ArgumentOutOfRangeException("Division", value, "The division must not be negative.");
				_division = value; 
			} 
		}
		/// <summary>Gets the number of tracks in the sequence.</summary>
		public int NumberOfTracks { get { return _tracks.Count; } }
		/// <summary>Sets the format to the given value.</summary>
		/// <param name="format">The format to which to set the sequence.</param>
		private void SetFormat(int format)
		{
			if (format < 0 || format > 2) throw new ArgumentOutOfRangeException("format", format, "Format must be 0, 1, or 2.");
			_format = format;
		}
		#endregion

		#region Midi Tracks
		/// <summary>Adds a track to the MIDI sequence.</summary>
		/// <returns>The new track as added to the sequence.  Modifications made to the track will be reflected.</returns>
		public MidiTrack AddTrack()
		{
			// Create a new track, add it, and return it
			MidiTrack track = new MidiTrack();
			AddTrack(track);
			return track;
		}

		/// <summary>Adds a track to the MIDI sequence.</summary>
		/// <param name="track">The complete track to be added.</param>
		public void AddTrack(MidiTrack track)
		{
			// Make sure the track is valid and that is hasn't already been added.
			if (track == null) throw new ArgumentNullException("track");
			if (_tracks.Contains(track)) throw new ArgumentException("This track is already part of the sequence.");

			// If this is format 0, we can only have 1 track
			if (_format == 0 && _tracks.Count >= 1) throw new InvalidOperationException("Format 0 MIDI files can only have 1 track.");

			// Add the track.
			_tracks.Add(track);
		}

		/// <summary>Removes a track that has been adding to the MIDI sequence.</summary>
		/// <param name="track">The track to be removed.</param>
		public void RemoveTrack(MidiTrack track)
		{
			// Remove the track
			_tracks.Remove(track);
		}

		/// <summary>Gets or sets the track at the specified index.</summary>
		public MidiTrack this[int index]
		{
			get { return (MidiTrack)_tracks[index]; }
			set { _tracks[index] = value; }
		}

		/// <summary>Gets the tracks that have been added to the sequence.</summary>
		/// <returns>An array of all tracks that have been added to the sequence.</returns>
		public MidiTrack [] GetTracks()
		{
			return (MidiTrack[])_tracks.ToArray(typeof(MidiTrack));
		}

		/// <summary>Gets an enumerator for the tracks in the sequence.</summary>
		/// <returns>An enumerator for the tracks in the sequence.</returns>
		public IEnumerator GetEnumerator()
		{
			// Return the tracks enumerator
			return _tracks.GetEnumerator();
		}
		#endregion

		#region Saving the Sequence
		/// <summary>Writes the sequence to a string in human-readable form.</summary>
		/// <returns>A human-readable representation of the tracks and events in the sequence.</returns>
		public override string ToString()
		{
			// Create a writer, dump to it, return the string
			StringWriter writer = new StringWriter();
			ToString(writer);
			return writer.ToString();
		}

		/// <summary>Dumps the MIDI sequence to the writer in human-readable form.</summary>
		/// <param name="writer">The writer to which the track should be written.</param>
		public void ToString(TextWriter writer)
		{
			// Validate the writer
			if (writer == null) throw new ArgumentNullException("writer");
		
			// Info on sequence
			writer.WriteLine("MIDI Sequence");
			writer.WriteLine("Format: " + _format);
			writer.WriteLine("Tracks: " + _tracks.Count);
			writer.WriteLine("Division: " + _division);
			writer.WriteLine("");

			// Print out each track
			foreach(MidiTrack track in this) writer.WriteLine(track.ToString());
		}

		/// <summary>Creates a MIDI file at the specified path and writes the sequence to it.</summary>
		/// <param name="path">The MIDI file to create and to which the sequence should be written.</param>
		public void Save(string path)
		{
			// Validate input
			if (path == null) throw new ArgumentNullException("path", "No path provided.");

			// Create the output file and save to it
			using (FileStream stream = new FileStream(path, FileMode.Create)) Save(stream);
		}

		/// <summary>Writes the MIDI sequence to the output stream.</summary>
		/// <param name="outputStream">The stream to which the MIDI sequence should be written.</param>
		public void Save(Stream outputStream)
		{
			// Check parameters
			if (outputStream == null) throw new ArgumentNullException("outputStream");
			if (!outputStream.CanWrite) throw new ArgumentException("Can't write to stream.", "outputStream");

			// Check valid state (as best we can check)
			if (_tracks.Count < 1) throw new InvalidOperationException("No tracks have been added.");

			// Write out the main header for the sequence
			WriteHeader(outputStream, _tracks.Count);

			// Write out each track in the order it was added to the sequence
			for(int i=0; i<_tracks.Count; i++)
			{
				// Write out the track to the stream
				((MidiTrack)_tracks[i]).Write(outputStream);
			}
		}

		/// <summary>Writes a MIDI file header out to the stream.</summary>
		/// <param name="outputStream">The output stream to which the header should be written.</param>
		/// <param name="numTracks">The number of tracks that will be a part of this sequence.</param>
		/// <remarks>This functionality is automatically performed during a Save.</remarks>
		public void WriteHeader(Stream outputStream, int numTracks)
		{
			// Check parameters
			if (outputStream == null) throw new ArgumentNullException("outputStream");
			if (!outputStream.CanWrite) throw new ArgumentException("Can't write to stream.", "outputStream");
			if (numTracks < 1) throw new ArgumentOutOfRangeException("numTracks", numTracks, "Sequences require at least 1 track.");

			// Write out the main header for the sequence
			MThdChunkHeader mainHeader = new MThdChunkHeader(_format, numTracks, _division);
			mainHeader.Write(outputStream);
		}
		#endregion

		#region Importing MIDI files
		/// <summary>Determines whether a MIDI file is valid.</summary>
		/// <param name="path">The path to the MIDI file.</param>
		/// <returns>Whether the MIDI file is valid.</returns>
		/// <remarks>
		/// Note that in order to determine the file's validity, the entire file
		/// must be parsed.  This operation should not be used if Import will also
		/// be used (rather, catch any exceptions thrown by Import.
		/// </remarks>
		public static bool IsValid(string path)
		{
			try 
			{
				// Try to load the file; if we can return true
				Import(path);
				return true;
			}
			catch
			{
				// Couldn't load it; return false
				return false;
			}
		}

		/// <summary>Reads a MIDI stream into a new MidiSequence.</summary>
		/// <param name="path">The path to the MIDI file to be parsed.</param>
		/// <returns>A MidiSequence containing the parsed MIDI data.</returns>
		public static MidiSequence Import(string path)
		{
			// Open the file
			using (FileStream inputStream = new FileStream(path, FileMode.Open)) 
			{
				// Parse it and return the sequence
				return Import(inputStream);
			}
		}

		/// <summary>Reads a MIDI stream into a new MidiSequence.</summary>
		/// <param name="inputStream">The stream containing the MIDI data.</param>
		/// <returns>A MidiSequence containing the parsed MIDI data.</returns>
		public static MidiSequence Import(Stream inputStream)
		{
			// Validate input
			if (inputStream == null) throw new ArgumentNullException("inputStream");
			if (!inputStream.CanRead) throw new ArgumentException("Stream must be readable.", "inputStream");
		
			// Read in the main MIDI header
			MThdChunkHeader mainHeader = MThdChunkHeader.Read(inputStream);

			// Read in all of the tracks
			MTrkChunkHeader [] trackChunks = new MTrkChunkHeader[mainHeader.NumberOfTracks];
			for(int i=0; i<mainHeader.NumberOfTracks; i++)
			{
				trackChunks[i] = MTrkChunkHeader.Read(inputStream);
			}

			// Create the MIDI sequence
			MidiSequence sequence = new MidiSequence(mainHeader.Format, mainHeader.Division);
			for(int i=0; i<mainHeader.NumberOfTracks; i++)
			{
				sequence.AddTrack(MidiParser.ParseToTrack(trackChunks[i].Data));
			}
			return sequence;
		}
		#endregion

		#region Modifying
		#region Transpose
		/// <summary>Transposes a MIDI sequence up/down the specified number of half-steps.</summary>
		/// <param name="sequence">The sequence to be transposed.</param>
		/// <param name="steps">The number of steps up(+) or down(-) to transpose the sequence.</param>
		public static void Transpose(MidiSequence sequence, int steps)
		{
			// Transpose the sequence; do not transpose drum tracks
			Transpose(sequence, steps, false);
		}

		/// <summary>Transposes a MIDI sequence up/down the specified number of half-steps.</summary>
		/// <param name="sequence">The sequence to be transposed.</param>
		/// <param name="steps">The number of steps up(+) or down(-) to transpose the sequence.</param>
		/// <param name="includeDrums">Whether drum tracks should also be transposed.</param>
		/// <remarks>If the step value is too large or too small, notes may wrap.</remarks>
		public static void Transpose(MidiSequence sequence, int steps, bool includeDrums)
		{
			// If the sequence is null, throw an exception.
			if (sequence == null) throw new ArgumentNullException("sequence");
				
			// Modify each track
			foreach(MidiTrack track in sequence)
			{
				// Modify each event
				foreach(MidiEvent ev in track.Events)
				{
					// If the event is not a voice MIDI event but the channel is the
					// drum channel and the user has chosen not to include drums in the
					// transposition (which makes sense), skip this event.
					NoteVoiceMidiEvent nvme = ev as NoteVoiceMidiEvent;
					if (nvme == null ||
						(!includeDrums && nvme.Channel == (byte)SpecialChannels.Percussion)) 
						continue;

					// If the event is a NoteOn, NoteOff, or Aftertouch, shift the note
					// according to the supplied number of steps.
					nvme.Note = (byte)((nvme.Note + steps) % 128);
				}
			}
		}
		#endregion

		#region Trimming
		/// <summary>Trims a MIDI file to a specified length.</summary>
		/// <param name="sequence">The sequence to be copied and trimmed.</param>
		/// <param name="totalTime">The requested time length of the new MIDI sequence.</param>
		/// <returns>A MIDI sequence with only those events that fell before the requested time limit.</returns>
		public static MidiSequence Trim(MidiSequence sequence, long totalTime)
		{
			// Create a new sequence to mimic the old
			MidiSequence newSequence = new MidiSequence(sequence.Format, sequence.Division);
			
			// Copy each track up to the specified time limit
			foreach(MidiTrack track in sequence)
			{
				// Create a new track in the new sequence to match the old track in the old sequence
				MidiTrack newTrack = newSequence.AddTrack();

				// Convert all times in the old track to deltas
				track.Events.ConvertDeltasToTotals();
				
				// Copy over all events that fell before the specified time
				for(int i=0; i<track.Events.Count && track.Events[i].DeltaTime < totalTime; i++)
				{
					newTrack.Events.Add(track.Events[i].Clone());
				}

				// Convert all times back (on both new and old tracks; the new one inherited the totals)
				track.Events.ConvertTotalsToDeltas();
				newTrack.Events.ConvertTotalsToDeltas();

				// If the new track lacks an end of track, add one
				if (!newTrack.HasEndOfTrack) newTrack.Events.Add(new EndOfTrack(0));
			}

			// Return the new sequence
			return newSequence;
		}
		#endregion

		#region Format Conversion
		/// <summary>Converts a MIDI sequence from its current format to the specified format.</summary>
		/// <param name="sequence">The sequence to be converted.</param>
		/// <param name="format">The format to which we want to convert the sequence.</param>
		/// <returns>The converted sequence.</returns>
		/// <remarks>
		/// This may or may not return the same sequence as passed in.
		/// Regardless, the reference passed in should not be used after this call as the old
		/// sequence could be unusable if a different reference was returned.
		/// </remarks>
		public static MidiSequence Convert(MidiSequence sequence, int format)
		{
			return Convert(sequence, format, FormatConversionOptions.None);
		}

		/// <summary>Converts a MIDI sequence from its current format to the specified format.</summary>
		/// <param name="sequence">The sequence to be converted.</param>
		/// <param name="format">The format to which we want to convert the sequence.</param>
		/// <param name="options">Options used when doing the conversion.</param>
		/// <returns>The converted sequence.</returns>
		/// <remarks>
		/// This may or may not return the same sequence as passed in.
		/// Regardless, the reference passed in should not be used after this call as the old
		/// sequence could be unusable if a different reference was returned.
		/// </remarks>
		public static MidiSequence Convert(MidiSequence sequence, int format, FormatConversionOptions options)
		{
			// Validate the parameters
			if (sequence == null) throw new ArgumentNullException("sequence");
			if (format < 0 || format > 2) throw new ArgumentOutOfRangeException("format", format, "The format must be 0, 1, or 2.");

			// Handle the simple cases
			if (sequence.Format == format) return sequence; // already in requested format
			if (format != 0 || sequence.NumberOfTracks == 1) // only requires change in format #
			{
				// Change the format and return the same sequence
				sequence.SetFormat(format);
				return sequence;
			}
			
			// Now the hard one, converting to format 0.  
			// We need to combine all tracks into 1.
			MidiSequence newSequence = new MidiSequence(format, sequence.Division);
			MidiTrack newTrack = newSequence.AddTrack();

			// Iterate through all events in all tracks and change deltaTimes to actual times.
			// We'll then be able to sort based on time and change them back to deltas later
			foreach(MidiTrack track in sequence) track.Events.ConvertDeltasToTotals();

			// Add all events to new track (except for end of track markers!)
			int trackNumber = 0;
			foreach(MidiTrack track in sequence)
			{
				foreach(MidiEvent midiEvent in track.Events)
				{
					// If this event has a channel, and if we're storing tracks as channels, copy to it
					if ((options & FormatConversionOptions.CopyTrackToChannel) > 0 &&
						(midiEvent is VoiceMidiEvent) &&
						trackNumber >= 0 && trackNumber <= 0xF)
					{
						((VoiceMidiEvent)midiEvent).Channel = (byte)trackNumber;
					}

					// Add all events, except for end of track markers (we'll add our own)
					if (!(midiEvent is EndOfTrack)) newTrack.Events.Add(midiEvent);
				}
				trackNumber++;
			}

			// Sort the events
			newTrack.Events.SortByTime();

			// Now go back through all of the events and update the times to be deltas
			newTrack.Events.ConvertTotalsToDeltas();

			// Put an end of track on for good measure as we've already taken out
			// all of the ones that were in the original tracks.
			newTrack.Events.Add(new EndOfTrack(0)); 

			// Return the new sequence
			return newSequence;
		}

		/// <summary>Options used when performing a format conversion.</summary>
		public enum FormatConversionOptions
		{
			/// <summary>No special formatting.</summary>
			None,
			/// <summary>
			/// Uses the number of the track as the channel for all events on that track.
			/// Only valid if the number of the track is a valid track number.
			/// </summary>
			CopyTrackToChannel
		}
		#endregion
		#endregion
	}
}