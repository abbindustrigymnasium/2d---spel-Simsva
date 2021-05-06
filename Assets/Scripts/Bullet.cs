using UnityEngine;

public class Bullet : MonoBehaviour {
  public static Vector2 bottomLeft, topRight;
  public float speed = 0f;
  public bool follow = false; // WIP

  void FixedUpdate() {
    transform.Translate(transform.up * speed * Time.fixedDeltaTime);

    // Kill if out of bounds, more reliable than edge colliders
    if(transform.position.x > topRight.x || transform.position.x < bottomLeft.x
      || transform.position.y > topRight.y || transform.position.y < bottomLeft.y)
      Object.Destroy(gameObject);
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if(transform.tag == "Enemy" && collider.tag == "Player")
      Object.Destroy(gameObject);
  }
}
