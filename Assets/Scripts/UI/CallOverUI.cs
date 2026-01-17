using UnityEngine;

public class CallGameOverUI : MonoBehaviour
{
    [SerializeField] private Vector3 firstPosition = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private bool isCatched = false;//敵に捕まったかどうか
    [SerializeField] private float uiAppearFrame = 2.0f;//UIの出現時間    
    [SerializeField] private float decelTimeSpeed = 1.0f;//減速する速さ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //UIを初期位置に設定(画面外)
        transform.position = firstPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //捕まった場合(仮)
        if (Input.GetKeyDown(KeyCode.Z))
        {
#if UNITY_EDITOR
            isCatched = true; //捕まったことにする
#endif
        }
        //捕まっているなら
        if (isCatched)
        {
            uiAppearFrame -= decelTimeSpeed * Time.deltaTime;//UIの表示時間を減らしていく
            Vector3 newPosition = transform.position;        //オブジェクトの座標を代入
            newPosition = new Vector3(Screen.width * 0.5f,
                           Screen.height * 0.5f, 0);         //宣言した変数に原点を代入             
            transform.position = newPosition;                //新しく作った変数をオブジェクトに入れなおす
        }
        //UIの表示時間が0以下になったら
        if (uiAppearFrame <= 0.0f)
        {
            // すでに実装されている(-Zan)

            //Destroy(gameObject); //オブジェクトを破棄する
            //SceneManager.LoadScene("Title");
        }
    }
}
