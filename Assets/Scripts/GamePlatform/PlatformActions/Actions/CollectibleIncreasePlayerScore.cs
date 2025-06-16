using Core;
using GamePlatform.PlatformActions.Base;
using UnityEngine;

namespace GamePlatform.PlatformActions.Actions {
    public class CollectibleIncreasePlayerScore : ActionBase {
        
        public CollectibleIncreasePlayerScore() : base(actOnlyOnce:true) {}
        
        protected override void InternalAct() {
            MySceneManager.Instance.IncreasePlayerScore();
            Destroy(gameObject);
        }
        
        public void OnTriggerStay2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player")) {
                Act();
            }
        }
    }
}