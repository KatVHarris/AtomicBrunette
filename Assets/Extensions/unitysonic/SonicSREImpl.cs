using UnityEngine;
using System.Collections;
using System;
using Rosettastone.Speech;
using System.Collections.Generic;

public sealed class SonicSREImpl : SonicInterfaces.SonicSREInterface
{

	private static string DEVICE_IDENTIFIER = "SRE_VR_DEMO";

	private SpeechEngine speechEngine;
	private Rosettastone.Speech.Logger logger;
 
	private SpeechSession activeSession;
	private LoadModelSession configureSession;
	private ListenForPhrasesSession lfpSession;
	private PlaySoundSession lfpPlaybackSession;
	private ReadingTrackerSession readingTrackerSession;	
	private OpenEndedSession openEndedSession;
	//private ReadingTrackerPlaybackSession readingTrackerPlaybackSession;

	private SonicInterfaces.ConfigurationCallback configCallback;
	private SonicInterfaces.ListenForPhrasesCallback lfpCallback;
	private SonicInterfaces.ListenForPhrasesPlaybackCallback listenForPhrasesPlaybackCallback;
	private SonicInterfaces.ReadingTrackerCallback readingTrackerCallback;
	private SonicInterfaces.OpenEndedCallback openEndedCallback;
	//private SonicInterfaces.ReadingTrackerPlaybackCallback readingTrackerPlaybackCallback;

	private SoundObject soundObject;
	private string readingTrackerText;
	private string readingTrackerHypXML;

	private static readonly SonicSREImpl _instance = new SonicSREImpl();

	private SonicSREImpl()
	{
		logger = new Rosettastone.Speech.Logger();
		speechEngine = UnitySonic.Instance.createSpeechEngine(true, DEVICE_IDENTIFIER, logger);
	}

	public static SonicSREImpl Instance
	{
		get
		{
			return _instance;
		}
	}
		
	// -------- Configuration / Model Loading ---------

	public void configure(SonicInterfaces.Language language, SonicInterfaces.VoiceType voiceType, SonicInterfaces.Difficulty difficulty, SonicInterfaces.ConfigurationCallback callback)
	{
		this.configCallback = callback;

		if(speechEngine == null) {
			if(configCallback == null) {
				return;
			}
			configCallback.onConfigureError("Speech engine not initialized.");
			return;
		}

		if(activeSession != null) {
			if(configCallback == null) {
				return;
			}
			configCallback.onConfigureError("Speech Engine already has active session");
			return;
		}

		if(configureSession != null) {
			configureSession = null;
		}
			
		String langCode = language.ToString();
		String voiceTypeStr = voiceType.ToString();

		SpeechModelDescriptor speechDescriptor = new SpeechModelDescriptor(langCode, voiceTypeStr);
		speechEngine.getSpeechModelProvider().setIdentifier("Callisto");

		configureSession = speechEngine.createLoadModelSession(speechDescriptor, (int)difficulty);

		configureSession.OnStart += LoadModelSession_OnStart;
		configureSession.OnUpdate += LoadModelSession_OnUpdate;
		configureSession.OnEnd += LoadModelSession_OnEnd;

		activeSession = configureSession;

		configureSession.run();
	}
		
	private void LoadModelSession_OnStart ()
	{
		if (this.configCallback != null) {
			this.configCallback.onConfigureStart();
		}
	}

	private void LoadModelSession_OnUpdate ()
	{
		if (this.configCallback != null) {
			this.configCallback.onConfigureUpdate (configureSession.getProgress ());
		}
	}

	private void LoadModelSession_OnEnd()
	{
		if (this.configCallback != null) {
			if (configureSession.getState() ==  Task.TaskState.SUCCEEDED) {
				configCallback.onConfigureComplete ();
			} 
			else if (configureSession.getState() == Task.TaskState.ERRORED) {
				configCallback.onConfigureError (configureSession.errorMessage);
			}
			else {
				configCallback.onConfigureError ("Configure stopped");
			}
		}

		configureSession = null;
		activeSession = null;

	}

	public void ConfigureUnsubscribe()
	{
		configureSession.OnStart -= LoadModelSession_OnStart;
		configureSession.OnUpdate -= LoadModelSession_OnUpdate;
		configureSession.OnEnd -= LoadModelSession_OnEnd;
	}

	// -------- Listen For Phrases Recognition ---------

