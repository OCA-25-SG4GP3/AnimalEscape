using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioClipEventSO _playBGMEvent;
    [SerializeField] private AudioClipEventSO _playSFXEvent;
    [SerializeField] private VoidEventSO _stopBGMEvent;
    [SerializeField] private VoidEventSO _stopSFXEvent;

    private void OnEnable()
    {
        _playBGMEvent.OnEventInvoked += PlayBGM;
        _playSFXEvent.OnEventInvoked += PlaySFX;
        _stopBGMEvent.OnEventInvoked += StopBGM;
        _stopSFXEvent.OnEventInvoked += StopSFX;
    }

    private void OnDisable()
    {
        _playBGMEvent.OnEventInvoked -= PlayBGM;
        _playSFXEvent.OnEventInvoked -= PlaySFX;
        _stopBGMEvent.OnEventInvoked -= StopBGM;
        _stopSFXEvent.OnEventInvoked -= StopSFX;
    }

    private void PlayBGM(AudioClip clip, bool isLoop)
    {
        if (_bgmAudioSource.clip == clip) return;

        _bgmAudioSource.clip = clip;
        _bgmAudioSource.loop = isLoop;
        _bgmAudioSource.Play();
    }

    private void PlaySFX(AudioClip clip, bool isLoop)
    {
        if (isLoop)
        {
            if (_sfxAudioSource.clip == clip && _sfxAudioSource.isPlaying) return;

            _sfxAudioSource.clip = clip;
            _sfxAudioSource.loop = isLoop;
            _sfxAudioSource.Play();
        }
        else
        {
            _sfxAudioSource.PlayOneShot(clip);
        }
    }

    private void StopBGM()
    {
        _bgmAudioSource.Stop();
    }

    private void StopSFX()
    {
        _sfxAudioSource.Stop();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            _bgmAudioSource.mute = true;
            _sfxAudioSource.mute = true;
        }
        else
        {
            _bgmAudioSource.mute = false;
            _sfxAudioSource.mute = false;
        }
    }
}
