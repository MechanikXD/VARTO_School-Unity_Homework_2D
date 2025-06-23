using GamePlatform.PlatformActions.Base;
using UI.Controllers;
using UI.Models;
using UnityEngine;

namespace GamePlatform.PlatformActions.Actions {
    public class CollectibleIncreasePlayerScore : ActionBase {
        public CollectibleIncreasePlayerScore() : base(actOnlyOnce: true) { }

        protected override void InternalAct() {
            SessionModel.CurrentScore += 1;
            GameUIController.OnScoreUpdate();
            Destroy(gameObject);
        }
        
        public void OnTriggerStay2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player")) Act();
        }
    }
}