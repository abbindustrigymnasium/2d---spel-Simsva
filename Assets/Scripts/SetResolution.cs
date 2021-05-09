using UnityEngine;

public class SetResolution : MonoBehaviour {
  void Awake() {
    // Runs at startup and changes resolution to 640x480
    Screen.SetResolution(640, 480, FullScreenMode.Windowed, 60);
  }  
}
