using UnityEngine;

public class PlayerDistanceManager : MonoBehaviour //またはPlayerManager
{
    [SerializeField] Transform _player1; public Transform Player1 => _player1;
    [SerializeField] Transform _player2; public Transform Player2 => _player2;
    [SerializeField] float _maxDistance = 15f;

    void Start()
    {
        // Get players from PlayerInputManager if not assigned
        if (_player1 == null || _player2 == null)
        {
            var playerInputManager = PlayerInputManager.Instance;
            if (playerInputManager != null)
            {
                if (_player1 == null && playerInputManager.Player1 != null)
                    _player1 = playerInputManager.Player1.transform;
                if (_player2 == null && playerInputManager.Player2 != null)
                    _player2 = playerInputManager.Player2.transform;
            }
        }
    }

    void LateUpdate()
    {
        // If only one player �� do nothing
        if (_player1 == null || _player2 == null) return;

        Vector3 dir = _player2.position - _player1.position;
        float distance = dir.magnitude;

        if (distance > _maxDistance)
        {
            Vector3 midpoint = (_player1.position + _player2.position) / 2f;
            dir.Normalize();

            _player1.position = midpoint - dir * _maxDistance / 2f;
            _player2.position = midpoint + dir * _maxDistance / 2f;
        }
    }

    public bool HaveAllPlayersCaught() //全員捕まえたか TODO move this to GameManager
    {
        return Player1.GetComponent<PlayerInfo>().hasCaught && Player2.GetComponent<PlayerInfo>().hasCaught;
    }
}
