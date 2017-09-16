using UnityEngine;
using System.Collections;
using Rosettastone.Speech;

public class UnityPlaySoundObjectTask : PlaySoundTask {
	protected AudioClip _clip;
	protected MonoBehaviour _behaviour;
	SoundObject _soundObject;
	
	public UnityPlaySoundObjectTask(SoundObject soundObject, 
		MonoBehaviour behaviour, Rosettastone.Speech.Logger logger, string identifier): base(null, logger, identifier ) {
		_soundObject = soundObject;
		_behaviour = behaviour;
	}
		
	//TODO this class is heavily copied from UnityPlaySoundTask
	protected override void startPlaying() {
		try {
			SoundFragment soundFragment = _soundObject.getDataReadOnly();
			uint numSamples = soundFragment.getSizeInSamples();
			if( numSamples > 0 ) {
				short[] samples = new short[ numSamples ];
				soundFragment.getSamples( samples, (uint)samples.Length );
				_clip = bufferToAudioClip( samples, soundFragment.getFormat().sampleRate() );
				_behaviour.GetComponent<AudioSource>().clip = _clip;
				_behaviour.StartCoroutine( PlaySound () );
			} else {
				taskError("No samples to play (samples=" + numSamples + ")");
			}
		} catch {
			taskError("problem in startPlaying()");
		}
	}
	IEnumerator PlaySound() {
		_behaviour.GetComponent<AudioSource>().Play();
		while( _behaviour.GetComponent<AudioSource>().isPlaying ) {
			yield return null;
		}
		if( getState() == Task.TaskState.INTERRUPTING ) {
			taskInterrupt();
		} else { 
			taskComplete();
		}
	}
	public override bool interrupt() {
		setState(Task.TaskState.INTERRUPTING);
		_behaviour.GetComponent<AudioSource>().Stop();
		return true;
	}

	AudioClip bufferToAudioClip(short[] ssamples, int sampleRate) {
		AudioClip clip= AudioClip.Create("recording", ssamples.Length, 1, sampleRate, false, false);
		clip.SetData( shortSamplesToFloatSamples( ssamples ), 0);
		return clip;
	}
	
	float[] shortSamplesToFloatSamples(short[] ssamples) {
		float[] fsamples= new float[ssamples.Length];
		for (int idx= 0; idx < ssamples.Length; idx++) {
			fsamples[idx]= ((float)ssamples[idx] / short.MaxValue);
		}
		return fsamples;
	}
}
