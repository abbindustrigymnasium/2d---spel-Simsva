using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Timeline : BaseTimeline {
  private IEnumerator TaskBurst1(int id, int count, float spawnTimeMs = 1000f, float moveTimeMs = 10000f, float waitTimeMs = 200f, float stopDistance = 2f) {
    GameObject[] enemies = new GameObject[count];

    // Calculate spread of enemies
    float distance = Mathf.Abs(stage.bottomLeft.x-stage.topRight.x)/(count+1);

    for(int i = 0; i < count; i++) {
      // Spawn enemy
      enemies[i] = stage.SpawnEnemy(
        id,
        new Vector3(
          // Skip first "slot" (i+1)
          stage.bottomLeft.x + (i+1)*distance,
          stage.topRight.y+1
        )
      );

      // Actions exeucted for each enemy
      List<IEnumerator> actions = new List<IEnumerator>() {
        MoveEnemySmooth(moveTimeMs * stopDistance/13, enemies[i], enemies[i].transform.position + stopDistance*Vector3.down),
        WaitMs(waitTimeMs),
        MoveEnemySmooth(moveTimeMs * (13-stopDistance)/13, enemies[i], enemies[i].transform.position + (13 - stopDistance)*Vector3.down),
        KillEnemy(enemies[i])
      };

      // Pyramid formation
      //float distance = 2f*(2-(3))/(count-1) * Mathf.Abs(i - (count-1)/2f)+(3);
      //Debug.Log(distance);

      // Run actions for enemy asynchronously and wait
      stage.StartSequentialCoroutine(actions, enemies[i]);
      yield return new WaitForSeconds(spawnTimeMs/count / 1000);
    }
    
    yield return null;
  }

  private IEnumerator Test(int id, Vector3 pos) {
    stage.SpawnEnemy(id, pos);
    yield return null;
  }

  public override void Init() {
    // Add tasks
    //AddTask(3000, TaskBurst1(0, 6, spawnTimeMs: 3000, waitTimeMs: 250f));
    AddTask(0, Test(0, new Vector3(-2, 0)));
    AddTask(5000, Log("bruh"));
    AddTask(10000, Log("yeet"));
  }
}
