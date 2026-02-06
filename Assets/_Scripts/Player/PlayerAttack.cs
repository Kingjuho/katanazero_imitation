using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        _anim.SetTrigger("Attack");
    }
}
