using GamePlatform.PlatformActions.Base;
using Player;
using UnityEngine;

namespace GamePlatform.PlatformActions.Actions {
    public class PlayerDetectJump : ActionBase {
        private PlayerController _player;

        public void Start() {
            _player = GetComponent<PlayerController>();
            _player.HasJumped += (_, args) => {
                if (args.PlayerHasJumped) Act();
            };
        }
        
        protected override void InternalAct() {
            Debug.Log("Player has jumped!");
        }
    }
}