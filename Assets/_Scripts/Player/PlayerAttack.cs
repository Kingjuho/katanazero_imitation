using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] float _power = 5f;     // 공격력

    private Animator _anim;                 // Animator 컴포넌트
    private Rigidbody2D _rb;                // Rigidbody2D 컴포넌트
    public GameObject _slash;               // PlayerSlash 프리팹

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    /** 공격 **/
    public void Attack()
    {
        // 마우스 위치 계산
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 방향 및 각도 계산
        Vector3 dir = mousePos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // 생성 및 초기화
        GameObject go = Instantiate(_slash, transform.position, Quaternion.identity);
        PlayerSlash slashScript = go.GetComponent<PlayerSlash>();

        // 각도 주입
        if (slashScript != null) slashScript.Initialize(transform, rotation);

        // 애니메이터 트리거 호출
        _anim.SetTrigger("Attack");

        // 플립 여부에 따른 힘 방향 변경
        if (transform.localScale.x == 1)
            _rb.AddForce(Vector2.right * _power, ForceMode2D.Impulse);
        else
            _rb.AddForce(Vector2.left * _power, ForceMode2D.Impulse);
    }
}