	public void listenForPhrases(List<string> phrases, SonicInterfaces.ListenForPhrasesCallback callback)
	{
		this.lfpCallback = callback;

		if(speechEngine == null) {
			if(lfpCallback == null) {
				return;
			}
			lfpCallback.onLFPError("Speech engine not initialized.");
			return;
		}
			
		if(activeSession != null) {
			if (lfpCallback == null) {
				return;
			}
			lfpCallback.onLFPError("Speech Engine already has active session.");
			return;

		}

		if(lfpSession != null) {
			lfpSession = null;
		}

		lfpSession = speechEngine.createListenForPhrasesSession(phrases.ToArray());
		lfpSession.withDefaultPromptSound();
		lfpSession.withMetadata(sonic.AUDIO_METADATA_ENERGY);
		lfpSession.withBeginOfSpeechTimeout(SonicInterfaces.defaultBeginOfSpeechTimeout);
		lfpSession.withEndOfSpeechTimeout(SonicInterfaces.defaultEndOfSpeechTimeout);
		lfpSession.withDuration(SonicInterfaces.defaultDurationTimeout);

		lfpSession.OnStart += LFPSession_OnStart;
		lfpSession.OnUpdate += LFPSession_OnUpdate;
		lfpSession.OnEnd += LFPSession_OnEnd;

		activeSession = lfpSession;
		lfpSession.run();
	}

	private void LFPSession_OnStart()
	{
		if (this.lfpCallback != null) {
			this.lfpCallback.onLFPStart();
		}	
	}

	private void LFPSession_OnUpdate()
	{
		Hypothesis hypothesis = null;
		if (lfpSession.hasHypothesis ()) {
			hypothesis = lfpSession.getHypothesis ();
		}
		string response = (hypothesis != null) ? hypothesis.getResponse () : "";

		if(this.lfpCallback != null) {
			this.lfpCallback.onLFPUpdate(lfpSession.getMeanFrameEnergy(), response);
		}
	}

	private void LFPSession_OnEnd()
	{
		if (lfpSession.getState() == Task.TaskState.ERRORED)
		{
			if (this.lfpCallback != null) {
				lfpCallback.onLFPError (lfpSession.errorMessage);
			}
		}
		else
		{
			SonicInterfaces.ListenForPhrasesResult result = new SonicInterfaces.ListenForPhrasesResult();

			if(isResultAvailable(lfpSession)) {

				Hypothesis hyp = lfpSession.getHypothesis ();

				result.response = hyp.getResponse();
				if(lfpSession.getState() == Task.TaskState.SUCCEEDED) {
					result.pronunciationScore = hyp.grading.score;
					result.passed = lfpSession.getPassed();
					result.overallScore = getOverallPhraseScore(lfpSession);
					result.audioQuality = (SonicInterfaces.AudioQuality) lfpSession.getAudioQuality ().calibration_result;

					result.stopReason = (SonicInterfaces.StopReason) lfpSession.getStopReason();

					List<SonicInterfaces.WordResult> words = new List<SonicInterfaces.WordResult> ();

					for (int i = 0; i < (int) hyp.wordListLength(); i++) {
						WordHypothesis wordHyp = hyp.words [i];

						SonicInterfaces.WordResult word = new SonicInterfaces.WordResult ();
						word.text = wordHyp.text;
						word.beginTime = wordHyp.alignment.bTime;
						word.endTime = wordHyp.alignment.eTime;
						word.passed = (wordHyp.grading.accept == 1);
						word.pronunciationScore = wordHyp.grading.score;
						words.Add (word);
					}

					result.wordResults = words;
				}
			}

			if(this.soundObject != null) {
				soundObject = null;
			}

			soundObject = lfpSession.detachSoundObject();


			if (this.lfpCallback != null) {				
				lfpCallback.onLFPComplete (result);
			}
		}

		lfpSession = null;
		activeSession = null;

	}

	public void LFPUnsubscribe()
	{
		lfpSession.OnSuccess -= LFPSession_OnStart;
		lfpSession.OnUpdate -= LFPSession_OnUpdate;
		lfpSession.OnEnd -= LFPSession_OnEnd;
	}

	// -------- Listen For Phrases Playback ---------


