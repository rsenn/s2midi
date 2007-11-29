// Stephen Toub
// stoub@microsoft.com
//
// MidiDefinitions.cs
// Constants and enumerations for dealing with MIDI.

#region Namespaces
using System;
#endregion

namespace Toub.Sound.Midi
{
	/// <summary>Defines channels reserved for special purposes.</summary>
	[Serializable]
	public enum SpecialChannels : byte
	{
		#region Defined Channels
		/// <summary>General MIDI percussion channel</summary>
		Percussion = 9 // Channel 10 (1-based) is reserved for the percussion map
		#endregion
	}

	/// <summary>General MIDI Instrument Patch Map.</summary>
	[Serializable]
	public enum GeneralMidiInstruments : byte
	{
		#region Pianos
		/// <summary>Acoustic Grand</summary>
		AcousticGrand = 0,
		/// <summary>Bright Acoustic</summary>
		BrightAcoustic = 1,
		/// <summary>Electric Grand</summary>
		ElectricGrand = 2,
		/// <summary>Honky Tonk</summary>
		HonkyTonk = 3,
		/// <summary>Electric Piano 1</summary>
		ElectricPiano1 = 4,
		/// <summary>Electric Piano 2</summary>
		ElectricPiano2 = 5,
		/// <summary>Harpsichord</summary>
		Harpsichord = 6,
		/// <summary>Clav</summary>
		Clav = 7,
		#endregion

		#region Chrome Percussion
		/// <summary>Celesta</summary>
		Celesta = 8,
		/// <summary>Glockenspiel</summary>
		Glockenspiel = 9,
		/// <summary>Music Box</summary>
		MusicBox = 10,
		/// <summary>Vibraphone</summary>
		Vibraphone = 11,
		/// <summary>Marimba</summary>
		Marimba = 12,
		/// <summary>Xylophone</summary>
		Xylophone = 13,
		/// <summary>Tubular Bells</summary>
		TubularBells = 14,
		/// <summary>Dulcimer</summary>
		Dulcimer = 15,
		#endregion

		#region Organ
		/// <summary>Drawbar Organ</summary>
		DrawbarOrgan = 16,
		/// <summary>Percussive Organ</summary>
		PercussiveOrgan = 17,
		/// <summary>Rock Organ</summary>
		RockOrgan = 18,
		/// <summary>Church Organ</summary>
		ChurchOrgan = 19,
		/// <summary>Reed Organ</summary>
		ReedOrgan = 20,
		/// <summary>Accoridan</summary>
		Accoridan = 21,
		/// <summary>Harmonica</summary>
		Harmonica = 22,
		/// <summary>Tango Accordian</summary>
		TangoAccordian = 23,
		#endregion

		#region Guitar
		/// <summary>Nylon Acoustic Guitar</summary>
		NylonAcousticGuitar = 24,
		/// <summary>Steel Acoustic Guitar</summary>
		SteelAcousticGuitar = 25,
		/// <summary>Jazz Electric Guitar</summary>
		JazzElectricGuitar = 26,
		/// <summary>Clean Electric Guitar</summary>
		CleanElectricGuitar = 27,
		/// <summary>Muted Electric Guitar</summary>
		MutedElectricGuitar = 28,
		/// <summary>Overdriven Guitar</summary>
		OverdrivenGuitar = 29,
		/// <summary>Distortion Guitar</summary>
		DistortionGuitar = 30,
		/// <summary>Guitar Harmonics</summary>
		GuitarHarmonics = 31,
		#endregion

		#region Bass
		/// <summary>Acoustic Bass</summary>
		AcousticBass = 32,
		/// <summary>Finger Electric Bass</summary>
		FingerElectricBass = 33,
		/// <summary>Pick Electric Bass</summary>
		PickElectricBass = 34,
		/// <summary>Fretless Bass</summary>
		FretlessBass = 35,
		/// <summary>Slap Bass 1</summary>
		SlapBass1 = 36,
		/// <summary>Slap Bass 2</summary>
		SlapBass2 = 37,
		/// <summary>Synth Bass 1</summary>
		SynthBass1 = 38,
		/// <summary>Synth Bass 2</summary>
		SynthBass2 = 39,
		#endregion

