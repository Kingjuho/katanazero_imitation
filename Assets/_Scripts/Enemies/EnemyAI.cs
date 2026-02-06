using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // 적 상태 열거형
    private enum State { Idle, Patrol, Attack }

    [Header("기본 설정")]
    [SerializeField] private float _moveSpeed = 2f;         // 이동 속도
    [SerializeField] private float _patrolDistance = 5f;    // 정찰 범위
    [SerializeField] private Transform _groundCheck;        // 낭떠러지 체크
    [SerializeField] private LayerMask _groundLayer;        // 땅 레이어

    [Header("전투 설정")]
    [SerializeField] private float _detectionRange = 6f;    // 감지 거리
    [SerializeField] private LayerMask _targetLayer;        // 플레이어 레이어
    [SerializeField] private GameObject _bulletPrefab;      // 총알 프리팹
    [SerializeField] private Transform _firePoint;          // 총구 위치
    [SerializeField] private float _fireRate = 1.0f;        // 연사 속도

    private Rigidbody2D _rb;                                // Rigidbody2D 컴포넌트
    private Animator _anim;                                 // Animator 컴포넌트

    private State _currentState;                            // 현재 상태
    private float _fireTimer;                               // 발사 타이머
    private Vector2 _startPos;                              // 순찰 시작 위치
    private bool _isFacingRight = true;                     // 시선 방향(오른쪽)
    private bool _isTurning = false;                        // 현재 방향 전환 여부

    // 애니메이터 컨디션 해시
    private readonly int _hashRun = Animator.StringToHash("isRunning");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _startPos = transform.position;
        _currentState = State.Patrol;
    }

    private void Update()
    {
        // FSM으로 관리
        switch (_currentState)
        {
            case State.Idle:
                _rb.linearVelocity = Vector2.zero;
                break;
            case State.Patrol:
                Patrol();
                DetectForPlayer();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    /** 정찰 **/
    private void Patrol()
    {
        // 방향 전환 중이면 리턴
        if (_isTurning) return;

        // 시선 방향으로 이동
        _rb.linearVelocity = new Vector2((_isFacingRight ? 1 : -1) * _moveSpeed, _rb.linearVelocity.y);
        
        // 애니메이터 설정
        _anim.SetBool(_hashRun, true);

        // 낭떠러지 체크
        bool isGroundAhead = Physics2D.Raycast(_groundCheck.position, Vector2.down, 1f, _groundLayer);
        if (!isGroundAhead)
        {
            Flip();
            return;
        }

        // 정찰 거리 체크
        float xDiff = transform.position.x - _startPos.x;
        // 오른쪽 방향
        if (xDiff > _patrolDistance && _isFacingRight)
            Flip();
        // 왼쪽 방향
        else if (xDiff < -_patrolDistance && !_isFacingRight)
            Flip();
    }

    /** 플레이어 감지 **/
    private void DetectForPlayer()
    {
        // 시선 방향으로 레이캐스트 발사
        Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _detectionRange, _targetLayer); ;

        // 플레이어 감지
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // 공격 상태로 전환
            _anim.SetBool(_hashRun, false);
            _currentState = State.Attack;
            _rb.linearVelocity = Vector2.zero;
        }
    }

    /** 플레이어 공격 **/
    private void Attack()
    {
        // 시선 방향으로 레이캐스트 발사
        Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _detectionRange, _targetLayer);

        // 플레이어 감지 실패
        if (hit.collider == null || !hit.collider.CompareTag("Player"))
        {
            // 순찰 상태로 전환
            _currentState = State.Patrol;
            return;
        }

        // 공격 쿨타임 체크
        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0)
        {
            Fire();
            _fireTimer = _fireRate;
        }
    }

    /** 발포 **/
    private void Fire()
    {
        // 총알 생성
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);

        // 애니메이터 설정
        _anim.SetTrigger("Attack");

        // 방향 주입
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            Vector2 shootDir = _isFacingRight ? Vector2.right : Vector2.left;
            bulletScript.Initialize(shootDir);
        }
    }

    /** 방향 전환 **/
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
    }

    /** 디버깅용 기즈모 **/
    private void OnDrawGizmos()
    {
        // 바닥 체크 (초록색)
        if (_groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_groundCheck.position, Vector2.down * 1f);
        }

        // 플레이어 감지 (빨간색)
        Gizmos.color = Color.red;
        Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawRay(transform.position, direction * _detectionRange);
    }
}
