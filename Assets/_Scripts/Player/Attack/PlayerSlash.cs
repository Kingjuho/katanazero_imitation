using UnityEngine;

public class PlayerSlash : MonoBehaviour
{
    private Transform _player;
    private Vector3 _offset;

    public void Initialize(Transform player, Quaternion rotation)
    {
        _player = player;
        transform.position = _player.position;
        transform.rotation = rotation;
    }

    private void Update()
    {
        // 플레이어가 존재하면 따라다니도록
        if (_player != null)
            transform.position = _player.position;
        else
            Destroy();
    }

    public void Destroy() => Destroy(gameObject);
}
