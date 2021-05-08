using UnityEngine;

public enum PickupType {
  Point = 0,
  Power
}

public class Pickup : MonoBehaviour {
  public static float dropSpeed = 1f;
  public float value = 0;
  public PickupType type = 0;
  // Used to save the y position score when above the collection line (or for constant value)
  public float fixedScore = 0f;

  // Calculates score from the y position of the pickup
  public float GetScore() {
    return Mathf.Lerp(1500, 10000, (transform.position.y - StageHandler.bottomLeft.y) / (StageHandler.topRight.y - StageHandler.bottomLeft.y));
  }

  void FixedUpdate() {
    // Constantly move down
    transform.Translate(Vector3.down * dropSpeed * Time.fixedDeltaTime, Space.World);

    // Kill when off-screen
    if(transform.position.y < StageHandler.bottomLeft.y)
      Object.Destroy(gameObject);
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if(collider.CompareTag("Player")) {
      Object.Destroy(gameObject);
    }
  }
}
