using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    public float Angle = 45f;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player") //視界の範囲内の当たり判定
        {
            Debug.Log("視界の範囲内");

            //視界の角度内に収まっているか
            Vector3 posDelta = other.transform.position - transform.position;
            float playerAngle = Vector3.Angle(transform.forward, posDelta);

            if (playerAngle < Angle) //player_angleがangleに収まっているかどうか
            {
                Debug.Log("監視カメラがプレイヤーを発見しました！");
                ////オブジェクトの色を赤に変更する
                //other.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }
}
