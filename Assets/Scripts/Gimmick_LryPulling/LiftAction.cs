using UnityEngine;

public class LiftAction : MonoBehaviour
{
    const float base_pos_y = -1.5f;
    const float max_pos_y = 4.0f;

    public float speed = 0.5f; // è„Ç…ìÆÇ≠ë¨Ç≥Åi1ïbÇ†ÇΩÇËÇÃíPà Åj

    public bool lift_flag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lift_flag)
        {
            UpLift();
        }
        else
        {
            DownLift();
        }
    }

    void UpLift()
    {
        //è„å¿Ç‹Ç≈è„Ç™Ç¡ÇƒÇΩÇÁÇªÇÍà»è„è„Ç™ÇÁÇ»Ç¢
        Vector3 Up_move = transform.position + (Vector3.up * speed * Time.deltaTime);
        if (Up_move.y >= max_pos_y)
        {

        }
        //è„Ç∞ÇÈ
        else
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

    void DownLift()
    {
        //è„å¿Ç‹Ç≈è„Ç™Ç¡ÇƒÇΩÇÁÇªÇÍà»è„â∫Ç™ÇÁÇ»Ç¢
        Vector3 Down_move = transform.position - (Vector3.up * speed * Time.deltaTime);
        if (Down_move.y <= base_pos_y)
        {
            
        }
        //â∫Ç∞ÇÈ
        else
        {
            transform.Translate(-(Vector3.up * speed * Time.deltaTime));
        }
    }
}
