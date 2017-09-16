using UnityEngine;
using System;
using System.IO;
using System.Text;
using Rosettastone.Speech;
using System.Collections;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class UnitySaveSoundLogToWebServiceTask : SaveSoundLogTask {
	private string _baseURL;
	protected string _deviceIdentifier;
	MonoBehaviour _behaviour;
	
	public UnitySaveSoundLogToWebServiceTask(
		string baseURL,
		MonoBehaviour behaviour,
		string deviceIdentifier ) : base() {
		_baseURL = baseURL;
		_behaviour = behaviour;
		_deviceIdentifier = deviceIdentifier;
	}
	
	protected override void saveSoundLog ( string soundLog ) {
		postSoundLogToWS( soundLog );
		// Fire-and-forget
		taskComplete();
	}
	
	protected void postSoundLogToWS( string soundLog ) {
		Debug.Log("Saving sound log to web service");
        string url = _baseURL + "/save_sound_log";
        WWWForm form = new WWWForm();
        form.AddField("xml", soundLog);
		form.AddField("device_id", _deviceIdentifier );
        WWW www = new WWW(url, form);
        _behaviour.StartCoroutine(WaitForRequest(www));
    }
 
    IEnumerator WaitForRequest(WWW www) {
        yield return www;
        // check for errors
        if (www.error == null) {
			Debug.Log("Saved sound log");
        } else {
			Debug.Log("Failed to save sound log: " + www.error);
        }
    }
}