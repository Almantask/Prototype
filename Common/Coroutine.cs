using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Common {
    public class Coroutine {
        public bool Pausable { get; private set; }
        public bool Canceled { get; set; }

        private bool _started;
        private readonly IEnumerator<YieldInstruction> _coroutine;

        public YieldInstruction LastInstruction {
            get {
                if(!_started) return null;
                return _coroutine.Current;
            }
        }

        public Coroutine(IEnumerator<YieldInstruction> coroutine, bool pausable) {
            this._coroutine = coroutine;
            _started = false;
            Pausable = pausable;
        }

        public bool MoveNext() {
            if(Canceled) return false;
            _started = true;
            return _coroutine.MoveNext();
        }
    }

    public abstract class YieldInstruction {
        public virtual bool ResumeImmediately { get { return false; } }
        public abstract bool Completed(CoroutineManager manager, Coroutine coroutine);
    }

    public class WaitForSeconds : YieldInstruction {
        private readonly float startingTime;
        private readonly float waitTime;

        public WaitForSeconds(float waitTime) {
            startingTime = Time.time;
            this.waitTime = waitTime;
        }

        public override bool Completed(CoroutineManager manager, Coroutine parent) {
            return startingTime + waitTime <= Time.time;
        }
    }

    public class PauseCoroutine : YieldInstruction {
        private readonly bool _paused;

        public PauseCoroutine(bool pause) {
            _paused = pause;
        }

        public override bool ResumeImmediately {
            get { return true; }
        }

        public override bool Completed(CoroutineManager manager, Coroutine coroutine) {
            if(_paused) {
                manager.Pause(coroutine);
            } else {
                manager.Continue();
            }
            return true;
        }
    }
}