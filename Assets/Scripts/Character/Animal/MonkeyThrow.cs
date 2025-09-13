using UnityEngine;
using UnityEngine.UI;

public class MonkeyThrow : MonoBehaviour
{
    [SerializeField] private GameObject thrownObjPrefab;
    [SerializeField] private GameObject throwPositionObj;
    [SerializeField] private float throwForce = 1000.0f;
    [SerializeField] private Cooldown throwCd = new();
    [SerializeField] private Text cooldownText;

    void Update()
    {
        cooldownText.text = throwCd.GetCooldownRemainingSecond().ToString("F2") + "s";

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (!throwCd.IsCooldown)
            {
                Throw();
                throwCd.StartCooldown();
            }
        }
    }

    void Throw()
    {
        GameObject inst = Instantiate(thrownObjPrefab, throwPositionObj.transform.position, throwPositionObj.transform.rotation);
        Rigidbody rb = inst.GetComponent<Rigidbody>();
        rb.AddForce(throwPositionObj.transform.forward * throwForce, ForceMode.Force);
    }
}
