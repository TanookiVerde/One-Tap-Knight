﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionPanel : MenuPanel {
    public GameObject levelButtonPrefab;
    public Transform buttonRoot;

    public void Start()
    {
        CreateLevelButtons(MemoryCard.Load());
    }
    public void OpenLevel(int levelIndex)
    {
        Transition.transition.TransiteTo("TestScene");
    }
    private void CreateLevelButtons(AdventureLog log)
    {
        for (int i = 0; i < log.levels.Count; i++)
        {
            var b = Instantiate(levelButtonPrefab, buttonRoot);
            b.GetComponent<LevelButton>().Initialize(log.levels[i], i);
            b.GetComponent<Button>().interactable = i <= GetLastUnlockedLevel(log);
        }
    }
    private int GetLastUnlockedLevel(AdventureLog log)
    {
        int i;
        for (i = 0; i < log.levels.Count; i++)
        {
            if (!log.levels[i].completed)
                break;
        }
        return i;
    }
}