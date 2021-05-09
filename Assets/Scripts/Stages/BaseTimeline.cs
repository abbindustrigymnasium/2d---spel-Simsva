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

  // Methods
  // Calculates equal spacing across stage
  protected float StageSpread(int count) {
    return StageHandler.size.x/(count+1);
  }

  // Useful IEnumerators
  protected IEnumerator Log(string text) {
    Debug.Log(text);
    yield return null;
  }

  // Used in List<IEnumerator> instead of WaitForSeconds because of type
  protected IEnumerator WaitMs(float ms) {
    yield return new WaitForSeconds(ms/1000f);
  }

  protected IEnumerator SpawnEnemy(int id, Vector3 pos, bool hasAi = false, float hpOverride = -1f) {
    StageHandler.SpawnEnemy(id, pos, hasAi: hasAi, hpOverride: hpOverride);
    yield return null;
  }

  protected IEnumerator DeleteEnemy(GameObject enemy, float t = 0f) {
    StageHandler.DestroyEnemy(enemy, t);
    yield return null;
  }

  // Play background music
  protected IEnumerator PlaySong(int id) {
    MusicHandler.PlaySong(id);
    yield return null;
  }

  // Stop background music
  protected IEnumerator StopSong() {
    MusicHandler.StopSong();
    yield return null;
  }

  // Set background image
  protected IEnumerator SetBackground(int id) {
    StageHandler.SetBackground(id);
    yield return null;
  }

  // Toggle enemy AI
  protected IEnumerator ToggleEnemyAI(GameObject enemy, bool hasAi) {
    if(enemy.GetComponent<BaseAI>() != null) {
      enemy.GetComponent<Enemy>().hasAi = hasAi;
      enemy.GetComponent<BaseAI>().enabled = hasAi;
    }
    yield return null;
  }

  // Add task to list
  protected void AddTask(int timeMs, IEnumerator task) {
    tasks.Add(new Task(timeMs, task));
  }

  // Loop through tasks
  public IEnumerator Run() {
    // Initialize and sort tasks
    Init();
    tasks.Sort((x, y) => x.timeMs.CompareTo(y.timeMs));

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
