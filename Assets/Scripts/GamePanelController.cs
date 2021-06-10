using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePanelController : MonoBehaviour
{
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private Slider slider = default;

    private EventTrigger trigger = default;
    private int countTap = 0;
    private bool tap;
    private Vector3 startMousePosition;
    private int currencySwipeDistance = 0;

    void Awake()
    {
        trigger = GetComponent<EventTrigger>();

        //Отследить нажатие
        EventTrigger.Entry _pointerDown = new EventTrigger.Entry();
        _pointerDown.eventID = EventTriggerType.PointerDown;
        _pointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });

        EventTrigger.Entry _pointerDrag = new EventTrigger.Entry();
        _pointerDrag.eventID = EventTriggerType.Drag;
        _pointerDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        EventTrigger.Entry _pointerUp = new EventTrigger.Entry();
        _pointerUp.eventID = EventTriggerType.PointerUp;
        _pointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });

        trigger.triggers.Add(_pointerDown);
        trigger.triggers.Add(_pointerDrag);
        trigger.triggers.Add(_pointerUp);
    }

    void OnEnable()
    {
        GameManager.PrepareLvl += PrepareControlPanel;
    }

    void OnDisable()
    {
        GameManager.PrepareLvl -= PrepareControlPanel;
    }

    public void FixedUpdate()
    {
        if (countTap == 1)
        {
            return;
        } 

        if (gameStorageSO.Pressed)
        {
            if (tap)
            {
                PlayerController.BuildFirstThreeBoard?.Invoke();
                tap = false;
            }

            PlayerController.BuildBoard?.Invoke(currencySwipeDistance);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        ++countTap;

        startMousePosition = Input.mousePosition;

        tap = true;
        gameStorageSO.Pressed = true;

        if (gameStorageSO.GameState == GameState.OnStart)
        {
            Debug.Log("Start lvl");
            GameManager.StartGame.Invoke();
        }
    }

    public void OnDrag(PointerEventData data)
    {
        currencySwipeDistance = (int)(Input.mousePosition.y - startMousePosition.y) / 8;
        slider.value = currencySwipeDistance;
    }

    public void OnPointerUp(PointerEventData data)
    {
        slider.value = 0;
        gameStorageSO.Pressed = false;
    }

    void PrepareControlPanel()
    {
        tap = false;
        countTap = 0;
        gameStorageSO.Pressed = false;
        slider.value = 0;
    }
}