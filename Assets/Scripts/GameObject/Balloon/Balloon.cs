using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private bool _isLucky = false;
    private GameObject _keyPrefab;

    public void SetAsLucky(GameObject key)
    {
        _isLucky = true;
        _keyPrefab = key;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shot"))
        {
            if (_isLucky && _keyPrefab != null)
            {
                Instantiate(_keyPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    public void DestroyByPlayerShot()
    {
        if (_isLucky && _keyPrefab != null)
        {
            SpawnKey();
        }
        Destroy(gameObject);
    }

    public GameObject SpawnKey()
    {
        return Instantiate(_keyPrefab, transform.position, Quaternion.identity);
    }
}