using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageHandler : MonoBehaviour {
  public static StageHandler instance;

  private SpriteRenderer background;

  public List<GameObject> enemies, pickups;
  public List<Sprite> backgrounds;
  public static Vector2 bottomLeft, topRight, center, length;

  // Stages
  private Stage1 stage1;

  // Methods
  // Check if point is inside stage
  public static bool InStageBounds(Vector2 pos) {
    return pos.x < topRight.x && pos.x > bottomLeft.x
        && pos.y < topRight.y && pos.y > bottomLeft.y;
  }

  // Instantiate new enemy
  public static GameObject SpawnEnemy(int id, Vector3 pos, bool hasAi = false, float hpOverride = -1f) {
    GameObject enemy = Instantiate(instance.enemies[id], pos, Quaternion.identity, instance.transform.Find("Enemies"));

    enemy.GetComponent<Enemy>().hasAi = hasAi;
    BaseAI ai = enemy.GetComponent<BaseAI>();
    if(ai) ai.enabled = hasAi;
    if(hpOverride > 0f) enemy.GetComponent<Enemy>().hp = hpOverride;

    return enemy;
  }

  // Instantiate new pickup
  public static GameObject SpawnPickup(int id, Vector3 pos, int valueOverride = -1) {
    GameObject pickup = Instantiate(instance.pickups[id], pos, Quaternion.identity, instance.transform.Find("Pickups"));
    if(valueOverride > 0) pickup.GetComponent<Pickup>().value = valueOverride;
    return pickup;
  }

  // Sends all pickups to player
  public static void CollectAllPickups(bool useConstantScore = false) {
    foreach(Transform pickup in instance.transform.Find("Pickups")) {
      Pickup pickupScript = pickup.GetComponent<Pickup>();

      // Get a fixed score multiplier from the original position
      // useConstantScore uses the default fixedScore of the pickup
      if(!useConstantScore)
        pickupScript.fixedScore = pickupScript.GetScore();

      instance.StartCoroutine(MoveToPlayer(pickup.gameObject, .3f));
    }
  }

  // Wrapper for Object.Destroy
  public static void DestroyEnemy(GameObject enemy, float t) {
    Object.Destroy(enemy, t);
  }

  // Kill all enemies and drop pickups
  public static void KillAllEnemies() {
    foreach(Transform enemy in instance.transform.Find("Enemies")) {
      enemy.gameObject.GetComponent<Enemy>().Die();
    }
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

  public static void SetBackground(int id) {
    instance.background.sprite = instance.backgrounds[id];
  }

  // Coroutines
  // Move object to player
  private static IEnumerator MoveToPlayer(GameObject obj, float time) {
    Vector3 startPos = obj.transform.position;
    float startTime = 0;

    while(startTime < time) {
      // Break if object gets deleted
      if(obj == null) yield break;

      obj.transform.position = Vector3.Lerp(startPos, PlayerController.instance.transform.position, startTime/time);
      startTime += Time.deltaTime;
      yield return null;
    }
    obj.transform.position = PlayerController.instance.transform.position;
  }

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

    background = GetComponent<SpriteRenderer>();

    // Stages
    stage1 = GetComponent<Stage1>();

    // Stage bounds
    bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(32, 16, 0));
    topRight   = Camera.main.ScreenToWorldPoint(new Vector3(32 + 384, 16 + 448, 0));
    center     = new Vector2((bottomLeft.x + topRight.x)/2, (bottomLeft.y+topRight.y)/2);
    length     = new Vector2(Mathf.Abs(bottomLeft.x - topRight.x), Mathf.Abs(bottomLeft.y - topRight.y));

    transform.position = center;
  }

  void Start() {
    // Pickup dropSpeed
    Pickup.dropSpeed = 1f;

    // Start stage 1
    stage1.StartStage();
  }
}
