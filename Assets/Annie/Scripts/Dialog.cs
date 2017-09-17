using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour {

	public AudioClip[] dialog;
    
	private int current;
	AudioSource audioSource;
	public string key;
    bool isPlayingAudio;
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
        isPlayingAudio = false;
		//dialog = GetComponents<AudioClip> ();
		//current = 0;
	}

	void PlayNext(){
		audioSource.PlayOneShot (dialog [current]);
		current = (current + 1) % dialog.Length;
	}

	public void PlayAtIndex(int index){
        isPlayingAudio = true;
		audioSource.PlayOneShot (dialog [index]);
	}



	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (key)) {
			print ("space key was pressed");	
			PlayNext ();
		}

        if (isPlayingAudio)
        {

        }
	}
}
