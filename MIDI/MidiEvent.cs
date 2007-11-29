// Stephen Toub
// stoub@microsoft.com
//
// MidiEvent.cs
// Classes to represent MIDI events (voice, meta, system, etc).

#region Namespaces
using System;
using System.IO;
using System.Text;
using System.Collections;
#endregion

namespace Toub.Sound.Midi
{
	#region Event Collections
	/// <summary>A collection of MIDI events.</summary>
	[Serializable]
	public class MidiEventCollection : ICollection, ICloneable
	{
		#region Member Variables
		/// <summary>The list of MIDI events in the collection.</summary>
		private ArrayList _events;
		#endregion

		#region Construction
		/// <summary>Initializes the collection.</summary>
		public MidiEventCollection()
		{
			// Initialize the list
			_events = new ArrayList();
		}

		/// <summary>Intializes the collection.</summary>
		/// <param name="events">The MIDI events with which to initialize the collection.</param>
		public MidiEventCollection(MidiEvent [] events)
		{
			// Validate the input
			if (events == null) throw new ArgumentNullException("events");

			// Initialize the list with the given array of events
			_events = new ArrayList(events.Length);
			foreach(MidiEvent ev in events) _events.Add(ev);
		}

		/// <summary>Intializes the collection.</summary>
		/// <param name="c">The collection of MIDI events with which to initialize the collection.</param>
		public MidiEventCollection(MidiEventCollection c)
		{
			// Validate the input
			if (c == null) throw new ArgumentNullException("c");

			// Initialize the list with the given collection of events
			_events = new ArrayList(c);
		}
		#endregion

		#region Adding and Removing Events
		/// <summary>Adds a MIDI event message to the collection.</summary>
		/// <param name="message">The event to be added.</param>
		/// <returns>The position at which the event was added.</returns>
		public virtual int Add(MidiEvent message)
		{
			if (message == null) throw new ArgumentNullException("message");
			return _events.Add(message);
		}

		/// <summary>Adds a collection of MIDI event messages to the collection.</summary>
		/// <param name="messages">The events to be added.</param>
		/// <returns>The position at which the first event was added.</returns>
		public virtual int Add(MidiEventCollection messages)
		{
			// Validate the input
			if (messages == null) throw new ArgumentNullException("messages");
			if (messages.Count == 0) return -1;

			// Store the count of the list (the inserted position of the first new element).
			int insertionPos = _events.Count;

			// Add the events
			_events.AddRange(messages);

			// Return the position of the first
			return insertionPos;
		}

		/// <summary>Inserts a MIDI event message into the collection at the specified index.</summary>
		/// <param name="index">The index at which to insert into the collection.</param>
		/// <param name="message">The event to be added.</param>
		/// <returns>The position at which the event was added.</returns>
		public virtual void Insert(int index, MidiEvent message)
		{
			_events.Insert(index, message);
		}

		/// <summary>Determines whether the message is in the collection.</summary>
		/// <param name="message">The message for which to search.</param>
		/// <returns>Whether the specified message is in the collection.</returns>
		public virtual bool Contains(MidiEvent message)
		{
			return _events.Contains(message);
		}

		/// <summary>Removes an event message from the collection.</summary>
		/// <param name="message">The event to be removed.</param>
		public virtual void Remove(MidiEvent message)
		{
			_events.Remove(message);
		}

		/// <summary>Gets or sets the MIDI event at the specified index into the collection.</summary>
		public virtual MidiEvent this[int index]
		{
			get { return (MidiEvent)_events[index]; }
			set { _events[index] = value; }
		}
		#endregion

		#region Sorting (internal only)
		/// <summary>Converts the delta times on all events to from delta times to total times.</summary>
		internal void ConvertDeltasToTotals()
		{
			// Update all delta times to be total times
			long total = this[0].DeltaTime;
			for(int i=1; i<Count; i++)
			{
				total += this[i].DeltaTime;
				this[i].DeltaTime = total;
			}
		}

		/// <summary>Converts the delta times on all events from total times back to delta times.</summary>
		internal void ConvertTotalsToDeltas()
		{
			// Update all total times to be deltas
			long lastValue = 0;
			for(int i=0; i<Count; i++)
			{
				long tempTime = this[i].DeltaTime;
				this[i].DeltaTime -= lastValue;
				lastValue = tempTime;
			}
		}

		/// <summary>Sorts the events based on deltaTime.</summary>
		internal void SortByTime()
		{
			// Sort by dela time
			_events.Sort(new EventComparer());
		}

		/// <summary>Enables comparison of two events based on delta times.</summary>
		protected class EventComparer : IComparer
		{
			#region Implementation of IComparer
			/// <summary>Compares two MidiEvents based on delta times.</summary>
			/// <param name="x">The first MidiEvent to compare.</param>
			/// <param name="y">The second MidiEvent to compare.</param>
			/// <returns>-1 if x.DeltaTime is larger, 0 if they're the same, 1 otherwise.</returns>
			public int Compare(object x, object y)
			{
				// Get the MidiEvents
				MidiEvent eventX = x as MidiEvent;
				MidiEvent eventY = y as MidiEvent;

				// Make sure they're valid
				if (eventX == null) throw new ArgumentNullException("x");
				if (eventY == null) throw new ArgumentNullException("y");

				// Compare the delta times
				return eventX.DeltaTime.CompareTo(eventY.DeltaTime);
			}
			#endregion
		}
		#endregion

		#region Implementation of ICloneable
		/// <summary>Creates a shallow-copy of the collection.</summary>
		/// <returns>A shallow copy of this collection.</returns>
		public virtual MidiEventCollection Clone() { return new MidiEventCollection(this); }

		/// <summary>Creates a shallow-copy of the collection.</summary>
		/// <returns>A shallow copy of this collection.</returns>
		object ICloneable.Clone() { return Clone(); }
		#endregion

		#region Implementation of ICollection
		/// <summary>Copies the collection of MIDI events to a MIDI events array.</summary>
		/// <param name="array">The array to which the events should be copied.</param>
		/// <param name="index">The index at which to start copying in the destination array.</param>
		public virtual void CopyTo(MidiEvent [] array, int index)
		{
			// Copy to the array
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>Copies the collection of MIDI events to a MIDI events array.</summary>
		/// <param name="array">The array to which the events should be copied.</param>
		/// <param name="index">The index at which to start copying in the destination array.</param>
		void ICollection.CopyTo(System.Array array, int index)
		{
			// Copy to the array
			_events.CopyTo(array, index);
		}

		/// <summary>Gets whether the collection is synchronized.</summary>
		public virtual bool IsSynchronized { get { return _events.IsSynchronized; } }

		/// <summary>Gets the number of events in the collection.</summary>
		public virtual int Count { get { return _events.Count; } }

		/// <summary>Gets the synchronization root for this object.</summary>
		public virtual object SyncRoot { get { return _events.SyncRoot; } }
		#endregion

		#region Implementation of IEnumerable
		/// <summary>Gets an enumerator for the collection.</summary>
		/// <returns>An enumerator for the events in the collection.</returns>
		public virtual IEnumerator GetEnumerator() { return _events.GetEnumerator(); }
		#endregion
	}
	#endregion

