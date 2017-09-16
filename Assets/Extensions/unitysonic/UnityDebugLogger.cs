using UnityEngine;
using System.Collections;
using Rosettastone.Speech;

public class UnityDebugLogger : Rosettastone.Speech.StringLogger {
	public UnityDebugLogger( string context ) : base( context ) { }
	public override void processLogMessage (string context, Rosettastone.Speech.SRELogLevel level, string message) {
		UnityEngine.Debug.Log(context + " " + level.ToString() + ":" + message );
	}
}

