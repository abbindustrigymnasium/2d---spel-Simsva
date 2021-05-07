using UnityEngine;

public class Bullet : MonoBehaviour {
  public static float rotSpeed = 500f;
  public float speed = 0f;
  public bool follow = false;

  void FixedUpdate() {
    // Follows enemies, i.e. it only works for friendly bullets
    if(follow) {
      GameObject target = StageHandler.GetClosestEnemy(transform.position);
      if(target != null) {
        // The magnitude of the cross product provides rotation amount
        float rot = Vector3.Cross(transform.right, Vector3.Normalize(target.transform.position - transform.position)).z;

        transform.Rotate(Vector3.forward, rot * rotSpeed * Time.fixedDeltaTime);
      }
    }

    // Use Vector3 instead of transform.right because of the relativeTo parameter
    transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);

    // Kill if out of bounds, more reliable than edge colliders
    if(!StageHandler.InStageBounds(transform.position))
      Object.Destroy(gameObject);
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if(transform.CompareTag("Enemy") && collider.CompareTag("Player"))
      Object.Destroy(gameObject);
    else if(transform.CompareTag("Friendly") && collider.CompareTag("Enemy"))
      Object.Destroy(gameObject);
  }
}
