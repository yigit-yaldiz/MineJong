using BaseAssets;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundEffect { Win, Lose, ButtonClick };
    public static SoundManager Instance { get; private set; }

    public List<Sound> soundList;

    private void OnValidate()
    {
        if (soundList == null)
            return;
        if (soundList.Count <= 0)
            return;

        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].name = soundList[i].sound.ToString();
        }
    }

    [System.Serializable]
    public class Sound
    {
        [HideInInspector]
        public string name;
        public SoundEffect sound;
        [Range(0, 3)]
        public float volume = 1;
        public List<AudioClip> clips;
        private AudioSource source;

        public void Init(GameObject parent)
        {
            source = parent.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
        }

        public void PlaySound(bool loop = false, bool dontPlayIfPlaying = false)
        {
            if (source.isPlaying && dontPlayIfPlaying)
                return;

            if (clips.Count <= 0)
            {
                Debug.LogWarning(sound + " is not attached to SoundManager.");
                return;
            }
            source.clip = clips[Random.Range(0, clips.Count)];

            if (source.clip == null)
            {
                Debug.LogWarning("There is no sound clip on " + name);
                return;
            }
            source.volume = volume;
            source.loop = loop;
            source.Play();
        }
        public void StopSound()
        {
            source.Stop();
        }
    }

    private void Awake()
    {
        Instance = this;

        GameObject soundHolder = new GameObject("SOUNDS");

        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].Init(soundHolder);
        }
    }

    public void PlaySound(SoundEffect sound, bool loop = false, bool dontPlayIfPlaying = false)
    {
        if (!Settings.Instance.sounds)
            return;

        Sound s = GetSound(sound);
        if (s == null)
        {
            Debug.LogWarning(sound + " is not attached to SoundManager.");
            return;
        }
        s.PlaySound(loop, dontPlayIfPlaying);
    }
    public void StopSound(SoundEffect sound)
    {
        if (!Settings.Instance.sounds)
            return;

        Sound s = GetSound(sound);
        if (s == null)
        {
            Debug.LogWarning(sound + " is not attached to SoundManager.");
            return;
        }
        s.StopSound();
    }

    private Sound GetSound(SoundEffect sound)
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            if (soundList[i].sound == sound)
                return soundList[i];
        }
        return null;
    }
}
