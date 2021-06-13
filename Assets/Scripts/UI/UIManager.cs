using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Object references")]
    [SerializeField] private GameObject panel_winnerGameOver = default;
    [SerializeField] private Text centerText_WinnerGameOver = default;
    [SerializeField] private GameObject panel_Game = default;

    private void OnEnable()
    {
        GameManager.EndOfLevel += PrepareEndOfLevel;
        GameManager.PrepareLvl += PrepareLevel;
    }

    private void OnDisable()
    {
        GameManager.EndOfLevel -= PrepareEndOfLevel;
        GameManager.PrepareLvl -= PrepareLevel;
    }

    private void PrepareLevel()
    {
        panel_winnerGameOver.SetActive(false);
        centerText_WinnerGameOver.text = "";


        panel_Game.SetActive(true);
    }

    void PrepareEndOfLevel(bool winner)
    {
        if (winner) centerText_WinnerGameOver.text = "Win"; else centerText_WinnerGameOver.text = "Game over";

        panel_Game.SetActive(false);
        panel_winnerGameOver.SetActive(true);
    }
}