	public void playbackListenForPhrases(int startTimeMS, SonicInterfaces.ListenForPhrasesPlaybackCallback callback)
	{
		this.listenForPhrasesPlaybackCallback = callback;

		if(speechEngine == null) {
			if(listenForPhrasesPlaybackCallback == null) {
				return;
			}
			listenForPhrasesPlaybackCallback.onLFPPlaybackError("Speech engine not initialized.");
			return;
		}

		if(activeSession != null) {
			if (listenForPhrasesPlaybackCallback == null) {
				return;
			}
			listenForPhrasesPlaybackCallback.onLFPPlaybackError("Speech Engine already has active session.");
			return;
		}

		if(soundObject == null) {
			if (listenForPhrasesPlaybackCallback == null) {
				return;
			}
			listenForPhrasesPlaybackCallback.onLFPPlaybackError("No recording to playback.");
			return;

		}

		if(lfpPlaybackSession != null) {
			lfpPlaybackSession = null;
		}

		lfpPlaybackSession = speechEngine.createPlaySoundSession(soundObject);
		lfpPlaybackSession.withStartTimeMS(startTimeMS);

		lfpPlaybackSession.OnStart += PlaybackLFPSession_OnStart;
		lfpPlaybackSession.OnUpdate += PlaybackLFPSession_OnUpdate;
		lfpPlaybackSession.OnEnd += PlaybackLFPSession_OnEnd;

		activeSession = lfpPlaybackSession;

		lfpPlaybackSession.run();

	}

	private void PlaybackLFPSession_OnStart()
	{
		if (this.listenForPhrasesPlaybackCallback != null) {
			this.listenForPhrasesPlaybackCallback.onLFPPlaybackStart();
		}	
	}

	private void PlaybackLFPSession_OnUpdate()
	{
		if(this.listenForPhrasesPlaybackCallback != null) {
			this.listenForPhrasesPlaybackCallback.onLFPPlaybackUpdate(lfpPlaybackSession.getCurrentTimeMS());
		}
	}

	private void PlaybackLFPSession_OnEnd()
	{
		if (this.listenForPhrasesPlaybackCallback != null)
		{
			if (lfpPlaybackSession.getState() == Task.TaskState.ERRORED)
			{
				listenForPhrasesPlaybackCallback.onLFPPlaybackError(lfpPlaybackSession.errorMessage);
			}
			else
			{
				listenForPhrasesPlaybackCallback.onLFPPlaybackComplete();
			}
		}

		activeSession = null;
		lfpPlaybackSession = null;
	}

	public void ListenForPhrasesPlaybackUnsubscribe()
	{
		lfpPlaybackSession.OnSuccess -= LFPSession_OnStart;
		lfpPlaybackSession.OnUpdate -= LFPSession_OnUpdate;
		lfpPlaybackSession.OnEnd -= LFPSession_OnEnd;
	}

	// -------- OpenEnded Recognition ---------

	public void openEndedListen(List<string> phrases, SonicInterfaces.OpenEndedCallback callback)
	{
		this.openEndedCallback = callback;

		if(speechEngine == null) {
			if(openEndedCallback == null) {
				return;
			}
			openEndedCallback.onOpenEndedError("Speech engine not initialized.");
			return;
		}

		if(activeSession != null) {
			if (openEndedCallback == null) {
				return;
			}
			openEndedCallback.onOpenEndedError("Speech Engine already has active session.");
			return;

		}

		if(openEndedSession != null) {
			openEndedSession = null;
		}

		openEndedSession = speechEngine.createOpenEndedSession(phrases.ToArray());
		openEndedSession.withDefaultPromptSound();
		openEndedSession.withMetadata(sonic.AUDIO_METADATA_ENERGY);
		openEndedSession.withBeginOfSpeechTimeout(SonicInterfaces.defaultBeginOfSpeechTimeout);
		openEndedSession.withEndOfSpeechTimeout(SonicInterfaces.defaultEndOfSpeechTimeout);
		openEndedSession.withDuration(SonicInterfaces.defaultDurationTimeout);

		openEndedSession.OnStart += OpenEndedSession_OnStart;
		openEndedSession.OnUpdate += OpenEndedSession_OnUpdate;
		openEndedSession.OnEnd += OpenEndedSession_OnEnd;

		activeSession = openEndedSession;
		openEndedSession.run();
	}

	private void OpenEndedSession_OnStart()
	{
		if (this.openEndedCallback != null) {
			this.openEndedCallback.onOpenEndedStart();
		}	
	}

	private void OpenEndedSession_OnUpdate()
	{
		Hypothesis hypothesis = null;
		if (openEndedSession.hasHypothesis ()) {
			hypothesis = openEndedSession.getHypothesis ();
		}
		string response = (hypothesis != null) ? hypothesis.getResponse () : "";

		if(this.openEndedCallback != null) {
			this.openEndedCallback.onOpenEndedUpdate(openEndedSession.getMeanFrameEnergy(), response);
		}
	}

