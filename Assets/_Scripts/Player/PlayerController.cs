using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]

public class PlayerController : MonoBehaviour
{
    // PlayerHealth _health;           // 체력 컴포넌트
    PlayerMovement _movement;       // 이동 컴포넌트
    

    void Start()
    {
        _movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        
    }
}
