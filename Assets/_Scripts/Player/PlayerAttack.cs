using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] float _power = 5f;     // 공격력

    private Animator _anim;                 // Animator 컴포넌트
    private Rigidbody2D _rb;                // Rigidbody2D 컴포넌트
    public GameObject _slash;               // PlayerSlash 프리팹

    void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Attack();
    }

    /** 공격 **/
    void Attack()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        
        // 애니메이터 트리거 호출
        _anim.SetTrigger("Attack");

        // 플립 여부
        bool isFlip = false;

        // 플립 여부에 따른 힘 방향 변경
        if (transform.localScale.x == -1)
        {
            _rb.AddForce(Vector2.right * _power, ForceMode2D.Impulse);
            isFlip = true;
        }
        else
            _rb.AddForce(Vector2.left * _power, ForceMode2D.Impulse);

        // Slash 오브젝트 생성
        GameObject go = Instantiate(_slash, transform.position, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().flipX = isFlip;
    }
}
