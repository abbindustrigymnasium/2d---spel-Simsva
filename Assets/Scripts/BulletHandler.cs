using UnityEngine;

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

  public static void ShootBullet(Vector3 pos, float rot, bool friendly, Color color, float velocity=0f) {
    GameObject newBullet = Instantiate(staticBullet, pos, Quaternion.AngleAxis(rot, Vector3.forward), staticTransform);
    newBullet.GetComponent<SpriteRenderer>().color = color;
    newBullet.tag = friendly ? "Friendly" : "Enemy";
    newBullet.GetComponent<Bullet>().speed = velocity;
  }

  public static void ShootBullet(Vector3 pos, float rot, bool friendly, float velocity=0f) {
    ShootBullet(pos, rot, friendly, Color.red, velocity);
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
