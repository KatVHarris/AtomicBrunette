using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Rosettastone.Speech;

namespace Rosettastone.Speech {

public class UnityRunOnMainUtil : IRunOnThreadUtil {
    public override void run(IRunnable runnable) {
        UnitySonic.RunOnMainThread(runnable);
    }
}

}