using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameStorage", fileName = "GameStorageSO")]
public class GameStorage : ScriptableObject
{
    [SerializeField] private GameState gameState = GameState.OnStart;
    [SerializeField] private float scateboardVelocity = 0f;
    [SerializeField] private int numberBoardsOnScateboard = 0;
    [SerializeField] private int countLevel = 1;
    [SerializeField] private int currentLevel = 0;

    public GameState GameState { get => gameState; set => gameState = value; }
    public float DeltaScateboardForce { get => scateboardVelocity; set => scateboardVelocity = value; }
    public int NumberBoardsOnScateboard { get => numberBoardsOnScateboard; set => numberBoardsOnScateboard = value; }
    public bool Pressed { get; set; }
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public int CountLevel { get => countLevel; set => countLevel = value; }
}

