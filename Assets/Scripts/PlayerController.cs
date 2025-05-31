using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Transform playerTransform;
    private SpriteRenderer _playerRenderer;
    
    [SerializeField] private float moveSpeed;
    private float _moveDirection;
    private Vector2 _nextPosition;
    
    [SerializeField] private float jumpStrength;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float gravity;
    private InputAction _jumpButton;
    private float _currentJumpDuration;
    private float _currentAirborneDuration;
    private const float MaxRayCastLength = 0.3f;
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
            _nextPosition.y += ReversePolynomialSmoothing(_currentJumpDuration, jumpStrength, jumpDuration);
            _currentJumpDuration += Time.deltaTime;
        }
        else {
            _isJumping = false;
            _currentJumpDuration = 0;
            if (IsGrounded()) {
                _currentAirborneDuration = 0;
                _nextPosition.y = 0;
            }
            else {
                _currentAirborneDuration += Time.deltaTime;
                _nextPosition.y = -PolynomialSmoothing(_currentAirborneDuration, gravity, 1);
            }
        }
    }

    // Player have to NOT have collider, because this will break 
    private bool IsGrounded() {
        // Kinda bad implementation, at this point should've used rigidBody to detect collision,
        // but I'm too stubborn to do so... (RigidBody sucks)
        var position = transform.position;
        var furtherRayCastHit = Physics2D.Raycast(position, Vector2.down, MaxRayCastLength + 0.1f);
        var closerRayCastHit = Physics2D.Raycast(position, Vector2.down, MaxRayCastLength);
        // Basically check that we are not IN the ground by doing 2 rayCasts with slight margin 
        return closerRayCastHit.collider == null && furtherRayCastHit.collider != null;
    }
    
    void Start() {
        MySceneManager.Instance.SetupScene(new Vector2(0.2f, -0.7f - 1));
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

    private float ReversePolynomialSmoothing(float time, float weight, float maxTime) {
        return (1 / weight) * (-time * time + maxTime * maxTime);
    }

    private float PolynomialSmoothing(float time, float weight, float maxTime) {
        return weight / maxTime * time * time + 1;
    }
}