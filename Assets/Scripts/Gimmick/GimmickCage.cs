using UnityEngine;

public class GimmickCage : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onPickupKeyEvent;
    [SerializeField] private IntEventSO _onCageOpenEvent;
    [SerializeField] private bool _canOpen = false;
    [SerializeField] private int _gimmickID = 0;
    public const int MAX_TIME = 3;
    public float time = 0;

    public bool in_flag;

    private void OnEnable()
    {
        _onPickupKeyEvent.OnEventInvoked += Unlock;
    }

    private void OnDisable()
    {
        _onPickupKeyEvent.OnEventInvoked -= Unlock;
    }

    // Update is called once per frame
    void Update()
    {
        if (time >= MAX_TIME)
        {
            _onCageOpenEvent.InvokeEvent(_gimmickID);
            Destroy(gameObject);
        }

        if (in_flag)
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
        if (other.CompareTag("Player") && _canOpen) // "Player" タグで判定
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
    
    private void Unlock()
    {
        _canOpen = true;
    }
}