	#region Voice Events
	/// <summary>Midi event to stop playing a note.</summary>
	[Serializable]
	public sealed class NoteOff : NoteVoiceMidiEvent
	{
		#region Member Variables
		/// <summary>The velocity of the note (0x0 to 0x7F).</summary>
		private byte _velocity;
		/// <summary>The category status byte for NoteOff messages.</summary>
		private const byte _CATEGORY = 0x8;
		#endregion

		#region Construction
		/// <summary>Initialize the NoteOff MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The name of the MIDI note to stop sounding ("C0" to "G10").</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		public NoteOff(long deltaTime, byte channel, string note, byte velocity) : 
			this(deltaTime, channel, GetNoteValue(note), velocity)
		{
		}

		/// <summary>Initialize the NoteOff MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="percussion">The percussion instrument to sound.</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		/// <remarks>Channel 10 (internally 0x9) is assumed.</remarks>
		public NoteOff(long deltaTime, GeneralMidiPercussion percussion, byte velocity) : 
			this(deltaTime, (byte)SpecialChannels.Percussion, GetNoteValue(percussion), velocity)
		{
		}

		/// <summary>Initialize the NoteOff MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The MIDI note to stop sounding (0x0 to 0x7F).</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		public NoteOff(long deltaTime, byte channel, byte note, byte velocity) : 
			base(deltaTime, _CATEGORY, channel, note)
		{
			Velocity = velocity;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_velocity.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the data
			outputStream.WriteByte(_velocity);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the velocity of the note (0x0 to 0x7F).</summary>
		public byte Velocity 
		{ 
			get { return _velocity; } 
			set 
			{ 
				if (value < 0 || value > 127) throw new ArgumentOutOfRangeException("Velocity", value, "The velocity must be in the range from 0 to 127.");
				_velocity = value; 
			} 
		}

		/// <summary>The second parameter as sent in the MIDI message.</summary>
		protected override byte Parameter2 { get { return _velocity; } }
		#endregion
	}

	/// <summary>Midi event to play a note.</summary>
	[Serializable]
	public sealed class NoteOn : NoteVoiceMidiEvent
	{
		#region Member Variables
		/// <summary>The velocity of the note (0x0 to 0x7F).</summary>
		private byte _velocity;
		/// <summary>The category status byte for NoteOn messages.</summary>
		private const byte _CATEGORY = 0x9;
		#endregion

		#region Construction
		/// <summary>Initialize the NoteOn MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The name of the MIDI note to sound ("C0" to "G10").</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		public NoteOn(long deltaTime, byte channel, string note, byte velocity) : 
			this(deltaTime, channel, GetNoteValue(note), velocity)
		{
		}

		/// <summary>Initialize the NoteOn MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="percussion">The percussion instrument to sound.</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		/// <remarks>Channel 10 (internally 0x9) is assumed.</remarks>
		public NoteOn(long deltaTime, GeneralMidiPercussion percussion, byte velocity) : 
			this(deltaTime, (byte)SpecialChannels.Percussion, GetNoteValue(percussion), velocity)
		{
		}

		/// <summary>Initialize the NoteOn MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The MIDI note to sound (0x0 to 0x7F).</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		public NoteOn(long deltaTime, byte channel, byte note, byte velocity) : 
			base(deltaTime, _CATEGORY, channel, note)
		{
			Velocity = velocity;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_velocity.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the data
			outputStream.WriteByte(_velocity);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the velocity of the note (0x0 to 0x7F).</summary>
		public byte Velocity 
		{ 
			get { return _velocity; } 
			set 
			{ 
				if (value < 0 || value > 127) throw new ArgumentOutOfRangeException("Velocity", value, "The velocity must be in the range from 0 to 127.");
				_velocity = value; 
			} 
		}

		/// <summary>The second parameter as sent in the MIDI message.</summary>
		protected override byte Parameter2 { get { return _velocity; } }
		#endregion

		#region Static Methods
		/// <summary>Create a complete note (both on and off messages).</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The name of the MIDI note to sound ("C0" to "G10").</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		/// <param name="duration">The duration of the note.</param>
		public static MidiEventCollection Complete(
			long deltaTime, byte channel, string note, byte velocity, long duration)
		{
			return Complete(deltaTime, channel, GetNoteValue(note), velocity, duration);
		}

		/// <summary>Create a complete note (both on and off messages).</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="percussion">The percussion instrument to sound.</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		/// <param name="duration">The duration of the note.</param>
		/// <remarks>Channel 10 (internally 0x9) is assumed.</remarks>
		public static MidiEventCollection Complete(
			long deltaTime, GeneralMidiPercussion percussion, byte velocity, long duration)
		{
			return Complete(deltaTime, (byte)SpecialChannels.Percussion, GetNoteValue(percussion), velocity, duration);
		}

		/// <summary>Create a complete note (both on and off messages) with a specified duration.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The MIDI note to sound (0x0 to 0x7F).</param>
		/// <param name="velocity">The velocity of the note (0x0 to 0x7F).</param>
		/// <param name="duration">The duration of the note.</param>
		public static MidiEventCollection Complete(
			long deltaTime, byte channel, byte note, byte velocity, long duration)
		{
			MidiEventCollection c = new MidiEventCollection();
			c.Add(new NoteOn(deltaTime, channel, note, velocity));
			c.Add(new NoteOff(duration, channel, note, velocity));
			return c;
		}
		#endregion
	}

	/// <summary>MIDI event to modify a note according to the aftertouch of a key.</summary>
	[Serializable]
	public sealed class Aftertouch : NoteVoiceMidiEvent
	{
		#region Member Variables
		/// <summary>The pressure of the note (0x0 to 0x7F).</summary>
		private byte _pressure;
		/// <summary>The category status byte for Aftertouch messages.</summary>
		private const byte _CATEGORY = 0xA;
		#endregion

		#region Construction
		/// <summary>Initialize the Aftertouch MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The name of the MIDI note to modify ("C0" to "G10").</param>
		/// <param name="pressure">The velocity of the note (0x0 to 0x7F).</param>
		public Aftertouch(long deltaTime, byte channel, string note, byte pressure) : 
			this(deltaTime, channel, GetNoteValue(note), pressure)
		{
		}

		/// <summary>Initialize the Aftertouch MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="percussion">The percussion instrument to modify.</param>
		/// <param name="pressure">The pressure of the note (0x0 to 0x7F).</param>
		/// <remarks>Channel 10 (internally 0x9) is assumed.</remarks>
		public Aftertouch(long deltaTime, GeneralMidiPercussion percussion, byte pressure) : 
			this(deltaTime, (byte)SpecialChannels.Percussion, GetNoteValue(percussion), pressure)
		{
		}

