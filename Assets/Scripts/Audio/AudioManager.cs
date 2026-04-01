using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    AudioClip coinClip;
    AudioClip missClip;
    AudioClip crashClip;
    AudioSource source;

    void Awake()
    {
        Instance = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;

        coinClip = Resources.Load<AudioClip>("Audio/coin");
        missClip = Resources.Load<AudioClip>("Audio/miss");
        crashClip = Resources.Load<AudioClip>("Audio/crash");
    }

    public void PlayCoin()
    {
        if (coinClip != null) source.PlayOneShot(coinClip);
    }

    public void PlayNearMiss()
    {
        if (missClip != null) source.PlayOneShot(missClip);
    }

    public void PlayCrash()
    {
        if (crashClip != null) source.PlayOneShot(crashClip);
    }
}
