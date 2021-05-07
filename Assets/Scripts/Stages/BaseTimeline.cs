using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Task {
  public int timeMs;
  public IEnumerator task;

  public Task(int timeMs, IEnumerator task) {
    this.timeMs = timeMs;
    this.task = task;
  }
}

public class BaseTimeline {
  // List of tasks in timeline (must be in chronological order)
  protected List<Task> tasks = new List<Task>();

  // Useful IEnumerators
  protected IEnumerator Log(string text) {
    Debug.Log(text);
    yield return null;
  }

  // Used in List<IEnumerator> instead of WaitForSeconds because of type
  protected IEnumerator WaitMs(float ms) {
    yield return new WaitForSeconds(ms/1000f);
  }

  protected IEnumerator DeleteEnemy(GameObject enemy, float t = 0f) {
    StageHandler.DestroyEnemy(enemy, t);
    yield return null;
  }

  protected IEnumerator MoveEnemy(float timeMs, GameObject enemy, Vector3 endPos) {
    Vector3 startPos = enemy.transform.position;
    float startTime = 0;

    while(startTime < timeMs/1000) {
      // Break if enemy is killed
      if(enemy == null) yield break;

      enemy.transform.position = Vector3.Lerp(startPos, endPos, startTime/(timeMs/1000));
      startTime += Time.deltaTime;
      yield return null;
    }
    enemy.transform.position = endPos;
  }

  protected IEnumerator MoveEnemySmooth(float timeMs, GameObject enemy, Vector3 endPos) {
    Vector3 startPos = enemy.transform.position;
    float startTime = 0;

    while(startTime < timeMs/1000) {
      if(enemy == null) yield break;

      enemy.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, startTime/(timeMs/1000)));
      startTime += Time.deltaTime;
      yield return null;
    }
    enemy.transform.position = endPos;
  }

  // Add task to list
  protected void AddTask(int timeMs, IEnumerator task) {
    tasks.Add(new Task(timeMs, task));
  }

  // Loop through tasks
  public IEnumerator Run() {
    // Initialize tasks
    Init();

    Int64 start = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    foreach(Task task in tasks) {
      int delay = task.timeMs - (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - start);

      if(delay > 0)
        yield return new WaitForSeconds(delay/1000f);

      StageHandler.instance.StartCoroutine(task.task);
    }
  }

  public virtual void Init() {}
}
