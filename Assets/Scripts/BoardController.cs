using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speedRotate = 100f;

    void FixedUpdate()
    {
        transform.Rotate(0, speedRotate, 0, Space.Self);
    }
}
