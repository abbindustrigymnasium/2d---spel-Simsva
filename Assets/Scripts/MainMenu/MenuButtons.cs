using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {
  // Handle button press from its name
  void HandleButton(string name) {
    switch(name) {
    case "StartGame":
      // Load Game scene and unloads MainMenu
      SceneManager.LoadScene("Game", LoadSceneMode.Single);
      break;
    
    case "Quit":
      Application.Quit(0);
      break;
    }
  }

  void Start() {
    // Default color scheme for menu buttons
    ColorBlock btnColors = new ColorBlock();
    btnColors.normalColor       = Color.HSVToRGB(0f, 0f, .3f);
    btnColors.highlightedColor  = Color.HSVToRGB(0f, 0f, 1f);
    btnColors.pressedColor      = Color.HSVToRGB(0f, 0f, .7f);
    btnColors.selectedColor     = Color.HSVToRGB(0f, 0f, 1f);
    btnColors.disabledColor     = Color.HSVToRGB(0f, 0f, .3f);
    btnColors.colorMultiplier = 1f;
    btnColors.fadeDuration = .1f;

    foreach(Transform btnTransform in transform) {
      Button btn = btnTransform.GetComponent<Button>();

      btn.colors = btnColors;

      btn.onClick.AddListener(() => HandleButton(btnTransform.name));
    }
  }
}
