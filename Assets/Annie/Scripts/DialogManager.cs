using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour {

    //public List<GameObject> speakers;
	private List<string> speakerOrder = new List<string> {"Prep","Operator","Prep","Operator","Player","Prep"};
	private List<string> failSpeakers = new List<string> { "PrepFail" };
	private bool isPlaying;
	private int currentSpeakerIndex;
	private int currentFailSpeakerIndex;
	// Use this for initialization
	void Start () {
		currentSpeakerIndex = 0;
		isPlaying = false;
	}
	
	// Update is called once per frame
	void Update () {
		string currentSpeakerName = speakerOrder [currentSpeakerIndex];
		if (isPlaying) {
			if (currentSpeakerName == "Player") {
				isPlaying = GameObject.Find (currentSpeakerName).GetComponent<SRE> ().isActive ();
				//isPlaying = GameObject.Find (currentSpeakerName).GetComponent<SREPhrase> ().isActive ();
			} else {
				isPlaying = GameObject.Find ("/Characters/" + speakerOrder [currentSpeakerIndex]).GetComponent<Dialog> ().isActive ();
			}

			if (!isPlaying) {
				currentSpeakerIndex += 1;
			}
			//Debug.Log (currentSpeakerIndex + "is playing");
		} else if (!isPlaying && (currentSpeakerIndex < speakerOrder.Count)) {
			Debug.Log ("index: " + currentSpeakerIndex);
			if (currentSpeakerName == "Player") {
				SRE playerSRE = GameObject.Find ("Player").GetComponent<SRE> ();
				//SREPhrase playerSRE = GameObject.Find ("Player").GetComponent<SREPhrase> ();
				playerSRE.Begin ();
			} else {
				Dialog speakerDia = GameObject.Find ("/Characters/" + speakerOrder [currentSpeakerIndex]).GetComponent<Dialog> ();
				speakerDia.PlayNext ();
				//Debug.Log ("got speaker");
				isPlaying = true;
				Debug.Log (currentSpeakerIndex + "started");
			}
		} else if (currentSpeakerIndex >= speakerOrder.Count) {
			Debug.Log ("done");
		}
	}
/*
    public List<GameObject> speakers;

    int sequence;

    private void Start()
    {
        sequence = 0;
    }

    void StartAllDialog()
    {
        StartClipfromSpeaker(sequence, "Investigator");
    }

    void FinishedClip(string speakerName, int currentSequence)
    {

    }

    void StartClipfromSpeaker(int index, string speakerName)
    {
        for (int i = 0; i < speakers.Count; i++)
        {
            if (speakers[i].transform.name == speakerName)
            {
                speakers[i].GetComponent<Dialog>().PlayAtIndex(sequence);
            }
        }
    }
*/
}
