using System.Collections;
using UnityEngine;

public class BurstAI : BaseAI {
  public float shotSpeed = 4f, burstSpread = 1f, burstTime = .5f;
  public int burstCount = 10;

  void Start() {
    bulletData = new BulletData(
      Vector3.zero,
      0f,
      false,
      Color.green,
      shotSpeed,
      id: 1
    );
  }

  public override void Shoot()
  {
    bulletData.pos = transform.position;
    bulletData.rot = BulletHandler.AngleToPlayer(transform.position);

    StartCoroutine(BulletHandler.ShootBurst(
      bulletData,
      burstSpread,
      burstCount,
      burstTime
    ));
  }
}
