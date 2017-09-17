using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Documentation:
// https://speech.rosettastone.com/vr_hackathon/

/// <summary>
/// An Example of how to configure and use the SRE for a Reading Tracker Session. Setup the language, voice type, difficulty and text in the Editor. Click on the text at runtime to execute the tracker.
/// </summary>
public class SRE : MonoBehaviour, SonicInterfaces.ConfigurationCallback, SonicInterfaces.ReadingTrackerCallback {

	public List<Text> lineTextList;

	private Dictionary<int, Text> indexedLineTextDict = new Dictionary<int, Text>() ;

	public GUIText text;

	private Color highlightColor = Color.yellow;
	private Color warningColor = Color.yellow;
	private Color errorColor = Color.red;
	private Color unspokenColor = Color.gray;

	private bool activeSession = true;

	public SonicInterfaces.Language language = SonicInterfaces.Language.SPANISH;
	public SonicInterfaces.VoiceType voiceType = SonicInterfaces.VoiceType.FEMALE;
	public SonicInterfaces.Difficulty difficulty = SonicInterfaces.Difficulty.Level1;

	TextMutator mutator;

	void Start(){
		int indexCount = 0;
		for (int i = 0; i < lineTextList.Count; ++i) {
			Debug.Log ("index: " + indexCount);
			indexedLineTextDict.Add (indexCount, lineTextList [i]);
			indexCount += lineTextList [i].text.Length+1;
		}
	}

	void Concat(string CurrentText, string newText){
		// spaces
	}

	string PhraseAsString(){
		string phrase = "";
		for(int i=0; i<lineTextList.Count; ++i) {
			Debug.Log ("loop " + lineTextList[i].text);
			phrase += lineTextList[i].text;
			phrase += " ";
		}
		Debug.Log("Concate phrase: "+phrase);
		return phrase;
	}

	void Awake () {
		//Initialize for desired language, voice type, difficulty and callback delegate (callback delegate = this).
		SonicSREImpl.Instance.configure(language, voiceType,  difficulty, this);
		//And object that handles the manipulation of our display string
		//mutator = new TextMutator(text.text);
	}

	void Update(){
		if (Input.GetKeyDown("space") && !activeSession ) {
			// change color
			// lineTextList [0].color = Color.red;
			SonicSREImpl.Instance.readingTracker(PhraseAsString(), this);
		}
	}

	void OnMouseDown() {
		//You can call interrupt if you wish to interrupt the session
		//SonicSREImpl.Instance.interrupt();
		if (!activeSession) {
			//Reset text and start the Reading Tracker.
			text.text = mutator.washText();
			SonicSREImpl.Instance.readingTracker(text.text, this);
		}
	}

	#region ReadingTracker

	/// <summary>
	/// Called when the Reading Tracker session begins.
	/// </summary>
	void SonicInterfaces.ReadingTrackerCallback.onReadingTrackerStart () {

		activeSession = true;
	}

	/// <summary>
	/// Called when the Reading Tracker Updates. Microphone energy can be used to display audio input to the user. 
	/// Current word index can be found in the Update Result.
	/// </summary>
	/// <param name="energy">Energy.</param>
	/// <param name="updateResult">Update result.</param>
	void SonicInterfaces.ReadingTrackerCallback.onReadingTrackerUpdate (float energy, SonicInterfaces.ReadingTrackerUpdateResult updateResult) {
		Debug.Log ("Update");
		indexedLineTextDict [updateResult.wordStartIndex].color = Color.yellow;
		//mutator.washText();
		//text.text = mutator.highlight(updateResult.wordStartIndex, updateResult.wordLength, highlightColor);
	}

	/// <summary>
	/// Called when the Reading Tracker completes. Word by word scoring can be found within the result.
	/// </summary>
	/// <param name="result">Result.</param>
	void SonicInterfaces.ReadingTrackerCallback.onReadingTrackerComplete (SonicInterfaces.ReadingTrackerResult result) {
		Debug.Log ("Complete");
		//mutator.washText();

		//Since we are inserting, appending and otherwise adjusting the length of our display string: work in reverse to preserve indices.
		List<SonicInterfaces.ReadingTrackerWordResult> reversedWords = new List<SonicInterfaces.ReadingTrackerWordResult>(result.words); 
		reversedWords.Reverse();
		Debug.Log ("num words: " + reversedWords.Count);
		foreach(SonicInterfaces.ReadingTrackerWordResult word in reversedWords) {
			Debug.Log ("word: "+word.text +" score: "+ word.score + "index: "+word.startIndex);
			if (0 == word.spoken) {
				Debug.Log ("0: " + word.text);
				indexedLineTextDict [word.startIndex].color = Color.red;
				//mutator.highlight(word.startIndex, word.length, unspokenColor);
			}
			//This score can be between -1 (unspoken) and 10 (perfect). You can define your threshold as needed.
			else if (2 > word.score) {
				Debug.Log ("2: " + word.text);
				Debug.Log (indexedLineTextDict [word.startIndex]);
				indexedLineTextDict [word.startIndex].color = Color.blue;
				//mutator.highlight(word.startIndex, word.length, errorColor);
			} else if (4 > word.score) {
				Debug.Log ("4: " + word.text);
				indexedLineTextDict [word.startIndex].color = Color.green;
				//mutator.highlight(word.startIndex, word.length, warningColor);
			} else {
				Debug.Log (">4: " + word.text);
				indexedLineTextDict [word.startIndex].color = Color.cyan;
			}
		
		}

		//text.text = mutator.currentText();

		activeSession = false;
	}

	void SonicInterfaces.ReadingTrackerCallback.onReadingTrackerError (string message) {
		Debug.Log(message);

		activeSession = false;
	}

	#endregion

	#region Configuration

	void SonicInterfaces.ConfigurationCallback.onConfigureStart () {
		activeSession = true;
		Debug.Log("Configuration Begin");
	}

	void SonicInterfaces.ConfigurationCallback.onConfigureUpdate (float progress) {
		Debug.Log("Configuration: " + progress.ToString());
	}

	void SonicInterfaces.ConfigurationCallback.onConfigureComplete () {
		activeSession = false;
		Debug.Log("Configuration Complete. Select text to begin.");
	}

	void SonicInterfaces.ConfigurationCallback.onConfigureError (string message) {
		Debug.Log(message);
	}

	#endregion
}


