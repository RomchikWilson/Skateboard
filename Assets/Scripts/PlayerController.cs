using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static Action<int> BuildBoard = default;
    public static Action BuildFirstThreeBoard = default;

    [Header("Object references")]
    [SerializeField] private ParticleSystem particle = default;
    [SerializeField] private GameObject bottom1;
    [SerializeField] private GameObject bottom2;
    [SerializeField] private GameObject prefabBoard = default;
    [SerializeField] private List<GameObject> boardsOnScateboard;

    [Header("Camera & UI")]
    [SerializeField] private Slider slider = default;
    [SerializeField] private Transform cameraPoint = default;

    [Header("Settings")]
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private GameObject parentOfBoards = default;
    [SerializeField] private int numberOfBoards = 100;    
    [SerializeField] private float maxBoardSpeed = 5f;
    [SerializeField] private float maxBoardSpeedAngular = 5f;
    [SerializeField] private float deadlyHeight = 5f;

    private List<GameObject> boards = default;
    private float velocityPlayer = 1f;
    private Rigidbody rigidBody;
    private int numberBoardsOnScateboard;
    private int countBoardsOnScateboard;
    private int indexSpawnBoard = 0;
    private Vector3 positionBottom;
    private Vector3 positionLastBoard;
    private int touchingTheGround;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    void Awake()
    {
        countBoardsOnScateboard = boardsOnScateboard.Count;
        
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        velocityPlayer = gameStorageSO.DeltaScateboardForce;
        rigidBody = GetComponent<Rigidbody>();

        BuildBoard += SpawnBoard;
        BuildFirstThreeBoard += SpawnFirstThreeBoard;
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
        CameraController.SetTargetAction?.Invoke(cameraPoint);

        transform.position = startingPosition;
        transform.rotation = startingRotation;

        particle.Stop();

        touchingTheGround = 0;
        indexSpawnBoard = 0;

        for (int i = 0; i < boardsOnScateboard.Count; i++)
        {
            boardsOnScateboard[i].SetActive(false);
        }

        if (boards != null)
        {        
            for (int i = 0; i < boards.Count; i++)
            {
                boards[i].transform.localPosition = Vector3.zero;
            }
        }

        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        InitialBoardSpawn();
    }

    private void FixedUpdate()
    {
        if (gameStorageSO.GameState == GameState.InGame && touchingTheGround > 0)
        {
            rigidBody.AddForce(rigidBody.transform.forward  * velocityPlayer, ForceMode.Impulse);
        }

        //transform.eulerAngles = new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, -45, 45), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, rigidBody.velocity.z > maxBoardSpeed ? maxBoardSpeed : rigidBody.velocity.z);
        rigidBody.angularVelocity = new Vector3(rigidBody.angularVelocity.x > maxBoardSpeedAngular ? maxBoardSpeed : rigidBody.angularVelocity.x, rigidBody.angularVelocity.y, rigidBody.angularVelocity.z);

        //Проигрыш при падении
        if (transform.position.y <= deadlyHeight)
        {
            Debug.Log("Player is in the deadly zone");
            CameraController.SetTargetAction?.Invoke(default);
            GameManager.EndOfLevel?.Invoke(false);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        switch (collision.gameObject.tag) 
        {
            case "Boards":
                Debug.Log("Player touched the board");
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

                break;

            case "Barrier":
                Debug.Log("Player touched the barrier");
                particle.Stop();
                CameraController.SetTargetAction?.Invoke(default);
                GameManager.EndOfLevel?.Invoke(false);

                break;

            case "FinishPoint":
                Debug.Log("Player finished");
                particle.Stop();
                GameManager.EndOfLevel?.Invoke(true);
                rigidBody.AddForce(-rigidBody.velocity);

                break;

            default:
                break;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && (gameStorageSO.GameState == GameState.InGame || gameStorageSO.GameState == GameState.OnStart)) touchingTheGround++;
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && gameStorageSO.GameState == GameState.InGame) touchingTheGround--;
    }

    void SpawnBoard(int currencySwipeDistance)
    {
        positionBottom = bottom1.transform.position; //Получаем нижнюю точку скейтборда
        Vector3 positionNewBoard = new Vector3(positionBottom.x, positionLastBoard.y, positionBottom.z + .76f); //Получаем позицию новой доски
        numberBoardsOnScateboard = gameStorageSO.NumberBoardsOnScateboard; //Получаем количество досок на скейтборде

        if (numberBoardsOnScateboard == 0) return;

        if (Vector3.Distance(positionLastBoard, positionNewBoard) < .38f) return;

        positionLastBoard.z += .38f;
        positionLastBoard.y += slider.value / 260;

        boards[indexSpawnBoard].transform.position = positionLastBoard; //Спавним доску перед скейтом

        boards[indexSpawnBoard].transform.LookAt(boards[indexSpawnBoard == 0 ? numberOfBoards - 1 : indexSpawnBoard - 1].transform);

        boardsOnScateboard[numberBoardsOnScateboard - 1].SetActive(false); //Убираем 1 доску с скейта

        if (++indexSpawnBoard > numberOfBoards - 1) indexSpawnBoard = 0;

        gameStorageSO.NumberBoardsOnScateboard--;
    }
    
    void SpawnFirstThreeBoard()
    {
        positionBottom = touchingTheGround > 0 ? bottom1.transform.position : bottom2.transform.position;

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

            if (++indexSpawnBoard > numberOfBoards - 1) indexSpawnBoard = 0;
        }

        //Спавним вторую доску
        if (--countBoardsSpawn >= 0)
        {
            boards[indexSpawnBoard].transform.position = new Vector3(positionBottom.x, positionBottom.y, positionBottom.z); //Спавним доску под скейтом

            positionLastBoard = boards[indexSpawnBoard].transform.position; //Запоминаем позицию последней заспавненой доски

            if (++indexSpawnBoard > numberOfBoards - 1) indexSpawnBoard = 0;
        }

        //Спавним третью доску
        if (--countBoardsSpawn >= 0)
        {
            boards[indexSpawnBoard].transform.position = new Vector3(positionBottom.x, positionBottom.y, positionBottom.z + .38f); //Спавним доску под скейтом

            positionLastBoard = boards[indexSpawnBoard].transform.position; //Запоминаем позицию последней заспавненой доски

            if (++indexSpawnBoard > numberOfBoards - 1) indexSpawnBoard = 0;
        }
    }

    void InitialBoardSpawn()
    {
        boards = new List<GameObject>(numberOfBoards);

        for (int i = 0; i < numberOfBoards; i++) 
        {
            boards.Add(Instantiate(prefabBoard, parentOfBoards.transform));
        }
    }
}
