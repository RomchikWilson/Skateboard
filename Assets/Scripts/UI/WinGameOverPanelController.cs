using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinGameOverPanelController : MonoBehaviour
{
    private EventTrigger trigger = default;

    void Awake()
    {
        trigger = GetComponent<EventTrigger>();

        //Отследить клик
        EventTrigger.Entry _pointerClick = new EventTrigger.Entry();
        _pointerClick.eventID = EventTriggerType.PointerClick;
        _pointerClick.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });

        trigger.triggers.Add(_pointerClick);
    }

    public void OnPointerClick(PointerEventData data)
    {
        GameManager.PrepareLvl?.Invoke();
    }
}
