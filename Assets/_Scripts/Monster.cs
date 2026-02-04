using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [SerializeField] Transform _target;     // 타겟
    NavMeshAgent _agent;                    // 네비메시 에이전트

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;  // 회전 비활성화
        _agent.updateUpAxis = false;    // Y축 회전 비활성화
    }

    void Update()
    {
        // 타겟 위치로 이동
        _agent.SetDestination(_target.position);
    }
}
