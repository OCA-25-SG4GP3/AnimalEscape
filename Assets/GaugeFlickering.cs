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
            PlayGaugeFlicker();
        }

    }

    public void PlayGaugeFlicker() //一応デバグからも呼びますので、publicにしています
    {
        anim.Play("GaugeFlickerImage");
    }
}
