using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource audioSource; // 効果音再生用のAudioSource
    public AudioClip select;
    public AudioClip submit;
    public AudioClip menuOpen;
    public AudioClip cancel;
    public AudioClip door1;
    public AudioClip footStep1;
    public AudioClip equip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでもオブジェクトを保持
        }
        else
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip);
        }
    }

    // ボタンがクリックされたときの効果音
    public void PlaySelect(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(select);
    }

    // ボタンがクリックされたときの効果音
    public void PlaySubmit(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(submit);
    }

    // メニューが開いたときの効果音
    public void PlayMenuOpen(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(menuOpen);
    }

    // メニューが閉じたときの効果音
    public void PlayCancel(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(cancel);
    }

    public void DoorOpen1(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(door1);
    }

    public void Equip(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(equip);
    }
}