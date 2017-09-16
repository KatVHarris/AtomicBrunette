using System;
using System.Text;
using Rosettastone.Speech;
using UnityEngine;
using System.IO;

public class UnityLogger : MultiLogger {
	private Rosettastone.Speech.Logger _mainLogger;
	private Rosettastone.Speech.Logger _fileLogger;
	private Rosettastone.Speech.Logger _strLogger;

	protected StringBuilder _logText= new StringBuilder();
	public override string ToString() { return _logText.ToString(); }
	public void truncate() { _logText.Length= 0; }

	public UnityLogger( string context ) : base() {
		_mainLogger= makeMainLogger(context);
		addLogger( _mainLogger );

		_strLogger= makeStringLogger(context, _logText);
		addLogger( _strLogger );
		
		_fileLogger= makeFileLogger(context);
		addLogger( _fileLogger );
	}

	protected Rosettastone.Speech.Logger makeMainLogger(string context) {
		// The regular unity logger is useful for when in the editor,
		// but its busted on iOS 6. Let's just use the default platform logger
		// when we're not in the editor.
		#if UNITY_EDITOR
			return new UnityDebugLogger(context);
		#else
			return new Rosettastone.Speech.Logger(context);
		#endif	
	}
	
	protected Rosettastone.Speech.Logger makeFileLogger(string context) {
		string baseDir = Application.persistentDataPath + "/appLogs";
		Directory.CreateDirectory(baseDir);
		string filename = baseDir + "/" + DateTime.Now.ToOADate ().ToString () + "_" + context + ".txt";
		debug ("logger", "logging to " + filename);
		return new FileLogger( context, filename );		
	}
	
	protected Rosettastone.Speech.Logger makeStringLogger(string context, StringBuilder logText) {
		return new StringLogger(context, logText);
	}
}