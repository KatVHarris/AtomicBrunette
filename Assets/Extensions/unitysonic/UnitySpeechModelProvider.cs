using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Rosettastone.Speech;

public class UnitySpeechModelProvider : SpeechModelProvider {
	protected string _configDir;
	protected Rosettastone.Speech.Logger _logger;

	public UnitySpeechModelProvider( Rosettastone.Speech.Logger logger ) : base("", logger) {
		_configDir = makeConfigDir();
		_logger = logger;
	}

	public override GetSpeechModelTask createPlatformGetSpeechModelTask (SpeechModelDescriptor descriptor) {
		UnityGetSpeechModelTask result = new UnityGetSpeechModelTask( _configDir, descriptor, _logger );
		return result;
	}
	
	private static string makeConfigDir() {
		string configDir= Application.temporaryCachePath + "/config/";
		Directory.CreateDirectory(configDir);
		return configDir;
	}
	
	public override string getConfigDir() {
		return _configDir;
	}
}