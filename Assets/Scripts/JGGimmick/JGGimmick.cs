using UnityEngine;
using System.Collections.Generic;

public class JGGimmick : MonoBehaviour
{
    // 使用しないリスト(色をランダムにさせる用)
    public List<int> numberList;
    // 使用するリスト
    public List<int> usenumberList = new List<int>();
    private int randomNum;
    private int choiceNum;

    // 使用するマテリアルの数
    public int UseMatNum;
    // マテリアルは4種類0～3
    [SerializeField] Material[] materialArray = new Material[4];
    Material cubeMaterial;
    private int count;

    // ボタンの正解数
    private int trueBottonNum;

    // クリア判定
    bool IsGimmickClear;
    // タイマー
    float timer;

    public int ColorChangeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;

        trueBottonNum = 1;

        IsGimmickClear = false;

        count = 0;

        // 0は初期色固定(白)
        usenumberList.Add(0);
        for (int i = 1; i < UseMatNum; i++)
        {
            //numberListの中から0以外ランダムで1つを選ぶ
            randomNum = numberList[Random.Range(1, numberList.Count)];
            //選んだオブジェクトをuseListに追加
            usenumberList.Add(randomNum);
            //選んだ数のリスト番号を取得
            choiceNum = numberList.IndexOf(randomNum);
            //同じリスト番号をnumberListから削除
            numberList.RemoveAt(choiceNum);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //マテリアル変更関係

        {
            // 時間経過
            timer += Time.deltaTime;

            // 色が変更するまでの時間
            if (timer >= ColorChangeTime)
            {
                timer = 0;
                count++;
                // 参照するマテリアル番号
                if (count > materialArray.Length - 1)
                {
                    count = 0;
                }
                // マテリアル変更
                GetComponent<MeshRenderer>().material = materialArray[usenumberList[count]];
            }
        }

        // ギミッククリア
        if (IsGimmickClear == true)
        {
            Debug.Log("ギミッククリア");
        }

    }

    // 押したボタンが正しいかどうか
    public void CheckBotton(int bottonNum)
    {
        if (bottonNum == usenumberList[trueBottonNum])
        {
            // 正しければtrueBottonNumを1足して次のステップに
            trueBottonNum++;

            // trueBottonNumがマテリアルと同じになればクリア
            if (trueBottonNum == UseMatNum)
            {
                IsGimmickClear = true;
            }
        }
        else
        {
            // 間違っていれば最初から
            trueBottonNum = 1;
        }
    }
}
