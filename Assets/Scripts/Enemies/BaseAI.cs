using UnityEngine;

public class BaseAI : MonoBehaviour {
  [HideInInspector]
  public bool inBounds;
  [HideInInspector]
  public float nextFire;

  public float firedelay = 2f, shotSpeed = 4f;
  public BulletData bulletData;

  public virtual void Shoot(Vector3 player) {}
}
