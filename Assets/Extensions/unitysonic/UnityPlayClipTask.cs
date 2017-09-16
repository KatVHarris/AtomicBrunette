using UnityEngine;
using System.Collections;
using Rosettastone.Speech;

public class UnityPlayClipTask : PlaySoundTask {
	protected AudioClip _clip;
	protected MonoBehaviour _behaviour;
	public UnityPlayClipTask( MonoBehaviour behaviour, AudioClip clip, 
		Rosettastone.Speech.Logger logger, string identifier = "" ) : base(null, logger, identifier) {
		_behaviour = behaviour;
		_clip = clip;
	}
	
	protected override void startPlaying() {
		_behaviour.GetComponent<AudioSource>().clip = _clip;
		_behaviour.StartCoroutine( PlaySound () );
	}
	
	public override bool interrupt () {
		setState(Task.TaskState.INTERRUPTING);
		_behaviour.GetComponent<AudioSource>().Stop();
		return true;
	}
	
	IEnumerator PlaySound() {
		_behaviour.GetComponent<AudioSource>().Play();
		while( _behaviour.GetComponent<AudioSource>().isPlaying ) {
			yield return null;
		}
		if( getState() == Task.TaskState.INTERRUPTING ) {
			taskInterrupt();
		}
		else taskComplete();
	}
}
