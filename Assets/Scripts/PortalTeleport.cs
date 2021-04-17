﻿using System.Collections;
using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
    //the OVRPlayerController object
    public GameObject player;
    private Vector3 prev_position;
    public Camera portalView;
    public AudioSource teled;
    public OVRScreenFade cameraFader;

    private void Start()
    {
        cameraFader = GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>();
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (player.transform.position.x > 8 || player.transform.position.z > 8 ||
                player.transform.position.x < -8 || player.transform.position.z < -8)
            {
                cameraFader.FadeOut();
                StartCoroutine(TelepHome());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            cameraFader.FadeOut();
            StartCoroutine(TelepReturn());
        }
    }
    
    IEnumerator TelepHome()
    {
        yield return new WaitForSeconds(2f);
        portalView.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.4f, player.transform.position.z);
        portalView.transform.forward = player.transform.forward;

        prev_position = player.transform.position;
        player.SetActive(false);
        teled.Play();
        player.transform.position = Vector3.zero;
        player.transform.LookAt(transform);
        player.SetActive(true);

        cameraFader.FadeIn();
    }

    IEnumerator TelepReturn()
    {
        yield return new WaitForSeconds(2f);

        player.SetActive(false);
        player.transform.position = prev_position;
        player.transform.forward = portalView.transform.forward;
        teled.Play();
        player.SetActive(true);

        cameraFader.FadeIn();
    }
}
