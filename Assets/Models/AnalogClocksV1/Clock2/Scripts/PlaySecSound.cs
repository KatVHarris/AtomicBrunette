using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySecSound : MonoBehaviour {
    [SerializeField]
    private AudioClip soundTick, soundTak = null;
    private AudioSource aSource;
    private bool tick = true;

	// Use this for initialization
	void Start () {
        aSource = GetComponent<AudioSource>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (tick)
        {
            aSource.PlayOneShot(soundTick);
            tick = false;
        }else
        {
            aSource.PlayOneShot(soundTak);
            tick = true;
        }
    }
}
