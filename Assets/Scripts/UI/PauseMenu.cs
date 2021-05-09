using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour {
  private static PauseMenu instance;

  public EventSystem eventSystem;
  public GameObject firstSelected;

  private RectTransform rectTransform;
  private Image background;

  public static void TogglePause(bool show) {
    instance.background.enabled = show;
    foreach(Transform child in instance.transform) {
      child.gameObject.SetActive(show);
    }

    if(show) {
      Time.timeScale = 0f;
      instance.eventSystem.SetSelectedGameObject(instance.firstSelected);
    } else {
      Time.timeScale = 1f;
    }
  }

  void Awake() {
    instance = this;

    rectTransform = GetComponent<RectTransform>();
    background = GetComponent<Image>();
  }

  void Start() {
    TogglePause(false);

    rectTransform.position = Camera.main.WorldToScreenPoint(StageHandler.center);
    rectTransform.sizeDelta = StageHandler.pixelSize;
  }

  void Update() {
    if(Input.GetButtonDown("Pause"))
      TogglePause(!background.enabled);
  }
}
