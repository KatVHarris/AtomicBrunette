using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rosettastone.Speech;

namespace Rosettastone.Speech {

public class UnitySonic : MonoBehaviour {
    /*
     * STATIC METHODS AND PROPERTIES
     */
     
    private static UnitySonic _instance;
    private static bool _isInitialized;
    
    public static UnitySonic Instance {
        get {
            Initialize();
            return _instance;
        }
    }
    
    public static void Initialize() {
        if(!_isInitialized) {
            if(!Application.isPlaying) {
                return;
            }
            
            _isInitialized = true;
            
            // Create a game object in the world that will be responsible for Sonic
            GameObject holder = new GameObject("UnitySonic");
            _instance = holder.AddComponent<UnitySonic>();
			holder.AddComponent<AudioSource>(); // add an audio source
        }
    }
    
    public static void RunOnMainThread(IRunnable runnable) {
        lock(Instance._runnables) {
            Instance._runnables.Add(runnable);
        }
    }

    /*
     * INSTANCE METHODS AND PROPERTIES
     */

#if UNITY_EDITOR
    private UnityRunOnThreadUtil _asrThreadUtil;
#else
    private RunOnCppThreadUtil _asrThreadUtil;
#endif

	private UnityRunOnMainUtil _mainThreadUtil;
    private List<IRunnable> _runnables = new List<IRunnable>();
    private List<IRunnable> _currentRunnables = new List<IRunnable>();
    
    public UnitySpeechEngine createSpeechEngine(bool enableSoundLogging, string deviceIdentifier, Logger logger = null) {
        UnitySpeechEngine sre = new UnitySpeechEngine(enableSoundLogging, deviceIdentifier, logger);

    #if UNITY_EDITOR
        _asrThreadUtil = new UnityRunOnThreadUtil();
    #else
        _asrThreadUtil = new RunOnCppThreadUtil();
    #endif
        sre.setAsrThreadUtil(_asrThreadUtil);
        
        _mainThreadUtil = new UnityRunOnMainUtil();
        sre.setMainThreadUtil(_mainThreadUtil);
        
        return sre;
    }
     
    /*
     * MONOBEHAVIOUR METHODS
     */
    void Awake() {
		_instance = this;
		_isInitialized = true;
	}
	
	void Update() {
	    lock(_runnables) {
	        _currentRunnables.Clear();
	        _currentRunnables.AddRange(_runnables);
	        _runnables.Clear();
	    }
	    
	    foreach(IRunnable runnable in _currentRunnables) {
			runnable.swigCMemOwn = true; // take ownership
	        runnable.run();
	        runnable.Dispose();
	    }
	}
}

}