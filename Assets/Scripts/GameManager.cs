using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameStorage gameStorageSO = default;

    public static Action StartGame = default;
    public static Action PrepareLvl = default;

    void Awake()
    {
        PrepareLvl += PrepareGameLVL;
        PrepareLvl.Invoke();
    }

    void PrepareGameLVL()
    {
        gameStorageSO.GameState = GameState.OnStart;
        gameStorageSO.NumberBoardsOnScateboard = 0;
    }
}
