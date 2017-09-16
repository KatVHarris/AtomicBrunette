using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class AssetHelper {
	// @see: http://docs.unity3d.com/Documentation/Manual/StreamingAssets.html
	public static string getAssetPath() {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return Application.dataPath + "/Raw/";
		} else if (Application.platform == RuntimePlatform.Android) {
			return Application.temporaryCachePath + "/assets/";
		} else if (Application.platform == RuntimePlatform.OSXPlayer) {
			return Application.dataPath + "/Data/StreamingAssets/";
		} else {
			return Application.dataPath + "/StreamingAssets/";
		}
	}

	public static string extractSingleAsset(string assetPath) {
		string extractedAssetPath= Path.Combine(getAssetPath(), assetPath);

		if (Application.platform == RuntimePlatform.Android) {
			string jarPath= Path.Combine("jar:file://" + Application.dataPath + "!/assets/", assetPath);
			
			string assetDir= Path.GetDirectoryName(extractedAssetPath);
			Directory.CreateDirectory(assetDir);
			
			LoggerFactory.makeLogger("AssetHelper").debug("extractSingleAsset", "extracting " + jarPath + " to " + extractedAssetPath);
			
			WWW unpacker= new WWW(jarPath);
			while (!unpacker.isDone) { }
			File.WriteAllBytes(extractedAssetPath, unpacker.bytes);
		}

		return extractedAssetPath;
	}
	
	// desktop/web doesn't support mp3. wtf??
	public static string getAudioExtension() {
		#if UNITY_IPHONE || UNITY_ANDROID
			return "mp3";
		#else
			return "ogg";
		#endif
	}
	
	public static AudioClip loadAudioClipAsset(string assetPath) {
		assetPath= "file://" + extractSingleAsset(assetPath);
		WWW www= new WWW(assetPath);
		return www.GetAudioClip();
	}
}

