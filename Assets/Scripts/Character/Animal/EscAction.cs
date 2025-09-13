using UnityEngine;
using System.Collections;

public class EscapeAnimalAction : MonoBehaviour
{
    //=============================================
    public bool IsFree = false;  //動物が折から出ているかどうか
    public float MoveSpeed = 0.1f;     
    public float ChangeInterval = 3.0f;
    private Vector3 moveDirection;       
    //==============================================
    // 前回の行動（true=移動, false=停止）
    private bool wasMoving = false;
    // 次の行動までの残り時間
    private float timer;
    //==============================================
    // 行動を開始する範囲の中心点
    //public Transform CenterPoint;
    // 行動を開始する範囲の半径
    public float EscapeRadius = 15.0f;
    // GoalオブジェクトのTransform
    private Transform goalTransform;
    // 脱出処理が完了したかどうかのフラグ
    private bool hasEscaped = false;
    //==============================================
    // 自身を破棄する距離
    public float destructionDistance = 2.0f;
    //==============================================

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {       
        //=================================================================
        // 初期タイマーを設定
        timer = ChangeInterval;
        //=================================================================
        // "Goal"タグのオブジェクトを探して取得
        GameObject goalObject = GameObject.FindGameObjectWithTag("Goal");
        if (goalObject != null)
        {
            goalTransform = goalObject.transform;
        }
        else
        {
            Debug.LogError("Goalオブジェクトが見つかりません。");
        }
    }
    // Update is called once per frame
    void Update()
    {         
        //動物が檻から出ていないなら
        if(!IsFree)
        {
            CageAnimalAct();
        }
        else
        {
            // 檻から出て、かつGoalオブジェクトが存在する場合
            if (goalTransform != null)
            {
                // 逃げられてない場合檻からの脱出処理
                if (!hasEscaped)
                {
                    EscapeFromRange();
                }
                // 逃げれてる場合ゴールへの追跡処理
                else
                {
                    MoveToGoal();
                }
            }
        }        
        //ゴールに到着したら自身を破棄する処理
        MoveToDestory();
    }

    // 指定範囲からの脱出処理
    private void EscapeFromRange()
    {
        // 中心点から外へワープするランダムな方向を計算
        Vector3 randomDirection = Random.onUnitSphere;
        // Y軸の方向は0にして地面を移動するようにする
        randomDirection.y = 0;
        randomDirection.Normalize();

        // 中心点から指定された半径の外側にワープする位置を決定
        //Vector3 warpPosition = CenterPoint.position + randomDirection * EscapeRadius;
        Vector3 warpPosition = transform.position + randomDirection * EscapeRadius;

        // オブジェクトを新しい位置へ瞬間移動させる
        transform.position = new Vector3(warpPosition.x,1.5f,warpPosition.z);

        // ワープ完了フラグを立てる
        IsFree = true;
        hasEscaped = true;
        Debug.Log("範囲外へのワープが完了しました。");
    }

    // Goalオブジェクトへの移動処理
    private void MoveToGoal()
    {
        // Goalへの方向を計算
        Vector3 directionToGoal = (goalTransform.position - transform.position).normalized;
        // その方向に移動
        transform.position += directionToGoal * MoveSpeed * Time.deltaTime;
    }

    //ゴールに着いたら自身を破棄する処理
    void MoveToDestory()
    {
        // 自身とゴールオブジェクトの距離を計算
        float distanceToGoal = Vector3.Distance(transform.position, goalTransform.position);

        // 距離が指定した値以下になったら自身を破棄
        if (distanceToGoal <= destructionDistance)
        {
            // 自身を破棄
            Destroy(gameObject);
            Debug.Log("ゴールに到達したため、オブジェクトを破棄しました。");
        }
    }

    private void DecideNextAction()
    {
        // 次の行動が「移動」になる確率
        float moveChance = 0.5f;

        // もし前回の行動が「停止」だった場合、次の行動が「移動」になる確率を高くする
        if (!wasMoving)
        {
            moveChance = 0.8f; // 例：停止の後は80%の確率で移動
        }
        // 確率に基づいて行動を決定
        if (Random.value < moveChance)
        {
            // 移動する場合
            // 新しいランダムな移動方向を決定
            moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            wasMoving = true;
            //Debug.Log("移動します"); //一旦消します(Zan)
        }
        else
        {
            // 停止する場合
            // 移動方向をゼロにする
            moveDirection = Vector3.zero;
            wasMoving = false;
            //Debug.Log("停止します"); //一旦消します(Zan)
        }
    }
    void CageAnimalAct()
    {
        // タイマーを減らす
        timer -= Time.deltaTime;

        // タイマーが0以下になったら新しい行動を決定する
        if (timer <= 0)
        {
            DecideNextAction();
            // タイマーをリセット
            timer = ChangeInterval;
        }
        // 現在の移動方向に基づいてオブジェクトを移動させる
        transform.position += moveDirection * MoveSpeed * Time.deltaTime;

        
    }
    void OnCollisionEnter(Collision collision)
    {
        //"Wall"と衝突したら
        // 衝突したオブジェクトのタグが "Wall" であるかチェック
        if (collision.gameObject.CompareTag("Wall"))
        {           
            // X軸とZ軸の移動方向を反転         
            moveDirection.x *= -1.0f;
            moveDirection.z *= -1.0f;                      
        }
    }
}

