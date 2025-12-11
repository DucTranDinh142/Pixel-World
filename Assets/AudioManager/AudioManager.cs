using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundSource;
    public AudioSource effectSource;
    public AudioClip backgroundClip;
    public AudioClip placeClip;
    public AudioClip destroyClip;

    private void Start()
    {
        PlayBackgroungClip();
    }

    public void PlayBackgroungClip()
    {
        backgroundSource.clip = backgroundClip;
        backgroundSource.Play();
    }
    public void PlayPlaceClip()
    {
        effectSource.clip = placeClip;
        effectSource.Play();
    }
    public void PlayDestroyClip()
    {
        effectSource.clip = destroyClip;
        effectSource.Play();
    }
}
