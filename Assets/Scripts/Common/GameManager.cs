using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object references")]
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private GameObject parentLevel = default;
    [SerializeField] private List<GameObject> levels = default;

    public static Action StartGame = default;
    public static Action PrepareLvl = default;
    public static Action<bool> EndOfLevel = default;

    void Awake()
    {
        EndOfLevel += PrepareEndOfLevel;
        PrepareLvl += PrepareGameLVL;
        PrepareLvl.Invoke();
    }

    void PrepareGameLVL()
    {
        gameStorageSO.GameState = GameState.OnStart;
        gameStorageSO.NumberBoardsOnScateboard = 0;

        int i = 0;
        foreach (Transform child in parentLevel.transform)
        {
            DestroyImmediate(child.gameObject);
            i += 1;
        }

        Instantiate(levels[gameStorageSO.CurrentLevel], parentLevel.transform);        
    }

    void PrepareEndOfLevel(bool winner)
    {
        gameStorageSO.GameState = GameState.OnFinish;

        if (winner) 
        {
            gameStorageSO.CurrentLevel = gameStorageSO.CurrentLevel + 1 >= gameStorageSO.CountLevel ? 0 : gameStorageSO.CurrentLevel + 1;
        }
    }
}
