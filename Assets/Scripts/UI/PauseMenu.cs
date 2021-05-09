using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum PauseMode {
  Show = 0,
  Hide,
  GameOver,
  Toggle
}

public class PauseMenu : MonoBehaviour {
  private static PauseMenu instance;

  public GameObject resumeButton, restartButton;
  private bool paused, gameOver;

  private RectTransform rectTransform;
  private Image background;
  private TMP_Text pauseTitle;

  public static void TogglePause(PauseMode mode) {
    // Reset after a game over
    if(instance.gameOver) {
      instance.gameOver = false;
      instance.pauseTitle.SetText("Paused");
      instance.resumeButton.GetComponent<Button>().interactable = true;
    }

    // Enables buttons at first for handling events
    foreach(Transform child in instance.transform) {
      child.gameObject.SetActive(true);
    }

    bool enabled = false;
    switch(mode) {
    case PauseMode.Show:
      enabled = true;

      Time.timeScale = 0f;
      EventSystem.current.SetSelectedGameObject(instance.resumeButton);
      break;
    
    case PauseMode.Hide:
      enabled = false;

      Time.timeScale = 1f;
      break;

    case PauseMode.GameOver:
      instance.gameOver = true;
      enabled = true;

      Time.timeScale = 0f;
      // You can't resume after a game over
      instance.resumeButton.GetComponent<Button>().interactable = false;
      instance.pauseTitle.SetText("Game Over");
      EventSystem.current.SetSelectedGameObject(instance.restartButton);
      break;
    
    case PauseMode.Toggle:
      TogglePause(instance.paused ? PauseMode.Hide : PauseMode.Show);
      return;
    }
    instance.paused = enabled;

    // Enable/disable background and buttons
    instance.background.enabled = enabled;
    foreach(Transform child in instance.transform) {
      child.gameObject.SetActive(enabled);
    }
  }

  void Awake() {
    instance = this;

    gameOver = false;
    paused = false;

    rectTransform = GetComponent<RectTransform>();
    background = GetComponent<Image>();
    pauseTitle = GetComponentInChildren<TMP_Text>();
  }

  void Start() {
    TogglePause(PauseMode.Hide);

    rectTransform.position = Camera.main.WorldToScreenPoint(StageHandler.center);
    rectTransform.sizeDelta = StageHandler.pixelSize;
  }

  void Update() {
    if(Input.GetButtonDown("Pause") && !gameOver)
      TogglePause(PauseMode.Toggle);
  }
}
