using System;

namespace Player.EventSystem {
    public class OnJumpEventArgs : EventArgs {
        public bool PlayerHasJumped;

        public OnJumpEventArgs(bool playerHasJumped) {
            PlayerHasJumped = playerHasJumped;
        }
    }
}