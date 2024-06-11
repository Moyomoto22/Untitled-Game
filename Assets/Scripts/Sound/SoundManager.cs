using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource audioSource; // ���ʉ��Đ��p��AudioSource
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
            DontDestroyOnLoad(gameObject); // �V�[�����ׂ��ł��I�u�W�F�N�g��ێ�
        }
        else
        {
            Destroy(gameObject); // �d������C���X�^���X��j��
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

    // �{�^�����N���b�N���ꂽ�Ƃ��̌��ʉ�
    public void PlaySelect(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(select);
    }

    // �{�^�����N���b�N���ꂽ�Ƃ��̌��ʉ�
    public void PlaySubmit(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(submit);
    }

    // ���j���[���J�����Ƃ��̌��ʉ�
    public void PlayMenuOpen(float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySound(menuOpen);
    }

    // ���j���[�������Ƃ��̌��ʉ�
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