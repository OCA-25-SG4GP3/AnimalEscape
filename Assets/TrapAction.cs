using Unity.VisualScripting;
using UnityEngine;

public class TrapAction : MonoBehaviour
{
    public bool isTouch = false;     //プレイヤーが罠にかかったかどうか 
    public float freezeTime = 10.0f;  //プレイヤーが罠にかかった時の停止時間
    private float freezeCount = 0.0f;//プレイヤーが罠にかかっている時間

    void Update()
    {
        //プレイヤーが罠につかまっているなら
        if(isTouch)
        {
            //止まっている時間を増やす
            freezeCount += Time.deltaTime;
            //止まっている時間が既定の停止時間を超えたら
            if(freezeCount >= freezeTime)
            {
                //プレイヤーの拘束を解除する
                isTouch = false;                                
            }
        }
        else 
        {
            //プレイヤーが罠にかかってないならなにもしない
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 触れてきたオブジェクトのタグが "Player" であるかを確認
        if (other.gameObject.tag =="Player")
        {
            //自身を破棄破棄するのは
            Destroy(gameObject);
            Debug.Log("罠に触れたので罠が撤去されました");
            //罠にかかった状態にする
            isTouch = true;
        }
    }
}

