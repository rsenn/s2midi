// Stephen Toub
// stoub@microsoft.com
//
// MidiHeaders.cs
// Classes and structs that represent MIDI file and track headers.

#region Namespaces
using System;
using System.IO;
#endregion

namespace Toub.Sound.Midi
{
	#region File and Track Headers
	/// <summary>"MTrk" header for writing out tracks.</summary>
	/// <remarks>"MTrkChunkHeader" is a bit of a misnomer as it includes all of the data for the track, as well, in byte form.</remarks>
	[Serializable]
	internal struct MTrkChunkHeader
	{
		#region Member Variables
		/// <summary>Additional chunk header data.</summary>
		private ChunkHeader _header;
		/// <summary>Data for which this is a header.</summary>
		private byte [] _data;
		#endregion

		#region Construction
		/// <summary>Initialize the MTrk chunk header.</summary>
		/// <param name="data">The track data for which this is a header.</param>
		public MTrkChunkHeader(byte [] data)
		{
			_header = new ChunkHeader(
				MTrkID, // 0x4d54726b = "MTrk"
				data != null ? data.Length : 0);
			_data = data;
		}
		#endregion

		#region Properties
		/// <summary>Gets additional chunk header data.</summary>
		public ChunkHeader Header { get { return _header; } }
		/// <summary>Gets the data for which this is a header.</summary>
		public byte [] Data { get { return _data; } }
		/// <summary>Gets the MTrk header id.</summary>
		private static byte [] MTrkID { get { return new byte[]{0x4d,0x54,0x72,0x6b}; } }
		#endregion

		#region Validation
		/// <summary>Validates that a header is correct as an MThd header.</summary>
		/// <param name="header">The header to be validated.</param>
		private static void ValidateHeader(ChunkHeader header)
		{
			byte [] validHeader = MTrkID;
			for(int i=0; i<4; i++) 
			{
				if (header.ID[i] != validHeader[i]) throw new InvalidOperationException("Invalid MThd header.");
			}
			if (header.Length <= 0) throw new InvalidOperationException("The length of the MThd header is incorrect.");
		}
		#endregion

		#region Writing
		/// <summary>Writes the track header out to the stream.</summary>
		/// <param name="outputStream">The stream to which the header should be written.</param>
		public void Write(Stream outputStream)
		{
			// Validate the stream
			if (outputStream == null) throw new ArgumentNullException("outputStream");
			if (!outputStream.CanWrite) throw new ArgumentException("Cannot write to stream.", "outputStream");

			// Write out the main header followed by all of the data
			_header.Write(outputStream);
			if (_data != null) outputStream.Write(_data, 0, _data.Length);
		}
		#endregion

		#region Reading
		/// <summary>Read in an MTrk chunk from the stream.</summary>
		/// <param name="inputStream">The stream from which to read the MTrk chunk.</param>
		/// <returns>The MTrk chunk read.</returns>
		public static MTrkChunkHeader Read(Stream inputStream)
		{
			// Validate input
			if (inputStream == null) throw new ArgumentNullException("inputStream");
			if (!inputStream.CanRead) throw new ArgumentException("Stream must be readable.", "inputStream");
		
			// Read in a header from the stream and validate it
			ChunkHeader header = ChunkHeader.Read(inputStream);
			ValidateHeader(header);

			// Read in the data (amount specified in the header)
			byte [] data = new byte[header.Length];
			if (inputStream.Read(data, 0, data.Length) != data.Length) throw new InvalidOperationException("Not enough data in stream to read MTrk chunk.");

			// Return the new chunk
			return new MTrkChunkHeader(data);
		}
		#endregion
	}

	/// <summary>"MThd" header for writing out MIDI files.</summary>
	[Serializable]
	internal struct MThdChunkHeader
	{
		#region Member Variables
		/// <summary>Additional chunk header data.</summary>
		private ChunkHeader _header;
		/// <summary>The format for the MIDI file (0, 1, or 2).</summary>
		private int _format;
		/// <summary>The number of tracks in the MIDI sequence.</summary>
		private int _numTracks;
		/// <summary>Specifies the meaning of the delta-times</summary>
		private int _division;
		#endregion