		#region Strings
		/// <summary>Violin</summary>
		Violin = 40,
		/// <summary>Viola</summary>
		Viola = 41,
		/// <summary>Cello</summary>
		Cello = 42,
		/// <summary>Contrabass</summary>
		Contrabass = 43,
		/// <summary>Tremolo Strings</summary>
		TremoloStrings = 44,
		/// <summary>Pizzicato Strings</summary>
		PizzicatoStrings = 45,
		/// <summary>Orchestral Strings</summary>
		OrchestralStrings = 46,
		/// <summary>Timpani</summary>
		Timpani = 47,
		#endregion

		#region Ensemble
		/// <summary>String Ensemble 1</summary>
		StringEnsemble1 = 48,
		/// <summary>String Ensemble 2</summary>
		StringEnsemble2 = 49,
		/// <summary>Synth Strings 1</summary>
		SynthStrings1 = 50,
		/// <summary>Synth Strings 2</summary>
		SynthStrings2 = 51,
		/// <summary>Choir Aahs</summary>
		ChoirAahs = 52,
		/// <summary>Voice Oohs</summary>
		VoiceOohs = 53,
		/// <summary>Synth Voice</summary>
		SynthVoice = 54,
		/// <summary>Orchestra Hit</summary>
		OrchestraHit = 55,
		#endregion

		#region Brass
		/// <summary>Trumpet</summary>
		Trumpet = 56,
		/// <summary>Trombone</summary>
		Trombone = 57,
		/// <summary>Tuba</summary>
		Tuba = 58, 
		/// <summary>Muted Trumpet</summary>
		MutedTrumpet = 59,
		/// <summary>French Horn</summary>
		FrenchHorn = 60,
		/// <summary>Brass Section</summary>
		BrassSection = 61,
		/// <summary>Synth Brass 1</summary>
		SynthBrass1 = 62,
		/// <summary>Synth Brass 2</summary>
		SynthBrass2 = 63,
		#endregion

		#region Reed
		/// <summary>Soprano Sax</summary>
		SopranoSax = 64,
		/// <summary>Alto Sax</summary>
		AltoSax = 65,
		/// <summary>Tenor Sax</summary>
		TenorSax = 66,
		/// <summary>Baritone Sax</summary>
		BaritoneSax = 67,
		/// <summary>Oboe</summary>
		Oboe = 68,
		/// <summary>English Horn</summary>
		EnglishHorn = 69,
		/// <summary>Bassoon</summary>
		Bassoon = 70,
		/// <summary>Clarinet</summary>
		Clarinet = 71,
		#endregion
		
		#region Pipe
		/// <summary>Piccolo</summary>
		Piccolo = 72,
		/// <summary>Flute</summary>
		Flute = 73,
		/// <summary>Recorder</summary>
		Recorder = 74,
		/// <summary>Pan Flute</summary>
		PanFlute = 75,
		/// <summary>Blown Bottle</summary>
		BlownBottle = 76,
		/// <summary>Skakuhachi</summary>
		Skakuhachi = 77,
		/// <summary>Whistle</summary>
		Whistle = 78,
		/// <summary>Ocarina</summary>
		Ocarina = 79,
		#endregion

		#region Synth Lead
		/// <summary>Square Lead</summary>
		SquareLead = 80,
		/// <summary>Sawtooth Lead</summary>
		SawtoothLead = 81,
		/// <summary>Calliope Lead</summary>
		CalliopeLead = 82,
		/// <summary>Chiff Lead</summary>
		ChiffLead = 83,
		/// <summary>Charang Lead</summary>
		CharangLead = 84,
		/// <summary>Voice Lead</summary>
		VoiceLead = 85,
		/// <summary>Fifths Lead</summary>
		FifthsLead = 86,
		/// <summary>Base Lead</summary>
		BaseLead = 87,
		#endregion

