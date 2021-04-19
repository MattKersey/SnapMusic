using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Celebrate : MonoBehaviour
{
    public GameObject timeMessage;
    public GameObject swapsMessage;

    public void ShowMessages(float time, int num)
    {
        Debug.Log("About to add time");
        AddTimeToMessage(time);
        Debug.Log("Finished time. Adding swaps");
        AddNumOfSwapsToMessage(num);
        Debug.Log("Finished swaps. changing materials");
        ChangeMaterials();
    }

    public void AddTimeToMessage(float time)
    {
        timeMessage.GetComponent<SimpleHelvetica>().Text += TimeToString(time);
        timeMessage.GetComponent<SimpleHelvetica>().GenerateText();
    }

    private string TimeToString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddNumOfSwapsToMessage(int num)
    {
        swapsMessage.GetComponent<SimpleHelvetica>().Text += num.ToString();
        swapsMessage.GetComponent<SimpleHelvetica>().GenerateText();
    }

    private void ChangeMaterials()
    {
        for(int i=0; i < transform.childCount; i++)
        {
            GameObject message = transform.GetChild(i).gameObject;
            if (!message.CompareTag("Ignore-Me"))
            {
                Material mat = message.GetComponent<Renderer>().material;
                Debug.Log(message.gameObject.name);
                for (int j = 0; j < message.transform.childCount; j++)
                {
                    GameObject letter = message.transform.GetChild(j).gameObject;
                    if (letter.activeSelf)
                    {
                        letter.GetComponent<Renderer>().material = mat;
                    }
                }
            }
        }
    }
}
