using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BulletData {
  public Vector3 pos;
  public float rot;
  public bool friendly;
  public Color color;
  public float velocity;
  public bool follow;
  public int id;

  public BulletData(Vector3 pos, float rot, bool friendly, Color color, float velocity, bool follow = false, int id = 0) {
    this.pos = pos;
    this.rot = rot;
    this.friendly = friendly;
    this.color = color;
    this.velocity = velocity;
    this.follow = follow;
    this.id = id;
  }
}

public class BulletHandler : MonoBehaviour {
  public static BulletHandler instance; 

  public List<GameObject> bullets;
  public float rotSpeed = 2f;

  void Awake() {
    instance = this;
  }

  void Start() {
    // Orbs
    Orb.bulletData = new BulletData(
      Vector3.zero,
      0f,
      true,
      Color.blue,
      PlayerController.instance.shotSpeed,
      true
    );
  }

  void Update() {
    Bullet.rotSpeed = rotSpeed;
  }

  // Shoot one bullet
  public static void ShootBullet(BulletData data) {
    GameObject newBullet = Instantiate(instance.bullets[data.id], data.pos, Quaternion.AngleAxis(data.rot, Vector3.forward), instance.transform);
    // Friendly bullets render under player
    newBullet.GetComponent<SpriteRenderer>().sortingOrder = data.friendly ? -2 : 2;
    newBullet.transform.Find("Mid").GetComponent<SpriteRenderer>().sortingOrder = data.friendly ? -1 : 3;
    newBullet.GetComponent<SpriteRenderer>().color = data.color;
    newBullet.tag = data.friendly ? "Friendly" : "Enemy";

    Bullet bulletScript = newBullet.GetComponent<Bullet>();
    bulletScript.speed = data.velocity;
    bulletScript.follow = data.follow;
  }

  // Shoot spread bullets
  public static void ShootSplit(BulletData data, float spread, int count) {
    float difference = spread/(count-1);
    data.rot -= spread/2;

    for(int i = 0; i < count; i++) {
      ShootBullet(data);
      data.rot += difference;
    }
  }

  // Shoot bullet burst
  public static IEnumerator ShootBurst(BulletData data, float spread, int count, float time, GameObject requiredObj = null) {
    bool hasRequiredObj = requiredObj != null;
    float delay = time/count;

    for(int i = 0; i < count; i++) {
      // Stop if requiredObj was destroyed
      if(hasRequiredObj && requiredObj == null) yield break;

      BulletData newData = data;
      if(hasRequiredObj) newData.pos = requiredObj.transform.position;
      newData.rot = data.rot + Random.Range(-spread, spread);
      ShootBullet(newData);

      yield return new WaitForSeconds(delay);
    }

    yield return null;
  }

  // Shoot bullets in a spiral
  public static IEnumerator ShootSpiral(BulletData data, int count, float time, GameObject requiredObj = null) {
    bool hasRequiredObj = requiredObj != null;
    float delay = time/count;
    float angle = 360f/count;

    for(int i = 0; i < count; i++) {
      // Stop if requiredObj was destroyed
      if(hasRequiredObj && requiredObj == null) yield break;

      BulletData newData = data;
      if(hasRequiredObj) newData.pos = requiredObj.transform.position;
      newData.rot = data.rot + i * angle;
      ShootBullet(newData);

      yield return new WaitForSeconds(delay);
    }

    yield return null;
  }

  // Mode flags:
  //  0x1 - Kill enemy bullets
  //  0x2 - Kill friendly bullets
  // Make console command?
  public static void KillAllBullets(byte mode) {
    GameObject[] children = new GameObject[instance.transform.childCount];

    int i = 0;
    foreach(Transform child in instance.transform) {
      if(((mode & 0x1) != 0 && child.CompareTag("Enemy"))
      || ((mode & 0x2) != 0 && child.CompareTag("Friendly"))) {
        children[i] = child.gameObject;
        i++;
      }
    }

    // Can't destroy bullets while looping through them
    foreach(GameObject child in children) {
      Destroy(child);
    }
  }

  // Turn all bullets to score pickups (for bombs, spell cards, etc.)
  public static void BulletsToScore() {
    GameObject[] bullets = new GameObject[instance.transform.childCount];

    int i = 0;
    foreach(Transform bullet in instance.transform) {
      if(bullet.CompareTag("Enemy")) {
        bullets[i] = bullet.gameObject;
        i++;

        StageHandler.SpawnPickup(4, bullet.position);
      }
    }

    // Can't destroy bullets while looping through them
    foreach(GameObject bullet in bullets) {
      Destroy(bullet);
    }
  }

  // Get angle to player
  public static float AngleToPlayer(Vector2 pos) {
    return Vector2.SignedAngle(Vector2.up, (Vector2)PlayerController.instance.transform.position - pos);
  }
}
