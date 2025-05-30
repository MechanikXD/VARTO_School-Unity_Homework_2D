using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpStrength;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float gravity;
    private SpriteRenderer _playerRenderer;

    private float _moveDirection;
    private InputAction _jumpButton;
    private Vector2 _nextPosition;
    private float _currentJumpDuration;
    private const float MaxRayCastLength = 0.2f;    // I'm fine this current value BUT ray cast ends within player.
    private bool _isJumping;
    
    /// Teleports player to given coordinates. Should be used with Time.deltaTime for smoother movement.
    private void MoveTo(Vector2 newPlayerPosition) {
        var newPosition = playerTransform.position;
        newPosition.x += newPlayerPosition.x;
        newPosition.y += newPlayerPosition.y;
        playerTransform.position = newPosition;
    }
    /// Function to handle vertical movement (gravity, jump)
    private void HandleVerticals() {
        // We ignore gravity while jumping
        if (_jumpButton.IsPressed() && _currentJumpDuration < jumpDuration && _isJumping) {
            _nextPosition.y += PolynomialSmoothing(_currentJumpDuration, jumpStrength, jumpDuration);
            _currentJumpDuration += Time.deltaTime;
        }
        else {
            _isJumping = false;
            _currentJumpDuration = 0;
            if (IsGrounded()) {
                _nextPosition.y = 0;
            }
            else {
                _nextPosition.y = -gravity;
            }
        }
    }

    // Player HAVE TO NO HAVE collider, because this will break 
    private bool IsGrounded() {
        var rayCastHit = Physics2D.Raycast(transform.position, Vector2.down, MaxRayCastLength);
        return rayCastHit.collider != null;
    }
    
    void Start() {
        _jumpButton = GetComponent<PlayerInput>().actions["Jump"];
        _playerRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        // Store this one in boolean. Otherwise we may be able to jump after
        // jump duration is ended but player haven't landed on ground yet... 
        if (_jumpButton.IsPressed() && IsGrounded()) {
            _isJumping = true;
            MySceneManager.Instance.ChangeAllColorsInScene();   // Call on jump
        }
        HandleVerticals();
        MoveTo(_nextPosition * Time.deltaTime);
    }

    void OnMove(InputValue moveVector) {
        var move = moveVector.Get<Vector2>();
        if (move.x < 0) {   // Flip player sprite based on movement direction 
            _playerRenderer.flipX = false;
        }
        else if (move.x > 0) {
            _playerRenderer.flipX = true;
        }
        _nextPosition.x = move.x * moveSpeed;
    }

    private float PolynomialSmoothing(float time, float weight, float maxTime) {
        return (1 / weight) * (-time * time + maxTime * maxTime);
    }
}