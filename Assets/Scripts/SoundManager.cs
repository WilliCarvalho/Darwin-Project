using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip bgm1;
    [SerializeField] AudioClip bgm2;
    [SerializeField] AudioSource audioSource;

    public void Play_1()
    {
        audioSource.clip = bgm1;
        audioSource.Play();
    }

    public void Play_2()
    {
        audioSource.clip = bgm2;
        audioSource.Play();
    }
}
