using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const string ThemeName = "Theme";

    private static AudioManager _instance;

    public Sound[] sounds;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            _instance.ChangeTheme(FindSound(ThemeName)?.clip);
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Start()
    {
        if (_instance != this)
            return;

        Play(ThemeName);
    }

    public void Play(string name)
    {
        Sound s = FindSound(name);
        if (s?.source == null)
        {
            Debug.LogWarning($"AudioManager: no sound called '{name}'.");
            return;
        }

        s.source.Play();
    }

    private void ChangeTheme(AudioClip clip)
    {
        Sound theme = FindSound(ThemeName);
        if (clip == null || theme?.source == null || theme.source.clip == clip)
            return;

        theme.clip = clip;
        theme.source.clip = clip;
        theme.source.Play();
    }

    private Sound FindSound(string name)
    {
        return Array.Find(sounds, sound => sound.name == name);
    }
}
