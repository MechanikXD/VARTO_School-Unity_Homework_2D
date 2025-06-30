using System;
using UnityEngine;

namespace GamePlatform.PlatformActions.Base {
    public abstract class ActionBase : MonoBehaviour {
        private readonly bool _actOnlyOnce;
        private readonly bool _actOnStart;
        private bool _hasExecuted;

        protected ActionBase(bool actOnStart=false, bool actOnlyOnce=false) {
            _actOnlyOnce = actOnlyOnce;
            _actOnStart = actOnStart;
        }
        
        private void Start() {
            if (!_actOnStart) return;

            Act();
        }

        protected abstract void InternalAct();

        protected void Act(Func<bool> predicate=null) {
            if (_actOnlyOnce && _hasExecuted) return;

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