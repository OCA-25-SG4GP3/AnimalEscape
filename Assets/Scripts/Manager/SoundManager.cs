using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioClipEventSO _playBGMEvent;
    [SerializeField] private AudioClipEventSO _playSFXEvent;
    [SerializeField] private VoidEventSO _stopBGMEvent;
    [SerializeField] private VoidEventSO _stopSFXEvent;
    [SerializeField] private VoidEventSO _onOpenOptionMenuEvent;
    [SerializeField] private GameObject _optionMenu;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [Header("Temp")]
    [SerializeField] private GameObject _eventSystem;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(_eventSystem);
        _bgmVolumeSlider.value = _bgmAudioSource.volume;
        _sfxVolumeSlider.value = _sfxAudioSource.volume;
    }

    private void OnEnable()
    {
        _playBGMEvent.OnEventInvoked += PlayBGM;
        _playSFXEvent.OnEventInvoked += PlaySFX;
        _stopBGMEvent.OnEventInvoked += StopBGM;
        _stopSFXEvent.OnEventInvoked += StopSFX;
        _onOpenOptionMenuEvent.OnEventInvoked += OpenOptionMenu;
    }

    private void OnDisable()
    {
        _playBGMEvent.OnEventInvoked -= PlayBGM;
        _playSFXEvent.OnEventInvoked -= PlaySFX;
        _stopBGMEvent.OnEventInvoked -= StopBGM;
        _stopSFXEvent.OnEventInvoked -= StopSFX;
        _onOpenOptionMenuEvent.OnEventInvoked -= OpenOptionMenu;
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

    private void OpenOptionMenu()
    {
        _optionMenu.SetActive(true);
        _bgmVolumeSlider.Select();
    }

    private void StopBGM()
    {
        _bgmAudioSource.Stop();
    }

    private void StopSFX()
    {
        _sfxAudioSource.Stop();
    }

    public void ChangeBGMVolume()
    {
        _bgmAudioSource.volume = _bgmVolumeSlider.value;
    }

    public void ChangeSEVolume()
    {
        _sfxAudioSource.volume = _sfxVolumeSlider.value;
    }
}
