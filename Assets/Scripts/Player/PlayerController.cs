using UnityEngine;

public class PlayerController : MonoBehaviour {
  public static PlayerController instance;

  // Components
  private Rigidbody2D rb;
  private CircleCollider2D collider2d;
  private Animator anim;
  private SpriteRenderer focusHitbox;

  // Prefabs
  public GameObject orbPrefab;

  // Movement/controls
  public float movementSpeed, focusMovementSpeed;
  private Vector2 movement;
  private Vector2 movementRangeMin, movementRangeMax;
  private bool focus, shooting;

  // Shooting
  public float firerate, shotSpeed;
  [HideInInspector]
  public float damage;
  private int orbs;
  private float nextFire;
  private BulletData bulletData;

  // Score
  private uint score, hiScore; // TODO: Move hiScore to external file 
  private int lives, bombs;
  private float power;
  
  // Testing, remove later
  public float testPower;

  // Reset score, lives, etc. to default
  void ResetScore(int startLives = 2, int startBombs = 3) {
    score = 0; hiScore = 0;
    lives = startLives; bombs = startBombs;
    ChangePower(0f);
    
    Score.UpdateScore(score); Score.UpdateHiScore(hiScore);
    Score.UpdatePlayer(lives); Score.UpdateBombs(bombs);
  }

  // Actions
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

      BulletHandler.KillAllBullets(1);
    }
  }

  // Add/remove orbs
  void UpdateOrb(int newOrbs) {
    newOrbs = Mathf.Clamp(newOrbs, 0, 4);
    if(orbs != newOrbs) {
      orbs = newOrbs;
      Orb.orbsEnabled = newOrbs;

      // Enable orb GameObjects
      for(int i = 0; i < 4; i++) {
        GameObject orb = transform.Find("Orbs").Find("Orb" + i.ToString()).gameObject;

        orb.SetActive(i < newOrbs);
      }
    }
  }

  // Update power value
  void ChangePower(float newPower) {
    newPower = Mathf.Clamp(newPower, 0f, 5f);
    testPower = newPower; // Remove

    power = newPower;
    Score.UpdatePower(newPower);
    damage = Mathf.Lerp(1f, 5f, newPower/5);

    UpdateOrb(Mathf.Clamp(Mathf.FloorToInt(newPower), 0, 4));
  }

  // Shoot
  void HandleShot(BulletData data, float power) {
    switch(Mathf.FloorToInt(power)) {
    case 0:
    case 1:
      BulletHandler.ShootSplit(data, 3f, 2);
      break;
    case 2:
    case 3:
      BulletHandler.ShootSplit(data, 3f, 3);
      break;
    case 4:
      BulletHandler.ShootSplit(data, 4f, 3);
      break;
    default:
      BulletHandler.ShootSplit(data, 4f, 4);
      break;
    }

    foreach(Transform orb in transform.Find("Orbs")) {
      if(orb.gameObject.activeSelf)
        orb.GetComponent<Orb>().Shoot();
    }
  }

  void Awake() {
    // Save static reference to instance
    instance = this;

    // Get Components
    rb = GetComponent<Rigidbody2D>();
    collider2d = GetComponent<CircleCollider2D>();
    anim = GetComponent<Animator>();
    focusHitbox = transform.Find("FocusHitbox").GetComponent<SpriteRenderer>();

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
      90f,
      true,
      Color.red,
      shotSpeed
    );

    // Initialize orbs
    for(int i = 0; i < 4; i++) {
      GameObject orb = Instantiate(orbPrefab, transform.Find("Orbs"));
      orb.SetActive(false);
      orb.GetComponent<Orb>().id = i;
      orb.name = "Orb" + i.ToString();
    }
  }

  void Update() {
    // Handle controls
    focus = Input.GetButton("Focus");
    focusHitbox.enabled = focus;
    Orb.radius = focus ? .4f : .6f;

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
    if(collider.CompareTag("Enemy"))
      Die();
  }
}