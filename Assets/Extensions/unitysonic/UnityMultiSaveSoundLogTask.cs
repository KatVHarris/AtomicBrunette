using UnityEngine;
using System;
using System.IO;
using System.Text;
using Rosettastone.Speech;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class UnityMultiSaveSoundLogTask : SaveSoundLogTask {
	private List<SaveSoundLogTask> _saveSoundLogTasks = new List<SaveSoundLogTask>();
	public UnityMultiSaveSoundLogTask() : base() { }
	
	protected override void saveSoundLog ( string soundLog ) {
		Debug.Log("Multi Save sound log " + soundLog );
		foreach( SaveSoundLogTask task in _saveSoundLogTasks ) {
			task.soundLogToSave = this.soundLogToSave;
			task.run();
		}
		taskComplete();
	}
	
	public void addSaveSoundLogTask( SaveSoundLogTask task ) {
		_saveSoundLogTasks.Add( task );
	}
}
