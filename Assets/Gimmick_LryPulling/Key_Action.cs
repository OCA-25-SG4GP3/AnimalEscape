using UnityEngine;

public class Key_Action : MonoBehaviour
{
    public const int MAX_TIME = 3; 
    public float time = 0;

    public bool in_flag;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(time >= MAX_TIME)
        {
            Debug.Log("Œ®‚ªŠJ‚«‚Ü‚µ‚½");
        }

        if(in_flag)
        {
            if (time <= MAX_TIME)
            {
                time += Time.deltaTime;
            }
            else
            {
                time = MAX_TIME;
            }
        }
        else
        {
            if (time >= 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                time = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // "Player" ƒ^ƒO‚Å”»’è
        {
            in_flag = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            in_flag = false;
            
        }
    }
}
