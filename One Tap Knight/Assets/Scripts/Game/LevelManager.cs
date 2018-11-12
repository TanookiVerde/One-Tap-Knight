﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    private CameraMovement cameraMovement;
    private KnightController knight;
    private EndPanel endPanel;
    private GameOverPanel gameOverPanel;
    private Timer timer;

    private void Start()
    {
        knight = FindObjectOfType<KnightController>();
        endPanel = FindObjectOfType<EndPanel>();
        gameOverPanel = FindObjectOfType<GameOverPanel>();
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        timer = FindObjectOfType<Timer>();
        StartCoroutine(LevelLoop());
    }
    private IEnumerator LevelLoop()
    {
        Transition.transition.InstaShow();
        Transition.transition.TransiteFrom();
        yield return cameraMovement.StartAnimation();
        cameraMovement.StartFollowing();
        while (!LevelFinished())
        {
            if (!IsPlayerAlive())
            {
                yield return new WaitForSeconds(1f);
                gameOverPanel.Show();
                yield break;
            }
            knight.MovementLoop();
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        print("READED FINISHED");
        endPanel.Show();
    }
    private bool IsPlayerAlive() { return knight != null; }
    private bool LevelFinished() { return knight.finishedLevel; }
}
