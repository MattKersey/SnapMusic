using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRotator : MonoBehaviour
{
    public Transform player;

    void Start()
    {     
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(90f, player.localEulerAngles.y, 0f);
    }
}
