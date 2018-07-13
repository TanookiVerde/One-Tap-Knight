﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelManager : MonoBehaviour {
    [Header("Stats")]
    [SerializeField] public static int collectedCoins;

    [Header("Panels")]
    [SerializeField] private StartPanel startTextPanel;
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private EndLevelPanel endLevelPanel;

    [Header("Player")]
    [SerializeField] private Transform playerInitialPosition;
    [SerializeField] private GameObject playerPrefab;

    private Character player;
    private PausePanel pause;

    private void Start()
    {
        CreateAndGetPlayer();
        GetPause();
        StartCoroutine(LevelState());
    }
    private IEnumerator LevelState()
    {
        startTextPanel.SetActive(true);
        yield return WaitForPlayerInitialInput();
        startTextPanel.SetActive(false);
        while (IsPlayerAlive() && !IsLevelFinished())
        {
            if(!IsGamePaused())
                player.Action();
            yield return null;
        }
        if (!IsPlayerAlive())
        {
            gameOverPanel.SetActive(true);
        } else if (IsLevelFinished())
        {
            SaveAndLoad.FinishAndSaveLevel(collectedCoins == Coin.totalCoin, collectedCoins == 0);
            player.Stop();
            yield return new WaitForSeconds(player.timeToFinish);
            endLevelPanel.SetActive(true);
        }
        Coin.ResetTotalCoin();
        collectedCoins = 0;
    }
    #region QoL Functions
    private IEnumerator WaitForPlayerInitialInput()
    {
        while (!Input.GetMouseButton(0)) yield return null;
    }
    private void CreateAndGetPlayer()
    {
        player = Instantiate(playerPrefab, playerInitialPosition.position, Quaternion.identity).GetComponent<Character>();
        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);
        Camera.main.GetComponent<CameraMovement>().goToFollow = player.gameObject;
    }
    private void GetPause(){
        pause = FindObjectOfType<PausePanel>();
    }
    private bool IsPlayerAlive()
    {
        return player != null;
    }
    private bool IsLevelFinished()
    {
        return player.FinishedLevel();
    }
    private bool IsGamePaused(){
        return pause.paused;
    }
    #endregion
}