		#region Construction
		/// <summary>Initialize the MThd chunk header.</summary>
		/// <param name="format">
		/// The format for the MIDI file (0, 1, or 2).
		/// 0 - a single multi-channel track
		/// 1 - one or more simultaneous tracks
		/// 2 - one or more sequentially independent single-track patterns
		/// </param>
		/// <param name="numTracks">The number of tracks in the MIDI file.</param>
		/// <param name="division">
		/// The meaning of the delta-times in the file.
		/// If the number is zero or positive, then bits 14 thru 0 represent the number of delta-time 
		/// ticks which make up a quarter-note. If number is negative, then bits 14 through 0 represent
		/// subdivisions of a second, in a way consistent with SMPTE and MIDI time code.
		/// </param>
		public MThdChunkHeader(int format, int numTracks, int division)
		{
			// Verify the parameters
			if (format < 0 || format > 2) throw new ArgumentOutOfRangeException("format", format, "Format must be 0, 1, or 2.");
			if (numTracks < 1) throw new ArgumentOutOfRangeException("numTracks", numTracks, "There must be at least 1 track.");
			if (division < 1) throw new ArgumentOutOfRangeException("division", division, "Division must be at least 1.");

			_header = new ChunkHeader(
				MThdID, // 0x4d546864 = "MThd"
				6);	// 2 bytes for each of the format, num tracks, and division == 6
			_format = format;
			_numTracks = numTracks;
			_division = division;
		}
		#endregion

		#region Properties
		/// <summary>Gets additional chunk header data.</summary>
		public ChunkHeader Header { get { return _header; } }
		/// <summary>Gets the format for the MIDI file (0, 1, or 2).</summary>
		public int Format { get { return _format; } }
		/// <summary>Gets the number of tracks in the MIDI sequence.</summary>
		public int NumberOfTracks { get { return _numTracks; } }
		/// <summary>Gets the meaning of the delta-times</summary>
		public int Division { get { return _division; } }
		/// <summary>Gets the id for an MThd header.</summary>
		private static byte [] MThdID { get { return new byte[]{0x4d,0x54,0x68,0x64}; } }
		#endregion

		#region Validation
		/// <summary>Validates that a header is correct as an MThd header.</summary>
		/// <param name="header">The header to be validated.</param>
		private static void ValidateHeader(ChunkHeader header)
		{
			byte [] validHeader = MThdID;
			for(int i=0; i<4; i++) 
			{
				if (header.ID[i] != validHeader[i]) throw new InvalidOperationException("Invalid MThd header.");
			}
			if (header.Length != 6) throw new InvalidOperationException("The length of the MThd header is incorrect.");
		}
		#endregion

		#region Writing
		/// <summary>Writes the MThd header out to the stream.</summary>
		/// <param name="outputStream">The stream to which the header should be written.</param>
		public void Write(Stream outputStream)
		{
			// Validate the stream
			if (outputStream == null) throw new ArgumentNullException("outputStream");
			if (!outputStream.CanWrite) throw new ArgumentException("Cannot write to stream.", "outputStream");

			// Write out the main header
			_header.Write(outputStream);

			// Add format
			outputStream.WriteByte((byte)((_format & 0xFF00) >> 8));
			outputStream.WriteByte((byte)(_format & 0x00FF));
			
			// Add numTracks
			outputStream.WriteByte((byte)((_numTracks & 0xFF00) >> 8));
			outputStream.WriteByte((byte)(_numTracks & 0x00FF));

			// Add division
			outputStream.WriteByte((byte)((_division & 0xFF00) >> 8));
			outputStream.WriteByte((byte)(_division & 0x00FF));
		}
		#endregion

