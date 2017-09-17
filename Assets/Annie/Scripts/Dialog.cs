using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour {

	public AudioClip[] dialog;
	private int current;
	AudioSource audioSource;
	public string key;
	public string id;
	private bool hasNext;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		//dialog = GetComponents<AudioClip> ();
		current = 0;
		hasNext = (current < dialog.Length);
	}
		

	public bool isActive(){
		return audioSource.isPlaying;
	}

	public void PlayNext(){
		if (hasNext) {
			audioSource.PlayOneShot (dialog [current]);
			current = (current + 1);
			hasNext = current < dialog.Length;
		} else {
			Debug.Log ("does not have next clip");
		}

	}

	public void PlayAtIndex(int index){
		audioSource.PlayOneShot (dialog [index]);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (key)) {
			print ("space key was pressed");	
			PlayNext ();
		}
	}
}
