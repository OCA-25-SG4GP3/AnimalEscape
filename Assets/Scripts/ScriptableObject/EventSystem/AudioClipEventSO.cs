using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AudioClipEventSO", menuName = "Event/AudioClipEventSO")]
public class AudioClipEventSO : DescriptionBaseSO
{
    public UnityAction<AudioClip, bool> OnEventInvoked;

    public void InvokeEvent(AudioClip audio, bool isLoop = false)
    {
        OnEventInvoked?.Invoke(audio, isLoop);
    }
}
