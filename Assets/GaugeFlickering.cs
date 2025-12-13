using UnityEngine;
using UnityEngine.UI;

public class GaugeFlickering : MonoBehaviour
{
    [SerializeField]private Animator anim;
    [SerializeField]private Slider zooKeeperSlide;

    private const float ONE_THIRD = 0.666666f;    

    //Animator animator = GetComponent<Animator>();
    //animator.Play("GaugeFlickerImage");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //残りのゲージが三分の一以下なら
        if(zooKeeperSlide.value >= zooKeeperSlide.maxValue* ONE_THIRD)
        //if(zooKeeperSlide.value >= 100.0f)
        {
            anim.Play("GaugeFlickerImage");
        }
        
    }
}
