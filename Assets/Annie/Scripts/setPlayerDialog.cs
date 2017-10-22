using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections;

public class setPlayerDialog : MonoBehaviour
{
	public setPlayerDialog ()
	{
	}

	private bool Load(string filename){
		try {
			string line;
			StreamReader sr = new StreamReader (filename, Encoding.Default);
			using (sr) {
				do {
					line = sr.ReadLine ();

					if (line != null) {
						// Parse to add to canvas
					}

				} while (line != null);
				sr.Close ();
				return true;
			}
		} catch (Exception e) {
			Debug.LogException(e, this);
		}
		return false;
	}
}


