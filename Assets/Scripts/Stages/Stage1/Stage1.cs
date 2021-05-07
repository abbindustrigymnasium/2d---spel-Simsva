using System.Collections;
using UnityEngine;

public class Stage1 : MonoBehaviour {
  Stage1Timeline timeline;

  public void StartStage() {
    timeline = new Stage1Timeline();

    StartCoroutine(timeline.Run());
  }  
}