		#region Synth Pad
		/// <summary>NewAge Pad</summary>
		NewAgePad = 88,
		/// <summary>Warm Pad</summary>
		WarmPad = 89,
		/// <summary>Polysynth Pad</summary>
		PolysynthPad = 90,
		/// <summary>Choir Pad</summary>
		ChoirPad = 91,
		/// <summary>Bowed Pad</summary>
		BowedPad = 92,
		/// <summary>Metallic Pad</summary>
		MetallicPad = 93,
		/// <summary>Halo Pad</summary>
		HaloPad = 94,
		/// <summary>Sweep Pad</summary>
		SweepPad = 95,
		#endregion

		#region Synth Effects
		/// <summary>Rain</summary>
		Rain = 96,
		/// <summary>Soundtrack</summary>
		Soundtrack = 97,
		/// <summary>Crystal</summary>
		Crystal = 98,
		/// <summary>Atmosphere</summary>
		Atmosphere = 99,
		/// <summary>Brightness</summary>
		Brightness = 100,
		/// <summary>Goblin</summary>
		Goblin = 101,
		/// <summary>Echos</summary>
		Echos = 102,
		/// <summary>SciFi</summary>
		SciFi = 103,
		#endregion

		#region Ethnic
		/// <summary>Sitar</summary>
		Sitar = 104,
		/// <summary>Banjo</summary>
		Banjo = 105,
		/// <summary>Shamisen</summary>
		Shamisen = 106,
		/// <summary>Koto</summary>
		Koto = 107,
		/// <summary>Kalimba</summary>
		Kalimba = 108,
		/// <summary>Bagpipe</summary>
		Bagpipe = 109,
		/// <summary>Fiddle</summary>
		Fiddle = 110,
		/// <summary>Shanai</summary>
		Shanai = 111,
		#endregion

		#region Percussive
		/// <summary>Tinkle Bell</summary>
		TinkleBell = 112,
		/// <summary>Agogo</summary>
		Agogo = 113,
		/// <summary>Steel Drums</summary>
		SteelDrums = 114,
		/// <summary>Woodblock</summary>
		Woodblock = 115,
		/// <summary>TaikoD rum</summary>
		TaikoDrum = 116,
		/// <summary>Melodic Tom</summary>
		MelodicTom = 117,
		/// <summary>Synth Drum</summary>
		SynthDrum = 118,
		/// <summary>Reverse Cymbal</summary>
		ReverseCymbal = 119,
		#endregion

		#region Sound Effects
		/// <summary>Guitar Fret Noise</summary>
		GuitarFretNoise = 120,
		/// <summary>Breath Noise</summary>
		BreathNoise = 121,
		/// <summary>Seashore</summary>
		Seashore = 122,
		/// <summary>Bird Tweet</summary>
		BirdTweet = 123,
		/// <summary>Telephone Ring</summary>
		TelephoneRing = 124,
		/// <summary>Helicopter</summary>
		Helicopter = 125,
		/// <summary>Applause</summary>
		Applause = 126,
		/// <summary>Gunshot</summary>
		Gunshot = 127
		#endregion
	}

