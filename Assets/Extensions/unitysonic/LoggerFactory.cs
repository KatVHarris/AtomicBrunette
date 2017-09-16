using UnityEngine;
using System.Collections;
using Rosettastone.Speech;

public class LoggerFactory {
	public static Rosettastone.Speech.Logger makeLogger(string context) {
		// The regular unity logger is useful for when in the editor,
		// but its busted on iOS 6. Let's just use the default platform logger
		// when we're not in the editor.
		#if UNITY_EDITOR
			return new UnityLogger(context);
		#else
			return new Rosettastone.Speech.Logger(context);
		#endif		
	}
}

