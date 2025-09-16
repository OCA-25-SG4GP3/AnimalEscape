using UnityEngine;

public class MainGateController : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onAllGimmickClearEvent;

    private void OnEnable()
    {
        _onAllGimmickClearEvent.OnEventInvoked += OpenGate;
    }

    private void OnDisable()
    {
        _onAllGimmickClearEvent.OnEventInvoked -= OpenGate;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OpenGate()
    {
        Debug.Log("OpenGate");
        Destroy(gameObject);
    }
}
