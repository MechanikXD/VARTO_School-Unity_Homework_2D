using GamePlatform.PlatformActions.Base;
using UI;
using UI.Models;
using UnityEngine;

namespace GamePlatform.PlatformActions.Actions {
    public class PlatformChangeColor : ActionBase {
        [SerializeField] private SpriteRenderer target;
        [SerializeField] private Color changeColor = Color.yellow;

        public PlatformChangeColor() : base(actOnlyOnce: true) { }

        protected override void InternalAct() {
            target.color = changeColor;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.CompareTag("Player")) {
                Act();
            }
        }
    }
}