	private void OpenEndedSession_OnEnd()
	{
		if (openEndedSession.getState() == Task.TaskState.ERRORED)
		{
			if (this.openEndedCallback != null) {
				openEndedCallback.onOpenEndedError (openEndedSession.errorMessage);
			}
		}
		else
		{
			SonicInterfaces.OpenEndedResult result = new SonicInterfaces.OpenEndedResult();

			if(isResultAvailable(openEndedSession)) {

				Hypothesis hyp = openEndedSession.getHypothesis ();

				result.response = hyp.getResponse();
				if(openEndedSession.getState() == Task.TaskState.SUCCEEDED) {
					result.pronunciationScore = hyp.grading.score;
					result.audioQuality = (SonicInterfaces.AudioQuality) openEndedSession.getAudioQuality ().calibration_result;

					result.stopReason = (SonicInterfaces.StopReason) openEndedSession.getStopReason();

					List<SonicInterfaces.WordResult> words = new List<SonicInterfaces.WordResult> ();

					for (int i = 0; i < (int) hyp.wordListLength(); i++) {
						WordHypothesis wordHyp = hyp.words [i];

						SonicInterfaces.WordResult word = new SonicInterfaces.WordResult ();
						word.text = wordHyp.text;
						word.beginTime = wordHyp.alignment.bTime;
						word.endTime = wordHyp.alignment.eTime;
						word.passed = (wordHyp.grading.accept == 1);
						word.pronunciationScore = wordHyp.grading.score;
						words.Add (word);
					}

					result.wordResults = words;
				}
			}

			if(this.soundObject != null) {
				soundObject = null;
			}

			soundObject = openEndedSession.detachSoundObject();

			if (this.openEndedCallback != null) {				
				openEndedCallback.onOpenEndedComplete (result);
			}
		}

		openEndedSession = null;
		activeSession = null;

	}

	public void openEndedUnsubscribe()
	{
		openEndedSession.OnSuccess -= OpenEndedSession_OnStart;
		openEndedSession.OnUpdate -= OpenEndedSession_OnUpdate;
		openEndedSession.OnEnd -= OpenEndedSession_OnEnd;
	}

	// -------- Reading Tracker Recognition ---------

	public void readingTracker (string text, SonicInterfaces.ReadingTrackerCallback callback)
	{
		this.readingTrackerCallback = callback;

		if(speechEngine == null) {
			if(readingTrackerCallback == null) {
				return;
			}
			readingTrackerCallback.onReadingTrackerError("Speech engine not initialized.");
			return;
		}

		if(activeSession != null) {
			if (readingTrackerCallback == null) {
				return;
			}
			readingTrackerCallback.onReadingTrackerError("Speech Engine already has active session.");
			return;

		}

		if(readingTrackerSession != null) {
			readingTrackerSession = null;
		}

		readingTrackerSession = speechEngine.createReadingTrackerSession(text, false);
		readingTrackerSession.withDefaultPromptSound();
		readingTrackerSession.withMetadata(sonic.AUDIO_METADATA_ENERGY);
		readingTrackerSession.withBeginOfSpeechTimeout(SonicInterfaces.defaultBeginOfSpeechTimeout);
		readingTrackerSession.withEndOfSpeechTimeout(SonicInterfaces.defaultEndOfSpeechTimeout);
		readingTrackerSession.withDuration(SonicInterfaces.defaultDurationTimeout);

		readingTrackerSession.OnStart += ReadingTrackerSession_OnStart;
		readingTrackerSession.OnUpdate += ReadingTrackerSession_OnUpdate;
		readingTrackerSession.OnEnd += ReadingTrackerSession_OnEnd;

		readingTrackerText = text;
		activeSession = readingTrackerSession;
		readingTrackerSession.run();
	}

	private void ReadingTrackerSession_OnStart()
	{
		if (this.readingTrackerCallback != null) {
			this.readingTrackerCallback.onReadingTrackerStart();
		}	
	}

	private void ReadingTrackerSession_OnUpdate()
	{

		if(this.readingTrackerCallback != null) {
			SonicInterfaces.ReadingTrackerUpdateResult response = new SonicInterfaces.ReadingTrackerUpdateResult();
			response.energy = readingTrackerSession.getMeanFrameEnergy();
			ReadingTrackerWindow _window = readingTrackerSession.getWindow ();
			response.wordStartIndex = _window.curr.char_index;
			response.wordLength = _window.curr.char_len;        
			this.readingTrackerCallback.onReadingTrackerUpdate(readingTrackerSession.getMeanFrameEnergy(), response);
		}
	}

