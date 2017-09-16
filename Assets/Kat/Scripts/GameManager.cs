using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private int _level;
    private int _score;
    private int _voiceType;

    public GameObject MenuSystem; 

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Play animation i
    // - Animation i hasCompleted

    // change Scene
    void ChangeScene(int sceneNumber)
    {

    }


}
