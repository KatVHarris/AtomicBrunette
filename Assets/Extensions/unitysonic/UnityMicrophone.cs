using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Rosettastone.Speech;

public class UnityMicrophone : AudioDevice {
	private bool _dispatchedStart = false;
	public List<float> _samples;
	public string _micName= null;
	public int _sampleRate= 16000;
	public MonoBehaviour behaviour;
	public bool isRecording = false;
	
	public UnityMicrophone( MonoBehaviour behaviour ) {
		this.behaviour = behaviour;
	}

	public void prepareToStart () {
		_samples= new List<float>();
		behaviour.StartCoroutine(RunMicrophone(new object[]{ 1, _samples }));
	}
	
	public override void start() {
		isRecording = true;
		_dispatchedStart = false;
	}
	
	public override void stop() {
		Microphone.End(_micName);
	}
	
	IEnumerator RunMicrophone(object[] parms) {
		int idxParm= 0;
		int clipSampleRate= _sampleRate;
		int clipSecs= (int)parms[idxParm++];
		List<float> buffer= (List<float>)parms[idxParm++];
		
		int lastMicPos= 0;
		int chunkSamples= (int)(0.05 * clipSampleRate);
		int clipSamples= clipSecs * clipSampleRate;
		buffer.Capacity= clipSamples;
		
		AudioClip clip= Microphone.Start(_micName, true, clipSecs, clipSampleRate);
		while (Microphone.IsRecording(_micName)) {
			int micPos= Microphone.GetPosition(_micName);

			if (micPos - lastMicPos > chunkSamples) {
				processSamples(clip, lastMicPos, micPos - lastMicPos, buffer );
				lastMicPos= micPos;
			} else 	if (micPos < lastMicPos) {
				// we've looped around the circular buffer
				processSamples(clip, lastMicPos, clipSamples - lastMicPos, buffer );
				processSamples(clip, 0, micPos+1, buffer );			
				lastMicPos= micPos;
			}
			
			yield return null;
		}
		isRecording = false;
		onStopped();
	}
	
	short[] floatSamplesToShortSamples(float[] fsamples) {
		short[] ssamples= new short[fsamples.Length];
		for (int idx= 0; idx < fsamples.Length; idx++) {
			ssamples[idx]= (short)(fsamples[idx] * short.MaxValue);
		}
		return ssamples;
	}
	
	void processSamples(AudioClip clip, int pos, int numSamples, List<float> buffer) {
		if( isRecording ) {
			if( !_dispatchedStart ) {
				_dispatchedStart = true;
				onStarted();
			}
			float[] fsamples= new float[numSamples];
			clip.GetData(fsamples, pos);
			buffer.AddRange(fsamples);
	
			//bool changed;
			short[] ssamples= floatSamplesToShortSamples(fsamples);
			onAudioData(ssamples, ssamples.Length);
		}
	}
}