using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JGGimmickBotton : MonoBehaviour
{
    public int BottonNum;



    GameObject _parent;//  = transform.root.gameObject;
    //JGGimmick o = _parent.GetComponent<JGGimmick>();


    // 次に当たるまでの時間
    //float timer;
    // 当たるまでのクールダウン
    //bool hitcd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _parent = transform.root.gameObject;

        //hitcd = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (hitcd)
        //{
        //    timer += Time.deltaTime;
        //    if (timer >= 1.0f)
        //    {
        //        hitcd = false;
        //    }
        //}
    }

    void OnCollisionExit()
    {
        JGGimmick jGGimmick = _parent.GetComponent<JGGimmick>();
        //if (hitcd == false)
        //{
            jGGimmick.CheckBotton(BottonNum);
            //hitcd = true;
            Debug.Log("ボタン"+BottonNum);
        //}
        //hitcd = true;
    }

    
}
