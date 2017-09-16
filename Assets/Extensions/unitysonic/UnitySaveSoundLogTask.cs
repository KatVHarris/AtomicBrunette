using UnityEngine;
using System;
using System.IO;
using System.Text;
using Rosettastone.Speech;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class UnitySaveSoundLogTask : SaveSoundLogTask {
	protected string _deviceIdentifier;
	public UnitySaveSoundLogTask( string deviceIdentifier ) : base() {
		_deviceIdentifier = deviceIdentifier;
	}
	protected override void saveSoundLog ( string soundLog ) {
		//TODO delete old sound logs?
		try {
			string baseDir = Application.persistentDataPath + "/soundLogs";
			Directory.CreateDirectory(baseDir);
			string filename = baseDir + "/" + _deviceIdentifier + "." + DateTime.Now.ToOADate ().ToString () + ".xml";
			Debug.Log("Saving sound log to " + filename );
			FileStream fileStream = new FileStream( filename, FileMode.Create, FileAccess.Write, FileShare.Read);
			StreamWriter streamWriter = new StreamWriter( fileStream );
			streamWriter.Write ( soundLog );
			streamWriter.Flush();
			streamWriter.Close();
			fileStream.Close();
		} catch {
			taskError("error saving sound log");
			return;
		}
		taskComplete();
	}
}