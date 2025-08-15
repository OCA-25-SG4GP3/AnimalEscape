using UnityEngine;

public class RopeAction2 : MonoBehaviour
{
    const float base_pos_y = 2.5f;
    const float max_pos_y = -2.5f;

    public float speed = -0.5f; // è„Ç…ìÆÇ≠ë¨Ç≥Åi1ïbÇ†ÇΩÇËÇÃíPà Åj

    public bool rope_flag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rope_flag)
        {
            DownRope();
        }
        else
        {
            UpRope();
        }
    }

    void DownRope()
    {
        //è„å¿Ç‹Ç≈è„Ç™Ç¡ÇƒÇΩÇÁÇªÇÍà»è„â∫Ç∞ÇÍÇ»Ç¢
        Vector3 Up_move = transform.position - (Vector3.up * speed * Time.deltaTime);
        if (Up_move.y <= max_pos_y)
        {

        }
        //â∫Ç∞ÇÈ
        else
        {
            transform.Translate(-(Vector3.up * speed * Time.deltaTime));
            
        }
    }

    void UpRope()
    {
        //è„å¿Ç‹Ç≈è„Ç™Ç¡ÇƒÇΩÇÁÇªÇÍà»è„è„Ç™ÇÁÇ»Ç¢
        Vector3 Down_move = transform.position + (Vector3.up * speed * Time.deltaTime);
        if (Down_move.y >= base_pos_y)
        {

        }
        //è„Ç∞ÇÈ
        else
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }
}