	/// <summary>General MIDI Percussion Patch Map.</summary>
	[Serializable]
	public enum GeneralMidiPercussion : byte
	{
		#region Sounds
		/// <summary>Bass Drum</summary>
		BassDrum = 35,
		/// <summary>Bass Drum 1</summary>
		BassDrum1 = 36,
		/// <summary>Side Stick</summary>
		SideStick = 37,
		/// <summary>Acoustic Snare</summary>
		AcousticSnare = 38,
		/// <summary>Hand Clap</summary>
		HandClap = 39,
		/// <summary>Electric Snare</summary>
		ElectricSnare = 40,
		/// <summary>Low Floor Tom</summary>
		LowFloorTom = 41,
		/// <summary>Closed Hi Hat</summary>
		ClosedHiHat = 42,
		/// <summary>High Floor Tom</summary>
		HighFloorTom = 43,
		/// <summary>Pedal Hi Hat</summary>
		PedalHiHat = 44,
		/// <summary>Low Tom</summary>
		LowTom = 45,
		/// <summary>Open Hi Hat</summary>
		OpenHiHat = 46,
		/// <summary>Low Mid Tom</summary>
		LowMidTom = 47,
		/// <summary>Hi Mid Tom</summary>
		HiMidTom = 48,
		/// <summary>Crash Cymbal 1</summary>
		CrashCymbal1 = 49,
		/// <summary>High Tom</summary>
		HighTom = 50,
		/// <summary>Ride Cymbal</summary>
		RideCymbal = 51,
		/// <summary>Chinese Cymbal</summary>
		ChineseCymbal = 52,
		/// <summary>Ride Bell</summary>
		RideBell = 53,
		/// <summary>Tambourine</summary>
		Tambourine = 54,
		/// <summary>Splash Cymbal</summary>
		SplashCymbal = 55,
		/// <summary>Cowbell</summary>
		Cowbell = 56,
		/// <summary>Crash Cymbal 2</summary>
		CrashCymbal2 = 57,
		/// <summary>Vibraslap</summary>
		Vibraslap = 58,
		/// <summary>Ride Cymbal 2</summary>
		RideCymbal2 = 59,
		/// <summary>Hi Bongo</summary>
		HiBongo = 60,
		/// <summary>Low Bongo</summary>
		LowBongo = 61,
		/// <summary>Mute Hi Conga</summary>
		MuteHiConga = 62,
		/// <summary>Open Hi Conga</summary>
		OpenHiConga = 63,
		/// <summary>Low Conga</summary>
		LowConga = 64,
		/// <summary>High Timbale</summary>
		HighTimbale = 65,
		/// <summary>Low Timbale</summary>
		LowTimbale = 66,
		/// <summary>High Agogo</summary>
		HighAgogo = 67,
		/// <summary>Low Agogo</summary>
		LowAgogo = 68,
		/// <summary>Cabasa</summary>
		Cabasa = 69,
		/// <summary>Maracas</summary>
		Maracas = 70,
		/// <summary>Short Whistle</summary>
		ShortWhistle = 71,
		/// <summary>Long Whistle</summary>
		LongWhistle = 72,
		/// <summary>Short Guiro</summary>
		ShortGuiro = 73,
		/// <summary>Long Guiro</summary>
		LongGuiro = 74,
		/// <summary>Claves</summary>
		Claves = 75,
		/// <summary>Hi Wood Block</summary>
		HiWoodBlock = 76,
		/// <summary>Low Wood Block</summary>
		LowWoodBlock = 77,
		/// <summary>Mute Cuica</summary>
		MuteCuica = 78,
		/// <summary>Open Cuica</summary>
		OpenCuica = 79,
		/// <summary>Mute Triangle</summary>
		MuteTriangle = 80,
		/// <summary>Open Triangle</summary>
		OpenTriangle = 81
		#endregion
	}

