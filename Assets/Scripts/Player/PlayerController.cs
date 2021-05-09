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
  private bool paralyzed = false;
  private Vector2 movement;
  private Vector2 movementRangeMin, movementRangeMax;
  private bool focus, shooting;
  private Vector2 startPos;

  // Shooting
  public float firerate, shotSpeed;
  [HideInInspector]
  public float damage;
  [HideInInspector]
  public bool invincible = false;
  private int orbs;
  private float nextFire;
  private BulletData bulletData;

  // Effects
  public GameObject deathEffect; // Make private?
  private SpriteRenderer bombEffect;

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

  // Stop controls for time seconds
  private IEnumerator Paralyze(float time) {
    paralyzed = true;
    yield return new WaitForSeconds(time);
    paralyzed = false;
  }

  // Spawn death visual effect at pos
  // TODO: Shake effect
  private IEnumerator DeathVisualEffect(float fadeTime, float scaleTime, float rotateSpeed, Vector3 pos) {
    GameObject deathObject = Instantiate(deathEffect, pos, Quaternion.identity);
    SpriteRenderer deathSprite = deathObject.GetComponent<SpriteRenderer>();

    yield return ScaleRotateEffect(fadeTime, scaleTime, rotateSpeed, deathSprite);
    Destroy(deathObject);

    yield return null;
  }

  // After delay seconds: Kill all enemies and convert all enemy bullets to score pickups, then collect them
  private IEnumerator BombEffect(float delay) {
    yield return new WaitForSeconds(delay);
    StageHandler.KillAllEnemies();
    BulletHandler.BulletsToScore();
    StageHandler.CollectAllPickups(useConstantScore: true);
  }

  // Scale, rotate and fade out effect
  private IEnumerator ScaleRotateEffect(float fadeTime, float scaleTime, float rotateSpeed, SpriteRenderer effect) {
    effect.transform.localScale = Vector3.zero;
    effect.color = Color.white;
    effect.enabled = true;

    float startTime = 0;
    while(startTime < scaleTime) {
      effect.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, startTime/scaleTime);
      effect.transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.World);

      startTime += Time.deltaTime;
      yield return null;
    }
    effect.transform.localScale = Vector3.one;

    startTime = 0;
    while(startTime < fadeTime) {
      effect.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), startTime/fadeTime);
      effect.transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.World);

      startTime += Time.deltaTime;
      yield return null;
    }
    effect.enabled = false;

    yield return null;
  }

  private void DropPower(Vector3 startPos, float amount) {
    // Calculate number of big and small pickups needed
    // Large pickup = .12 power, small pickup = .03 power
    // TODO: Get pickup values in Start()?
    int bigPickup = Mathf.FloorToInt(amount/.12f);
    int smallPickup = Mathf.FloorToInt((amount-.12f*bigPickup)/.03f);

    // Spawn pickups above player
    while(smallPickup > 0 && bigPickup > 0) {
      if(smallPickup > 0) {
        StageHandler.SpawnPickup(1, startPos, Random.insideUnitCircle + StageHandler.center + 1.5f * Vector2.up, .2f);
        smallPickup--;
      }
      if(bigPickup > 0) {
        StageHandler.SpawnPickup(3, startPos, Random.insideUnitCircle + StageHandler.center + 1.5f * Vector2.up, .2f);
        bigPickup--;
      }
    }
  }

  // Actions
  private void Die() {
    Vector3 deathPos = transform.position;

    // Move to spawn, stop controls and become invincible
    StartCoroutine(DeathVisualEffect(1f, .6f, 80f, deathPos));
    StartCoroutine(Invincibility(3f));
    StartCoroutine(Paralyze(.3f));
    transform.position = startPos;

    // Lose 35% of power and drop 30%
    // Drops power above death position
    DropPower(deathPos, power * 0.30f);
    ChangePower(power * 0.65f);

    // Kill bullets
    BulletHandler.BulletsToScore();

    if(lives > 0) {
      lives--;
      Score.UpdatePlayer(lives);
    } else {
      // Pause and disable resuming on game over
      PauseMenu.TogglePause(PauseMode.GameOver);
      Debug.Log("Game Over");
    }
  }

  private void Bomb() {
    if(bombs > 0) {
      bombs--;
      Score.UpdateBombs(bombs);

      // Start bomb visual and "physical" effects
      StartCoroutine(ScaleRotateEffect(3f, .8f, 80f, bombEffect));
      StartCoroutine(BombEffect(.6f));
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
    bombEffect = transform.Find("BombEffect").GetComponent<SpriteRenderer>();

    // Default firerate
    if(firerate <= 0)
      firerate = 15f;
  }

  void Start() {
    // Initialize orbs
    for(int i = 0; i < 4; i++) {
      GameObject orb = Instantiate(orbPrefab, transform.Find("Orbs"));
      orb.SetActive(false);
      orb.GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
      orb.GetComponent<SpriteRenderer>().sortingOrder = 2;
      orb.GetComponent<Orb>().id = i;
      orb.name = "Orb" + i.ToString();
    }

    // Set spawn position 1 unit above the bottom center of the stage
    startPos = new Vector3(
      (StageHandler.bottomLeft.x + StageHandler.topRight.x)/2,
      StageHandler.bottomLeft.y + 1
    );

    ResetStats(); 

    // Movement bounds (requires camera scale 4.8f because pixel perfect camera)
    // Lots of constants I don't know where to store
    Vector2 playerCollisionSize = new Vector2(11.5f, 22.5f);
    Vector2 targetResolution = new Vector2(640, 480);
    movementRangeMin = Camera.main.ViewportToWorldPoint(new Vector2((32f + playerCollisionSize.x)/targetResolution.x, (16f + playerCollisionSize.y)/targetResolution.y));
    movementRangeMax = Camera.main.ViewportToWorldPoint(new Vector2((32f - playerCollisionSize.x + StageHandler.pixelSize.x)/targetResolution.x, (16f - playerCollisionSize.y + StageHandler.pixelSize.y)/targetResolution.y));

    // Default bullet data
    bulletData = new BulletData(
      Vector3.zero,
      0f,
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
    if(paralyzed) {
      anim.SetFloat("Horizontal", 0);
      anim.SetBool("Moving", false);
    } else {
      anim.SetFloat("Horizontal", movement.x);
      anim.SetBool("Moving", movement.x != 0);
    }

    // Test power
    ChangePower(testPower);
  }

  void FixedUpdate() {
    // Move if not paralyzed
    if(paralyzed)
      rb.velocity = Vector2.zero;
    else
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
        
        AddScore((uint)Mathf.RoundToInt(pickup.value * multiplier));
        break;

      case PickupType.Power:
        ChangePower(power + 0.03f * pickup.value);
        break;
      }
    }
  }
}