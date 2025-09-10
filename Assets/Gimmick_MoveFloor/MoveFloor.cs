using UnityEngine;

public class MoveFloor : MonoBehaviour
{
    Rigidbody rb;

    public string move_Key;
    public float move_Speed;
    public float change_Scale_Speed;
    public float stop_Time;

    float timer;


    Vector3 PrevPosition; // 1フレーム前の位置
    Vector3 FirstPos;	// 初期位置
    Vector3 Scale; // 初期サイズ

    enum MODE
    {
        WAIT, // 押されるまで待機
        MOVE, // 移動
        HITWALL, // 一定時間停止
        CHANGESCALE, // スケールを0まで徐々に減少させる
        RESET // 初期状態
    }
    MODE FloorMode; //状態


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Transform myTransform = this.transform;
        FirstPos = myTransform.position;
        Scale = transform.localScale; 
        PrevPosition = transform.position;
        FloorMode = MODE.WAIT;
    }

    // 移動関数
    void Movefloor()
    {

        // 前進
        rb.linearVelocity = move_Speed * transform.forward;
        // 現在位置取得
        Vector3 position = transform.position;

        // 前進できなければHITWALLに移行
        float dis = Vector3.Distance(position, PrevPosition);

        // 現在速度計算
        var velocity = (position - PrevPosition) / Time.deltaTime;

        if (dis < 0.001f)
        {
            FloorMode = MODE.HITWALL;
        }

        // 前フレーム位置を更新
        PrevPosition = position;
    }

    // 一定時間停止関数
    void stopfloor()
    {
        timer += Time.deltaTime;
        if(stop_Time <= timer)
        {
            timer = 0.0f;
            FloorMode = MODE.CHANGESCALE;
        }
    }

    // サイズ変更関数
    void Changescale()
    {
        Vector3 localScale = transform.localScale;
        localScale.x -= change_Scale_Speed * Time.deltaTime;
        localScale.y -= change_Scale_Speed * Time.deltaTime;
        localScale.z -= change_Scale_Speed * Time.deltaTime;
        transform.localScale = localScale;
        if (transform.localScale.x <= 0.01f)
        {
            FloorMode = MODE.RESET;
        }
    }

    void Reset()
    {
        transform.position = FirstPos;
        transform.localScale = Scale;
        FloorMode = MODE.WAIT;
    }

    // Update is called once per frame
    void Update()
    {
        switch(FloorMode)
        {
            case MODE.WAIT:
                if (move_Key == "q" || move_Key == "Q")
                {
                    if (Input.GetKey(KeyCode.Q))
                    {
                        FloorMode = MODE.MOVE;
                    }
                }
                if (move_Key == "w" || move_Key == "W")
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        FloorMode = MODE.MOVE;
                    }
                }
                if (move_Key == "e" || move_Key == "E")
                {
                    if (Input.GetKey(KeyCode.E))
                    {
                        FloorMode = MODE.MOVE;
                    }
                }
                break;
            case MODE.MOVE:
                Movefloor();

                break;
            case MODE.HITWALL:
                stopfloor();
                break;
            case MODE.CHANGESCALE:
                Changescale();
                break;
            case MODE.RESET:
                Reset();
                break;
        }


        
    }
}
