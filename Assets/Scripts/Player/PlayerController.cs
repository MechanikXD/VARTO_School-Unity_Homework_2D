using UnityEngine;
using UnityEngine.InputSystem;
using Core;

namespace Player {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Vector2 startingPosition;
        private SpriteRenderer _playerRenderer;
        private Rigidbody2D _playerBody;

        [SerializeField] private float moveSpeed;
        private float _horizontalVelocity;

        [SerializeField] private float jumpStrength;
        [SerializeField] private float jumpDuration;
        private InputAction _jumpButton;
        private float _currentJumpDuration;
        private bool _isJumping;

        private bool HasVerticalVelocity(out float verticalVelocity) {
            // We ignore gravity while jumping
            if (_jumpButton.IsPressed() && _currentJumpDuration < jumpDuration && _isJumping) {
                _currentJumpDuration += Time.deltaTime;
                verticalVelocity = jumpStrength;
                return true;
            }
            else {
                _jumpButton.Reset();
                _isJumping = false;
                _currentJumpDuration = 0;
                verticalVelocity = 0;
                return false;
            }
        }

        private bool IsGrounded() => _playerBody.linearVelocity.y == 0;

        void Start() {
            MySceneManager.Instance.SetupScene(startingPosition);
            transform.position = new Vector3(startingPosition.x, startingPosition.y + 1f);
            _jumpButton = GetComponent<PlayerInput>().actions["Jump"];
            _playerRenderer = GetComponent<SpriteRenderer>();
            _playerBody = GetComponent<Rigidbody2D>();
        }

        private void Update() {
            // Store this one in boolean. Otherwise we may be able to jump after
            // jump duration is ended but player haven't landed on ground yet... 
            if (_jumpButton.IsPressed() && IsGrounded()) {
                _isJumping = true;
            }

            if (HasVerticalVelocity(out var velocity)) {
                _playerBody.linearVelocityY = velocity;
            }
            _playerBody.linearVelocityX = _horizontalVelocity;
        }

        void OnMove(InputValue moveVector) {
            var move = moveVector.Get<Vector2>();
            if (move.x < 0) {
                // Flip player sprite based on movement direction 
                _playerRenderer.flipX = false;
            }
            else if (move.x > 0) {
                _playerRenderer.flipX = true;
            }

            _horizontalVelocity = move.x * moveSpeed;
        }
    }
}