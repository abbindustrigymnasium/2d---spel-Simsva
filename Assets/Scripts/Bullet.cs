using UnityEngine;

public class Bullet : MonoBehaviour {
  public static Vector2 bottomLeft, topRight;
  public static float rotSpeed = 2f;
  public float speed = 0f;
  public bool follow = false;

  void FixedUpdate() {
    // Follows enemies, i.e. it only works for friendly bullets
    if(follow) {
      GameObject target = StageHandler.instance.GetClosestEnemy(transform.position);
      if(target != null) {
        // Length of cross product provides rotation amount
        float rot = Vector3.Cross(transform.up, Vector3.Normalize(target.transform.position - transform.position)).z;

        transform.Rotate(Vector3.forward, rot * rotSpeed * Time.fixedDeltaTime);
      }
    }

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
