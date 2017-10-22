using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour {
	public static DialogManager dialogManager;
    //public List<GameObject> speakers;
	private List<string> speakerOrder = new List<string> {"Prep","Operator","Prep","Pre","Player","Response", "Pre","Player","Response" };
	private List<string> failSpeakers = new List<string> { "PrepFail" };
	public int phraseNum;
	private bool isPlaying;
	private int currentSpeakerIndex;
	private int currentFailSpeakerIndex;
	// Use this for initialization
	public bool gamestarted;
	void Awake(){
		dialogManager = this;
	}
	void Start () {
		phraseNum=0;
		currentSpeakerIndex = 0;
		isPlaying = false;
	}

	public void SREDone(int sequence, float score){
		currentSpeakerIndex++;
		Debug.Log ("index after SREdone: " + currentSpeakerIndex);
		isPlaying = false;
		//GameManager.gameManager.UnloadPhrase (phraseNum);
		//phraseNum++;
	}

	// Update is called once per frame
	void Update () {
		if (gamestarted) {
			if (currentSpeakerIndex >= speakerOrder.Count) {
				Debug.Log ("Update: index at done: " + currentSpeakerIndex);
				// Create start mission Menu
				Debug.Log ("Update: done");

				GameManager.gameManager.DoneWithDialogScene (0);


			} else {
				string currentSpeakerName = speakerOrder [currentSpeakerIndex];
				// If someone is speaking, check if they're done
				if (isPlaying) {
					if (currentSpeakerName == "Player") {
						isPlaying = GameObject.Find (currentSpeakerName).GetComponent<SRE> ().isActive ();
						//isPlaying = GameObject.Find (currentSpeakerName).GetComponent<SREPhrase> ().isActive ();
					} else {
						isPlaying = GameObject.Find ("/Characters/" + speakerOrder [currentSpeakerIndex]).GetComponent<Dialog> ().isActive ();
						if (!isPlaying && currentSpeakerName == "Response") {
							GameManager.gameManager.UnloadPhrase (phraseNum);
							phraseNum++;
						}
					}

					if (!isPlaying) {
						Debug.Log ("update: is not playing: " + currentSpeakerIndex);
						currentSpeakerIndex += 1;
					}
					//Debug.Log (currentSpeakerIndex + "is playing");
				} 
				// if current is done, start the next one
				else if (!isPlaying && (currentSpeakerIndex < speakerOrder.Count)) {
					Debug.Log ("update: currrent speaker index: " + currentSpeakerIndex);
					if (currentSpeakerName == "Player") {
						Debug.Log ("update: is player");
						SRE playerSRE = GameObject.Find ("Player").GetComponent<SRE> ();
						//SREPhrase playerSRE = GameObject.Find ("Player").GetComponent<SREPhrase> ();
						//Update phrase prompt
						//GameManager.gameManager.LoadNextPhrase(phraseNum);
						playerSRE.LoadNextPhrase (phraseNum);
						Debug.Log ("loading phrase");
						playerSRE.Begin ();
						isPlaying = true;
					}
					else {
						Dialog speakerDia = GameObject.Find ("/Characters/" + speakerOrder [currentSpeakerIndex]).GetComponent<Dialog> ();
						speakerDia.PlayNext ();
						//Debug.Log ("got speaker");
						isPlaying = true;
						Debug.Log (currentSpeakerIndex + " started-Update");
					} 

					if (currentSpeakerName == "Pre") {
						GameManager.gameManager.LoadNextPhrase(phraseNum);
					}
				} 
			}
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
