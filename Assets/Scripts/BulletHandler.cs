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
      90f,
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
    newBullet.GetComponent<SpriteRenderer>().color = data.color;
    newBullet.tag = data.friendly ? "Friendly" : "Enemy";

    Bullet bulletScript = newBullet.GetComponent<Bullet>();
    bulletScript.speed = data.velocity;
    bulletScript.follow = data.follow;
  }

  // Shoot spread bullets
  public static void ShootSplit(BulletData data, float spread, int amount) {
    float difference = spread/(amount-1);
    data.rot -= spread/2;

    for(int i = 0; i < amount; i++) {
      ShootBullet(data);
      data.rot += difference;
    }
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
}
