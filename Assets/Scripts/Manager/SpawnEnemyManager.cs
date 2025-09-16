using System.Collections;
using UnityEngine;

public class SpawnEnemyManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField, Min(0.05f)] private float _spawnInterval = 1f; // 秒
    [SerializeField] private int _maxEnemies = 0; // 0 = 無制限

    [SerializeField] private TransformEventSO _onEnemySpawnEvent;
    [SerializeField] private VoidEventSO _onEnemyStopSpawnEvent;

    private bool _isSpawning = false;
    private int _currentCount = 0;

    private void OnEnable()
    {
        _onEnemySpawnEvent.OnEventInvoked += StartSpawning;
        _onEnemyStopSpawnEvent.OnEventInvoked += StopSpawning;
    }

    private void OnDisable()
    {
        _onEnemySpawnEvent.OnEventInvoked -= StartSpawning;
        _onEnemyStopSpawnEvent.OnEventInvoked -= StopSpawning;
    }

    public void StartSpawning(Transform spawnPoint)
    {
        _spawnPoint = spawnPoint;
        if (!_isSpawning) StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        _isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnLoop()
    {
        _isSpawning = true;

        while (_isSpawning)
        {
            if (_maxEnemies <= 0 || _currentCount < _maxEnemies)
            {
                SpawnOne();
            }

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnOne()
    {
        Instantiate(_enemyPrefab, _spawnPoint.position + new Vector3(0, -_spawnPoint.position.y, -50), _spawnPoint.rotation);
        _currentCount++;

        // 敵が破壊/無効化されたらカウントを減らすサンプル（敵スクリプト側で呼ぶ想定）
        // 例: Enemy.OnDie += () => _currentCount--;
        // ここでは簡単に、破棄時に減らすコンポーネントを付ける案を書いておくか検討してください.
    }
}
