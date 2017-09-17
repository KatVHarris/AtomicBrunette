using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager; // this is the Singleton
    private int _level;
    private int _score;
    private int _voiceType;

    public GameObject MenuSystem;
    private void Awake()
    {
        gameManager = this;
    }

    // Use this for initialization
    void Start () {
        //StartCoroutine(ChangeScene("Level1"));
        _level = 0; 
        DontDestroyOnLoad(gameObject);
	}
	
	/// <summary>
    /// UPDATE THIS PART FOR ON TRIGGER ENTER() METHOD! 
    /// You will need a rigid body on the cubes - that are set to not use gravity and are static
    /// as well as a colider that has the trigger button set to on!
    /// </summary>

    public void LoadNewLevel(string name)
    {
        Debug.Log("Loading");
        SceneManager.LoadSceneAsync(name);
    }
    
}
