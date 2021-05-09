using UnityEngine;

public class BasicAI : BaseAI {
  public float shotSpeed = 4f;

  void Start() {
    bulletData = new BulletData(
      Vector3.zero,
      0f,
      false,
      Color.green,
      shotSpeed
    );
  }

  public override void Shoot()
  {
    SFXHandler.PlaySound("enemyshot");

    bulletData.pos = transform.position;
    bulletData.rot = BulletHandler.AngleToPlayer(transform.position);

    BulletHandler.ShootBullet(bulletData);
  }
}
