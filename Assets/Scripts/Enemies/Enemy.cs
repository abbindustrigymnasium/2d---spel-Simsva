using UnityEngine;

public class Enemy : MonoBehaviour {
  public float hp;
  public int scoreValue = 0, powerValue = 0;
  public BaseAI ai;
  [HideInInspector]
  public bool hasAi;
  private bool dead = false;

  private void TakeDamage(float damage) {
    if(hp < damage) {
      // Don't accidentally spawn multiple pickups
      if(!dead) {
        dead = true;

        // Spawn pickups
        for(int i = 0; i < scoreValue; i++) {
          StageHandler.SpawnPickup(0, (Vector2)transform.position + (Random.insideUnitCircle + Vector2.up) * .5f);
        }
        for(int i = 0; i < powerValue; i++) {
          StageHandler.SpawnPickup(1, (Vector2)transform.position + (Random.insideUnitCircle + Vector2.up) * .5f);
        }

        // Destroy object
        Object.Destroy(gameObject);
      }
    } else {
      hp -= damage;

      PlayerController.instance.AddScore((uint)Mathf.RoundToInt(damage * 10));
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
