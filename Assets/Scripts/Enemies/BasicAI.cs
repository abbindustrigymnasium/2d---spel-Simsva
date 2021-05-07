using UnityEngine;

public class BasicAI : BaseAI {
  void Start() {
    bulletData = new BulletData(
      Vector3.zero,
      0f,
      false,
      Color.green,
      shotSpeed
    );
  }

  public override void Shoot(Vector3 player)
  {
    Vector3 direction = player - transform.position;

    bulletData.pos = transform.position;
    bulletData.rot = Vector2.SignedAngle(Vector2.right, direction);

    BulletHandler.ShootBullet(bulletData);
  }
}
