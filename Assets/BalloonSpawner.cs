using UnityEngine;

public class BalloonManager : MonoBehaviour
{
    [SerializeField] private int        _balloonMax;            // 風船の最大数
    [SerializeField] private GameObject _balloonPrefab;        // 風船プレハブ
    [SerializeField] private GameObject _keyPrefab;            // 鍵プレハブ

    [Header("スポーン範囲")]
    [SerializeField] private Vector3 _spawnMin;   // スポーン範囲の最小値
    [SerializeField] private Vector3 _spawnMax;   // スポーン範囲の最大値

    void Start()
    {
        // ランダムで当たり風船のインデックスを決める
        int luckyIndex = Random.Range(0, _balloonMax);

        for (int i = 0; i < _balloonMax; i++)
        {
            // ランダム座標を決定
            Vector3 pos = new Vector3(
                Random.Range(_spawnMin.x, _spawnMax.x),
                Random.Range(_spawnMin.y, _spawnMax.y),
                Random.Range(_spawnMin.z, _spawnMax.z)
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
}
