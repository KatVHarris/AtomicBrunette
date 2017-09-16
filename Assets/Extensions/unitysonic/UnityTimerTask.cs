using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using Rosettastone.Speech;

public class UnityTimerTask : TimerTask {
	private int TICK = 10;
	
	protected DateTime _startTime;
	protected Thread _thread;
	protected Rosettastone.Speech.Logger _logger;
	
	public UnityTimerTask( Rosettastone.Speech.Logger logger, string identifier = "" ) : base(logger, identifier) {
		_logger= logger;
	}
	
	public override void customRun () {
		_startTime = DateTime.Now;
		_thread = new Thread(new ThreadStart(this.runTimer));
		_thread.Start();
	}

	public override bool interrupt () {
		lock(this) {
			setState(Task.TaskState.INTERRUPTING);
		}
		return true;
	}
	
	public override void reset () {
		lock(this) {
			_startTime = DateTime.Now;
		}
	}
	
	protected void runTimer() {
		int numRepetitions = 0;
		TaskState currState = Task.TaskState.RUNNING;
		
		while(true) {
		    lock(this) {
				currState = this.getState();
		    }
		    if(currState == Task.TaskState.RUNNING) {
		        DateTime currTime = DateTime.Now;
                double elapsed = currTime.Subtract(_startTime).TotalMilliseconds;
            
                if((uint)elapsed < delayInMS) {
                    Thread.Sleep (new TimeSpan(0, 0, 0, 0, TICK));
                    continue;
                }
                taskUpdate();
                
                if(repeatCount != 0 && repeatCount >= numRepetitions) {
                    break;
                }
                
                lock(this) {
                    _startTime = currTime;
                }
                numRepetitions++;
            } else {
                break;
            }
		}
		
		try {
            if( currState == Task.TaskState.INTERRUPTING ) {
                taskInterrupt();
            } else {
                taskComplete();
            }
        } catch (Exception e) {
            UnityEngine.Debug.LogError("Exception caught: " + e.Message);
        }
	}
}