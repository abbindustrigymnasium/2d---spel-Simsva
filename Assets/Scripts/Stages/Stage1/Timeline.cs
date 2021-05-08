using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Timeline : BaseTimeline {
  private IEnumerator TaskBurst1(int id, int count, float spawnTimeMs = 1000f, float moveTimeMs = 10000f, float waitTimeMs = 200f, float stopDistance = 2f, bool reverse = false) {
    GameObject[] enemies = new GameObject[count];

    // Calculate spread of enemies
    float distance = Mathf.Abs(StageHandler.bottomLeft.x-StageHandler.topRight.x)/(count+1);

    for(int i = 0; i < count; i++) {
      // Spawn enemy
      Vector3 pos;
      if(reverse) {
        pos = new Vector3(
          // Skip last "slot" (count-i)
          StageHandler.bottomLeft.x + (count-i)*distance,
          StageHandler.topRight.y+1
        );
      } else {
        pos = new Vector3(
          // Skip first "slot" (i+1)
          StageHandler.bottomLeft.x + (i+1)*distance,
          StageHandler.topRight.y+1
        );
      }

      enemies[i] = StageHandler.SpawnEnemy(id, pos, true);

      // Actions exeucted for each enemy
      List<IEnumerator> actions = new List<IEnumerator>() {
        MoveEnemySmooth(moveTimeMs * stopDistance/13, enemies[i], enemies[i].transform.position + stopDistance*Vector3.down),
        WaitMs(waitTimeMs),
        MoveEnemySmooth(moveTimeMs * (13-stopDistance)/13, enemies[i], enemies[i].transform.position + (13 - stopDistance)*Vector3.down),
        DeleteEnemy(enemies[i])
      };

      // Pyramid formation
      //float distance = 2f*(2-(3))/(count-1) * Mathf.Abs(i - (count-1)/2f)+(3);
      //Debug.Log(distance);

      // Run actions for enemy asynchronously and wait
      StageHandler.StartSequentialCoroutine(actions, enemies[i]);
      yield return new WaitForSeconds(spawnTimeMs/count / 1000);
    }
    
    yield return null;
  }

  public override void Init() {
    // Add tasks
    AddTask(0, PlaySong(1)); // Play "A Soul as Red as a Ground Cherry"
    AddTask(3000, TaskBurst1(0, 3, spawnTimeMs: 2500, waitTimeMs: 250f, stopDistance: 3f));
    AddTask(6500, TaskBurst1(1, 2, spawnTimeMs: 1500, waitTimeMs: 200f, stopDistance: 2f, reverse: true));
    AddTask(5000, Log("bruh"));
    AddTask(10000, Log("yeet"));
  }
}
