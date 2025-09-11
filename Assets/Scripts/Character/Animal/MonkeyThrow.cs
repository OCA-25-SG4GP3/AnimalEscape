using UnityEngine;

public class MonkeyThrow : MonoBehaviour
{
    [SerializeField] private GameObject thrownObjPrefab;
    [SerializeField] private GameObject throwPositionObj;
    [SerializeField] private float throwForce = 1000.0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Throw();
        }
    }

    void Throw()
    {
        GameObject inst = Instantiate(thrownObjPrefab, throwPositionObj.transform.position, throwPositionObj.transform.rotation);
        Rigidbody rb = inst.GetComponent<Rigidbody>();
        rb.AddForce(throwPositionObj.transform.forward * throwForce, ForceMode.Force);
    }
}
