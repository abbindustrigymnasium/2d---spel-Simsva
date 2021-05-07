using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageHandler : MonoBehaviour {
  public static StageHandler instance;

  public List<GameObject> enemies;
  public Vector3 bottomLeft, topRight;

  // Stages
  private Stage1 stage1;

  // Methods
  // Instantiate new enemy
  public GameObject SpawnEnemy(int id, Vector3 pos) {
    return Instantiate(enemies[id], pos, Quaternion.identity, transform.Find("Enemies"));
  }

  // Wrapper for Object.Destroy
  public void DestroyEnemy(GameObject enemy, float t) {
    Object.Destroy(enemy, t);
  }

  public GameObject GetClosestEnemy(Vector3 pos) {
    Transform closest = null;

    foreach(Transform enemy in transform.Find("Enemies")) {
      if(closest == null || Vector3.SqrMagnitude(enemy.position - pos) < Vector3.SqrMagnitude(closest.position - pos))
        closest = enemy;
    }

    if(closest == null)
      return null;
    else
      return closest.gameObject;
  }

  // Coroutines
  // Run several coroutines one after another
  private IEnumerator SequentialCoroutine(List<IEnumerator> routines) {
    foreach(IEnumerator routine in routines) {
      yield return StartCoroutine(routine);
    }
  }

  // Same as SequentialCoroutine, but stops if obj gets deleted
  private IEnumerator SequentialCoroutineObj(List<IEnumerator> routines, GameObject obj) {
    foreach(IEnumerator routine in routines) {
      if(obj == null) break;
      yield return StartCoroutine(routine);
    }
  }

  // Void wrapper for SequantialCoroutine
  public void StartSequentialCoroutine(List<IEnumerator> routines) {
    StartCoroutine(SequentialCoroutine(routines));
  }

  // Void wrapper for SequantialCoroutineObj
  public void StartSequentialCoroutine(List<IEnumerator> routines, GameObject requiredObject) {
    StartCoroutine(SequentialCoroutineObj(routines, requiredObject));
  }

  void Awake() {
    instance = this;

    stage1 = GetComponent<Stage1>();
  }

  void Start() {
    bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(32, 16, 0));
    topRight   = Camera.main.ScreenToWorldPoint(new Vector3(32 + 384, 16 + 448, 0));

    stage1.StartStage();
  }
}
