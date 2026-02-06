using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;         // 이동 속도
    [SerializeField] private float _jumpForce = 8f;         // 점프력

    [Header("Wall Action Settings")]
    [SerializeField] private float _wallSlidingSpeed = 2f;                  // 벽 슬라이드 속도
    [SerializeField] private Vector2 _wallJumpPower = new Vector2(8f, 8f);  // 벽 점프력
    [SerializeField] private float _wallJumpInputFreezeTime = 0.2f;         // 벽 점프 입력 대기 시간

    [Header("Detection Settings")]
    [SerializeField] private Transform _groundCheckPos;     // 땅 체크 좌표
    [SerializeField] private Transform _wallCheckPos;       // 벽 체크 좌표
    [SerializeField] private float _checkRadius = 0.3f;     // 체크 반경
    [SerializeField] private LayerMask _groundLayer;        // 땅 레이어
    [SerializeField] private LayerMask _wallLayer;          // 벽 레이어

    public bool IsGrounded { get; private set; }            // 착지 여부
    public bool IsWallSliding { get; private set; }         // 벽 슬라이드 여부
    public bool IsFacingRight { get; private set; } = true; // 오른쪽을 보고 있는지
    
    private bool _isTouchingWall;                           // 벽 밀착 여부
    private bool _canMove = true;                           // 벽 점프 직후 제어 불능 상태
    private float _currentInputX;                           // Controller에서 받아온 입력값

    private Rigidbody2D _rb;                                // Rigidbody 컴포넌트
    private Animator _anim;                                 // Animator 컴포넌트

    // Animator 해시 최적화
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
        CheckWallSlide();
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        CheckSurroundings();
        Move();
    }

    /** Input 핸들링 **/
    public void SetMoveInput(float inputX) => _currentInputX = inputX;
    public void TryJump()
    {
        if (!_canMove) return;
        if (IsGrounded)
            PerformJump(Vector2.up * _jumpForce);
        else if (IsWallSliding || (_isTouchingWall && !IsGrounded))
            PerformWallJump();
    }

    /** 이동 처리 함수 **/
    private void Move()
    {
        // 못 움직이는 상태면 리턴
        if (!_canMove) return;

        // 일반 이동 / 벽 이동
        if (!IsWallSliding)
            _rb.linearVelocity = new Vector2(_currentInputX * _moveSpeed, _rb.linearVelocity.y);
        else
        {
            // 벽 타기 로직
            float moveX = _currentInputX * _moveSpeed;
            float moveY = Mathf.Clamp(_rb.linearVelocity.y, -_wallSlidingSpeed, float.MaxValue);
            _rb.linearVelocity = new Vector2(moveX, moveY);
        }

        // 방향 전환 체크
        if (_currentInputX != 0)
            if ((_currentInputX > 0 && !IsFacingRight) || (_currentInputX < 0 && IsFacingRight))
                Flip();
    }

    /** 점프 **/
    private void PerformJump(Vector2 forceDir)
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        _rb.AddForce(forceDir, ForceMode2D.Impulse);
    }

    /** 벽점프 **/
    private void PerformWallJump()
    {
        IsWallSliding = false;

        // 벽 반대 방향 계산
        float direction = IsFacingRight ? -1f : 1f;
        Vector2 jumpForce = new Vector2(_wallJumpPower.x * direction, _wallJumpPower.y);

        // 점프와 동시에 방향 전환
        PerformJump(jumpForce);
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
        IsGrounded = Physics2D.OverlapCircle(_groundCheckPos.position, _checkRadius, _groundLayer);
        _isTouchingWall = Physics2D.OverlapCircle(_wallCheckPos.position, _checkRadius, _wallLayer);
    }

    /** 벽 슬라이드 체크 함수 **/
    private void CheckWallSlide()
    {
        // 벽에 닿으면 즉시 인식
        if (_isTouchingWall && !IsGrounded)
            IsWallSliding = true;
        else
            IsWallSliding = false;
    }

    /** 방향 전환 **/
    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    /** 애니메이션 상태 변경 **/
    private void UpdateAnimationState()
    {
        // 벽 타기 중이면 달리기 애니메이션 강제 off
        bool isRunning = Mathf.Abs(_currentInputX) > 0.1f && !IsWallSliding;
        _anim.SetBool(_hashRun, isRunning);

        bool isJumping = !IsGrounded && !IsWallSliding;
        _anim.SetBool(_hashJump, isJumping);

        _anim.SetBool(_hashGrab, IsWallSliding);
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