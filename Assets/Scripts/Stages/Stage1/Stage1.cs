using UnityEngine;

public class Stage1 : MonoBehaviour {
  Stage1Timeline timeline;

  public void StartStage() {
    Debug.Log("Stage 1");
    timeline = new Stage1Timeline();

    StartCoroutine(timeline.Run());
  }  
}
