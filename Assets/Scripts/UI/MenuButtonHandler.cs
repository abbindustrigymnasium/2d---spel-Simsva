using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour {
  // Handle button press from its name
  public void HandleButton(string name) {
    switch(name) {
    case "StartGame":
    case "RestartGame":
      // Load Game scene and unloads MainMenu
      SceneManager.LoadScene("Game", LoadSceneMode.Single);
      break;
    
    case "MainMenu":
      SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
      break;
    
    case "Resume":
      PauseMenu.TogglePause(PauseMode.Hide);
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
    btnColors.colorMultiplier   = 1f;
    btnColors.fadeDuration      = .1f;

    foreach(Transform btnTransform in transform) {
      Button btn = btnTransform.GetComponent<Button>();

      if(btn != null)
        btn.colors = btnColors;
    }
  }
}
