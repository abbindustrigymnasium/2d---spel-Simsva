using UnityEngine;

public class BaseAI : MonoBehaviour {
  // Used to check when enemy enters bounds
  [HideInInspector]
  public bool inBounds;
  [HideInInspector]
  public float nextFire;

  public float firedelay = 2f;
  public BulletData bulletData;


  public virtual void Shoot() {}
}
