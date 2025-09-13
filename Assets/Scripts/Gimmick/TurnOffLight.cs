using UnityEngine;

public class TurnOffLight : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onAlertTriggerEvent;
    [SerializeField] private VoidEventSO _onCageOpenEvent;

    private Light _light;

    private void OnEnable()
    {
        _onAlertTriggerEvent.OnEventInvoked += TurnOff;
        _onCageOpenEvent.OnEventInvoked += TurnOn;
    }

    private void OnDisable()
    {
        _onAlertTriggerEvent.OnEventInvoked -= TurnOff;
        _onCageOpenEvent.OnEventInvoked -= TurnOn;
    }

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void TurnOff()
    {
        _light.enabled = false;
    }

    private void TurnOn()
    {
        _light.enabled = true;
    }
}
