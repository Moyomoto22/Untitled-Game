using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    public List<AudioClip> clips;
    private float pitchRange = 0.1f;
    public AudioSource source;

    private void Awake()
    {
        // source = GetComponents<AudioSource>();
    }

    public void PlayFootstepSound()
    {
        source.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        source.PlayOneShot(clips[Random.Range(0, clips.Count)]);
    }

}