	/// <summary>IDs of MIDI-related manufacturers.</summary>
	[Serializable]
	public enum ManufacturerID : byte
	{
		#region Manufacturers
		/// <summary>Sequential Circuits</summary>
		SequentialCircuits = 1,
		/// <summary>Big Briar</summary>
		BigBriar = 2,
		/// <summary>Octave</summary>
		Octave = 3,
		/// <summary>Moog</summary>
		Moog = 4,
		/// <summary>Passport Designs</summary>
		PassportDesigns = 5,
		/// <summary>Lexicon</summary>
		Lexicon = 6,
		/// <summary>Kurzweil</summary>
		Kurzweil = 7,
		/// <summary>Fender</summary>
		Fender = 8,
		/// <summary>Gulbransen</summary>
		Gulbransen = 9,
		/// <summary>DeltaLabs</summary>
		DeltaLabs = 0x0A,
		/// <summary>SoundComp</summary>
		SoundComp = 0x0B,
		/// <summary>General Electro</summary>
		GeneralElectro = 0x0C,
		/// <summary>Techmar</summary>
		Techmar = 0x0D,
		/// <summary>Matthews Research</summary>
		MatthewsResearch = 0x0E,
		/// <summary>Oberheim</summary>
		Oberheim = 0x10,
		/// <summary>PAIA</summary>
		PAIA = 0x11,
		/// <summary>Simmons</summary>
		Simmons = 0x12,
		/// <summary>DigiDesign</summary>
		DigiDesign = 0x13,
		/// <summary>Fairlight</summary>
		Fairlight = 0x14,
		/// <summary>Peavey</summary>
		Peavey = 0x1B,
		/// <summary>J.L. Cooper</summary>
		JLCooper = 0x15,
		/// <summary>Lowery</summary>
		Lowery = 0x16,
		/// <summary>Lin</summary>
		Lin = 0x17,
		/// <summary>Emu</summary>
		Emu = 0x18,
		/// <summary>Bon Tempi</summary>
		BonTempi = 0x20,
		/// <summary>SIEL</summary>
		SIEL = 0x21,
		/// <summary>Synthe Axe</summary>
		SyntheAxe = 0x23,
		/// <summary>Hohner</summary>
		Hohner = 0x24,
		/// <summary>Crumar</summary>
		Crumar = 0x25,
		/// <summary>Solton</summary>
		Solton = 0x26,
		/// <summary>Jellinghaus Ms</summary>
		JellinghausMs = 0x27,
		/// <summary>CTS</summary>
		CTS = 0x28,
		/// <summary>PPG</summary>
		PPG = 0x29,
		/// <summary>Elka</summary>
		Elka = 0x2F,
		/// <summary>Cheetah</summary>
		Cheetah = 0x36,
		/// <summary>Waldorf</summary>
		Waldorf = 0x3E,
		/// <summary>Kawai</summary>
		Kawai = 0x40,
		/// <summary>Roland</summary>
		Roland = 0x41,
		/// <summary>Korg</summary>
		Korg = 0x42,
		/// <summary>Yamaha</summary>
		Yamaha = 0x43,
		/// <summary>Casio</summary>
		Casio = 0x44,
		/// <summary>Akai</summary>
		Akai = 0x45,
		#endregion

		#region Special Values
		/// <summary>This ID is for educational or development use only.</summary>
		EducationalUse = 0x7d
		#endregion
	}

	/// <summary>Half and whole value steps for the pitch wheel.</summary>
	[Serializable]
	public enum PitchWheelSteps
	{
		#region Defined Step Values
		/// <summary>A complete whole step up.</summary>
		WholeStepUp = 0x3FFF,
		/// <summary>3/4 steps up.</summary>
		ThreeQuarterStepUp = 0x3500,
		/// <summary>1/2 step up.</summary>
		HalfStepUp = 0x3000,
		/// <summary>1/4 step up.</summary>
		QuarterStepUp = 0x2500,
		/// <summary>No movement.</summary>
		NoStep = 0x2000,
		/// <summary>1/4 step down.</summary>
		QuarterStepDown = 0x1500,
		/// <summary>1/2 step down.</summary>
		HalfStepDown = 0x1000,
		/// <summary>3/4 steps down.</summary>
		ThreeQuarterStepDown = 0x500,
		/// <summary>A complete whole step down.</summary>
		WholeStepDown = 0x0
		#endregion
	}

	/// <summary>The tonality of the key signature (major or minor).</summary>
	[Serializable]
	public enum Tonality : byte
	{
		#region Major/Minor
		/// <summary>Key is major.</summary>
		Major = 0,
		/// <summary>Key is minor.</summary>
		Minor = 1
		#endregion
	}

