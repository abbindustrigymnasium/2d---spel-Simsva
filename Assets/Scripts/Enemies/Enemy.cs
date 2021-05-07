using UnityEngine;

public class Enemy : MonoBehaviour {
  public float hp;
  public BaseAI ai;
  [HideInInspector]
  public bool hasAi;

  private void TakeDamage(float damage) {
    if(hp < damage) {
      // Handle drops etc.

      Object.Destroy(gameObject);
    } else {
      hp -= damage;
    }
  }

  void Update() {
    if(hasAi && StageHandler.InStageBounds(transform.position)) {
      if(!ai.inBounds) {
        ai.nextFire = Time.time;
        ai.inBounds = true;
      }
    } else {
      ai.inBounds = false;
    }
  }

  void FixedUpdate() {
    if(hasAi && StageHandler.InStageBounds(transform.position)) {
      if(ai.inBounds) {
        while(Time.time > ai.nextFire) {
          ai.Shoot(PlayerController.instance.transform.position);

          ai.nextFire += ai.firedelay;
        }
      }
    }
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if(collider.CompareTag("Friendly")) {
      TakeDamage(PlayerController.instance.damage);
    }
  }
}
