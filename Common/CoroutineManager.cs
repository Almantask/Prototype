using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Common {
    public class CoroutineManager : MonoBehaviour {
        private List<Coroutine> _coroutines;
        private Coroutine _coroutineInPause;

        private void Awake() {
            _coroutines = new List<Coroutine>();
        }

        public void AddCoroutine(IEnumerator<YieldInstruction> coroutine, bool pausable) {
            var result = new Coroutine(coroutine, pausable);
            _coroutines.Add(result);
        }

        private void Start() {
            AddCoroutine(Test(), true);
            AddCoroutine(Test2(), true);
            StartProcessing();
        }

        public void StartProcessing() {
            StartCoroutine(ProcessCoroutines());
        }

        private IEnumerator ProcessCoroutines() {
            Debug.Log("Processing...");
            while(_coroutines.Count != 0) {
                List<Coroutine> coroutinesToRun;
                if(_coroutineInPause == null) {
                    coroutinesToRun = _coroutines.ToList();
                } else {
                    coroutinesToRun = _coroutines.Where(x => !x.Pausable).ToList();
                    if(!coroutinesToRun.Contains(_coroutineInPause)) {
                        coroutinesToRun.Insert(0, _coroutineInPause);
                    }
                }
                Debug.Log("Coroutine queue count is: " + coroutinesToRun.Count);
                for(var index = 0; index < coroutinesToRun.Count; index++) {
                    var coroutine = coroutinesToRun[index];
                    if(coroutine.LastInstruction != null && !coroutine.LastInstruction.Completed(this, coroutine)) {
                        continue;
                    }
                    if(!coroutine.MoveNext()) {
                        _coroutines.Remove(coroutine);
                    } else if(coroutine.LastInstruction?.ResumeImmediately ?? false) {
                        index--;
                    }
                    index++;
                }
                yield return null;
            }
        }

        public void Pause(Coroutine coroutine) {
            _coroutineInPause = coroutine;
        }

        public void Continue() {
            _coroutineInPause = null;
        }

        private IEnumerator<YieldInstruction> Test() {
            yield return new PauseCoroutine(true);
            yield return new WaitForSeconds(5f);
            Debug.Log("Waiting...");
            yield return new WaitForSeconds(1f);
            Debug.Log("TEST2");
            yield return new PauseCoroutine(false);
        }

        private IEnumerator<YieldInstruction> Test2() {
            Debug.Log("I am here because other is Paused.");
            yield return null;
        }
    }
}