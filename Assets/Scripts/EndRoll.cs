using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EndRoll : MonoBehaviour
{
    [SerializeField] float textScrollSpeed = 30;
    [SerializeField] float limitPosition = 730f;
    private bool isStopEndRoll;
    private Coroutine endRollCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //　エンドロールが終了した時
        if (isStopEndRoll)
        {
            endRollCoroutine = StartCoroutine(GoToNextScene());
        }
        else
        {
            //　エンドロール用テキストがリミットを越えるまで動かす
            if (transform.position.y <= limitPosition)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + textScrollSpeed * Time.deltaTime);
            }
            else
            {
                isStopEndRoll = true;
            }
        }
    }

    IEnumerator GoToNextScene()
    {
        //　5秒間待つ
        yield return new WaitForSeconds(5f);

        if (Input.GetKeyDown("space"))
        {
            StopCoroutine(endRollCoroutine);
            SceneManager.LoadScene("EndRollStartScene");
        }

        yield return null;
    }
}
