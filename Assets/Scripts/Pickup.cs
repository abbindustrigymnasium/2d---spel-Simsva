using UnityEngine;

public enum PickupType {
  Point = 0,
  Power
}

public class Pickup : MonoBehaviour {
  public static float dropSpeed;
  public int value = 0;
  public PickupType type = 0;
  public bool collected = false;

  // Calculates score from the y position of the pickup
  public int GetScore() {
    return Mathf.RoundToInt(Mathf.Lerp(1000, 3000, (transform.position.y - StageHandler.bottomLeft.y) / (StageHandler.topRight.y - StageHandler.bottomLeft.y)));
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