	/// <summary>The number of sharps or flats in the key signature.</summary>
	[Serializable]
	public enum Key // Note: this should be sbyte, but sbyte is not CLS-compliant, so we'll leave it as int
	{
		#region Defined Keys
		/// <summary>Key has no sharps or flats.</summary>
		NoFlatsOrSharps = 0,
		/// <summary>Key has 1 flat.</summary>
		Flat1 = -1,
		/// <summary>Key has 2 flats.</summary>
		Flat2 = -2,
		/// <summary>Key has 3 flats.</summary>
		Flat3 = -3,
		/// <summary>Key has 4 flats.</summary>
		Flat4 = -4,
		/// <summary>Key has 5 flats.</summary>
		Flat5 = -5,
		/// <summary>Key has 6 flats.</summary>
		Flat6 = -6,
		/// <summary>Key has 7 flats.</summary>
		Flat7 = -7,
		/// <summary>Key has 1 sharp.</summary>
		Sharp1 = 1,
		/// <summary>Key has 2 sharps.</summary>
		Sharp2 = 2,
		/// <summary>Key has 3 sharps.</summary>
		Sharp3 = 3,
		/// <summary>Key has 4 sharps.</summary>
		Sharp4 = 4,
		/// <summary>Key has 5 sharps.</summary>
		Sharp5 = 5,
		/// <summary>Key has 6 sharps.</summary>
		Sharp6 = 6,
		/// <summary>Key has 7 sharps.</summary>
		Sharp7 = 7
		#endregion
	}

