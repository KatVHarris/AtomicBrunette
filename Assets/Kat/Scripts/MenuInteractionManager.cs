using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInteractionManager : MonoBehaviour {
	public string LEVEL_NAME;
    public GameObject StartMenu;

	void Start(){
		LEVEL_NAME = "ElSavador";
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered: "+ other.transform.name);
        
        if (other.tag == "Menu")
        {
            Debug.Log(other.transform.name);
            // check to see name then do stuff
            if(other.transform.name == "StartMenuQuad")
            {
                // Start Playing Dialog
                GameManager.gameManager.StartDialog(0);
                ///other.transform.gameObject.SetActive(false);
                StartMenu.SetActive(false);
            }

			if(other.transform.name == "MissionMenuQuad")
			{
				GameManager.gameManager.LoadLevelRequest (LEVEL_NAME);

			}
        }
    }
}
