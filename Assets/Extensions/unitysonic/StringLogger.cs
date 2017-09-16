using UnityEngine;
using System.Collections;
using System.Text;

public class StringLogger : Rosettastone.Speech.StringLogger {
	public StringBuilder stringLog;

	public StringLogger( string context, StringBuilder log ) : base( context ) {
		this.stringLog= log;
	}

	public override void processLogMessage (string context, Rosettastone.Speech.SRELogLevel level, string message) {
		string newmsg= context + " " + level.ToString() + ":" + message;
		stringLog.AppendLine(newmsg);
	}
}

