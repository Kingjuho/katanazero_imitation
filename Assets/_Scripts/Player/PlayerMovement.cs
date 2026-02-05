using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 12f;

    [Header("Wall Action Settings")]
    [SerializeField] private float _wallSlidingSpeed = 2f;
    [SerializeField] private float _wallJumpPowerX = 8f;
    [SerializeField] private float _wallJumpPowerY = 12f;
    [SerializeField] private float _wallJumpInputFreezeTime = 0.2f;

    [Header("Detection Settings")]
    [SerializeField] private Transform _groundCheckPos;
    [SerializeField] private Transform _wallCheckPos;
    [SerializeField] private float _checkRadius = 0.3f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    private bool _isGrounded;
    private bool _isTouchingWall;
    private bool _isWallSliding;
    private bool _canMove = true;

    private float _horizontalInput;
    private bool _isFacingRight = true;

    private Rigidbody2D _rb;
    private Animator _anim;

    private readonly int _hashRun = Animator.StringToHash("isRunning");
    private readonly int _hashJump = Animator.StringToHash("isJumping");
    private readonly int _hashGrab = Animator.StringToHash("isGrabbing");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        CheckWallSlide();
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
        Move();
    }

    private void HandleInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (_isGrounded)
            {
                PerformJump(Vector2.up * _jumpForce);
            }
            // 땅이 아니고 벽에 닿아있으면 점프
            else if (_isTouchingWall)
            {
                PerformWallJump();
            }
        }

        if (_canMove && _horizontalInput != 0)
        {
            if ((_horizontalInput > 0 && !_isFacingRight) || (_horizontalInput < 0 && _isFacingRight))
            {
                Flip();
            }
        }
    }

    private void Move()
    {
        if (!_canMove) return;

        if (!_isWallSliding)
        {
            _rb.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rb.linearVelocity.y);
        }
        else
        {
            // 벽 타기 로직
            float moveX = _horizontalInput * _moveSpeed;
            float moveY = _rb.linearVelocity.y;

            // 벽에 닿아있을 때 내려가는 속도 제한 (올라가는 속도는 건드리지 않음)
            if (moveY < -_wallSlidingSpeed)
            {
                moveY = -_wallSlidingSpeed;
            }

            _rb.linearVelocity = new Vector2(moveX, moveY);
        }
    }

    private void CheckWallSlide()
    {
        // [핵심 수정] y < 0 조건을 제거하거나 완화하여 벽에 닿으면 즉시 인식하도록 함
        // 여기서는 올라가는 중에도 벽 판정을 인정하되, Move()에서 속도 제한은 내려갈 때만 걸리게 처리함.
        if (_isTouchingWall && !_isGrounded)
        {
            _isWallSliding = true;
        }
        else
        {
            _isWallSliding = false;
        }
    }

    private void PerformJump(Vector2 forceDir)
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(forceDir, ForceMode2D.Impulse);
    }

    private void PerformWallJump()
    {
        _isWallSliding = false;

        float direction = _isFacingRight ? -1f : 1f;
        Vector2 jumpForce = new Vector2(direction * _wallJumpPowerX, _wallJumpPowerY);

        PerformJump(jumpForce);

        // 점프와 동시에 방향 전환
        Flip();

        StartCoroutine(DisableMoveRoutine());
    }

    private IEnumerator DisableMoveRoutine()
    {
        _canMove = false;
        yield return new WaitForSeconds(_wallJumpInputFreezeTime);
        _canMove = true;
    }

    private void CheckSurroundings()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheckPos.position, _checkRadius, _groundLayer);
        _isTouchingWall = Physics2D.OverlapCircle(_wallCheckPos.position, _checkRadius, _wallLayer);
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void UpdateAnimationState()
    {
        // [핵심 수정] 벽 타기 중이면 '달리기' 애니메이션 강제 off
        bool isRunning = Mathf.Abs(_horizontalInput) > 0.1f && !_isWallSliding;
        _anim.SetBool(_hashRun, isRunning);

        bool isJumping = !_isGrounded && !_isWallSliding;
        _anim.SetBool(_hashJump, isJumping);

        _anim.SetBool(_hashGrab, _isWallSliding);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_groundCheckPos) Gizmos.DrawWireSphere(_groundCheckPos.position, _checkRadius);

        Gizmos.color = Color.red;
        if (_wallCheckPos) Gizmos.DrawWireSphere(_wallCheckPos.position, _checkRadius);
    }
}