		/// <summary>Initialize the Aftertouch MIDI event message.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The MIDI note to modify (0x0 to 0x7F).</param>
		/// <param name="pressure">The pressure of the note (0x0 to 0x7F).</param>
		public Aftertouch(long deltaTime, byte channel, byte note, byte pressure) : 
			base(deltaTime, _CATEGORY, channel, note)
		{
			Pressure = pressure;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_pressure.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the data
			outputStream.WriteByte(_pressure);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the pressure of the note (0x0 to 0x7F).</summary>
		public byte Pressure 
		{ 
			get { return _pressure; } 
			set 
			{ 
				if (value < 0 || value > 127) throw new ArgumentOutOfRangeException("Pressure", value, "The pressure must be in the range from 0 to 127.");
				_pressure = value; 
			} 
		}

		/// <summary>The second parameter as sent in the MIDI message.</summary>
		protected override byte Parameter2 { get { return _pressure; } }
		#endregion
	}

	/// <summary>
	/// MIDI event to modify the tone with data from a pedal, lever, or other device; 
	/// also used for miscellaneous controls such as volume and bank select.
	/// </summary>
	[Serializable]
	public sealed class Controller : VoiceMidiEvent
	{
		#region Member Variables
		/// <summary>The type of controller message (0x0 to 0x7F).</summary>
		private byte _number;
		/// <summary>The value of the controller message (0x0 to 0x7F).</summary>
		private byte _value;
		/// <summary>The category status byte for Controller messages.</summary>
		private const byte _CATEGORY = 0xB;
		#endregion

		#region Construction
		/// <summary>Initialize the Controller MIDI event message.</summary>
		/// <param name="deltaTime">The delta-time since the previous message.</param>
		/// <param name="channel">The channel to which to write the message (0 through 15).</param>
		/// <param name="number">The type of controller message to be written.</param>
		/// <param name="value">The value of the controller message.</param>
		public Controller(long deltaTime, byte channel, Controllers number, byte value) : 
			this(deltaTime, channel, (byte)number, value)
		{
		}

		/// <summary>Initialize the Controller MIDI event message.</summary>
		/// <param name="deltaTime">The delta-time since the previous message.</param>
		/// <param name="channel">The channel to which to write the message (0 through 15).</param>
		/// <param name="number">The type of controller message to be written.</param>
		/// <param name="value">The value of the controller message.</param>
		public Controller(long deltaTime, byte channel, byte number, byte value) : 
			base(deltaTime, _CATEGORY, channel)
		{
			Number = number;
			Value = value;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			if (Enum.IsDefined(typeof(Controllers), _number)) 
			{
				sb.Append(((Controllers)_number).ToString());
			} 
			else 
			{
				sb.Append("0x");
				sb.Append(_number.ToString("X2"));
			}
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_value.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the data
			outputStream.WriteByte(_number);
			outputStream.WriteByte(_value);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets type of controller message to be written (0x0 to 0x7F).</summary>
		public byte Number 
		{ 
			get { return _number; } 
			set 
			{ 
				if (value < 0 || value > 255) throw new ArgumentOutOfRangeException("Number", value, "The number must be in the range from 0 to 127.");
				_number = value; 
			} 
		}

		/// <summary>Gets or sets the value of the controller message (0x0 to 0x7F).</summary>
		public byte Value 
		{ 
			get { return _value; } 
			set 
			{ 
				if (value < 0 || value > 127) throw new ArgumentOutOfRangeException("Value", value, "The value must be in the range from 0 to 127.");
				_value = value; 
			}
		}

		/// <summary>The first parameter as sent in the MIDI message.</summary>
		protected override byte Parameter1 { get { return _number; } }
		/// <summary>The second parameter as sent in the MIDI message.</summary>
		protected override byte Parameter2 { get { return _value; } }
		#endregion
	}

	/// <summary>MIDI event to select an instrument for the channel by assigning a patch number.</summary>
	[Serializable]
	public sealed class ProgramChange : VoiceMidiEvent
	{
		#region Member Variables
		/// <summary>The number of the program to which to change (0x0 to 0x7F).</summary>
		private byte _number;
		/// <summary>The category status byte for ProgramChange messages.</summary>
		private const byte _CATEGORY = 0xC;
		#endregion

		#region Construction
		/// <summary>Initialize the Controller MIDI event message.</summary>
		/// <param name="deltaTime">The delta-time since the previous message.</param>
		/// <param name="channel">The channel to which to write the message (0 through 15).</param>
		/// <param name="number">The instrument to which to change.</param>
		public ProgramChange(long deltaTime, byte channel, GeneralMidiInstruments number) : 
			this(deltaTime, channel, (byte)number)
		{
		}

		/// <summary>Initialize the Controller MIDI event message.</summary>
		/// <param name="deltaTime">The delta-time since the previous message.</param>
		/// <param name="channel">The channel to which to write the message (0 through 15).</param>
		/// <param name="number">The number of the program to which to change.</param>
		public ProgramChange(long deltaTime, byte channel, byte number) : 
			base(deltaTime, _CATEGORY, channel)
		{
			Number = number;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			if (Enum.IsDefined(typeof(GeneralMidiInstruments), _number)) 
			{
				sb.Append(((GeneralMidiInstruments)_number).ToString());
			} 
			else 
			{
				sb.Append("0x");
				sb.Append(_number.ToString("X2"));
			}
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the data
			outputStream.WriteByte(_number);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the number of the program to which to change (0x0 to 0x7F).</summary>
		public byte Number 
		{ 
			get { return _number; } 
			set 
			{ 
				if (value < 0 || value > 127) throw new ArgumentOutOfRangeException("Number", value, "The number must be in the range from 0 to 127.");
				_number = value; 
			} 
		}

		/// <summary>The first parameter as sent in the MIDI message.</summary>
		protected override byte Parameter1 { get { return _number; } }
		/// <summary>The second parameter as sent in the MIDI message.</summary>
		protected override byte Parameter2 { get { return 0; } }
		#endregion
	}

	/// <summary>MIDI event to apply pressure to a channel's currently playing notes.</summary>
	[Serializable]
	public sealed class ChannelPressure : VoiceMidiEvent
	{
		#region Member Variables
		/// <summary>The amount of pressure to be applied (0x0 to 0x7F).</summary>
		private byte _pressure;
		/// <summary>The category status byte for ChannelPressure messages.</summary>
		private const byte _CATEGORY = 0xD;
		#endregion

