using System.Linq;
using System.Globalization;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour {
  private static TMP_Text hiScore, score, player, bombs, power;

  public static void UpdateHiScore(uint newHiScore) { hiScore.text = newHiScore.ToString("000000000"); }
  public static void UpdateScore(uint newScore) { score.text = newScore.ToString("000000000"); }
  public static void UpdatePlayer(int newPlayer) { player.text = string.Concat(Enumerable.Repeat("★", newPlayer)); }
  public static void UpdateBombs(int newBombs) { bombs.text = string.Concat(Enumerable.Repeat("★", newBombs)); }
  public static void UpdatePower(float newPower) {
    if(newPower == 5f)
      power.text = "MAX";
    else
      power.text = newPower.ToString("0.00", CultureInfo.CreateSpecificCulture("ja-JP"));
  }


  void Awake() {
    hiScore = transform.Find("HiScore").GetComponent<TMP_Text>();
    score   = transform.Find("Score").GetComponent<TMP_Text>();
    player  = transform.Find("Player").GetComponent<TMP_Text>();
    bombs   = transform.Find("Bombs").GetComponent<TMP_Text>();
    power   = transform.Find("Power").GetComponent<TMP_Text>();
  }
}
