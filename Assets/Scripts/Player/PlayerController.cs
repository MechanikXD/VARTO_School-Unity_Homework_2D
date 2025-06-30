using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UI.Controllers;
using UI.Models;

namespace Player {
    public class PlayerController : MonoBehaviour {
        private SpriteRenderer _playerRenderer;
        private Rigidbody2D _playerBody;
        [SerializeField] private int fallDistanceToGameOver;

        [SerializeField] private float moveSpeed = 5;
        private float _horizontalVelocity;

        [SerializeField] private float jumpStrength = 7;
        [SerializeField] private float jumpDuration = 0.4f;
        private InputAction _jumpButton;
        private float _currentJumpDuration;
        private bool _isJumping;

        void Awake() {
            InstantiateSelf();
            var _ = MySceneManager.Instance;    // Call scene manager on awake, so it can be instantiated
        }

        // Handle player movement
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
            
            GameUIController.OnHeightUpdate();
        }

        private void OnDestroy() {
            // Unsubscribe from events
            GameUIController.HeightUpdateEvent -= UpdateHeightCounter;
        }

        private void InstantiateSelf() {
            transform.position = new Vector3(0, 1f);
            _jumpButton = GetComponent<PlayerInput>().actions["Jump"];
            _playerRenderer = GetComponent<SpriteRenderer>();
            _playerBody = GetComponent<Rigidbody2D>();
            GameUIController.HeightUpdateEvent += UpdateHeightCounter;
        }

        private void UpdateHeightCounter() {
            var sessionHeightDifference = transform.position.y - SessionModel.CurrentHeight;
            if (sessionHeightDifference > 0) {
                SessionModel.CurrentHeight = (int)transform.position.y;
            }
            else if (sessionHeightDifference < -fallDistanceToGameOver) {
                MySceneManager.Instance.EndCurrentSession();
            }
        }
        
        private bool HasVerticalVelocity(out float verticalVelocity) {
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

        private bool IsGrounded() {
            // Linear velocity on Y != 0 when moving due to calculation errors 
            Vector2 linearVelocity = _playerBody.linearVelocity;
            var calculationError = 0.0001f;
            return linearVelocity.y > -calculationError && linearVelocity.y < calculationError;
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