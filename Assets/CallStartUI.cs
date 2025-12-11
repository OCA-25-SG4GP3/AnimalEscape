using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    //ゲームオーバー、クリアと同じようにぱっとだしてぱっと消すようにする
    [SerializeField] private Vector3 firstPosition = new Vector3(0.0f, 0.0f, 0.0f);
    //[SerializeField] private bool isCatched = false;//敵に捕まったかどうか
    //[SerializeField] private float uiAppearFrame = 2.0f;//UIの出現時間    
    //[SerializeField] private float decelTimeSpeed = 1.0f;//減速する速さ

    private float time = 0.0f;
    private bool isAppear = false;
    [SerializeField] private float speed = 300.0f;
    [SerializeField] private GameObject StartUiParent;
    
    private void Awake()
    {
        //初期位置設定
        Vector3 newPosition = transform.position;
        newPosition = new Vector3(-(Screen.width * 0.5f), Screen.height * 0.5f, 0.0f);
        transform.position = newPosition;

        isAppear = true;

        //UIを初期位置に設定(画面外)
        transform.position = firstPosition;        
    }

    // Update is called once per frame
    void Update()
    {   
        if(transform.position.x >= Screen.width * 2.0f)
        { 
            //Destroy(this.gameObject);
            Destroy(StartUiParent);
        }
        //transform += Vector2(0.1f,0.1f);
        Vector3 newPosition = transform.position;
        newPosition.x += speed * Time.deltaTime;
        transform.position = newPosition;
    }
}
