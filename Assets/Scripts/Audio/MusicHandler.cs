using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour {
  private static MusicHandler instance;

  private AudioSource audioSource;

  public List<AudioClip> songs;

  public static void StopSong() {
    instance.audioSource.Stop();
  }

  public static void PlaySong(int id) {
    if(instance.audioSource.isPlaying) StopSong();
    instance.audioSource.clip = instance.songs[id];
    instance.audioSource.Play();
  }

  public static void SetVolume(float volume) {
    instance.audioSource.volume = Mathf.Clamp01(volume);
  }

  void Awake() {
    instance = this;

    audioSource = GetComponent<AudioSource>();
    audioSource.loop = true;
    audioSource.Stop(); // Stop if playOnAwake is on
  }
}
