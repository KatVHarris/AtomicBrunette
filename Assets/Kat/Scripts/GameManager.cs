using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager gameManager; // this is the Singleton
    private int _level;
    private int _score;
    private int _voiceType;
    public GameObject DialogManagerObject;
    DialogManager DialogManager;

    public GameObject StartMenu;
    public GameObject MissionMenu;
    private void Awake()
    {
        gameManager = this;
    }

    // Use this for initialization
    void Start()
    {
        //StartCoroutine(ChangeScene("Level1"));
        _level = 0;
        DontDestroyOnLoad(gameObject);
        DialogManager = DialogManagerObject.GetComponent<DialogManager>();
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

    public void StartDialog(int DialogIndex)
    {
        DialogManager.dialogManager.gamestarted = true;
    }

    public void LoadNextPhrase(int phraseNum)
    {
        //isPlaying = GameObject.Find ("/Canvas/" + speakerOrder [currentSpeakerIndex]).GetComponent<Dialog> ().isActive ();

        //load canvas thing 
        Debug.Log("GM phraseNum: " + phraseNum);
        GameObject.Find("/Dialogue/Canvas/" + phraseNum).SetActive(true);


    }

    public void UnloadPhrase(int phraseNum)
    {
        GameObject.Find("/Dialogue/Canvas/" + phraseNum).SetActive(false);
    }

    public void DoneWithDialogScene(int curDialogSequence)
    {
        if (curDialogSequence == 0)
        {
            ShowMissionAcceptScreen();
        }
    }

    public void ShowMissionAcceptScreen()
    {
        MissionMenu.SetActive(true);
    }

    public void LoadLevelRequest(string levelName)
    {
        LoadNewLevel(levelName);
    }
}
