using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    //ゲームオーバー、クリアと同じように、少し止めてから処理
    [SerializeField] public bool isApear = false;  //UI表示中かどうか
    [SerializeField] private Vector3 firstPosition = new Vector3(0.0f, 0.0f, 0.0f);    
    [SerializeField] private float uiAppearSeconds = 4.0f;//UIの表示時間 (秒)
    [SerializeField] private float decelTimeSpeedNDeltaTime = 1.0f;//表示時間を減らす速度                                                                      

    [SerializeField] private GameObject fadeObject;

    private void Awake()
    {
        //UIを初期位置に設定
        transform.position = firstPosition;
        //UI表示
        isApear = true;        
    }

    // Update is called once per frame
    void Update()
    {
        //UI表示中の処理
        if (isApear)
        {                       
            //UIの表示時間を減らす
            uiAppearSeconds -= decelTimeSpeedNDeltaTime * Time.unscaledDeltaTime;                    

            //UIを画面中央に表示
            Vector3 newPosition = transform.position;
            newPosition = new Vector3(Screen.width * 0.5f,
                           Screen.height * 0.5f, 0);
            transform.position = newPosition;
        }
        if (uiAppearSeconds <= 2.0f)
        {
            //Destroy(fadeObject); //UI削除
        }

        //表示時間が0以下になったら
        if (uiAppearSeconds <= 0.0f)
        {            
            Time.timeScale = 1;　//ゲーム再開            
            isApear = false;     //表示終了            
            Destroy(gameObject); //UI削除       
        }

        //UI表示注はゲーム停止
        if (uiAppearSeconds > 0.0f)
        {
            //�o�����Ă���Ԃ̓Q�[���̎��Ԃ��~�߂�
            Time.timeScale = 0;
        }
    }
}
