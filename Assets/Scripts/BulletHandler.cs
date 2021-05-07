using UnityEngine;

public struct BulletData {
  public Vector3 pos;
  public float rot;
  public bool friendly;
  public Color color;
  public float velocity;
  public bool follow;

  public BulletData(Vector3 pos, float rot, bool friendly, Color color, float velocity, bool follow = false) {
    this.pos = pos;
    this.rot = rot;
    this.friendly = friendly;
    this.color = color;
    this.velocity = velocity;
    this.follow = follow;
  }
}

public class BulletHandler : MonoBehaviour {
  private static Transform staticTransform;
  public static GameObject staticBullet;

  public GameObject bullet;
  public float rotSpeed = 2f;

  void Awake() {
    staticTransform = transform;
    staticBullet = bullet;
  }

  void Start() {
    // Bullet bounds
    Bullet.bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(32, 16, 0));
    Bullet.topRight   = Camera.main.ScreenToWorldPoint(new Vector3(32 + 384, 16 + 448, 0));

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
    GameObject newBullet = Instantiate(staticBullet, data.pos, Quaternion.AngleAxis(data.rot, Vector3.forward), staticTransform);
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
  public static void KillAllBullets(byte mode) {
    GameObject[] children = new GameObject[staticTransform.childCount];

    int i = 0;
    foreach(Transform child in staticTransform) {
      if(((mode & 0x1) != 0 && child.CompareTag("Enemy"))
      || ((mode & 0x2) != 0 && child.CompareTag("Friendly"))) {
        children[i] = child.gameObject;
        i++;
      }
    }

    foreach(GameObject child in children) {
      Destroy(child);
    }
  }
}
