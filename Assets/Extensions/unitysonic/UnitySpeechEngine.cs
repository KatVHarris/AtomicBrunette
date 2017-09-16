using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rosettastone.Speech;
using System.IO;
using System;

namespace Rosettastone.Speech {

public class UnitySpeechEngine : SpeechEngine {
	protected AudioClip _promptSound;
	protected static AudioDevice sharedMic;
	public AudioDevice unityMic;
	protected AudioEnumerator _enumerator;

	protected Logger _logger;
	protected UnitySpeechModelProvider _provider;
	protected SpeechSession _lastActiveSession;
	private bool _saveSoundLogsToWS = false;
	private string _soundLogWSBaseURL;
	protected string _deviceIdentifier;
	
	public UnitySpeechEngine( bool enableSoundLogging, string deviceIdentifier,
			Logger logger = null ) : base(null, logger, enableSoundLogging) {
		_deviceIdentifier = deviceIdentifier;
		_promptSound = AssetHelper.loadAudioClipAsset("SpeechPrompt." + AssetHelper.getAudioExtension());
		
		if( logger != null ) {
			_logger = logger;
		} else {
			_logger= LoggerFactory.makeLogger("sre");
		}
		setLogger(_logger);
		
		if( sharedMic == null ) {
			AudioFactory factory = new AudioFactory();
	        _enumerator = factory.makePlatformAudioEnumerator();
			sharedMic = _enumerator.openInputDevice( _enumerator.getDefaultInputDeviceID() );
			sharedMic.swigCMemOwn = true;
		}
		unityMic = sharedMic;
		
		setMicrophone( unityMic );
		setAudioEnumerator( _enumerator );
		
		_provider= new UnitySpeechModelProvider(_logger);
		setSpeechModelProvider(_provider);
		setSpeakerAdaptation( false );
	}
		
	public void saveSoundLogsToWebService( string baseURL ) {
		_soundLogWSBaseURL = baseURL;
		_saveSoundLogsToWS = true;
	}
	
	public override void activateSession( SpeechSession session ) {
		if( _lastActiveSession == null || 
				_lastActiveSession.getState() == Task.TaskState.ERRORED ||
				_lastActiveSession.getState() == Task.TaskState.INTERRUPTED ||
				_lastActiveSession.getState() == Task.TaskState.SUCCEEDED ) {
			activeSession = session;
			// Save a reference to the last active session in case the client
			// isn't holding onto it. This ensures that the session doesn't get freed
			// before it's finished executing
			_lastActiveSession = SonicObjectCache.lookUpInstanceNoError<SpeechSession>( session.getSWIGCPtrSpeechSession().Handle );
		}
	}

	public override Logger getLogger() {
		return _logger;
	}
	
	public override AudioDevice getMicrophone() {
		return unityMic;
	}
		
	protected override PlaySoundTask createPlatformPlaySoundTask (SoundFragment sound) {
		return new UnityPlaySoundTask( UnitySonic.Instance, sound, getLogger() );
	}
	
	protected override PlaySoundTask createPlatformDefaultPlaySoundTask () {
		return new UnityPlayClipTask( UnitySonic.Instance, _promptSound, getLogger() );
	}
	
	protected override TimerTask createPlatformTimer () {
		return new UnityTimerTask( getLogger() );
	}
	
	protected override SaveSoundLogTask createPlatformSaveSoundLogTask () {
		if( _saveSoundLogsToWS ) {
			UnityMultiSaveSoundLogTask saveTask = new UnityMultiSaveSoundLogTask();
			saveTask.addSaveSoundLogTask( new UnitySaveSoundLogToWebServiceTask( _soundLogWSBaseURL, UnitySonic.Instance, _deviceIdentifier ) );
			saveTask.addSaveSoundLogTask( new UnitySaveSoundLogTask( _deviceIdentifier ) );
			return saveTask;
		} else {
			return new UnitySaveSoundLogTask( _deviceIdentifier );
		}
	}
	
	public override SpeechModelProvider getSpeechModelProvider () {
		return _provider;
	}
		
	/**
	 * Session factory methods
	 */
	
	public override ListenForPhrasesSession createListenForPhrasesSession (StdVectorOfStdString responses) {
		return new ListenForPhrasesSession( this, responses );
	}
	
	public override ListenForKeyphrasesSession createListenForKeyphrasesSession (StdVectorOfStdString keyphrases) {
		return new ListenForKeyphrasesSession( this, keyphrases );
	}
	
	public override OpenEndedSession createOpenEndedSession (StdVectorOfStdString phrases) {
		return new OpenEndedSession( this, phrases );
	}
	
	public override OpenEndedSubwordsSession createOpenEndedSubwordsSession (string xml) {
		return new OpenEndedSubwordsSession( this, xml );
	}
	
	public override GrammarSession createGrammarSession (string grammar) {
		return new GrammarSession( this, grammar );
	}
	
	public override VadSession createVadSession () {
		return new VadSession( this );
	}
		
	public override ReadingTrackerSession createReadingTrackerSession (string text, bool enableGrammar) {
		return new ReadingTrackerListenSession( this, text, enableGrammar );
	}
		
	public override ReadingTrackerPlaybackSession createReadingTrackerPlaybackSession (string text, SoundObject audio, Hypothesis hyp) {
		return new ReadingTrackerPlaybackSession( this, text, audio, hyp );
	}
		
	public override ReadingTrackerPlaybackSession createReadingTrackerPlaybackSession (string text, string url, Hypothesis hyp) {
		throw new ArgumentException("Playing back URLs is not supported in Unity");
	}
		
	public override CalibrateSession createCalibrateSession () {
		return new CalibrateSession( this );
	}
	
	public override LoadModelSession createLoadModelSession (SpeechModelDescriptor modelDescriptor, int difficulty) {
		return new LoadModelSession( this, modelDescriptor, difficulty );
	}

	public override PlaySoundSession createPlaySoundSession (SoundFragment soundFragment) {
		return new PlaySoundSession( this, soundFragment );
	}
	
	public override PlaySoundSession createPlaySoundSession (SoundObject soundObject) {
		return new PlaySoundSession( this, soundObject );
	}
	
	public override PlaySoundSession createPlaySoundSession (ListenSession listenSession) {
		return new PlaySoundSession( this, listenSession );
	}
		
	public override CheckMicAccessSession createCheckMicAccessSession () {
		return new CheckMicAccessSession( this );
	}
}
}
