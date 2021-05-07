using System.Collections.Generic;
using UnityEngine;

public class StageHandler : MonoBehaviour {
  public static StageHandler instance;

  public List<GameObject> enemies;
  public Vector3 bottomLeft, topRight;

  // Stages
  private Stage1 stage1;

  // Methods
  public GameObject SpawnEnemy(int id, Vector3 pos) {
    return Instantiate(enemies[id], pos, Quaternion.identity, transform.Find("Enemies"));
  }

  void Awake() {
    instance = this;

    stage1 = GetComponent<Stage1>();
  }

  void Start() {
    Debug.Log("Stage 1");
    stage1.StartStage();

    bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(32, 16, 0));
    topRight   = Camera.main.ScreenToWorldPoint(new Vector3(32 + 384, 16 + 448, 0));
  }
}
