using UnityEngine;

public class OpeningCameraAction : MonoBehaviour
{
    Vector3 defaultSpeed = new Vector3(0.01f, 0.01f, 0.01f);

    public float cameraSpeed = 0.01f; //カメラの移動速度  
    public float waitTimer = 0.0f;    //カメラがターゲットについた時の停止時間 
    private float waitTimerCount = 0.0f;
    private int targetNumber = 0;

    [SerializeField] GameObject pointA;
    [SerializeField] GameObject pointB;
    [SerializeField] GameObject pointC;
    [SerializeField] GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //速度に*0.9し続ければ減速すると思う


    }

    // Update is called once per frame
    void Update()
    {
        //ターゲットへのベクトルを求める
        //Vector3 toTarget =  pointA.transform.position - transform.position;
        //toTarget.Normalize();
        //transform.Translate(toTarget.x*cameraSpeed, toTarget.y * cameraSpeed, toTarget.z * cameraSpeed);

        //一つ目のターゲットに着いたらターゲットカウントを一つ増やす
        SetCameraTarget();
    }

    void SetCameraTarget()
    {
        switch (targetNumber)
        {
            case 0:
                {
                    Vector3 current = transform.position;
                    Vector3 target = new Vector3(pointA.transform.position.x,
                        pointA.transform.position.y + 7.0f,
                        pointA.transform.position.z - 7.0f);
                    float step = 3.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

                    if (transform.position == target)
                    {
                        waitTimerCount += 0.1f;
                        if (waitTimerCount >= waitTimer)
                        {
                            targetNumber++;
                            waitTimerCount = 0.0f;
                        }

                    }
                }
                break;

            case 1:
                {
                    Vector3 current = transform.position;
                    Vector3 target = new Vector3(pointB.transform.position.x,
                        pointB.transform.position.y + 7.0f,
                        pointB.transform.position.z - 7.0f);
                    float step = 3.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

                    if (transform.position == target)
                    {
                        waitTimerCount += 0.1f;
                        if (waitTimerCount >= waitTimer)
                        {
                            targetNumber++;
                            waitTimerCount = 0.0f;
                        }
                    }
                }
                break;
            case 2:
                {
                    Vector3 current = transform.position;
                    Vector3 target = new Vector3(pointC.transform.position.x,
                        pointC.transform.position.y + 7.0f,
                        pointC.transform.position.z - 7.0f);
                    float step = 3.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

                    if (transform.position == target)
                    {
                        waitTimerCount += 0.1f;
                        if (waitTimerCount >= waitTimer)
                        {
                            targetNumber++;
                            waitTimerCount = 0.0f;
                        }
                    }
                }
                break;
            case 3:
                {
                    Vector3 current = transform.position;
                    Vector3 target = new Vector3(player.transform.position.x,
                        player.transform.position.y + 7.0f,
                        player.transform.position.z - 7.0f);
                    float step = 3.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

                    //if (transform.position == target)
                    //{
                    //    waitTimerCount += 0.1f;
                    //    if (waitTimerCount >= waitTimer)
                    //    {
                    //        targetNumber++;
                    //    }
                    //}
                }
                break;
        }

    }
}
