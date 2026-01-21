using UnityEngine;
using UnityEngine.UI;

public class GaugeFlickering : MonoBehaviour
{
    [SerializeField]private Animator anim;         //再生するアニメーションを取得してくる
    [SerializeField]private Slider zooKeeperSlide; //参照するスライダーを取得してくる
    private const float ONE_THIRD = 0.666666f; //1/3を再現するための定数    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //残りのゲージが三分の一以下なら
        if(zooKeeperSlide.value >= zooKeeperSlide.maxValue* ONE_THIRD)
        {
            //アニメーションの速度を一定に戻し再生を始める
            anim.speed = 1.0f;
            PlayGaugeFlicker();
        }
        else
        {
            //それ以外なら点滅アニメーションの速度を0にしてレイヤーと再生開始
            //位置を固定します。
            anim.speed = 0.0f;
            anim.Play("GaugeFlickerImage",0,0.0f);            
        }
    }

    public void PlayGaugeFlicker() //一応デバグからも呼びますので、publicにしています
    {
        anim.Play("GaugeFlickerImage");
    }
}
