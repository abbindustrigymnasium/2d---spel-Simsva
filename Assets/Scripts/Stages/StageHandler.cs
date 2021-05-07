using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageHandler : MonoBehaviour {
  public static StageHandler instance;

  public List<GameObject> enemies;
  public static Vector2 bottomLeft, topRight;

  // Stages
  private Stage1 stage1;

  // Methods
  // Check if point is inside stage
  public static bool InStageBounds(Vector2 pos) {
    return pos.x < topRight.x && pos.x > bottomLeft.x
        && pos.y < topRight.y && pos.y > bottomLeft.y;
  }

  // Instantiate new enemy
  public static GameObject SpawnEnemy(int id, Vector3 pos, bool hasAi = false) {
    GameObject enemy = Instantiate(instance.enemies[id], pos, Quaternion.identity, instance.transform.Find("Enemies"));

    enemy.GetComponent<Enemy>().hasAi = hasAi;
    BaseAI ai = enemy.GetComponent<BaseAI>();
    if(ai) ai.enabled = hasAi;

    return enemy;
  }

  // Wrapper for Object.Destroy
  public static void DestroyEnemy(GameObject enemy, float t) {
    Object.Destroy(enemy, t);
  }

  public static GameObject GetClosestEnemy(Vector3 pos) {
    Transform closest = null;

    foreach(Transform enemy in instance.transform.Find("Enemies")) {
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
  private static IEnumerator SequentialCoroutine(List<IEnumerator> routines) {
    foreach(IEnumerator routine in routines) {
      yield return instance.StartCoroutine(routine);
    }
  }

  // Same as SequentialCoroutine, but stops if obj gets deleted
  private static IEnumerator SequentialCoroutineObj(List<IEnumerator> routines, GameObject obj) {
    foreach(IEnumerator routine in routines) {
      if(obj == null) break;
      yield return instance.StartCoroutine(routine);
    }
  }

  // Void wrapper for SequantialCoroutine
  public static void StartSequentialCoroutine(List<IEnumerator> routines) {
    instance.StartCoroutine(SequentialCoroutine(routines));
  }

  // Void wrapper for SequantialCoroutineObj
  public static void StartSequentialCoroutine(List<IEnumerator> routines, GameObject requiredObject) {
    instance.StartCoroutine(SequentialCoroutineObj(routines, requiredObject));
  }

  void Awake() {
    instance = this;

    stage1 = GetComponent<Stage1>();
  }

  void Start() {
    // Stage bounds
    bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(32, 16, 0));
    topRight   = Camera.main.ScreenToWorldPoint(new Vector3(32 + 384, 16 + 448, 0));

    stage1.StartStage();
  }
}
