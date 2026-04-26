// Ellison - short script for item audio manager
using UnityEngine;

public class ItemAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource sfxAudioSource;

    public static ItemAudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayItemUseSound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }
}
