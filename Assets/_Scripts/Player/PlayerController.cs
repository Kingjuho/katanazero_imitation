using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]

public class PlayerController : MonoBehaviour
{
    // PlayerHealth _health;           // 체력 컴포넌트
    PlayerMovement _movement;       // 이동 컴포넌트
    PlayerAttack _attack;           // 공격 컴포넌트
    

    void Start()
    {
        _movement = GetComponent<PlayerMovement>();
        _attack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        
    }
}
