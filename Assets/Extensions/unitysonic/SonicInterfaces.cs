using UnityEngine;
using System.Collections;
using Rosettastone.Speech;
using System.Collections.Generic;

public class SonicInterfaces {


	public enum Difficulty : int { Level1 = 1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, Level9, Level10 }

	public sealed class VoiceType
	{
		private readonly string _name;
		public readonly Values _value;

		private VoiceType(Values value, string name){
			this._name = name;
			this._value = value;
		}

		public override string ToString(){
			return _name;
		}

		public enum Values
		{
			MALE = 1,
			FEMALE = 2,
		}

		public static readonly VoiceType MALE = new VoiceType (Values.MALE, "male");
		public static readonly VoiceType FEMALE = new VoiceType (Values.FEMALE, "female");
	}

	public sealed class Language
	{
		private readonly string _name;
		public readonly Values _value;

		private Language(Values value, string name){
			this._name = name;
			this._value = value;
		}

		public override string ToString(){
			return _name;
		}

		public enum Values
		{
			ENGLISH = 1,
			CHINESE = 2,
			FARSI = 3,
			FRENCH = 4,
			GERMAN = 5,
			ITALIAN = 6,
			JAPANESE = 7,
			PORTUGUESE = 8,
			SPANISH = 9
		}

		public static readonly Language ENGLISH = new Language (Values.ENGLISH, "en-US");
		public static readonly Language CHINESE = new Language (Values.CHINESE, "en-US");
		public static readonly Language FARSI = new Language (Values.FARSI, "fa-IR");
		public static readonly Language FRENCH = new Language (Values.FRENCH, "fr-FR");
		public static readonly Language GERMAN = new Language (Values.GERMAN, "de-DE");
		public static readonly Language ITALIAN = new Language (Values.PORTUGUESE, "it-IT");
		public static readonly Language JAPANESE = new Language (Values.JAPANESE, "ja-JP");
		public static readonly Language PORTUGUESE = new Language (Values.PORTUGUESE, "pt-BR");
		public static readonly Language SPANISH = new Language (Values.SPANISH, "es-419");

	}

	public enum StopReason : int { 
		NONE = Rosettastone.Speech.StopReason.STOP_REASON_NONE,
		MANUAL = Rosettastone.Speech.StopReason.STOP_REASON_MANUAL, 
		AUTO = Rosettastone.Speech.StopReason.STOP_REASON_AUTO, 
		SILENCE_TIMEOUT = Rosettastone.Speech.StopReason.STOP_REASON_SILENCE_TIMEOUT, 
		DURATION_TIMEOUT = Rosettastone.Speech.StopReason.STOP_REASON_DURATION_TIMEOUT 
	}

	public enum AudioQuality : int {
		OK = Rosettastone.Speech.AudioQualityCalibrationResult.CALIBRATION_RESULT_OK,
		TOO_LOUD = Rosettastone.Speech.AudioQualityCalibrationResult.CALIBRATION_RESULT_TOO_LOUD,
		TOO_SOFT = Rosettastone.Speech.AudioQualityCalibrationResult.CALIBRATION_RESULT_TOO_SOFT,
		NO_SIGNAL = Rosettastone.Speech.AudioQualityCalibrationResult.CALIBRATION_RESULT_NO_SIGNAL

	}

	//// TIMEOUT VALUES in ms
	public static int defaultBeginOfSpeechTimeout = 10000;
	public static int defaultEndOfSpeechTimeout = 2000;
	public static int defaultDurationTimeout = -1;

	public interface ConfigurationCallback
	{
		void onConfigureStart ();
		void onConfigureUpdate (float progress);
		void onConfigureComplete ();
		void onConfigureError (string message);
	}

	public interface ListenForPhrasesCallback
	{
		void onLFPStart ();
		void onLFPUpdate (float energy, string response);
		void onLFPComplete (ListenForPhrasesResult result);
		void onLFPError (string message);
	}

	public interface ListenForPhrasesPlaybackCallback
	{
		void onLFPPlaybackStart ();
		void onLFPPlaybackUpdate (int currentTimeMS);
		void onLFPPlaybackComplete ();
		void onLFPPlaybackError (string message);
	}

