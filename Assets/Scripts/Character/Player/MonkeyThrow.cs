using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonkeyThrow : MonoBehaviour
{
    [SerializeField] private GameObject thrownObjPrefab;
    [SerializeField] private GameObject throwPositionObj;
    [SerializeField] private float throwForce = 1000.0f;
    [SerializeField] private Cooldown throwCd = new(1.8f);
    [SerializeField] private TMP_Text cooldownText;
    AnimalControlSimple animalControlSimple;
    void Awake()
    {
        animalControlSimple = GetComponent<AnimalControlSimple>();
    }

    void Update()
    {
        if (Input.GetKeyDown(animalControlSimple.inputKeys.specialAction))
        {
            if (!throwCd.IsCooldown)
            {
                Throw();
                throwCd.StartCooldown();
            }
        }

        float cdRemaining = throwCd.GetCooldownRemainingSecond();
        if (cdRemaining > 0)
        {
            cooldownText.text = cdRemaining.ToString("F2");
        }
        else cooldownText.text = "";

        PlayerInfo.UpdateCDText( cooldownText, throwCd);
    }

    void Throw()
    {
        GameObject inst = Instantiate(thrownObjPrefab, throwPositionObj.transform.position, throwPositionObj.transform.rotation);
        Rigidbody rb = inst.GetComponent<Rigidbody>();
        rb.AddForce(throwPositionObj.transform.forward * throwForce, ForceMode.Force);
    }
}
