using UnityEngine;

public class Orb : MonoBehaviour {
  public static int orbsEnabled = 0;
  public static float radius = 1f;
  public static float rotSpeed = 180f;
  public static BulletData bulletData;
  public int id = 0;

  public void Shoot() {
    BulletData data = bulletData;
    data.pos = transform.position;

    BulletHandler.ShootBullet(data);
  }

  void Update() {
    if(id < orbsEnabled) {
      if(orbsEnabled == 1) {
        transform.position = transform.parent.position + radius * Vector3.down;
      } else {
        //float angle = 3*Mathf.PI/2 - Mathf.PI/2 + id * Mathf.PI/(orbsEnabled - 1);
        float angle = Mathf.PI + id * Mathf.PI/(orbsEnabled - 1);

        transform.position = transform.parent.position + new Vector3(
          radius * Mathf.Cos(angle),
          radius * Mathf.Sin(angle)
        );
      }

      transform.Rotate(Vector3.forward*rotSpeed * Time.deltaTime);
    }
  }
}
