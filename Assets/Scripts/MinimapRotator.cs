using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRotator : MonoBehaviour
{
    public Transform player;
    private Vector3 orig;

    private void Start()
    {
        orig = transform.position;
    }

    void Update()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = player.eulerAngles.y;
        transform.rotation = Quaternion.Euler(euler);
    }
}
