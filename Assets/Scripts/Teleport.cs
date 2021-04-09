﻿using System.Collections;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform connectedStation;
    public Transform player;
    public GameObject justTeleported;
    public GameObject light;
    public GameObject cube;
    private bool inPort;
    private Vector3 orig;

    private void Start()
    {
        orig = light.transform.position;
    }

    private void Update()
    {
        if (inPort)
        {
            light.transform.position += new Vector3(0, .01f, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            cube.SetActive(true);
            if (!justTeleported.activeSelf)
            {
                inPort = true;
                StartCoroutine(startTele());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            inPort = false;
            light.transform.position = orig;
            justTeleported.SetActive(false);
        }
    }

    IEnumerator startTele()
    {
        yield return new WaitForSeconds(2.5f);
        if (inPort)
        {
            player.gameObject.SetActive(false);
            player.position = new Vector3(connectedStation.position.x, 0, connectedStation.position.z);
            player.gameObject.SetActive(true);
            light.transform.position = orig;
            inPort = false;
            justTeleported.SetActive(true);
        }
    }
}