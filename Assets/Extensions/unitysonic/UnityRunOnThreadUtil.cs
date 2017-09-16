using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Rosettastone.Speech;

namespace Rosettastone.Speech {

public class UnityRunOnThreadUtil : IRunOnThreadUtil {
    protected Queue<IRunnable> _runnableQueue;
    protected object _monitorObj;
    protected object _queueLock;
    protected Thread _thread;
        
    public UnityRunOnThreadUtil() {
		_runnableQueue = new Queue<IRunnable>();
		_queueLock = new object();
		_monitorObj = new object();
		_thread = new Thread(new ThreadStart(this.processRunnables));
		_thread.Name = "ASR Thread";
		_thread.Start();
	}
	
	~UnityRunOnThreadUtil() {
	    _thread.Abort();
	    _thread.Join();
	    Dispose();
	}
	    
    public void processRunnables() {
        while(true) {
            IRunnable runnable = WaitForData();
            runnable.run();
            runnable.Dispose();
        }
    }
    
    public override void run(IRunnable runnable) {
        lock(_queueLock) {
            // take ownership so the C++ object is also deallocated
            runnable.swigCMemOwn = true;
            _runnableQueue.Enqueue(runnable);
            Monitor.Pulse(_queueLock);
        }
    }
    
    IRunnable WaitForData() {
        lock(_queueLock) {
            if(0 == _runnableQueue.Count) {
                Monitor.Wait(_queueLock);
            }
            
            return _runnableQueue.Dequeue();
        }
    }
}

}