		#region Construction
		/// <summary>Initialize the Controller MIDI event message.</summary>
		/// <param name="deltaTime">The delta-time since the previous message.</param>
		/// <param name="channel">The channel to which to write the message (0 through 15).</param>
		/// <param name="pressure">The pressure to be applied.</param>
		public ChannelPressure(long deltaTime, byte channel, byte pressure) : 
			base(deltaTime, _CATEGORY, channel)
		{
			Pressure = pressure;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_pressure.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the data
			outputStream.WriteByte(_pressure);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the amount pressure to be applied (0x0 to 0x7F).</summary>
		public byte Pressure 
		{ 
			get { return _pressure; } 
			set 
			{ 
				if (value < 0 || value > 127) throw new ArgumentOutOfRangeException("Pressure", value, "The pressure must be in the range from 0 to 127.");
				_pressure = value; 
			} 
		}

		/// <summary>The first parameter as sent in the MIDI message.</summary>
		protected override byte Parameter1 { get { return _pressure; } }
		/// <summary>The second parameter as sent in the MIDI message.</summary>
		protected override byte Parameter2 { get { return 0; } }
		#endregion
	}
	
	/// <summary>MIDI event to modify the pitch of all notes played on the channel.</summary>
	[Serializable]
	public sealed class PitchWheel : VoiceMidiEvent
	{
		#region Member Variables
		/// <summary>The upper 7-bits of the wheel position..</summary>
		private byte _upperBits;
		/// <summary>The lower 7-bits of the wheel position..</summary>
		private byte _lowerBits;
		/// <summary>The category status byte for PitchWheel messages.</summary>
		private const byte _CATEGORY = 0xE;
		#endregion

		#region Construction
		/// <summary>Initialize the Controller MIDI event message.</summary>
		/// <param name="deltaTime">The delta-time since the previous message.</param>
		/// <param name="channel">The channel to which to write the message (0 through 15).</param>
		/// <param name="steps">The amount of pitch change to apply.</param>
		public PitchWheel(long deltaTime, byte channel, PitchWheelSteps steps) : 
			this(deltaTime, channel, (int)steps)
		{
		}

		/// <summary>Initialize the Controller MIDI event message.</summary>
		/// <param name="deltaTime">The delta-time since the previous message.</param>
		/// <param name="channel">The channel to which to write the message (0 through 15).</param>
		/// <param name="upperBits">The upper 7 bits of the position.</param>
		/// <param name="lowerBits">The lower 7 bits of the position.</param>
		public PitchWheel(long deltaTime, byte channel, byte upperBits, byte lowerBits) : 
			base(deltaTime, _CATEGORY, channel)
		{
			UpperBits = upperBits;
			LowerBits = lowerBits;
		}

		/// <summary>Initialize the Controller MIDI event message.</summary>
		/// <param name="deltaTime">The delta-time since the previous message.</param>
		/// <param name="channel">The channel to which to write the message (0 through 15).</param>
		/// <param name="position">The position of the wheel.</param>
		public PitchWheel(long deltaTime, byte channel, int position) : 
			base(deltaTime, _CATEGORY, channel)
		{
			// Store the data (validation is implicit)
			Position = position;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append(Position.ToString());
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the data
			int position = Position;
			outputStream.WriteByte(Parameter1);
			outputStream.WriteByte(Parameter2);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the upper 7 bits of the position.</summary>
		public byte UpperBits 
		{ 
			get { return _upperBits; } 
			set 
			{ 
				if (_upperBits < 0 || _upperBits > 0x7F) throw new ArgumentOutOfRangeException("UpperBits", value, "Value must be in the range from 0x0 to 0x7F.");
				_upperBits = value; 
			} 
		}

		/// <summary>Gets or sets the lower 7 bits of the position.</summary>
		public byte LowerBits 
		{ 
			get { return _lowerBits; } 
			set 
			{ 
				if (_lowerBits < 0 || _lowerBits > 0x7F) throw new ArgumentOutOfRangeException("LowerBits", value, "Value must be in the range from 0x0 to 0x7F.");
				_lowerBits = value; 
			} 
		}

		/// <summary>Gets or sets the wheel position.</summary>
		public int Position
		{ 
			get { return CombineBytesTo14Bits(_upperBits, _lowerBits); } 
			set 
			{ 
				if (value < 0 || value > 0x3FFF) throw new ArgumentOutOfRangeException("Position", value, "Pitch wheel position must be in the range from 0x0 to 0x3FFF.");
				Split14BitsToBytes(value, out _upperBits, out _lowerBits); 
			}
		}

		/// <summary>The first parameter as sent in the MIDI message.</summary>
		protected override byte Parameter1 { get { return (byte)((Position & 0xFF00) >> 8); } }
		/// <summary>The second parameter as sent in the MIDI message.</summary>
		protected override byte Parameter2 { get { return (byte)(Position & 0xFF); } }
		#endregion
	}
	#endregion

	#region Meta Events
	/// <summary>A sequence number meta event message.</summary>
	[Serializable]
	public sealed class SequenceNumber : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>The sequence number for the event.</summary>
		private int _number;
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x0;
		#endregion

		#region Construction
		/// <summary>Intializes the sequence number meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="number">The sequence number for the event.</param>
		public SequenceNumber(long deltaTime, int number) : base(deltaTime, _METAID)
		{
			Number = number;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append(_number.ToString());
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Special meta event marker and the id of the event
			outputStream.WriteByte(0x02);
			outputStream.WriteByte((byte)((_number & 0xFF00) >> 8));
			outputStream.WriteByte((byte)(_number & 0xFF));
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the sequence number for the event.</summary>
		public int Number 
		{ 
			get { return _number; } 
			set 
			{ 
				if (value < 0 || value > 0xFFFF) throw new ArgumentOutOfRangeException("Number", value, "The number must be in the range from 0x0 to 0xFFFF.");
				_number = value; 
			} 
		}
		#endregion
	}

	/// <summary>A text meta event.</summary>
	[Serializable]
	public sealed class Text : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x1;
		#endregion

		#region Construction
		/// <summary>Initialize the text meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public Text(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A copyright meta event.</summary>
	[Serializable]
	public sealed class Copyright : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x2;
		#endregion

		#region Construction
		/// <summary>Initialize the copyright meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public Copyright(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A sequence/track name meta event.</summary>
	[Serializable]
	public sealed class SequenceTrackName : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x3;
		#endregion

		#region Construction
		/// <summary>Initialize the sequence/track name meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public SequenceTrackName(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A instrument name meta event.</summary>
	[Serializable]
	public sealed class Instrument : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x4;
		#endregion

		#region Construction
		/// <summary>Initialize the instrument meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public Instrument(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A lyric name meta event.</summary>
	[Serializable]
	public sealed class Lyric : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x5;
		#endregion

		#region Construction
		/// <summary>Initialize the lyric meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public Lyric(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A marker name meta event.</summary>
	[Serializable]
	public sealed class Marker : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x6;
		#endregion

		#region Construction
		/// <summary>Initialize the marker meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public Marker(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A cue point name meta event.</summary>
	[Serializable]
	public sealed class CuePoint : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x7;
		#endregion

		#region Construction
		/// <summary>Initialize the cue point meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public CuePoint(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A program name meta event.</summary>
	[Serializable]
	public sealed class ProgramName : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x8;
		#endregion

