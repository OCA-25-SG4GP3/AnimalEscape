using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private bool _isLucky = false;
    private GameObject _keyPrefab;

    // BalloonManager ã‹ã‚‰å½“ãŸã‚Šã‚’è¨­å®šã™ã‚‹
    public void SetAsLucky(GameObject key)
    {
        _isLucky = true;
        _keyPrefab = key;
    }

    // ä»–ã®ã‚ªãƒ–ã‚¸ã‚§ã‚¯ãƒˆï¼ˆä¾‹ãˆã°Shotï¼‰ã¨ã¶ã¤ã‹ã£ãŸã¨ãå‘¼ã°ã‚Œã‚‹
    void OnCollisionEnter(Collision collision)
    {
        // ã¶ã¤ã‹ã£ãŸç›¸æ‰‹ãŒ "Shot" ã‚¿ã‚°ã‚’æŒã£ã¦ã„ãŸã‚‰
        if (collision.gameObject.CompareTag("Shot"))
        {
            // å½“ãŸã‚Šé¢¨èˆ¹ãªã‚‰éµã‚’ç”Ÿæˆã™ã‚‹
            if (_isLucky && _keyPrefab != null)
            {
                Instantiate(_keyPrefab, transform.position, Quaternion.identity);
            }

            // è‡ªåˆ†ï¼ˆé¢¨èˆ¹ï¼‰ã‚’å‰Šé™¤ã™ã‚‹
            Destroy(gameObject);
        }
    }

    public void DestroyByPlayerShot()
    {
        // “–‚½‚è•—‘D‚È‚çŒ®‚ğ¶¬‚·‚é
        if (_isLucky && _keyPrefab != null)
        {
            SpawnKey(); //‚»‚Æ‚©‚çŒÄ‚Î‚ê‚é‰Â”\«‚ª‚ ‚é‚Ì‚ÅAŠÖ”‰»‚É‚µ‚Ü‚µ‚½iZanj
        }

        // ©•ªi•—‘Dj‚ğíœ‚·‚é
        Destroy(gameObject);
    }

    public GameObject SpawnKey()
    {
        return Instantiate(_keyPrefab, transform.position, Quaternion.identity);
    }
}
