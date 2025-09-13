using UnityEngine;

public class RopeAction2 : MonoBehaviour
{
    const float base_pos_y = 2.5f;
    const float max_pos_y = -2.5f;

    public float speed = -0.5f; // ��ɓ��������i1�b������̒P�ʁj

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
        //����܂ŏオ���Ă��炻��ȏ㉺����Ȃ�
        Vector3 Up_move = transform.position - (Vector3.up * speed * Time.deltaTime);
        if (Up_move.y <= max_pos_y)
        {

        }
        //������
        else
        {
            transform.Translate(-(Vector3.up * speed * Time.deltaTime));
            
        }
    }

    void UpRope()
    {
        //����܂ŏオ���Ă��炻��ȏ�オ��Ȃ�
        Vector3 Down_move = transform.position + (Vector3.up * speed * Time.deltaTime);
        if (Down_move.y >= base_pos_y)
        {

        }
        //�グ��
        else
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }
}
