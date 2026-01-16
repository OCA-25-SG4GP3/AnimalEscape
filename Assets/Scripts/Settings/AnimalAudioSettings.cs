using UnityEngine;

/// <summary>
/// Audio settings for animals - SE担当用
/// Create assets: Right-click → Create → Animal → AudioSettings
/// </summary>
[CreateAssetMenu(fileName = "AnimalAudioSettings", menuName = "Animal/AudioSettings")]
public class AnimalAudioSettings : ScriptableObject
{
    [Header("効果音 (Sound Effects)")]
    public AudioClip jumpSound;      // ジャンプ音
    public AudioClip landingSound;   // 着地音
    public AudioClip walkSound;      // 歩行音

    [Header("音声設定 (Audio Settings)")]
    [Range(0.1f, 3f)]
    public float soundPitch = 2.5f;

    [Range(0f, 1f)]
    public float volume = 1.0f;
}
