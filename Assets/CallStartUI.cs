using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    //ゲームオーバー、クリアと同じようにぱっとだしてぱっと消すようにする
    [SerializeField] private Vector3 firstPosition = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private bool isApear = false;  //文字を出現させるか
    [SerializeField] private float uiAppearFrame = 2.0f;//UIの出現時間    
    [SerializeField] private float decelTimeSpeedNDeltaTime = 1.0f;//出現時間が減速する速さ

    private void Awake()
    {
        //初期位置設定
        //Vector3 newPosition = transform.position;
        //newPosition = new Vector3(-(Screen.width * 0.5f), Screen.height * 0.5f, 0.0f);
        //transform.position = newPosition;
        //isAppear = true;

        //UIを初期位置に設定(画面外)
        transform.position = firstPosition;        
        //始まった瞬間出現させたいので
        isApear = true;
    }    

    // Update is called once per frame
    void Update()
    {        
        if (isApear)
        { 
            //uiAppearFrame -= decelTimeSpeed * Time.deltaTime;//UIの表示時間を減らしていく
            uiAppearFrame -= decelTimeSpeedNDeltaTime;//UIの表示時間を減らしていく
            Vector3 newPosition = transform.position;        //オブジェクトの座標を代入
            newPosition = new Vector3(Screen.width * 0.5f,
                           Screen.height * 0.5f, 0);         //宣言した変数に原点を代入             
            transform.position = newPosition;                //新しく作った変数をオブジェクトに入れなおす                        
        }
        //UIの表示時間が0以下になったら
        if (uiAppearFrame <= 0.0f)
        {
            Time.timeScale = 1;//時間停止を解除して
            isApear = false;     //出現を解除して
            Destroy(gameObject); //オブジェクトを破棄する           
        }
    }
    private void FixedUpdate()
    {
        //UIの表示時間が0超過なら
        if (uiAppearFrame > 0.0f)
        {
            //出現している間はゲームの時間を止める
            Time.timeScale = 0;          
        }
    }
}
