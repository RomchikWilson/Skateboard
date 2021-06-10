using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static Action<int> BuildBoard = default;
    public static Action BuildFirstThreeBoard = default;

    [SerializeField] private ParticleSystem particle = default;
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private List<GameObject> boardsOnScateboard;
    [SerializeField] private List<GameObject> boards;
    [SerializeField] private GameObject bottom;
    [SerializeField] private float maxBoardSpeed = 5f;
    [SerializeField] private float maxBoardSpeedAngular = 5f;
    [SerializeField] private Transform cameraPoint = default;
    [SerializeField] private Slider slider = default;

    private float velocityPlayer = 1f;
    private Rigidbody rigidBody;
    private int numberBoardsOnScateboard;
    private int countBoardsOnScateboard;
    private int countBoards;
    private int indexSpawnBoard = 0;
    private Vector3 positionBottom;
    private Vector3 positionLastBoard;

    void Awake()
    {
        countBoards = boards.Count;
        countBoardsOnScateboard = boardsOnScateboard.Count;

        velocityPlayer = gameStorageSO.DeltaScateboardForce;
        rigidBody = GetComponent<Rigidbody>();

        BuildBoard += SpawnBoard;
        BuildFirstThreeBoard += SpawnFirstThreeBoard;
    }

    private void Start()
    {
        CameraController.SetTargetAction?.Invoke(cameraPoint);
    }

    void OnEnable()
    {
        GameManager.StartGame += Starting;
        GameManager.PrepareLvl += PrepareScatebord;
    }

    void OnDisable()
    {
        GameManager.StartGame -= Starting;
        GameManager.PrepareLvl -= PrepareScatebord;
    }

    private void Starting()
    {
        particle.Play();
        gameStorageSO.GameState = GameState.InGame;
    }

    private void PrepareScatebord()
    {
        particle.Stop();
        indexSpawnBoard = 0;
    }

    private void FixedUpdate()
    {
        if (gameStorageSO.GameState == GameState.InGame)
        {
            rigidBody.AddForce(rigidBody.transform.forward  * velocityPlayer, ForceMode.Impulse);
            //rigidBody.AddTorque(-rigidBody.transform.right * velocityPlayer / 50f, ForceMode.Impulse);
        }
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, rigidBody.velocity.z > maxBoardSpeed ? maxBoardSpeed : rigidBody.velocity.z);
        rigidBody.angularVelocity = new Vector3(rigidBody.angularVelocity.x > maxBoardSpeedAngular ? maxBoardSpeed : rigidBody.angularVelocity.x, rigidBody.angularVelocity.y, rigidBody.angularVelocity.z);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Boards") 
        {
            collision.gameObject.SetActive(false);

            numberBoardsOnScateboard = gameStorageSO.NumberBoardsOnScateboard;
            int count = 0;

            if (numberBoardsOnScateboard + 5 > countBoardsOnScateboard)
            {
                count = countBoardsOnScateboard;
            }
            else
            {
                count = numberBoardsOnScateboard + 5;
            }

            for (int i = numberBoardsOnScateboard; i < count; i++) 
            {
                boardsOnScateboard[i].SetActive(true);
            }

            gameStorageSO.NumberBoardsOnScateboard = count;
        }
    }

    void SpawnBoard(int currencySwipeDistance)
    {
        positionBottom = bottom.transform.position; //Получаем нижнюю точку скейтборда
        Vector3 positionNewBoard = new Vector3(positionBottom.x, positionLastBoard.y, positionBottom.z + .76f); //Получаем позицию новой доски
        numberBoardsOnScateboard = gameStorageSO.NumberBoardsOnScateboard; //Получаем количество досок на скейтборде

        if (numberBoardsOnScateboard == 0) return;

        if (Vector3.Distance(positionLastBoard, positionNewBoard) < .38f) return;

        positionLastBoard.z += .38f;
        positionLastBoard.y += slider.value / 260;

        boards[indexSpawnBoard].transform.position = positionLastBoard; //Спавним доску перед скейтом

        boardsOnScateboard[numberBoardsOnScateboard - 1].SetActive(false); //Убираем 1 доску с скейта

        if (++indexSpawnBoard > countBoards - 1) indexSpawnBoard = 0;

        gameStorageSO.NumberBoardsOnScateboard--;
    }
    
    void SpawnFirstThreeBoard()
    {
        positionBottom = bottom.transform.position;

        numberBoardsOnScateboard = gameStorageSO.NumberBoardsOnScateboard; //Получаем количество досок на скейтборде

        if (numberBoardsOnScateboard == 0) return;

        int countBoardsSpawn = 0;

        int startOfCount = numberBoardsOnScateboard - 3 <= 0 ? 0 : numberBoardsOnScateboard - 4;
             
        for (int i = startOfCount; i < numberBoardsOnScateboard; i++)
        {
            boardsOnScateboard[i].SetActive(false); //Убираем 1 доску с скейта
            gameStorageSO.NumberBoardsOnScateboard--;
            countBoardsSpawn++;
        }

        //Спавним первую доску
        if (--countBoardsSpawn >= 0)
        {
            boards[indexSpawnBoard].transform.position = new Vector3(positionBottom.x, positionBottom.y, positionBottom.z - .38f); //Спавним доску под скейтом

            positionLastBoard = boards[indexSpawnBoard].transform.position; //Запоминаем позицию последней заспавненой доски

            if (++indexSpawnBoard > countBoards - 1) indexSpawnBoard = 0;
        }

        //Спавним вторую доску
        if (--countBoardsSpawn >= 0)
        {
            boards[indexSpawnBoard].transform.position = new Vector3(positionBottom.x, positionBottom.y, positionBottom.z); //Спавним доску под скейтом

            positionLastBoard = boards[indexSpawnBoard].transform.position; //Запоминаем позицию последней заспавненой доски

            if (++indexSpawnBoard > countBoards - 1) indexSpawnBoard = 0;
        }

        //Спавним третью доску
        if (--countBoardsSpawn >= 0)
        {
            boards[indexSpawnBoard].transform.position = new Vector3(positionBottom.x, positionBottom.y, positionBottom.z + .38f); //Спавним доску под скейтом

            positionLastBoard = boards[indexSpawnBoard].transform.position; //Запоминаем позицию последней заспавненой доски

            if (++indexSpawnBoard > countBoards - 1) indexSpawnBoard = 0;
        }
    }
}
