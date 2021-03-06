using UnityEngine;

public class Enemy : MonoBehaviour {
  public float hp;
  public int scoreValue = 0, powerValue = 0;
  public BaseAI ai;
  [HideInInspector]
  public bool hasAi;
  private bool dead = false, hitOnFrame = false;

  public void Die() {
    // Don't accidentally spawn multiple pickups
    if(!dead) {
      dead = true;

      // Play death sound
      SFXHandler.PlaySound("enemydeath");

      // Spawn pickups
      while(scoreValue > 0 && powerValue > 0) {
        if(scoreValue > 0) {
          StageHandler.SpawnPickup(0, transform.position, (Vector2)transform.position + (Random.insideUnitCircle + Vector2.up) * .5f, .2f);
          scoreValue--;
        }
        if(powerValue > 0) {
          StageHandler.SpawnPickup(1, transform.position, (Vector2)transform.position + (Random.insideUnitCircle + Vector2.up) * .5f, .2f);
          powerValue--;
        }
      }

      // Destroy object
      Destroy(gameObject);
    }
  }

  private void TakeDamage(float damage) {
    if(hp < damage) {
      Die();
    } else {
      hp -= damage;

      PlayerController.instance.AddScore((uint)Mathf.RoundToInt(damage * 25));
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

    if(hitOnFrame) {
      SFXHandler.PlaySound("enemyhit_loop", true);
      hitOnFrame = false;
    } else {
      SFXHandler.StopSound("enemyhit_loop");
    }
  }

  void FixedUpdate() {
    if(hasAi && StageHandler.InStageBounds(transform.position)) {
      if(ai.inBounds) {
        while(Time.time > ai.nextFire) {
          ai.Shoot();

          ai.nextFire += ai.firedelay;
        }
      }
    }
  }

  void OnTriggerEnter2D(Collider2D collider) {
    // Only take damage when on-screen
    if(collider.CompareTag("Friendly") && StageHandler.InStageBounds(transform.position)) {
      hitOnFrame = true;
      TakeDamage(PlayerController.instance.damage);
    }
  }
}