		#region Reading
		/// <summary>Read in an MThd chunk from the stream.</summary>
		/// <param name="inputStream">The stream from which to read the MThd chunk.</param>
		/// <returns>The MThd chunk read.</returns>
		public static MThdChunkHeader Read(Stream inputStream)
		{
			// Validate input
			if (inputStream == null) throw new ArgumentNullException("inputStream");
			if (!inputStream.CanRead) throw new ArgumentException("Stream must be readable.", "inputStream");
		
			// Read in a header from the stream and validate it
			ChunkHeader header = ChunkHeader.Read(inputStream);
			ValidateHeader(header);

			// Read in the format
			int format = 0;
			for(int i=0; i<2; i++) 
			{
				int val = inputStream.ReadByte();
				if (val < 0) throw new InvalidOperationException("The stream is invalid.");
				format <<= 8;
				format |= val;
			}

			// Read in the number of tracks
			int numTracks = 0;
			for(int i=0; i<2; i++) 
			{
				int val = inputStream.ReadByte();
				if (val < 0) throw new InvalidOperationException("The stream is invalid.");
				numTracks <<= 8;
				numTracks |= val;
			}
			
			// Read in the division
			int division = 0;
			for(int i=0; i<2; i++) 
			{
				int val = inputStream.ReadByte();
				if (val < 0) throw new InvalidOperationException("The stream is invalid.");
				division <<= 8;
				division |= val;
			}

			// Create a new MThd header and return it
			return new MThdChunkHeader(format, numTracks, division);
		}
		#endregion
	}

	/// <summary>Chunk header to store base MIDI chunk information.</summary>
	[Serializable]
	internal struct ChunkHeader
	{
		#region Member Variables
		/// <summary>The id representing this chunk header.</summary>
		private byte [] _id;
		/// <summary>The amount of data following the header.</summary>
		private long _length;
		#endregion

		#region Construction
		/// <summary>Initialize the ChunkHeader.</summary>
		/// <param name="id">The 4-byte header identifier.</param>
		/// <param name="length">The amount of data to be stored under this header.</param>
		public ChunkHeader(byte [] id, long length)
		{
			// Verify the parameters
			if (id == null) throw new ArgumentNullException("id");
			if (id.Length != 4) throw new ArgumentException("The id must be 4 bytes in length.", "id");
			if (length < 0) throw new ArgumentException("The length must be not be negative.", "length");

			// Store the data
			_id = id;
			_length = length;
		}
		#endregion

		#region Properties
		/// <summary>Gets the id representing this chunk header.</summary>
		public byte [] ID { get { return _id; } }
		/// <summary>Gets the amount of data following the header.</summary>
		public long Length { get { return _length; } }
		#endregion

		#region Output
		/// <summary>Writes the chunk header out to the stream.</summary>
		/// <param name="outputStream">The stream to which the header should be written.</param>
		public void Write(Stream outputStream)
		{
			// Validate the stream
			if (outputStream == null) throw new ArgumentNullException("outputStream");
			if (!outputStream.CanWrite) throw new ArgumentException("Cannot write to stream.", "outputStream");

			// Write out the id
			outputStream.WriteByte(_id[0]);
			outputStream.WriteByte(_id[1]);
			outputStream.WriteByte(_id[2]);
			outputStream.WriteByte(_id[3]);

			// Write out the length in big-endian
			outputStream.WriteByte((byte)((_length & 0xFF000000) >> 24));
			outputStream.WriteByte((byte)((_length & 0x00FF0000) >> 16));
			outputStream.WriteByte((byte)((_length & 0x0000FF00) >> 8));
			outputStream.WriteByte((byte)(_length & 0x000000FF));
		}
		#endregion

		#region Reading
		/// <summary>Reads a chunk header from the input stream.</summary>
		/// <param name="inputStream">The stream from which to read.</param>
		/// <returns>The chunk header read from the stream.</returns>
		public static ChunkHeader Read(Stream inputStream)
		{
			// Validate input
			if (inputStream == null) throw new ArgumentNullException("inputStream");
			if (!inputStream.CanRead) throw new ArgumentException("Stream must be readable.", "inputStream");
		
			// Read the id
			byte [] id = new byte[4];
			if (inputStream.Read(id, 0, id.Length) != id.Length) throw new InvalidOperationException("The stream is invalid.");

			// Read the length
			long length = 0;
			for(int i=0; i<4; i++)
			{
				int val = inputStream.ReadByte();
				if (val < 0) throw new InvalidOperationException("The stream is invalid.");
				length <<= 8;
				length |= (byte)val;
			}

			// Create a new header with the read data
			return new ChunkHeader(id, length);
		}
		#endregion
	}
	#endregion
}