	private void ReadingTrackerSession_OnEnd()
	{

		if (this.readingTrackerCallback != null)
		{
			if (readingTrackerSession.getState() == Task.TaskState.ERRORED)
			{
				readingTrackerCallback.onReadingTrackerError(readingTrackerSession.errorMessage);
			}
			else
			{
				if(this.soundObject != null) {
					soundObject = null;
				}
				soundObject = readingTrackerSession.detachSoundObject();
				readingTrackerHypXML = readingTrackerSession.getHypothesis().toXMLString();

				if (readingTrackerCallback != null) {
					readingTrackerCallback.onReadingTrackerComplete (getFinalReadingTrackerResult (readingTrackerSession));
				}
			}
		}

		activeSession = null;
		readingTrackerSession = null;

	}

	public void ReadingTrackerUnsubscribe()
	{
		readingTrackerSession.OnSuccess -= ReadingTrackerSession_OnStart;
		readingTrackerSession.OnUpdate -= ReadingTrackerSession_OnUpdate;
		readingTrackerSession.OnEnd -= ReadingTrackerSession_OnEnd;
	}

	// -------- Reading Tracker Playback ---------

	/* -- Currently not working properly 
	public void playbackReadingTracker (int startTimeMS, SonicInterfaces.ReadingTrackerPlaybackCallback callback)
	{
		this.readingTrackerPlaybackCallback = callback;

		if(speechEngine == null) {
			if(readingTrackerPlaybackCallback != null) {
				readingTrackerPlaybackCallback.onReadingTrackerPlaybackError("Speech engine not initialized.");
			}
			return;
		}

		if(activeSession != null) {
			if (readingTrackerPlaybackCallback != null) {
				readingTrackerPlaybackCallback.onReadingTrackerPlaybackError("Speech Engine already has active session.");
			}
			return;

		}

		if(readingTrackerPlaybackSession != null) {
			readingTrackerPlaybackSession = null;
		}

		if(soundObject == null || readingTrackerHypXML == null || readingTrackerText.Length == 0) {
			if(readingTrackerPlaybackCallback != null) {
				readingTrackerPlaybackCallback.onReadingTrackerPlaybackError("No recording to playback.");
			}
			return;
		}

		if(readingTrackerPlaybackSession != null) {
			readingTrackerPlaybackSession = null;
		}

		Hypothesis hyp = new Hypothesis();
		hyp.fromXMLString (readingTrackerHypXML);
		readingTrackerPlaybackSession = speechEngine.createReadingTrackerPlaybackSession(readingTrackerText, soundObject, hyp);
		readingTrackerPlaybackSession.withStartTimeMS(startTimeMS);

		
		readingTrackerPlaybackSession.OnStart += PlaybackReadingTrackerSession_OnStart;
		readingTrackerPlaybackSession.OnUpdate += PlaybackReadingTrackerSession_OnUpdate;
		readingTrackerPlaybackSession.OnEnd += PlaybackReadingTrackerSession_OnEnd;

		activeSession = readingTrackerPlaybackSession;
		readingTrackerPlaybackSession.run();

	}

	private void PlaybackReadingTrackerSession_OnStart()
	{
		if (this.readingTrackerPlaybackCallback != null) {
			this.readingTrackerPlaybackCallback.onReadingTrackerPlaybackStart();
		}	
	}

	private void PlaybackReadingTrackerSession_OnUpdate()
	{
		if(this.readingTrackerPlaybackCallback != null) {
			SonicInterfaces.ReadingTrackerUpdateResult response = new SonicInterfaces.ReadingTrackerUpdateResult();
			response.energy = readingTrackerPlaybackSession.getMeanFrameEnergy();
			ReadingTrackerWindow _window = readingTrackerPlaybackSession.getWindow ();
			response.wordStartIndex = _window.curr.char_index;
			response.wordLength = _window.curr.char_len;        
			this.readingTrackerPlaybackCallback.onReadingTrackerPlaybackUpdate(readingTrackerPlaybackSession.getCurrentTimeMS(), response);
		}
	}

	private void PlaybackReadingTrackerSession_OnEnd()
	{

		if (this.readingTrackerPlaybackSession != null)
		{
			if (readingTrackerPlaybackSession.getState() == Task.TaskState.ERRORED)
			{
				readingTrackerPlaybackCallback.onReadingTrackerPlaybackError(readingTrackerPlaybackSession.errorMessage);
			}
			else
			{
				if (readingTrackerPlaybackCallback != null) {
					readingTrackerPlaybackCallback.onReadingTrackerPlaybackComplete(getFinalReadingTrackerResult(readingTrackerPlaybackSession));
				}
			}
		}
		
		activeSession = null;
		readingTrackerPlaybackSession = null;

	}

	public void ReadingTrackerPlaybackUnsubscribe()
	{
		readingTrackerPlaybackSession.OnSuccess -= PlaybackReadingTrackerSession_OnStart;
		readingTrackerPlaybackSession.OnUpdate -= PlaybackReadingTrackerSession_OnUpdate;
		readingTrackerPlaybackSession.OnEnd -= PlaybackReadingTrackerSession_OnEnd;
	}

	*/

