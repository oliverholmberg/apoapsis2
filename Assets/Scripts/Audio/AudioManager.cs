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

        // Prime iOS audio session to avoid first-play delay
        source.volume = 0f;
        source.PlayOneShot(coinClip);
        source.volume = 1f;
    }

    public void PlayCoin()
    {
        if (coinClip != null) source.PlayOneShot(coinClip, 0.55f);
    }

    public void PlayNearMiss()
    {
        if (missClip != null) source.PlayOneShot(missClip, 1.5f);
    }

    public void PlayCrash()
    {
        if (crashClip != null) source.PlayOneShot(crashClip);
    }
}