	/// <summary>List of defined controllers.  All descriptions come from MidiRef4.</summary>
	[Serializable]
	public enum Controllers : byte
	{
		#region Controller Operations
		/// <summary>Switches between groups of sounds when more than 128 programs are in use.</summary>
		BankSelectCourse = 0,
		/// <summary>Sets the modulation wheel to a particular value.</summary>
		ModulationWheelCourse = 1,
		/// <summary>Often used to control aftertouch.</summary>
		BreathControllerCourse = 2,
		/// <summary>Often used to control aftertouch.</summary>
		FootPedalCourse = 4,
		/// <summary>The rate at which portamento slides the pitch between two notes.</summary>
		PortamentoTimeCourse = 5,
		/// <summary>Various.</summary>
		DataEntryCourse = 6,
		/// <summary>Volume level for a given channel.</summary>
		VolumeCourse = 7,
		/// <summary>Controls stereo-balance.</summary>
		BalanceCourse = 8,
		/// <summary>Where the stereo sound should be placed within the sound field.</summary>
		PanPositionCourse = 10,
		/// <summary>Percentage of volume.</summary>
		ExpressionCourse = 11,
		/// <summary>Various.</summary>
		EffectControl1Course = 12,
		/// <summary>Various.</summary>
		EffectControl2Course = 13,
		/// <summary>Various.</summary>
		GeneralPurposeSlider1 = 16,
		/// <summary>Various.</summary>
		GeneralPurposeSlider2 = 17,
		/// <summary>Various.</summary>
		GeneralPurposeSlider3 = 18,
		/// <summary>Various.</summary>
		GeneralPurposeSlider4 = 19,
		/// <summary>Switches between groups of sounds when more than 128 programs are in use.</summary>
		BankSelectFine = 32,
		/// <summary>Sets the modulation wheel to a particular value.</summary>
		ModulationWheelFine = 33,
		/// <summary>Often used to control aftertouch.</summary>
		BreathControllerFine = 34,
		/// <summary>Often used to control aftertouch.</summary>
		FootPedalFine = 36,
		/// <summary>The rate at which portamento slides the pitch between two notes.</summary>
		PortamentoTimeFine = 37,
		/// <summary>Various.</summary>
		DataEntryFine = 38,
		/// <summary>Volume level for a given channel.</summary>
		VolumeFine = 39,
		/// <summary>Controls stereo-balance.</summary>
		BalanceFine = 40,
		/// <summary>Where the stereo sound should be placed within the sound field.</summary>
		PanPositionFine = 42,
		/// <summary>Percentage of volume.</summary>
		ExpressionFine = 43,
		/// <summary>Various.</summary>
		EffectControl1Fine = 44,
		/// <summary>Various.</summary>
		EffectControl2Fine = 45,
		/// <summary>Lengthens release time of playing notes.</summary>
		HoldPedalOnOff = 64,
		/// <summary>The rate at which portamento slides the pitch between two notes.</summary>
		PortamentoOnOff = 65,
		/// <summary>Sustains notes that are already on.</summary>
		SustenutoPedalOnOff = 66,
		/// <summary>Softens volume of any notes played.</summary>
		SoftPedalOnOff = 67,
		/// <summary>Legato effect between notes.</summary>
		LegatoPedalOnOff = 68,
		/// <summary>Lengthens release time of playing notes.</summary>
		Hold2PedalOnOff = 69,
		/// <summary>Various.</summary>
		SoundVariation = 70,
		/// <summary>Controls envelope levels.</summary>
		SoundTimbre = 71,
		/// <summary>Controls envelope release times.</summary>
		SoundReleaseTime = 72,
		/// <summary>Controls envelope attack time.</summary>
		SoundAttackTime = 73,
		/// <summary>Controls filter's cutoff frequency.</summary>
		SoundBrightness = 74,
		/// <summary>Various.</summary>
		SoundControl6 = 75,
		/// <summary>Various.</summary>
		SoundControl7 = 76,
		/// <summary>Various.</summary>
		SoundControl8 = 77,
		/// <summary>Various.</summary>
		SoundControl9 = 78,
		/// <summary>Various.</summary>
		SoundControl10 = 79,
		/// <summary>Various.</summary>
		GeneralPurposeButton1OnOff = 80,
		/// <summary>Various.</summary>
		GeneralPurposeButton2OnOff = 81,
		/// <summary>Various.</summary>
		GeneralPurposeButton3OnOff = 82,
		/// <summary>Various.</summary>
		GeneralPurposeButton4OnOff = 83,
		/// <summary>Controls level of effects.</summary>
		EffectsLevel = 91,
		/// <summary>Controls level of tremulo.</summary>
		TremuloLevel = 92,
		/// <summary>Controls level of chorus.</summary>
		ChorusLevel = 93,
		/// <summary>Detune amount for device.</summary>
		CelesteLevel = 94,
		/// <summary>Level of phaser effect.</summary>
		PhaserLevel = 95,
		/// <summary>Causes data button's value to increment.</summary>
		DataButtonIncrement = 96,
		/// <summary>Causes data button's value to decrement.</summary>
		DataButtonDecrement = 97,
		/// <summary>Controls which parameter the button and data entry controls affect.</summary>
		NonRegisteredParameterFine = 98,
		/// <summary>Controls which parameter the button and data entry controls affect.</summary>
		NonRegisteredParameterCourse = 99,
		/// <summary>Controls which parameter the button and data entry controls affect.</summary>
		RegisteredParameterFine = 100,
		/// <summary>Controls which parameter the button and data entry controls affect.</summary>
		RegisteredParameterCourse = 101,
		/// <summary>Mutes all sounding notes.</summary>
		AllSoundOff = 120,
		/// <summary>Resets controllers to default states.</summary>
		AllControllersOff = 121,
		/// <summary>Turns on or off local keyboard.</summary>
		LocalKeyboardOnOff = 122,
		/// <summary>Mutes all sounding notes.</summary>
		AllNotesOff = 123,
		/// <summary>Turns Omni off.</summary>
		OmniModeOff = 124,
		/// <summary>Turns Omni on.</summary>
		OmniModeOn = 125,
		/// <summary>Enables Monophonic operation.</summary>
		MonoOperation = 126,
		/// <summary>Enables Polyphonic operation.</summary>
		PolyOperation = 127
		#endregion
	}
}
