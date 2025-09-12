using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private bool _isLucky = false;
    private GameObject _keyPrefab;

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
            // 当たり風船なら鍵を生成する
            if (_isLucky && _keyPrefab != null)
            {
                Instantiate(_keyPrefab, transform.position, Quaternion.identity);
            }

            // 自分（風船）を削除する
            Destroy(gameObject);
        }
    }
}
