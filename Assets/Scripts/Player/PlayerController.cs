using UnityEngine;
using UnityEngine.U2D;

public class PlayerController : MonoBehaviour {
  private Rigidbody2D rb;
  private CircleCollider2D collider2d;
  private Animator anim;
  private SpriteRenderer spriteRenderer, focusHitbox;
  private PixelPerfectCamera pixelCamera;

  public float movementSpeed, focusMovementSpeed;

  private Vector2 movement;
  private Vector2 movementRangeMin, movementRangeMax;
  private bool focus, shooting;

  private float nextFire;
  public float firerate, shotSpeed;
  private BulletData bulletData;

  private uint score, hiScore; // TODO: Move hiScore to external file 
  private int lives, bombs;
  private float power; public float testPower; // Remove
  public static float damage;

  // Reset score, lives, etc. to default
  void ResetScore() {
    ResetScore(2, 3);
  }

  void ResetScore(int startLives, int startBombs) {
    score = 0; hiScore = 0;
    lives = startLives; bombs = startBombs;
    ChangePower(0f);
    
    Score.UpdateScore(score); Score.UpdateHiScore(hiScore);
    Score.UpdatePlayer(lives); Score.UpdateBombs(bombs);
  }

  void Die() {
    if(lives > 0) {
      lives--;
      Score.UpdatePlayer(lives);
    } else {
      Debug.Log("Game Over");
    }
  }

  void Bomb() {
    Debug.Log("Bomb");

    if(bombs > 0) {
      bombs--;
      Score.UpdateBombs(bombs);

      BulletHandler.KillAllBullets();
    }
  }

  void ChangePower(float newPower) {
    newPower = Mathf.Clamp(newPower, 0f, 5f);
    testPower = newPower; // Remove

    power = newPower;
    Score.UpdatePower(newPower);
    damage = Mathf.Lerp(1f, 5f, newPower);
  }

  void HandleShot(BulletData data, float power) {
    switch(Mathf.Floor(power)) {
    case 0f:
      BulletHandler.ShootSplit(data, 5f, 3);
      break;
    case 1f:
      BulletHandler.ShootSplit(data, 3f, 3);
      break;
    case 2f:
      BulletHandler.ShootSplit(data, 4f, 4);
      break;
    case 3f:
      BulletHandler.ShootSplit(data, 3f, 4);
      break;
    case 4f:
      BulletHandler.ShootSplit(data, 5f, 5);
      break;
    default:
      BulletHandler.ShootSplit(data, 3f, 5);
      break;
    }
  }

  void Awake() {
    // Get Components
    rb = GetComponent<Rigidbody2D>();
    collider2d = GetComponent<CircleCollider2D>();
    anim = GetComponent<Animator>();
    focusHitbox = transform.Find("FocusHitbox").GetComponent<SpriteRenderer>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    pixelCamera = Camera.main.GetComponent<PixelPerfectCamera>();

    // Default firerate
    if(firerate <= 0)
      firerate = 15f;
  }

  void Start() {
    ResetScore();

    // Movement bounds (requires camera scale 4.8f because reasons)
    Vector3 bottomLeftWorld = Camera.main.ScreenToWorldPoint(new Vector3(32 + 11, 16 + 23, 0)); //Camera.main.ViewportToWorldPoint(new Vector3( 1f/20,  1f/30, 0f));
    Vector3 topRightWorld   = Camera.main.ScreenToWorldPoint(new Vector3(32 - 12 + 384, 16 - 22 + 448, 0)); //Camera.main.ViewportToWorldPoint(new Vector3(13f/20, 29f/30, 0f));

    movementRangeMin = new Vector2(bottomLeftWorld.x, bottomLeftWorld.y);
    movementRangeMax = new Vector2(topRightWorld.x,   topRightWorld.y);

    // Default bullet data
    bulletData = new BulletData(
      Vector3.zero,
      0f,
      true,
      Color.red,
      shotSpeed
    );
  }

  void Update() {
    // Handle controls
    focus = Input.GetButton("Focus");
    focusHitbox.enabled = focus;

    shooting = Input.GetButton("Shoot");
    // Reset fire timer
    if(Input.GetButtonDown("Shoot"))
      nextFire = Time.time;
    
    if(Input.GetButtonDown("Bomb"))
      Bomb();

    movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    movement.Normalize(); movement *= focus ? focusMovementSpeed : movementSpeed;

    // Animation
    anim.SetFloat("Horizontal", movement.x);
    anim.SetBool("Moving", movement.x != 0);

    // Test power
    ChangePower(testPower);
  }

  void FixedUpdate() {
    rb.velocity = movement * Time.fixedDeltaTime;

    // Clamp position to screen
    transform.position = new Vector3(
      Mathf.Clamp(transform.position.x, movementRangeMin.x, movementRangeMax.x),
      Mathf.Clamp(transform.position.y, movementRangeMin.y, movementRangeMax.y)
    );

    // Loop instead of if, for firerates higher than the framerate
    while(shooting && Time.time > nextFire) {
      // Update velocity and position
      bulletData.velocity = shotSpeed;
      bulletData.pos = transform.position + Vector3.up*0.3f;

      HandleShot(bulletData, power);

      nextFire += 1/firerate; // TODO: Change to firedelay
    }
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if(collider.gameObject.tag == "Enemy")
      Die();
  }
}