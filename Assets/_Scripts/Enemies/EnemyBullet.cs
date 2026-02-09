using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;   // 이동 속도
    [SerializeField] private float _lifeTime = 3f; // 3초 뒤 자동 소멸

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        // 3초 후 삭제
        Destroy(gameObject, _lifeTime);
    }

    /** 초기화 함수 **/
    public void Initialize(Vector2 direction)
    {
        // 이동
        if (_rb != null)
            _rb.linearVelocity = direction * _speed;

        // 방향 전환
        if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: 피격 로직
        if (collision.CompareTag("Player"))
            Destroy(gameObject);
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            Destroy(gameObject);
    }
}
