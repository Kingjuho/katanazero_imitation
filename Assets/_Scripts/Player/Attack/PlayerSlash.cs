using UnityEngine;

public class PlayerSlash : MonoBehaviour
{
    GameObject _player; // 플레이어 오브젝트
    Vector2 MousePos;   // 마우스 위치
    Vector3 dir;        // 플레이어, 마우스 위치의 방향 벡터

    float angle;        // 회전 각도
    Vector3 dirNo;      // 단위 방향 벡터

    public Vector3 direction = Vector3.right;   // 기본 방향 벡터

    void Start()
    {
        // 플레이어의 좌표값 호출
        _player = GameObject.FindGameObjectWithTag("Player");
        Transform playerTransform = _player.GetComponent<Transform>();
        
        // 마우스를 좌표로 치환
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);
        Vector3 Pos = new Vector3(MousePos.x, MousePos.y, 0);
        
        // 방향 벡터(벡터의 뺄셈)
        dir = Pos - playerTransform.position;

        // 각도 구하기
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        // 회전 적용
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = _player.transform.position;
    }


    public void Destroy()
    {
        Destroy(gameObject);
    }
}
