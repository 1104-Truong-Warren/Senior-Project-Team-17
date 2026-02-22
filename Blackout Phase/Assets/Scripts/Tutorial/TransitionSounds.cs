// Ellison
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransitionSounds : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayTransitionSound()
    {
        Debug.Log("Playing transition sound");
        audioSource.Play();
    }
}