	public interface OpenEndedCallback
	{
		void onOpenEndedStart ();
		void onOpenEndedUpdate (float energy, string response);
		void onOpenEndedComplete (OpenEndedResult result);
		void onOpenEndedError (string message);
	}

	public interface ReadingTrackerCallback
	{
		void onReadingTrackerStart ();
		void onReadingTrackerUpdate (float energy, ReadingTrackerUpdateResult updateResult);
		void onReadingTrackerComplete (ReadingTrackerResult result);
		void onReadingTrackerError (string message);
	}

//	Currently crashing...
//	public interface ReadingTrackerPlaybackCallback
//	{
//		void onReadingTrackerPlaybackStart ();
//		void onReadingTrackerPlaybackUpdate (int currentTimeMS, ReadingTrackerUpdateResult updateResult);
//		void onReadingTrackerPlaybackComplete (ReadingTrackerResult result);
//		void onReadingTrackerPlaybackError (string message);
//	}

	public interface SonicSREInterface {
		void configure (Language language, VoiceType voiceType, Difficulty difficulty, ConfigurationCallback callback);
		void listenForPhrases (List<string> phrases, ListenForPhrasesCallback callback);
		void playbackListenForPhrases (int startTimeMS, ListenForPhrasesPlaybackCallback callback);
		void readingTracker (string text, ReadingTrackerCallback callback);
		void openEndedListen (List<string> phrases, OpenEndedCallback callback);
//		void playbackReadingTracker (int startTimeMS, ReadingTrackerPlaybackCallback callback);
		void interrupt ();
	}

	public struct WordResult {
		public string text;           // text of the word
		public bool passed;                // determines if the pronunciation score passes a certain threshold
		public int pronunciationScore;     // score based on the pronunciation of the word

		public float beginTime;            // start time of the word being spoken in the recording
		public float endTime;              // end time of the word being spoken in the recording
	};

	public struct ListenForPhrasesResult {
		public string response;   // String containing all spoken words
		public float energy;           // Current energy level of mic input
		public AudioQuality audioQuality; // Quality of the received audio, as determined by SRE

		public int pronunciationScore; // Score based on pronunciation of spoken words
		public bool passed;            // Checks if overallScore passes a certain threshold

		public int overallScore;
		public StopReason stopReason;
		public float totalSeconds;

		public List<WordResult> wordResults;   // list of word results
	};


	public struct OpenEndedResult {
		public string response;   // String containing all spoken words
		public float energy;           // Current energy level of mic input
		public AudioQuality audioQuality; // Quality of the received audio, as determined by SRE

		public int pronunciationScore; // Score based on pronunciation of spoken words

		public StopReason stopReason;
		public float totalSeconds;

		public List<WordResult> wordResults;   // list of word results
	};

	public struct ReadingTrackerUpdateResult {
		////// Used for highlighting -- important during update
		/** The character index of the word in the original text */
		public int wordStartIndex;
		/** The length of the word in the original text */
		public int wordLength;

		/** Current energy level of microphone input */
		public float energy;
	};

	public struct ReadingTrackerWordResult {
		public string text;   // text of the word
		public int spoken;         // > 0 means word was spoken
		public int startIndex;     // start index of the word in the text
		public int length;         // length of the word
		public int score;          // score for the word
		public int passed;         // determines if the score is above a certain threshold
	};

	public struct ReadingTrackerMetrics {
		public float wordCountPerMinute;  // final reading speed

		public float totalSeconds;         // duration of recording
		public float speakingSeconds;      // duration of learner speaking in the recording

		public int spokenWordCount;        // number of words recognized in the recording
		public int totalWordCount;         // total number of words in the text    
	};

	public struct ReadingTrackerScore {
		public float accuracy;         // ratio of words spoken correctly vs total number of words
		public float pronunciation;    // score value focused on the pronunciation of words spoken
		public float final;            // overall score takes into account numSpoken vs numTotal
	};

	public struct ReadingTrackerResult {
		public ReadingTrackerMetrics metrics;
		public ReadingTrackerScore score;
		public StopReason stopReason;
		public AudioQuality audioQuality;
		public List<ReadingTrackerWordResult> words;
	}

}
