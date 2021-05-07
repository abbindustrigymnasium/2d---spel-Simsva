using System.Collections;
using UnityEngine;

public class Stage1Timeline : BaseTimeline {
  private IEnumerator TaskBurst1(int count) {
    GameObject[] enemies = new GameObject[count];

    for(int i = 0; i < count; i++) {
      enemies[i] = stage.SpawnEnemy(0, new Vector3(stage.bottomLeft.x + 1 + i*(Mathf.Abs(stage.bottomLeft.x-stage.topRight.x)-2)/(count-1), stage.topRight.y+1));

      float distance = 2f*(2-(3))/(count-1) * Mathf.Abs(i - (count-1)/2f)+(3);
      Debug.Log(distance);
      stage.StartCoroutine(MoveEnemySmooth(1500, enemies[i], enemies[i].transform.position + distance*Vector3.down));
      yield return new WaitForSeconds(1f);
    }

    /*for(int i = 0; i < 6; i++) {

    }*/
    
    yield return null;
  }

  public override void Init() {
    // Add tasks
    AddTask(3000, TaskBurst1(6));
    AddTask(5000, Log("bruh"));
    AddTask(10000, Log("yeet"));
  }
}
