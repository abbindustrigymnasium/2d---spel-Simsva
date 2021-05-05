using UnityEngine;

public class PlayerController : MonoBehaviour {
  private Rigidbody2D rb;
  private Animator anim;
  private SpriteRenderer focusHitbox;
  public float movementSpeed, focusMovementSpeed;

  private Vector2 movement;
  private bool focus;

  private uint score, hiScore; // TODO: Move hiScore to external file 
  private int lives, bombs;
  private float power;

  // Reset score, lives, etc. to default
  void ResetScore() {
    ResetScore(2, 3);
  }

  void ResetScore(int startLives, int startBombs) {
    score = 0; hiScore = 0;
    lives = startLives; bombs = startBombs;
    power = 2.7f;
    
    Score.UpdateScore(score); Score.UpdateHiScore(hiScore);
    Score.UpdatePlayer(lives); Score.UpdateBombs(bombs);
    Score.UpdatePower(power);
  }

  void Start() {
    ResetScore();

    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    focusHitbox = transform.Find("FocusHitbox").GetComponent<SpriteRenderer>();
  }

  void Update() {
    focus = Input.GetButton("Focus");
    focusHitbox.enabled = focus;

    movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    movement.Normalize(); movement *= focus ? focusMovementSpeed : movementSpeed;

    anim.SetFloat("Horizontal", movement.x);
    anim.SetBool("Moving", movement.x != 0);
  }

  void FixedUpdate() {
    rb.velocity = movement * Time.fixedDeltaTime;
  }
}