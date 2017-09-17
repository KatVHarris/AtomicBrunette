using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour {
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
}
