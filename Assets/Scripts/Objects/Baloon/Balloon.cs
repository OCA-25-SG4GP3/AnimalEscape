using UnityEngine;

public class Balloon : MonoBehaviour
{
    private bool _isLucky = false;       // 当たり風船かどうか
    private GameObject _keyPrefab;       // 鍵プレハブ

    // BalloonManager から当たりを設定する
    public void SetAsLucky(GameObject key)
    {
        _isLucky = true;
        _keyPrefab = key;
    }

    // 他のオブジェクト（例えばShot）とぶつかったとき呼ばれる
    void OnCollisionEnter(Collision collision)
    {
        // ぶつかった相手が "Shot" タグを持っていたら
        if (collision.gameObject.CompareTag("Shot"))
        {
            DestroyByPlayerShot(); //そとから呼ばれる可能性があるので、関数化にしました（Zan）
        }
    }

    public void DestroyByPlayerShot()
    {
        // 当たり風船なら鍵を生成する
        if (_isLucky && _keyPrefab != null)
        {
            SpawnKey(); //そとから呼ばれる可能性があるので、関数化にしました（Zan）
        }

        // 自分（風船）を削除する
        Destroy(gameObject);
    }

    public GameObject SpawnKey()
    {
        return Instantiate(_keyPrefab, transform.position, Quaternion.identity);
    }
}
