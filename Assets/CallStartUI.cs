using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    private float time = 0.0f;
    private bool isGrind = false;
    [SerializeField] private float speed = 300.0f;
    
    private void Awake()
    {
        //‰ŠúˆÊ’uÝ’è
        Vector3 newPosition = transform.position;
        newPosition = new Vector3(-(Screen.width * 0.5f),Screen.height * 0.5f,0.0f);
        transform.position = newPosition;
        isGrind = true;
    }

    // Update is called once per frame
    void Update()
    {   
        if(transform.position.x >= Screen.width * 2.0f)
        { 
            Destroy(this.gameObject);
        }
        //transform += Vector2(0.1f,0.1f);
        Vector3 newPosition = transform.position;
        newPosition.x += speed * Time.deltaTime;
        transform.position = newPosition;
    }

}
