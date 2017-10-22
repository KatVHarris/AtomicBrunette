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
	int offset;
	int coloredIndex;
	Text phraseComponent;

	// full phrase
	string phrase;
	// phrase broken by space
	public List<string> lineStringList;
	//public List<Text> lineTextList;

	// maps start character in the original phrase string to the word 
	private Dictionary<int, string> indexedLineStringDict = new Dictionary<int, string>() ;

	public GUIText text;

	private Color highlightColor = Color.yellow;
	private Color warningColor = Color.yellow;
	private Color errorColor = Color.red;
	private Color unspokenColor = Color.gray;

	private bool activeSession = true;
	private bool isPlaying = false;

	public SonicInterfaces.Language language = SonicInterfaces.Language.SPANISH;
	public SonicInterfaces.VoiceType voiceType = SonicInterfaces.VoiceType.FEMALE;
	public SonicInterfaces.Difficulty difficulty = SonicInterfaces.Difficulty.Level1;

	public int currentSequence;

	//TextMutator mutator;

	public bool isActive(){
		return isPlaying;
	}

	void Start(){
		offset = 0;
		currentSequence = 0;
		coloredIndex = -1;
		/*int indexCount = 0;
		for (int i = 0; i < lineTextList.Count; ++i) {
			Debug.Log ("index: " + indexCount);
			indexedLineTextDict.Add (indexCount, lineTextList [i]);
			indexCount += lineTextList [i].text.Length+1;
		}*/
	}

	private void RefreshDict(){
		indexedLineStringDict.Clear ();
		int indexCount = 0;
		for (int i = 0; i < lineStringList.Count; ++i) {
			Debug.Log ("index: " + indexCount);
			indexedLineStringDict.Add (indexCount, lineStringList [i]);
			indexCount += lineStringList [i].Length+1;
		}
	}

	public void LoadNextPhrase(int phraseNum){
		coloredIndex = -1;
		offset = 0;
		Text[] phrases;
        Debug.Log("pharseNum " + phraseNum);
		phrases = GameObject.Find("/Dialogue/Canvas/"+phraseNum).GetComponentsInChildren<Text>();
		lineStringList.Clear ();
		phraseComponent = phrases [0];
		phrase = phrases [0].text;
		lineStringList.AddRange(phrase.Split(' '));
		//lineStringList.InsertRange (0, words);
		foreach (string word in lineStringList) {
			//phrase  
			Debug.Log ("word in load: " + word);
		}
		RefreshDict ();
		
	}

	void Concat(string CurrentText, string newText){
		// spaces
	}

	string PhraseAsString(){
		string phrase = "";
		for(int i=0; i<lineStringList.Count; ++i) {
			//Debug.Log ("loop " + lineTextList[i].text);
			phrase += lineStringList[i];
			phrase += " ";
		}
		//Debug.Log("Concate phrase: "+phrase);
		return phrase;
	}

	void Awake () {
		//Initialize for desired language, voice type, difficulty and callback delegate (callback delegate = this).
		SonicSREImpl.Instance.configure(language, voiceType,  difficulty, this);
		//And object that handles the manipulation of our display string
		//mutator = new TextMutator(text.text);
	}

	public void Begin(){
		string phrase = PhraseAsString ();
		Debug.Log ("phrase: " + phrase);
		SonicSREImpl.Instance.readingTracker(phrase, this);
		Debug.Log("ready");
		isPlaying = true;
	}

	void Update(){
		if (Input.GetKeyDown("space") && !activeSession ) {
			// change color
			// lineTextList [0].color = Color.red;
			string phrase = PhraseAsString();
			Debug.Log("Update: phrase= "+phrase);
			//SonicSREImpl.Instance.
			SonicSREImpl.Instance.readingTracker(phrase, this);
            Debug.Log("ready");
		}
	}

	void OnMouseDown() {
		//You can call interrupt if you wish to interrupt the session
		//SonicSREImpl.Instance.interrupt();
		//if (!activeSession) {
		//	//Reset text and start the Reading Tracker.
		//	text.text = mutator.washText();
		//	SonicSREImpl.Instance.readingTracker(text.text, this);
		//}
	}

	#region ReadingTracker

	/// <summary>
	/// Called when the Reading Tracker session begins.
	/// </summary>
	void SonicInterfaces.ReadingTrackerCallback.onReadingTrackerStart () {
		Debug.Log ("activating");
		activeSession = true;
	}

	private void colorWord(int wordStartIndex, int wordLength, string color){
		Debug.Log ("Coloring");
		//if (updateResult.wordStartIndex == 0) {
		string openTag = "<color=" + color + ">";
		phraseComponent.text = phraseComponent.text.Insert (wordStartIndex + offset, openTag);
		Debug.Log ("after open tag phrase: " + phraseComponent.text + " length: " + phraseComponent.text.Length);
		offset += openTag.Length;
		//}
		string closeTag = "</color>";
		Debug.Log ("index: " + wordStartIndex + " length: " + wordLength + " offset: " + offset);
		phraseComponent.text = phraseComponent.text.Insert (wordStartIndex + wordLength + offset, closeTag);
		offset += closeTag.Length;
	}

	/// <summary>
	/// Called when the Reading Tracker Updates. Microphone energy can be used to display audio input to the user. 
	/// Current word index can be found in the Update Result.
	/// </summary>
	/// <param name="energy">Energy.</param>
	/// <param name="updateResult">Update result.</param>
	void SonicInterfaces.ReadingTrackerCallback.onReadingTrackerUpdate (float energy, SonicInterfaces.ReadingTrackerUpdateResult updateResult) {
		Debug.Log ("Reading tracker update word start index: "+updateResult.wordStartIndex);

		//Debug.Log (updateResult.wordStartIndex);
		//Debug.Log ("length: " + updateResult.wordLength);

		//Text phrase = GameObject.Find("/Dialog/Canvas/"+phraseNum).GetComponentsInChildren<Text>()[0];
		//if first word, put the open tag
		if( coloredIndex < updateResult.wordStartIndex){
			colorWord (updateResult.wordStartIndex,updateResult.wordLength,"yellow");
			/*
			Debug.Log ("Coloring");
			//if (updateResult.wordStartIndex == 0) {
			string openTag = "<color=yellow>";
			phraseComponent.text = phraseComponent.text.Insert (updateResult.wordStartIndex+offset, "<color=yellow>");
			Debug.Log ("after open tag phrase: " + phraseComponent.text + " length: "+phraseComponent.text.Length);
			offset += openTag.Length;
			//}
			string closeTag = "</color>";
			Debug.Log ("index: " + updateResult.wordStartIndex + " length: " + updateResult.wordLength + " offset: " + offset);
			phraseComponent.text = phraseComponent.text.Insert (updateResult.wordStartIndex + updateResult.wordLength + offset, closeTag);
			offset += closeTag.Length;*/
		}
		coloredIndex = updateResult.wordStartIndex;
		//indexedLineStringDict [updateResult.wordStartIndex].color = Color.yellow;
		//mutator.washText();
		//text.text = mutator.highlight(updateResult.wordStartIndex, updateResult.wordLength, highlightColor);
	}

	/// <summary>
	/// Called when the Reading Tracker completes. Word by word scoring can be found within the result.
	/// </summary>
	/// <param name="result">Result.</param>
	void SonicInterfaces.ReadingTrackerCallback.onReadingTrackerComplete (SonicInterfaces.ReadingTrackerResult result) {
		Debug.Log ("Complete");
		SonicInterfaces.ReadingTrackerScore score = result.score;


		phraseComponent.text = phrase;
		coloredIndex = -1;
		offset = 0;

		foreach(SonicInterfaces.ReadingTrackerWordResult word in result.words) {
			Debug.Log ("word: "+word.text +" score: "+ word.score + "index: "+word.startIndex);
			if (0 == word.spoken) {
				Debug.Log ("0: " + word.text);
				colorWord (word.startIndex, word.length, "red");
			}
			//This score can be between -1 (unspoken) and 10 (perfect). You can define your threshold as needed.
			else if (2 > word.score) {
				Debug.Log ("2: " + word.text);
				colorWord (word.startIndex, word.length, "cyan");
			} else if (4 > word.score) {
				Debug.Log ("4: " + word.text);
				colorWord (word.startIndex, word.length, "teal");
				//indexedLineTextDict [word.startIndex].color = Color.green;
				//mutator.highlight(word.startIndex, word.length, warningColor);
			} else {
				Debug.Log (">4: " + word.text);
				colorWord (word.startIndex, word.length, "green");
				//indexedLineTextDict [word.startIndex].color = Color.cyan;
			}
		}

		DialogManager.dialogManager.SREDone (currentSequence, score.final);
		//mutator.washText();
		/*
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
		
		}*/

		isPlaying = false;
		SonicSREImpl.Instance.interrupt();

		//text.text = mutator.currentText();
		Debug.Log("over");
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


