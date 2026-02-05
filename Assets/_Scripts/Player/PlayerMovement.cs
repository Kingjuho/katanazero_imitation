using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    float _moveSpeed = 5f;
    float _jump = 5f;
    
    Animator _animator;                 // Animator 컴포넌트
    Rigidbody2D _rigidbody2D;           // Rigidbody2D 컴포넌트
    SpriteRenderer _spriteRenderer;     // SpriteRenderer 컴포넌트
    Vector2 _direction;                 // 이동 방향

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
    }

    void Move()
    {
        // 수평 이동 입력 처리
        _direction.x = Input.GetAxisRaw("Horizontal");
        if (_direction.x < 0)      _spriteRenderer.flipX = true;    // 왼쪽 이동
        else if (_direction.x > 0) _spriteRenderer.flipX = false;   // 오른쪽 이동

        // 애니메이션 처리
        if (_direction.x != 0) _animator.SetBool("Run", true);
        else                   _animator.SetBool("Run", false);

        // 점프 입력 처리
        if (Input.GetKeyDown(KeyCode.Space))
            _rigidbody2D.AddForce(Vector2.up * _jump, ForceMode2D.Impulse);

        // 이동
        transform.Translate(_direction * _moveSpeed * Time.deltaTime);
    }
}