	public void interrupt() {
		if(activeSession != null) {
			activeSession.interrupt();
		}
	}

	private bool isResultAvailable(ListenSession session) {
		bool result = session.hasHypothesis();
		if(result) {
			result = session.getHypothesis().isFinal();
		}
		return result;
	}


	/** Functions used for computing overall score */
	private float getPhraseQuality(ListenForPhrasesSession session) {
		Hypothesis hyp = session.getHypothesis ();
		float quality = 0.0f;
		if (hyp.words.Count > 0) {
			float sumWordScore = 0.0f;
			for(int i = 0; i < hyp.words.Count; i++) {
				sumWordScore += (float) hyp.words[i].grading.score;
			}
			quality = sumWordScore / (float) (hyp.wordListLength());
		}
		return quality;
	}

	private float getPhraseAcceptance(ListenForPhrasesSession session) {
		Hypothesis hyp = session.getHypothesis();
		float acceptance = 0.0f;
		if (hyp.words.Count > 0) {
			int wordAcceptanceCount = hyp.words.Count;
			for(int i = 0; i < hyp.words.Count; i++) {
				if(hyp.words[i].grading.score < 7) {
					wordAcceptanceCount--;
				}
			}
			acceptance = 10.0f*( (float)wordAcceptanceCount / (float)hyp.words.Count );
		}
		return acceptance;
	}

	private int getOverallPhraseScore(ListenForPhrasesSession session) {
		Hypothesis hyp = session.getHypothesis();

		float quality = getPhraseQuality(session);
		float acceptance = getPhraseAcceptance(session);
		float phraseScore = (float) hyp.grading.score;

		float overallPhraseScore = (phraseScore + quality + acceptance)/3.0f;

		return (int)(overallPhraseScore + 0.5f);
	}

	private SonicInterfaces.ReadingTrackerResult getFinalReadingTrackerResult(ReadingTrackerSession session) {
		SonicInterfaces.ReadingTrackerResult res = new SonicInterfaces.ReadingTrackerResult();

		if (isResultAvailable (session)) {

			res.audioQuality = (SonicInterfaces.AudioQuality) session.getAudioQuality ().calibration_result;

			Reference _reference = session.getReference ();

			res.metrics.wordCountPerMinute = _reference.readingResults.rateOfSpeech.wcpm;
			res.metrics.totalSeconds = _reference.readingResults.duration.total_seconds;
			res.metrics.speakingSeconds = _reference.readingResults.duration.articulation_seconds;
			res.metrics.spokenWordCount = _reference.readingResults.wordCounts.num_spoken;
			res.metrics.totalWordCount = _reference.readingResults.wordCounts.num_total;

			res.score.accuracy = _reference.readingResults.scoring.accuracy;
			res.score.pronunciation = _reference.readingResults.scoring.overall_score;
			res.score.final = (float)res.score.pronunciation * ((float)res.metrics.spokenWordCount / (float)res.metrics.totalWordCount);

			res.stopReason = (SonicInterfaces.StopReason)readingTrackerSession.getStopReason ();
			List<SonicInterfaces.ReadingTrackerWordResult> words = new List<SonicInterfaces.ReadingTrackerWordResult> ();

			// iterate through WordHypotheses
			for (int i = 0; i < res.metrics.totalWordCount; i++) {
				ReferenceWord wordRef = _reference.getWord (i);

				SonicInterfaces.ReadingTrackerWordResult word = new SonicInterfaces.ReadingTrackerWordResult ();
				word.text = wordRef.text;
				word.spoken = wordRef.spoken;
				word.startIndex = wordRef.char_index;
				word.length = wordRef.char_len;
				word.score = wordRef.score;
				word.passed = wordRef.accept;
				words.Add (word);
			}

			res.words = words;
		}

		return res;
	}

}