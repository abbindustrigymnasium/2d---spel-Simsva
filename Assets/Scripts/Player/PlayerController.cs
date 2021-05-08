using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
  public static PlayerController instance;

  // Components
  private Rigidbody2D rb;
  private CircleCollider2D collider2d;
  private Animator anim;
  private SpriteRenderer sprite, focusHitbox;

  // Prefabs
  public GameObject orbPrefab;

  // Movement/controls
  public float movementSpeed, focusMovementSpeed;
  private Vector2 movement;
  private Vector2 movementRangeMin, movementRangeMax;
  private bool focus, shooting;
  private Vector2 startPos;

  // Shooting
  public float firerate, shotSpeed;
  [HideInInspector]
  public float damage;
  private bool invincible = false;
  private int orbs;
  private float nextFire;
  private BulletData bulletData;

  // Score
  public float collectionLine = .8f;
  private float collectionLineY;
  private uint score, hiScore; // TODO: Move hiScore to external file 
  private int lives, bombs;
  private float power;
  
  // Testing, remove later
  public float testPower;

  // Reset score, lives, etc. to default
  private void ResetStats(int startLives = 2, int startBombs = 3) {
    // Reset variables
    score = 0; hiScore = 0;
    lives = startLives; bombs = startBombs;
    ChangePower(1f);
    
    // Update UI
    Score.UpdateScore(score);  Score.UpdateHiScore(hiScore);
    Score.UpdatePlayer(lives); Score.UpdateBombs(bombs);

    // Reset position
    transform.position = startPos;
  }

  // Become invincivle for time seconds
  private IEnumerator Invincibility(float time) {
    invincible = true;
    sprite.color = new Color(1f, 1f, 1f, .8f);
    yield return new WaitForSeconds(time);
    invincible = false;
    sprite.color = new Color(1f, 1f, 1f, 1f);
  }

  private void DropPower(float amount) {
    // Calculate number of big and small pickups needed
    // Large pickup = .12 power, small pickup = .03 power
    // TODO: Get pickup values in Start()?
    int bigPickup = Mathf.FloorToInt(amount/.12f);
    int smallPickup = Mathf.FloorToInt((amount-.12f*bigPickup)/.03f);

    // Spawn pickups above player
    while(smallPickup > 0 && bigPickup > 0) {
      if(smallPickup > 0) {
        StageHandler.SpawnPickup(1, (Vector3)Random.insideUnitCircle + transform.position + Vector3.up);
        smallPickup--;
      }
      if(bigPickup > 0) {
        StageHandler.SpawnPickup(3, (Vector3)Random.insideUnitCircle + transform.position + Vector3.up);
        bigPickup--;
      }
    }
  }

  // Actions
  private void Die() {
    // Lose 35% of power and drop 30%
    // Drops power above death position
    DropPower(power * 0.30f);
    ChangePower(power * 0.65f);

    // Move to spawn and become invincible
    StartCoroutine(Invincibility(3f));
    transform.position = startPos;

    if(lives > 0) {
      lives--;
      Score.UpdatePlayer(lives);
    } else {
      Debug.Log("Game Over");
    }
  }

  private void Bomb() {
    if(bombs > 0) {
      bombs--;
      Score.UpdateBombs(bombs);

      // Kill all enemies and convert all enemy bullets to score pickups, then collect them
      // TODO: Shake effect + visuals
      BulletHandler.BulletsToScore();
      StageHandler.KillAllEnemies();
      StageHandler.CollectAllPickups(useConstantScore: true);
    }
  }

  // Add/remove orbs
  private void UpdateOrb(int newOrbs) {
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
  public void ChangePower(float newPower) {
    newPower = Mathf.Clamp(newPower, 0f, 5f);
    testPower = newPower; // Remove

    power = newPower;
    Score.UpdatePower(newPower);
    damage = Mathf.Lerp(1f, 5f, newPower/5);

    UpdateOrb(Mathf.Clamp(Mathf.FloorToInt(newPower), 0, 4));
  }

  // Add to score
  public void AddScore(uint scoreChange) {
    score += scoreChange;
    Score.UpdateScore(score);

    if(score > hiScore) {
      hiScore = score;
      Score.UpdateHiScore(hiScore);
    }
  }

  // Shoot
  private void HandleShot(BulletData data, float power) {
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
    sprite = GetComponent<SpriteRenderer>();
    focusHitbox = transform.Find("FocusHitbox").GetComponent<SpriteRenderer>();

    // Default firerate
    if(firerate <= 0)
      firerate = 15f;
  }

  void Start() {
    // Initialize orbs
    for(int i = 0; i < 4; i++) {
      GameObject orb = Instantiate(orbPrefab, transform.Find("Orbs"));
      orb.SetActive(false);
      orb.GetComponent<Orb>().id = i;
      orb.name = "Orb" + i.ToString();
    }

    // Set spawn position 1 unit above the bottom center of the stage
    startPos = new Vector3(
      (StageHandler.bottomLeft.x + StageHandler.topRight.x)/2,
      StageHandler.bottomLeft.y + 1
    );

    ResetStats();

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

    // Calculate collection line y
    collectionLineY = collectionLine*(StageHandler.topRight.y - StageHandler.bottomLeft.y) + StageHandler.bottomLeft.y;
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

    // Movement
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

    // Collect all pickups if above collection line
    if(transform.position.y > collectionLineY)
      StageHandler.CollectAllPickups();
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if(collider.CompareTag("Enemy")) {
      if(!invincible)
        Die();
    } else if(collider.CompareTag("Pickup")) {
      Pickup pickup = collider.GetComponent<Pickup>();

      switch(pickup.type) {
      case PickupType.Point:
        float multiplier = (pickup.fixedScore == 0) ? pickup.GetScore() : pickup.fixedScore;
        uint score = (uint)Mathf.RoundToInt(pickup.value * multiplier);

        Debug.Log(score);
        AddScore(score);
        break;

      case PickupType.Power:
        ChangePower(power + 0.03f * pickup.value);
        break;
      }
    }
  }
}