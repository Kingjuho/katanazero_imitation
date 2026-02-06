using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]

public class PlayerController : MonoBehaviour
{
    // 컴포넌트
    // PlayerHealth _health;           // 체력
    PlayerMovement _movement;       // 이동
    PlayerAttack _attack;           // 공격

    // 상태 제어
    public bool InputEnabled { get; set; } = true;
    
    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _attack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {   
        // 입력 비활성화 상태(컷신 재생 등)
        if (!InputEnabled)
        {
            _movement.SetMoveInput(0f);
            return;
        }

        HandleMovementInput();
        HandleAttackInput();
    }

    private void HandleMovementInput()
    {
        // 이동 입력
        float xInput = Input.GetAxisRaw("Horizontal");
        _movement.SetMoveInput(xInput);

        // 점프 입력
        if (Input.GetKeyDown(KeyCode.W))
            _movement.TryJump();
    }

    private void HandleAttackInput()
    {
        // 공격 입력
        if (Input.GetMouseButtonDown(0))
            _attack.Attack();
    }
}
