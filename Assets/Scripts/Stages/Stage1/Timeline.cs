using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Timeline : BaseTimeline {
  private IEnumerator TaskBurst1(int id, int count) {
    GameObject[] enemies = new GameObject[count];

    // Spawn count enemies
    for(int i = 0; i < count; i++) {
      enemies[i] = stage.SpawnEnemy(id, new Vector3(stage.bottomLeft.x + 1 + i*(Mathf.Abs(stage.bottomLeft.x-stage.topRight.x)-2)/(count-1), stage.topRight.y+1));

      // Actions exeucted for each enemy
      List<IEnumerator> actions = new List<IEnumerator>() {
        MoveEnemySmooth(1500, enemies[i], enemies[i].transform.position + 2*Vector3.down),
        WaitMs(3500),
        MoveEnemySmooth(8000, enemies[i], enemies[i].transform.position + 11*Vector3.down),
        KillEnemy(enemies[i])
      };

      // Pyramid formation
      //float distance = 2f*(2-(3))/(count-1) * Mathf.Abs(i - (count-1)/2f)+(3);
      //Debug.Log(distance);

      // Run actions for enemy asynchronously and wait
      stage.StartSequentialCoroutine(actions, enemies[i]);
      yield return new WaitForSeconds(1f);
    }
    
    yield return null;
  }

  public override void Init() {
    // Add tasks
    AddTask(3000, TaskBurst1(0, 6));
    AddTask(5000, Log("bruh"));
    AddTask(10000, Log("yeet"));
  }
}
