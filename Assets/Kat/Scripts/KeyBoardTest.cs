using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardTest : MonoBehaviour {

    public void Update()
    {
        // THIS LOADS MAIN!
        if (Input.GetKeyUp(KeyCode.T))
        {
            GameManager.gameManager.LoadNewLevel("Main");
        }

        // THIS LOADS LEVEL1
        if (Input.GetKeyUp(KeyCode.R))
        {
            //ll.LoadLevel("Level1");
            GameManager.gameManager.LoadNewLevel("Level1");
        }
    }
}
