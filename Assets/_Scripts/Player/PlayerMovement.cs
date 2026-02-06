using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;                 // 이동 속도
    [SerializeField] private float _jumpForce = 12f;                // 점프력

    [Header("Wall Action Settings")]
    [SerializeField] private float _wallSlidingSpeed = 2f;          // 벽 슬라이드 속도
    [SerializeField] private float _wallJumpPowerX = 8f;            // 벽 점프력(X축)
    [SerializeField] private float _wallJumpPowerY = 12f;           // 벽 점프력(Y축)
    [SerializeField] private float _wallJumpInputFreezeTime = 0.2f; // 벽 점프 입력 대기 시간

    [Header("Detection Settings")]
    [SerializeField] private Transform _groundCheckPos;             // 땅 체크 좌표
    [SerializeField] private Transform _wallCheckPos;               // 벽 체크 좌표
    [SerializeField] private float _checkRadius = 0.3f;             // 체크 반경
    [SerializeField] private LayerMask _groundLayer;                // 땅 레이어
    [SerializeField] private LayerMask _wallLayer;                  // 벽 레이어

    private bool _isGrounded;                                       // 착지 여부
    private bool _isTouchingWall;                                   // 벽 밀착 여부
    private bool _isWallSliding;                                    // 벽 슬라이드 여부
    private bool _canMove = true;                                   // 움직임 봉인 여부

    private float _horizontalInput;                                 // 좌우 입력
    private bool _isFacingRight = true;                             // 오른쪽을 보고 있는지

    private Rigidbody2D _rb;                                        // Rigidbody 컴포넌트
    private Animator _anim;                                         // Animator 컴포넌트

    // Animator 컨디션 해시
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

    /** 플레이어 입력 처리 **/
    private void HandleInput()
    {
        // 좌우 이동
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        // 점프
        if (Input.GetKeyDown(KeyCode.W))
        {
            // 땅에 있으면 점프, 벽에 있으면 벽점프
            if (_isGrounded)
                PerformJump(Vector2.up * _jumpForce);
            else if (_isTouchingWall)
                PerformWallJump();
        }

        // 방향 전환
        if (_canMove && _horizontalInput != 0)
        {
            if ((_horizontalInput > 0 && !_isFacingRight) || (_horizontalInput < 0 && _isFacingRight))
                Flip();
        }
    }

    /** 이동 처리 함수 **/
    private void Move()
    {
        // 못 움직이는 상태면 리턴
        if (!_canMove) return;

        // 일반 이동 / 벽 이동
        if (!_isWallSliding)
            _rb.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rb.linearVelocity.y);
        else
        {
            // 벽 타기 로직
            float moveX = _horizontalInput * _moveSpeed;
            float moveY = _rb.linearVelocity.y;

            // 벽에 닿아있을 때 내려가는 속도 제한
            if (moveY < -_wallSlidingSpeed) moveY = -_wallSlidingSpeed;

            _rb.linearVelocity = new Vector2(moveX, moveY);
        }
    }

    /** 벽 슬라이드 체크 함수 **/
    private void CheckWallSlide()
    {
        // 벽에 닿으면 즉시 인식
        if (_isTouchingWall && !_isGrounded)
            _isWallSliding = true;
        else
            _isWallSliding = false;
    }

    /** 점프 **/
    private void PerformJump(Vector2 forceDir)
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(forceDir, ForceMode2D.Impulse);
    }

    /** 벽점프 **/
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

    /** 벽에 닿았을 때 잠시 이동 못하게 하는 코루틴 함수 **/
    private IEnumerator DisableMoveRoutine()
    {
        _canMove = false;
        yield return new WaitForSeconds(_wallJumpInputFreezeTime);
        _canMove = true;
    }

    /** 현재 땅/벽에 닿아있는지 체크 **/
    private void CheckSurroundings()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheckPos.position, _checkRadius, _groundLayer);
        _isTouchingWall = Physics2D.OverlapCircle(_wallCheckPos.position, _checkRadius, _wallLayer);
    }

    /** 방향 전환 **/
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    /** 애니메이션 상태 변경 **/
    private void UpdateAnimationState()
    {
        // [핵심 수정] 벽 타기 중이면 '달리기' 애니메이션 강제 off
        bool isRunning = Mathf.Abs(_horizontalInput) > 0.1f && !_isWallSliding;
        _anim.SetBool(_hashRun, isRunning);

        bool isJumping = !_isGrounded && !_isWallSliding;
        _anim.SetBool(_hashJump, isJumping);

        _anim.SetBool(_hashGrab, _isWallSliding);
    }

    /** 디버깅용 기즈모 그리기 함수 **/
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_groundCheckPos) Gizmos.DrawWireSphere(_groundCheckPos.position, _checkRadius);

        Gizmos.color = Color.red;
        if (_wallCheckPos) Gizmos.DrawWireSphere(_wallCheckPos.position, _checkRadius);
    }
}