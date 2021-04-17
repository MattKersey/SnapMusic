using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    public Transform connectedStation;
    public Transform player;
    public GameObject justTeleported;
    public GameObject mapMarker;
    private bool inPort;
    public AudioSource hum;
    public AudioSource teled;
    public GameObject cameraObscurer;

    private void Start()
    {
    }

    //Call if something runs into the teleporter
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            //Set the minimap cube to active
            mapMarker.SetActive(true);

            //check to make sure the player hasn't just teleported
            if (!justTeleported.activeSelf)
            {
                //give audio cue
                hum.enabled = true;
                inPort = true;
                StartCoroutine(startTele());
            }
        }
    }

    //reset if player walks out of station
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            hum.enabled = false;
            inPort = false;

            //make sure player is able to teleport again
            justTeleported.SetActive(false);
        }
    }

    //coroutine to stall before player teleports
    IEnumerator startTele()
    {
        StartCoroutine(FadeScreen());
        yield return new WaitForSeconds(1f);
        //make sure player is still in the portal after delay
        if (inPort)
        {
            //turn OVRcontroller off temporarily (translation won't work correctly if it is on
            player.gameObject.SetActive(false);
            //set player to position of connected station
            player.position = new Vector3(connectedStation.position.x, 0, connectedStation.position.z);
            //change player rotation to look at center of maze
            player.LookAt(Vector3.zero);
            player.gameObject.SetActive(true);

            //reset booleans
            inPort = false;
            justTeleported.SetActive(true);
            hum.enabled = false;

            //play teleported sound
            teled.Play();
        }
        StartCoroutine(FadeScreen(false));
    }

    public IEnumerator FadeScreen(bool fade = true, int fadeSpeed = 2)
    {
        Color obscurer = cameraObscurer.GetComponent<Image>().color;
        float fadeAmount;

        if(fade)
        {
            while (obscurer.a < 1)
            {
                fadeAmount = obscurer.a + (fadeSpeed * Time.deltaTime);
                obscurer = new Color(obscurer.r, obscurer.g, obscurer.g, fadeAmount);
                cameraObscurer.GetComponent<Image>().color = obscurer;
                yield return null;
            }
        }
        else
        {
            while (obscurer.a > 0)
            {
                fadeAmount = obscurer.a - (fadeSpeed * Time.deltaTime);
                obscurer = new Color(obscurer.r, obscurer.g, obscurer.g, fadeAmount);
                cameraObscurer.GetComponent<Image>().color = obscurer;
                yield return null;
            }
        }
    }
}
