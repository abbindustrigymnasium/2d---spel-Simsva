using UnityEngine;

public struct BulletData {
  public Vector3 pos;
  public float rot;
  public bool friendly;
  public Color color;
  public float velocity;

  public BulletData(Vector3 pos, float rot, bool friendly, Color color, float velocity) {
    this.pos = pos;
    this.rot = rot;
    this.friendly = friendly;
    this.color = color;
    this.velocity = velocity;
  }
}

public class BulletHandler : MonoBehaviour {
  private static Transform staticTransform;
  public static GameObject staticBullet;

  public GameObject bullet;

  void Awake() {
    staticTransform = transform;
    staticBullet = bullet;
  }

  void Start() {
    // Bullet bounds
    Bullet.bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(32, 16, 0));
    Bullet.topRight   = Camera.main.ScreenToWorldPoint(new Vector3(32 + 384, 16 + 448, 0));

    //Debug.Log(Bullet.bottomLeft); Debug.Log(Bullet.topRight);
  }

  // Shoot one bullet
  public static void ShootBullet(BulletData data) {
    GameObject newBullet = Instantiate(staticBullet, data.pos, Quaternion.AngleAxis(data.rot, Vector3.forward), staticTransform);
    newBullet.GetComponent<SpriteRenderer>().color = data.color;
    newBullet.tag = data.friendly ? "Friendly" : "Enemy";
    newBullet.GetComponent<Bullet>().speed = data.velocity;
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

  public static void KillAllBullets() {
    GameObject[] children = new GameObject[staticTransform.childCount];

    int i = 0;
    foreach(Transform child in staticTransform) {
      children[i] = child.gameObject;
      i++;
    }

    foreach(GameObject child in children) {
      Destroy(child);
    }
  }
}