		#region Construction
		/// <summary>Initialize the program name meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public ProgramName(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A device name meta event.</summary>
	[Serializable]
	public sealed class DeviceName : TextMetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x9;
		#endregion

		#region Construction
		/// <summary>Initialize the device name meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="text">The text associated with the event.</param>
		public DeviceName(long deltaTime, string text) : base(deltaTime, _METAID, text) {}
		#endregion
	}

	/// <summary>A channel prefix meta event message.</summary>
	[Serializable]
	public sealed class ChannelPrefix : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>The prefix for the event.</summary>
		private byte _prefix;
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x20;
		#endregion

		#region Construction
		/// <summary>Intializes the channel prefix event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="prefix">The prefix for the event.</param>
		public ChannelPrefix(long deltaTime, byte prefix) : base(deltaTime, _METAID)
		{
			Prefix = prefix;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_prefix.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			outputStream.WriteByte(0x01);
			outputStream.WriteByte((byte)_prefix);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the prefix for the event.</summary>
		public byte Prefix 
		{ 
			get { return _prefix; } 
			set 
			{ 
				if (value < 0 || value > 0x7F) throw new ArgumentOutOfRangeException("Prefix", value, "The prefix must be in the range from 0x0 to 0x7F.");
				_prefix = value; 
			} 
		}
		#endregion
	}

	/// <summary>A MIDI port meta event message.</summary>
	[Serializable]
	public sealed class MidiPort : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>The port for the event.</summary>
		private byte _port;
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x21;
		#endregion

		#region Construction
		/// <summary>Intializes the MIDI port event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="port">The port for the event.</param>
		public MidiPort(long deltaTime, byte port) : base(deltaTime, _METAID)
		{
			Port = port;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_port.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			outputStream.WriteByte(0x01);
			outputStream.WriteByte((byte)_port);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the port for the event.</summary>
		public byte Port 
		{ 
			get { return _port; } 
			set 
			{ 
				if (value < 0 || value > 0x7F) throw new ArgumentOutOfRangeException("Port", value, "The port must be in the range from 0x0 to 0x7F.");
				_port = value; 
			} 
		}
		#endregion
	}

	/// <summary>An end of track meta event message.</summary>
	[Serializable]
	public sealed class EndOfTrack : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x2F;
		#endregion

		#region Construction
		/// <summary>Intializes the end of track meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		public EndOfTrack(long deltaTime) : base(deltaTime, _METAID) {}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// End of track
			outputStream.WriteByte(0x00);
		}
		#endregion
	}

	/// <summary>A tempo meta event message.</summary>
	[Serializable]
	public sealed class Tempo : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>The tempo for the event.</summary>
		private int _tempo;
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x51;
		#endregion

		#region Construction
		/// <summary>Intializes the tempo meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="value">The tempo for the event.</param>
		public Tempo(long deltaTime, int value) : base(deltaTime, _METAID)
		{
			Value = value;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append(_tempo.ToString());
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			outputStream.WriteByte(0x03);
			outputStream.WriteByte((byte)((_tempo & 0xFF0000) >> 16));
			outputStream.WriteByte((byte)((_tempo & 0x00FF00) >> 8));
			outputStream.WriteByte((byte)((_tempo & 0x0000FF)));
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the tempo for the event.</summary>
		public int Value 
		{ 
			get { return _tempo; }
			set 
			{ 
				if (value < 0 || value > 0xFFFFFF) throw new ArgumentOutOfRangeException("Tempo", value, "The tempo must be in the range from 0x0 to 0xFFFFFF.");
				_tempo = value; 
			} 
		}
		#endregion
	}

	/// <summary>An SMPTE offset meta event message.</summary>
	[Serializable]
	public sealed class SMPTEOffset : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>Hours for the event.</summary>
		private byte _hours;
		/// <summary>Minutes for the event.</summary>
		private byte _minutes;
		/// <summary>Seconds for the event.</summary>
		private byte _seconds;
		/// <summary>Frames for the event.</summary>
		private byte _frames;
		/// <summary>Fractional frames for the event.</summary>
		private byte _fractionalFrames;
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x54;
		#endregion

		#region Construction
		/// <summary>Intializes the SMTPE offset meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="hours">Hours for the event.</param>
		/// <param name="minutes">Minutes for the event.</param>
		/// <param name="seconds">Seconds for the event.</param>
		/// <param name="frames">Frames for the event.</param>
		/// <param name="fractionalFrames">Fractional frames for the event.</param>
		public SMPTEOffset(long deltaTime, 
			byte hours, byte minutes, byte seconds, byte frames, byte fractionalFrames) : 
			base(deltaTime, _METAID)
		{
			Hours = hours;
			Minutes = minutes;
			Seconds = seconds;
			Frames = frames;
			FractionalFrames = fractionalFrames;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_hours.ToString("X2"));
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_minutes.ToString("X2"));
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_seconds.ToString("X2"));
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_frames.ToString("X2"));
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_fractionalFrames.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			outputStream.WriteByte(0x05);
			outputStream.WriteByte(_hours);
			outputStream.WriteByte(_minutes);
			outputStream.WriteByte(_seconds);
			outputStream.WriteByte(_frames);
			outputStream.WriteByte(_fractionalFrames);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the hours for the event.</summary>
		public byte Hours 
		{ 
			get { return _hours; }
			set { _hours = value; }
		}

		/// <summary>Gets or sets the minutes for the event.</summary>
		public byte Minutes 
		{ 
			get { return _minutes; }
			set { _minutes = value; }
		}

		/// <summary>Gets or sets the seconds for the event.</summary>
		public byte Seconds 
		{ 
			get { return _seconds; }
			set { _seconds = value; }
		}

		/// <summary>Gets or sets the frames for the event.</summary>
		public byte Frames 
		{ 
			get { return _frames; }
			set { _frames = value; }
		}

		/// <summary>Gets or sets the fractional frames for the event.</summary>
		public byte FractionalFrames
		{ 
			get { return _fractionalFrames; }
			set { _fractionalFrames = value; }
		}
		#endregion
	}

	/// <summary>A time signature meta event message.</summary>
	[Serializable]
	public sealed class TimeSignature : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>Numerator of the time signature.</summary>
		private byte _numerator;
		/// <summary>Negative power of two, denominator of time signature.</summary>
		private byte _denominator;
		/// <summary>The number of MIDI clocks per metronome click.</summary>
		private byte _midiClocksPerClick;
		/// <summary>The number of notated 32nd notes per MIDI quarter note.</summary>
		private byte _numberOfNotated32nds;
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x58;
		#endregion

