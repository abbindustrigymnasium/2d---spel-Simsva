using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicHandler : MonoBehaviour {
  private static MusicHandler instance;

  private AudioSource audioSource;

  public List<AudioClip> songs;
  public TMP_Text bgmPopup;
  public int defaultSong = -1;

  public static void StopSong() {
    instance.audioSource.Stop();
  }

  public static void PlaySong(int id, bool popup = true) {
    if(id < 0 || id >= instance.songs.Capacity) {
      Debug.LogError("Song " + id.ToString() + " is out of range!");
    }

    if(instance.audioSource.isPlaying) StopSong();
    instance.audioSource.clip = instance.songs[id];
    instance.audioSource.Play();

    if(popup && instance.bgmPopup != null)
      instance.StartCoroutine(BgmPopup("BGM: " + instance.songs[id].name, 3f, .5f));
  }

  private static IEnumerator BgmPopup(string text, float time, float fadeTime) {
    TMP_Text popup = instance.bgmPopup;

    popup.SetText(text);
    popup.alpha = 0f;
    instance.bgmPopup.enabled = true;

    // Fade in and out + wait
    yield return FadePopup(0f, 1f, fadeTime);
    yield return new WaitForSeconds(time);
    yield return FadePopup(1f, 0f, fadeTime);

    instance.bgmPopup.enabled = false;
  }

  private static IEnumerator FadePopup(float from, float to, float time) {
    TMP_Text popup = instance.bgmPopup;
    float startTime = 0;

    while(startTime < time) {
      popup.alpha = Mathf.Lerp(from, to, startTime/time);
      startTime += Time.deltaTime;
      yield return null;
    }
    popup.alpha = to;
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

  void Start() {
    if(bgmPopup != null) {
      bgmPopup.enabled = false;
      bgmPopup.alpha = 0f;
    }

    if(defaultSong >= 0 && defaultSong < songs.Capacity)
      PlaySong(defaultSong, false);
  }
}
