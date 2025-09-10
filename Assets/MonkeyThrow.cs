using UnityEngine;

public class MonkeyThrow : MonoBehaviour
{
    [SerializeField] private GameObject thrownObjPrefab;
     [SerializeField] private GameObject throwPositionObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Throw();
        }
    }

    void Throw()
    {
        GameObject inst = Instantiate(thrownObjPrefab, throwPositionObj.transform.position, transform.rotation);
    }
}
