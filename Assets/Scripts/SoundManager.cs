using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void Play_1()
    {
        audioSource.UnPause();
    }

    public void Play_2()
    {
        audioSource.Pause();
    }
}
