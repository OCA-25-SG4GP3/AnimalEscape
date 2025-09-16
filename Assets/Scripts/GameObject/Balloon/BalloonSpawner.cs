using System.Collections.Generic;
using UnityEngine;

public class BalloonManager : MonoBehaviour
{
    [SerializeField] private int _balloonMax;
    [SerializeField] private GameObject _balloonPrefab;
    [SerializeField] private GameObject _keyPrefab;

    [Header("スポーン範囲")]
    [SerializeField] private Transform _spawnMin;
    [SerializeField] private Transform _spawnMax;
    
    void Start()
    {
        // ランダムで当たり風船のインデックスを決める
        int luckyIndex = Random.Range(0, _balloonMax);

        for (int i = 0; i < _balloonMax; i++)
        {
            // ランダム座標を決定
            Vector3 pos = new Vector3(
                Random.Range(_spawnMin.position.x, _spawnMax.position.x),
                Random.Range(_spawnMin.position.y, _spawnMax.position.y),
                Random.Range(_spawnMin.position.z, _spawnMax.position.z)
            );

            // 風船を生成
            GameObject balloon = Instantiate(_balloonPrefab, pos, Quaternion.identity);

            // Balloon スクリプトを取得して初期化
            Balloon balloonScript = balloon.GetComponent<Balloon>();


            // 選ばれた数字の風船を当たりに設定
            if (i == luckyIndex)
            {
                balloonScript.SetAsLucky(_keyPrefab);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 center = (_spawnMin.position + _spawnMax.position) / 2f;
        Vector3 size = new Vector3(
            Mathf.Abs(_spawnMin.position.x - _spawnMax.position.x),
            Mathf.Abs(_spawnMin.position.y - _spawnMax.position.y),
            Mathf.Abs(_spawnMin.position.z - _spawnMax.position.z)
        );

        Gizmos.DrawWireCube(center, size);
    }
}
