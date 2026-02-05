using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    float _moveSpeed = 5f;  // 이동 속도
    float _jump = 5f;       // 점프 힘

    Animator _animator;                 // Animator 컴포넌트
    Rigidbody2D _rigidbody2D;           // Rigidbody2D 컴포넌트
    SpriteRenderer _spriteRenderer;     // SpriteRenderer 컴포넌트
    Vector2 _direction;                 // 이동 방향
    
    const float _GROUND_CHECK_DISTANCE = 0.6f; // 땅 체크 거리

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _direction = Vector2.zero;
    }

    void Update()
    {
        Move();
        Jump();
    }

    void FixedUpdate()
    {
        CheckGroundState();
    }

    void Move()
    {
        // 수평 이동 입력 처리
        _direction.x = Input.GetAxisRaw("Horizontal");
        if (_direction.x < 0)      _spriteRenderer.flipX = true;    // 왼쪽 이동
        else if (_direction.x > 0) _spriteRenderer.flipX = false;   // 오른쪽 이동

        // 애니메이션 처리
        if (_direction.x != 0) _animator.SetBool("isRunning", true);
        else                   _animator.SetBool("isRunning", false);

        // 이동
        transform.Translate(_direction * _moveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        // 점프 입력 처리
        if (!Input.GetKeyDown(KeyCode.W)) return;

        // 이미 점프 중이면 무시
        if (_animator.GetBool("isJumping")) return;

        // 애니메이션 처리
        _animator.SetBool("isJumping", true);

        // 점프
        _rigidbody2D.linearVelocity = Vector2.zero; // 현재 속도 초기화
        _rigidbody2D.AddForce(Vector2.up * _jump, ForceMode2D.Impulse); // 위로 힘 추가
    }

    void CheckGroundState()
    {
        // 바닥에 닿아있는지 확인
        Debug.DrawRay(_rigidbody2D.position, Vector3.down, new Color(1, 0, 0, 1));
        RaycastHit2D rayHit = Physics2D.Raycast(_rigidbody2D.position, Vector3.down, _GROUND_CHECK_DISTANCE, LayerMask.GetMask("Ground"));

        // 땅에 닿아있으면 점프 애니메이션 해제
        bool isGrounded = rayHit.collider != null && rayHit.distance < _GROUND_CHECK_DISTANCE;
        
        if (isGrounded)  _animator.SetBool("isJumping", false);
        else             _animator.SetBool("isJumping", true);
    }
}
