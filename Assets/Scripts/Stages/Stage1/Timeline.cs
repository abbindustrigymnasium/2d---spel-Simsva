using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Timeline : BaseTimeline {
  private IEnumerator TaskBurst1(int id, int count, float spawnTimeMs = 1000f, float moveTimeMs = 10000f, float waitTimeMs = 200f, float stopDistance = 2f, bool reverse = false) {
    GameObject[] enemies = new GameObject[count];

    // Calculate spread of enemies
    float distance = StageSpread(count);

    for(int i = 0; i < count; i++) {
      // Spawn enemy
      Vector3 pos;
      if(reverse) {
        pos = new Vector3(
          // Skip last "slot" (i+1)
          StageHandler.topRight.x - (i+1)*distance,
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

  // Spawn two enemies
  // Negative centerDistance spreads enemies equally
  public IEnumerator TaskBurst2(int id, float moveTimeMs = 5000f, float waitTimeMs = 300f, float stopDistance = 3f, float centerDistance = -1f) {
    GameObject[] enemies = new GameObject[2];

    float spread;
    if(centerDistance < 0)
      spread = StageSpread(2)/2;
    else
      spread = centerDistance;

    for(int i = 0; i < 2; i++) {
      // Spawn first enemy to the left and the second to the right
      int sign;
      if(i == 0) sign = -1;
      else sign = 1;

      enemies[i] = StageHandler.SpawnEnemy(id, new Vector3(
        StageHandler.center.x + sign*spread,
        StageHandler.topRight.y + 1
      ), true);

      List<IEnumerator> actions = new List<IEnumerator>() {
        MoveEnemySmooth(moveTimeMs, enemies[i], enemies[i].transform.position + stopDistance*Vector3.down),
        WaitMs(waitTimeMs),
        MoveEnemySmooth(moveTimeMs, enemies[i], new Vector3(
          StageHandler.center.x + sign*StageHandler.size.x/2,
          enemies[i].transform.position.y - 2*stopDistance
        )),
        DeleteEnemy(enemies[i])
      };

      StageHandler.StartSequentialCoroutine(actions, enemies[i]);
    }

    yield return null;
  }

  // Spawns two enemies from side and shoots circle spray
  // Negative centerDistance spreads enemies equally
  public IEnumerator TaskBurst3(int id, BulletData bulletData, int count = 40, float waitTimeMs = 500f, float shootTimeMs = 500f, float moveTimeMs = 5000f, float enterDistance = 3f, float centerDistance = -1f) {
    GameObject[] enemies = new GameObject[2];

    float spread;
    if(centerDistance < 0)
      spread = StageSpread(2)/2;
    else
      spread = centerDistance;

    for(int i = 0; i < 2; i++) {
      int sign;
      if(i == 0) sign = -1;
      else sign = 1;

      // Spawn enemies
      enemies[i] = StageHandler.SpawnEnemy(id, new Vector3(
        StageHandler.center.x + sign*(StageHandler.size.x/2 + 1),
        StageHandler.topRight.y - enterDistance
      ), false);

      // Position where the enemy stops
      Vector3 endPos = new Vector3(
        StageHandler.center.x + sign*centerDistance,
        enemies[i].transform.position.y
      );

      // Change position and rotation of shot
      BulletData newBulletData = bulletData;
      newBulletData.pos = endPos;
      newBulletData.rot = BulletHandler.AngleToPlayer(enemies[i].transform.position);

      // Actions
      List<IEnumerator> actions = new List<IEnumerator>() {
        MoveEnemySmooth(moveTimeMs, enemies[i], endPos),
        BulletHandler.ShootSpiral(newBulletData, count, shootTimeMs/1000),
        WaitMs(waitTimeMs),
        ToggleEnemyAI(enemies[i], true)
      };

      StageHandler.StartSequentialCoroutine(actions, enemies[i]);
    }

    yield return null;
  }

  public override void Init() {
    // Bullet data used for tasks
    BulletData bulletData = new BulletData(
      Vector3.zero,
      0f,
      false,
      Color.green,
      2f,
      id: 2
    );

    // Initialize stage
    AddTask(0, PlaySong(1)); // Play "A Soul as Red as a Ground Cherry"
    AddTask(0, SetBackground(0)); // Use Stage1 background

    // Add tasks
    for(int i = 0; i < 3; i++) {
      AddTask(3000 + 500*i, TaskBurst2(0, stopDistance: 3f - .5f*i));

      AddTask(10000 + 500*i, TaskBurst2(0, stopDistance: 3f - .5f*i, centerDistance: StageHandler.size.x/8*(3-i)));
    }

    AddTask(17500, TaskBurst3(2, bulletData, count: 50, moveTimeMs: 2000f, centerDistance: 1f));
  }
}
