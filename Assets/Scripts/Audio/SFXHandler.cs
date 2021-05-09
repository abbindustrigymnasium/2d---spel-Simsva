using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public struct SFX {
  public int index;
  public AudioSource source;

  public SFX(int index, AudioSource audioSource) {
    this.index = index;
    this.source = audioSource;
  }
}

public class SFXHandler : MonoBehaviour {
  private static SFXHandler instance;

  public AudioMixerGroup mixerGroup;
  public List<AudioClip> soundEffects;
  [Range(0f, 1f)]
  public List<float> volume;
  
  private Dictionary<string, SFX> sfx;

  public static void PlaySound(string id, bool loop = false) {
    SFX audio = instance.sfx[id];

    audio.source.loop = loop;
    if(!audio.source.isPlaying)
      audio.source.Play();
  }

  public static void StopSound(string id) {
    instance.sfx[id].source.Stop();
  }

  void Awake() {
    instance = this;

    // TODO: Use list?
    sfx = new Dictionary<string, SFX>(soundEffects.Capacity);
    for(int i = 0; i < soundEffects.Capacity; i++) {
      GameObject obj = new GameObject(soundEffects[i].name, typeof(AudioSource));
      obj.transform.SetParent(transform);

      AudioSource audioSource = obj.GetComponent<AudioSource>();
      audioSource.clip = soundEffects[i];
      audioSource.playOnAwake = false;
      audioSource.volume = volume[i];
      audioSource.outputAudioMixerGroup = mixerGroup;

      sfx.Add(soundEffects[i].name, new SFX(i, audioSource));
    }
  }
}