		#region Construction
		/// <summary>Intializes the time signature meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="numerator">Numerator of the time signature.</param>
		/// <param name="denominator">Negative power of two, denominator of time signature.</param>
		/// <param name="midiClocksPerClick">The number of MIDI clocks per metronome click.</param>
		/// <param name="numberOfNotated32nds">The number of notated 32nd notes per MIDI quarter note.</param>
		public TimeSignature(long deltaTime, 
			byte numerator, byte denominator, byte midiClocksPerClick, byte numberOfNotated32nds) : 
			base(deltaTime, _METAID)
		{
			Numerator = numerator;
			Denominator = denominator;
			MidiClocksPerClick = midiClocksPerClick;
			NumberOfNotated32nds = numberOfNotated32nds;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_numerator.ToString("X2"));
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_denominator.ToString("X2"));
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_midiClocksPerClick.ToString("X2"));
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(_numberOfNotated32nds.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			outputStream.WriteByte(0x04);
			outputStream.WriteByte(_numerator);
			outputStream.WriteByte(_denominator);
			outputStream.WriteByte(_midiClocksPerClick);
			outputStream.WriteByte(_numberOfNotated32nds);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the numerator for the event.</summary>
		public byte Numerator 
		{ 
			get { return _numerator; } 
			set { _numerator = value; } 
		}

		/// <summary>Gets or sets the denominator for the event.</summary>
		public byte Denominator 
		{ 
			get { return _denominator; } 
			set { _denominator = value; } 
		}

		/// <summary>Gets or sets the MIDI clocks per click for the event.</summary>
		public byte MidiClocksPerClick 
		{ 
			get { return _midiClocksPerClick; } 
			set { _midiClocksPerClick = value; } 
		}

		/// <summary>Gets or sets the number of notated 32 notes per MIDI quarter note for the event.</summary>
		public byte NumberOfNotated32nds 
		{ 
			get { return _numberOfNotated32nds; } 
			set { _numberOfNotated32nds = value; } 
		}
		#endregion
	}

	/// <summary>A key signature meta event message.</summary>
	[Serializable]
	public sealed class KeySignature : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>Number of sharps or flats in the signature.</summary>
		private Key _key;
		/// <summary>Tonality of the signature.</summary>
		private Tonality _tonality;
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x59;
		#endregion

		#region Construction
		/// <summary>Intializes the meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="key">Key of the signature.</param>
		/// <param name="tonality">Tonality of the signature.</param>
		public KeySignature(long deltaTime, Key key, Tonality tonality) : 
			base(deltaTime, _METAID)
		{
			Key = key;
			Tonality = tonality;
		}

		/// <summary>Intializes the key signature meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="key">Key of the signature.</param>
		/// <param name="tonality">Tonality of the signature.</param>
		public KeySignature(long deltaTime, byte key, byte tonality) : 
			this(deltaTime, (Key)key, (Tonality)tonality)
		{
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append(_key.ToString());
			sb.Append("\t");
			sb.Append(_tonality.ToString());
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			outputStream.WriteByte(0x02);
			outputStream.WriteByte((byte)_key);
			outputStream.WriteByte((byte)_tonality);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the numerator for the event.</summary>
		public Key Key 
		{ 
			get { return _key; }
			set 
			{
				Key k = (Key)(sbyte)value;
				if (!Enum.IsDefined(typeof(Key), k)) throw new ArgumentOutOfRangeException("Key", value, "Not a valid key.");
				_key = k;
			} 
		}

		/// <summary>Gets or sets the denominator for the event.</summary>
		public Tonality Tonality
		{ 
			get { return _tonality; } 
			set 
			{ 
				if (!Enum.IsDefined(typeof(Tonality), value)) throw new ArgumentOutOfRangeException("Tonality", value, "Not a valid tonality.");
				_tonality = value;
			} 
		}
		#endregion
	}

	
	/// <summary>A proprietary meta event message.</summary>
	[Serializable]
	public sealed class Proprietary : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>The data of the event.</summary>
		private byte [] _data;
		/// <summary>The meta id for this event.</summary>
		private const byte _METAID = 0x7F;
		#endregion

		#region Construction
		/// <summary>Intializes the proprietary meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="data">The data associated with the event.</param>
		public Proprietary(long deltaTime, byte [] data) : 
			base(deltaTime, _METAID)
		{
			Data = data;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			if (_data != null) sb.Append("\t");
			sb.Append(DataToString(_data));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			WriteVariableLength(outputStream, _data != null ? _data.Length : 0);
			if (_data != null) outputStream.Write(_data, 0, _data.Length);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the data for the event.</summary>
		public byte [] Data { get { return _data; } set { _data = value; } }
		#endregion
	}

	/// <summary>An unknown meta event message.</summary>
	[Serializable]
	public sealed class UnknownMetaMidiEvent : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>The data of the event.</summary>
		private byte [] _data;
		#endregion

		#region Construction
		/// <summary>Intializes the meta event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="metaEventID">The event ID for this meta event.</param>
		/// <param name="data">The data associated with the event.</param>
		public UnknownMetaMidiEvent(long deltaTime, byte metaEventID, byte [] data) : 
			base(deltaTime, metaEventID)
		{
			Data = data;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			if (_data != null) sb.Append("\t");
			sb.Append(DataToString(_data));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			WriteVariableLength(outputStream, _data != null ? _data.Length : 0);
			if (_data != null) outputStream.Write(_data, 0, _data.Length);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the data for the event.</summary>
		public byte [] Data { get { return _data; } set { _data = value; } }
		#endregion
	}
	#endregion

	#region System Exclusive
	/// <summary>A system exclusive MIDI event.</summary>
	[Serializable]
	public class SystemExclusiveMidiEvent : MidiEvent
	{
		#region Member Variables
		/// <summary>The data for the event.</summary>
		private byte [] _data;
		#endregion

		#region Construction
		/// <summary>Intializes the meta MIDI event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="data">The data for the event.</param>
		public SystemExclusiveMidiEvent(long deltaTime, byte [] data) : base(deltaTime)
		{
			_data = data;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			if (_data != null) sb.Append("\t");
			sb.Append(DataToString(_data));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Event data
			outputStream.WriteByte(0xF0);
			WriteVariableLength(outputStream, 1 + (_data != null ? _data.Length : 0)); // "1+" for the F7 at the end
			if (_data != null) outputStream.Write(_data, 0, _data.Length);
			outputStream.WriteByte(0xF7);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the data for this event.</summary>
		public virtual byte [] Data { get { return _data; } set { _data = value; } }
		#endregion
	}
	#endregion

	#region Base Midi Events
	/// <summary>Represents a text meta event message.</summary>
	[Serializable]
	public abstract class TextMetaMidiEvent : MetaMidiEvent
	{
		#region Member Variables
		/// <summary>The text associated with the event.</summary>
		private string _text;
		#endregion

		#region Construction
		/// <summary>Intializes the meta MIDI event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="metaEventID">The ID of the meta event.</param>
		/// <param name="text">The text associated with the event.</param>
		protected TextMetaMidiEvent(long deltaTime, byte metaEventID, string text) : 
			base(deltaTime, metaEventID)
		{
			_text = text;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append(Text.ToString());
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Special meta event marker and the id of the event
			byte [] asciiBytes = Encoding.ASCII.GetBytes(_text);
			WriteVariableLength(outputStream, asciiBytes.Length);
			outputStream.Write(asciiBytes, 0, asciiBytes.Length);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the text associated with this event.</summary>
		public string Text 
		{ 
			get { return _text; } 
			set 
			{ 
				if (value == null) throw new ArgumentNullException("Text"); // Empty is ok, but null is not
				_text = value;
			}
		}
		#endregion
	}

	/// <summary>Represents a meta event message.</summary>
	[Serializable]
	public abstract class MetaMidiEvent : MidiEvent
	{
		#region Member Variables
		/// <summary>The ID of the meta event.</summary>
		private byte _metaEventID;
		#endregion

		#region Construction
		/// <summary>Intializes the meta MIDI event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="metaEventID">The ID of the meta event.</param>
		protected MetaMidiEvent(long deltaTime, byte metaEventID) : base(deltaTime)
		{
			_metaEventID = metaEventID;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(MetaEventID.ToString("X2"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Special meta event marker and the id of the event
			outputStream.WriteByte(0xFF);
			outputStream.WriteByte(_metaEventID);
		}
		#endregion

		#region Properties
		/// <summary>Gets the ID of this meta event.</summary>
		public byte MetaEventID { get { return _metaEventID; } }
		#endregion
	}

	/// <summary>Represents a voice category message that deals with notes.</summary>
	[Serializable]
	public abstract class NoteVoiceMidiEvent : VoiceMidiEvent
	{
		#region Member Variables
		/// <summary>The MIDI note to modify (0x0 to 0x7F).</summary>
		private byte _note;
		#endregion

		#region Construction
		/// <summary>Intializes the note voice MIDI event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="category">The category identifier (0x0 through 0xF) for this voice event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		/// <param name="note">The MIDI note to modify (0x0 to 0x7F).</param>
		protected NoteVoiceMidiEvent(long deltaTime, byte category, byte channel, byte note) : 
			base(deltaTime, category, channel)
		{
			Note = note;
		}
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			if (Channel == (byte)SpecialChannels.Percussion &&
				Enum.IsDefined(typeof(GeneralMidiPercussion), _note)) 
			{
				sb.Append(((GeneralMidiPercussion)_note).ToString()); // print out percussion name
			} 
			else 
			{
				sb.Append(GetNoteName(_note)); // print out note name
			}
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the data
			outputStream.WriteByte(_note);
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the MIDI note (0x0 to 0x7F).</summary>
		public byte Note 
		{ 
			get { return _note; } 
			set 
			{ 
				if (value < 0 || value > 127) throw new ArgumentOutOfRangeException("Note", value, "The note must be in the range from 0 to 127.");
				_note = value; 
			}
		}

		/// <summary>The first parameter as sent in the MIDI message.</summary>
		protected sealed override byte Parameter1 { get { return _note; } }
		#endregion
	}

	/// <summary>Represents a voice category message.</summary>
	[Serializable]
	public abstract class VoiceMidiEvent : MidiEvent
	{
		#region Member Variables
		/// <summary>The status identifier (0x0 through 0xF) for this voice event.</summary>
		private byte _category;
		/// <summary>The channel (0x0 through 0xF) for this voice event.</summary>
		private byte _channel;
		#endregion

		#region Construction
		/// <summary>Intializes the voice MIDI event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		/// <param name="category">The category identifier (0x0 through 0xF) for this voice event.</param>
		/// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
		protected VoiceMidiEvent(long deltaTime, byte category, byte channel) : base(deltaTime)
		{
			// Validate the parameters
			if (category < 0x0 || category > 0xF) throw new ArgumentOutOfRangeException("category", category, "Category values must be in the range from 0x0 to 0xF.");
			
			// Store the data
			_category = category;
			Channel = channel;
		}
		#endregion
		
		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append("\t");
			sb.Append("0x");
			sb.Append(Channel.ToString("X1"));
			return sb.ToString();
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public override void Write(Stream outputStream)
		{
			// Write out the base event information
			base.Write(outputStream);

			// Write out the status byte
			outputStream.WriteByte(GetStatusByte());
		}

		/// <summary>Gets the status byte for the message event.</summary>
		/// <returns>The status byte (combination of category and channel) for the message event.</returns>
		private byte GetStatusByte()
		{
			// The command is the upper 4 bits and the channel is the lower 4.
			return (byte)((((int)_category) << 4) | _channel);
		}
		#endregion

		#region Properties
		/// <summary>Gets the status identifier (0x0 through 0xF) for this voice event.</summary>
		public byte Category { get { return _category; } }
		/// <summary>Gets or sets the channel (0x0 through 0xF) for this voice event.</summary>
		public virtual byte Channel 
		{ 
			get { return _channel; } 
			set 
			{ 
				if (value < 0 || value > 0xF) throw new ArgumentOutOfRangeException("Channel", value, "The channel must be in the range from 0x0 to 0xF.");
				_channel = value; 
			} 
		}
		/// <summary>Gets the status byte for the event message (combination of category and channel).</summary>
		public byte Status { get { return GetStatusByte(); } }
		/// <summary>Gets the Dword that represents this event as a MIDI event message.</summary>
		internal int Message { get { return Status | (Parameter1 << 8) | (Parameter2 << 16); } }
		/// <summary>The first parameter as sent in the MIDI message.</summary>
		protected abstract byte Parameter1 { get; }
		/// <summary>The second parameter as sent in the MIDI message.</summary>
		protected abstract byte Parameter2 { get; }
		#endregion
	}

	/// <summary>A MIDI event, serving as the base class for all types of MIDI events.</summary>
	[Serializable]
	public abstract class MidiEvent : ICloneable
	{
		#region Member Variables
		/// <summary>The amount of time before this event.</summary>
		private long _deltaTime;
		#endregion

		#region Construction
		/// <summary>Initialize the event.</summary>
		/// <param name="deltaTime">The amount of time before this event.</param>
		protected MidiEvent(long deltaTime)
		{
			// Store the data
			DeltaTime = deltaTime;
		}
		#endregion

		#region Methods
		/// <summary>Write the event to the output stream.</summary>
		/// <param name="outputStream">The stream to which the event should be written.</param>
		public virtual void Write(Stream outputStream)
		{
			// Write out the delta time
			WriteVariableLength(outputStream, _deltaTime);
		}

		/// <summary>Combines two 7-bit values into a single 14-bit value.</summary>
		/// <param name="upper">The upper 7-bits.</param>
		/// <param name="lower">The lower 7-bits.</param>
		/// <returns>A 14-bit value stored in an integer.</returns>
		internal static int CombineBytesTo14Bits(byte upper, byte lower)
		{
			// Turn the two bytes into a 14 bit value
			int fourteenBits = upper;
			fourteenBits <<= 7;
			fourteenBits |= lower;
			return fourteenBits;
		}

		/// <summary>Splits a 14-bit value into two bytes each with 7 of the bits.</summary>
		/// <param name="bits">The value to be split.</param>
		/// <param name="upperBits">The upper 7 bits.</param>
		/// <param name="lowerBits">The lower 7 bits.</param>
		internal static void Split14BitsToBytes(int bits, out byte upperBits, out byte lowerBits)
		{
			lowerBits = (byte)(bits & 0x7F);
			bits >>= 7;
			upperBits = (byte)(bits & 0x7F);
		}

		/// <summary>Writes bytes for a long value in the special 7-bit form.</summary>
		/// <param name="outputStream">The stream to which the length should be written.</param>
		/// <param name="value">The value to be converted and written.</param>
		protected void WriteVariableLength(Stream outputStream, long value)
		{
			long buffer;

			// TODO: Clean this up!

			// Parse the value into bytes containing each set of 7-bits and a 1-bit marker
			// for whether there are more bytes in the length
			buffer = value & 0x7f;
			while ((value >>= 7) > 0) 
			{
				buffer <<= 8;
				buffer |= 0x80;
				buffer += (value & 0x7f);
			}

			// Get all of the bytes in correct order
			while(true)
			{
				outputStream.WriteByte((byte)(buffer & 0xFF));
				if ((buffer & 0x80) == 0) break; // if the marker bit is not set, we're done
				buffer >>= 8;
			}
		}

		/// <summary>Converts an array of bytes into human-readable form.</summary>
		/// <param name="data">The array to convert.</param>
		/// <returns>The string containing the bytes.</returns>
		protected static string DataToString(byte [] data)
		{
			if (data != null)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("[");
				for(int i=0; i<data.Length; i++)
				{
					// If we're not the first byte, output a comma as a separator
					if (i > 0) sb.Append(",");

					// Spit out the byte itself
					sb.Append("0x");
					sb.Append(data[i].ToString("X2"));
				}
				sb.Append("]");
				return sb.ToString();
			}
			else return "";
		}

		#region Note Conversion
		/// <summary>Gets the name of a note given its numeric value.</summary>
		/// <param name="note">The numeric value of the note.</param>
		/// <returns>The name of the note.</returns>
		public static string GetNoteName(byte note)
		{
			// Get the octave and the pitch within the octave
			int octave = note / 12;
			int pitch = note % 12;

			// Translate the pitch into a note name
			string name;
			switch(pitch)
			{
				case 0: name = "C"; break;
				case 1: name = "C#"; break;
				case 2: name = "D"; break;
				case 3: name = "D#"; break;
				case 4: name = "E"; break;
				case 5: name = "F"; break;
				case 6: name = "F#"; break;
				case 7: name = "G"; break;
				case 8: name = "G#"; break;
				case 9: name = "A"; break;
				case 10: name = "A#"; break;
				case 11: name = "B"; break;
				default: name = ""; break;
			}

			// Append the octave onto the name
			return name + octave;
		}

		/// <summary>Gets the note value for a string representing the name of a note.</summary>
		/// <param name="noteName">
		/// The name of the note, such as "C#4" or "Eb0".
		/// Valid names are in the range from "C0" (0) to "G10" (127).
		/// </param>
		/// <returns>The numeric value of the specified note.</returns>
		public static byte GetNoteValue(string noteName)
		{
			// Validate input
			if (noteName == null) throw new ArgumentNullException("noteName");
			if (noteName.Length < 2) throw new ArgumentOutOfRangeException("noteName", noteName, "Note names must be at least 2 characters in length.");

			int noteValue, curPos = 0;

			// Get's a value for the note name
			switch(Char.ToLower(noteName[curPos]))
			{
				case 'c': noteValue = 0; break;
				case 'd': noteValue = 2; break;
				case 'e': noteValue = 4; break;
				case 'f': noteValue = 5; break;
				case 'g': noteValue = 7; break;
				case 'a': noteValue = 9; break;
				case 'b': noteValue = 11; break;
				default: throw new ArgumentOutOfRangeException("noteName", noteName, "The note must be c, d, e, f, g, a, or b.");
			}
			curPos++;

			// Get a value for the # or b if one exists.  If we want to allow multiple
			// flats or sharps, just wrap this section of code in a loop.
			char nextChar = Char.ToLower(noteName[curPos]);
			if (nextChar == 'b') 
			{
				noteValue--; curPos++;
			}
			else if (nextChar == '#')
			{
				noteValue++; curPos++;
			}

			// Make sure we're still have more to read
			if (curPos >= noteName.Length) throw new ArgumentOutOfRangeException("noteName", noteName, "Octave must be specified.");

			// Get the value for the octave (0 through 10)
			int octave = noteName[curPos] - '0';
			if (octave == 1 && curPos < noteName.Length-1) // if the first digit is a 1 and if there are more digits
			{
				curPos++;
				octave = 10 + (noteName[curPos] - '0'); // the second digit better be 0, but just in case
			}
			if (octave < 0 || octave > 10) throw new ArgumentOutOfRangeException("noteName", noteName, "The octave must be in the range 0 to 10.");
		
			// Combine octave and note value into final noteValue
			noteValue = (octave * 12) + noteValue;
			if (noteValue < 0 || noteValue > 127) throw new ArgumentOutOfRangeException("noteName", noteName, "Notes must be in the range from C0 to G10.");
			return (byte)noteValue;
		}
		
		/// <summary>Gets the note value for the specific percussion.</summary>
		/// <param name="percussion">The percussion for which we need the note value.</param>
		/// <returns>The numeric value of the specified percussion.</returns>
		public static byte GetNoteValue(GeneralMidiPercussion percussion)
		{
			// The GeneralMidiPercussion enumeration already has the correct note values
			// built into it, so just cast the value to a byte and return it.
			return (byte)percussion;
		}
		#endregion
		#endregion

		#region To String
		/// <summary>Generate a string representation of the event.</summary>
		/// <returns>A string representation of the event.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.GetType().Name);
			sb.Append("\t");
			sb.Append(DeltaTime.ToString());
			return sb.ToString();
		}
		#endregion

		#region Properties
		/// <summary>Gets or sets the amount of time before this event.</summary>
		public virtual long DeltaTime 
		{ 
			get { return _deltaTime; } 
			set 
			{ 
				if (value < 0) throw new ArgumentOutOfRangeException("DeltaTime", value, "Delta times must be non-negative.");
				_deltaTime = value; 
			} 
		}
		#endregion

		#region Implementation of ICloneable
		/// <summary>Creates a shallow copy of the MIDI event.</summary>
		/// <returns>A shallow-clone of the MIDI event.</returns>
		public MidiEvent Clone() { return (MidiEvent)MemberwiseClone(); }

		/// <summary>Creates a shallow-copy of the object.</summary>
		/// <returns>A shallow-clone of the MIDI event.</returns>
		object ICloneable.Clone() { return Clone(); }
		#endregion
	}
	#endregion
}
