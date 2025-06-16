using System;
using UnityEngine;

namespace GamePlatform.PlatformActions.Base {
    public abstract class ActionBase : MonoBehaviour {
        protected readonly bool ActOnlyOnce;
        protected readonly bool ActOnStart;
        private bool _hasExecuted;

        protected ActionBase(bool actOnStart=false, bool actOnlyOnce=false) {
            ActOnlyOnce = actOnlyOnce;
            ActOnStart = actOnStart;
        }
        
        private void Start() {
            if (!ActOnStart) return;

            Act();
        }

        protected abstract void InternalAct();

        public void Act(Func<bool> predicate=null) {
            if (ActOnlyOnce && _hasExecuted) return;

            if (predicate != null) {
                if (!predicate()) return;

                InternalAct();
                _hasExecuted = true;
            }
            else {
                InternalAct();
                _hasExecuted = true;
            }
        }
    }
}