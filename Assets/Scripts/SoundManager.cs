using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioClip bgm1;
    AudioClip bgm2;
    AudioSource audioSource;

    void Mute()
    {

    }

    void Play_1()
    {
        audioSource.clip = bgm1;
    }

    void Play_2()
    {
        audioSource.clip = bgm1;
    }
}
