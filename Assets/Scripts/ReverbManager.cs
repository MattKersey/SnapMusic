using System.Collections.Generic;
using UnityEngine;

public class ReverbManager : MonoBehaviour
{
    // Reverb settings
    GameObject brickReverb;
    GameObject concreteReverb;
    GameObject marbleReverb;
    GameObject woodReverb;
    GameObject metalReverb;
    GameObject player;

    // Position Threshold Dictionary
    Dictionary<string, float[]> thresholds = new Dictionary<string, float[]>();
    float[] xthresholds = { 12.5f, 17.5f, 22.5f, 27.5f };
    float[] zthresholds = { 12.5f, 17.5f, 22.5f, 27.5f };

    void Start()
    {
        // Find required reverb rooms and player
        brickReverb = transform.Find("Brick Reverb").gameObject;
        concreteReverb = transform.Find("Concrete Reverb").gameObject;
        marbleReverb = transform.Find("Marble Reverb").gameObject;
        woodReverb = transform.Find("Wood Reverb").gameObject;
        metalReverb = transform.Find("Metal Reverb").gameObject;
        player = GameObject.Find("OVRPlayerController");

        // Setup reverb threshold locations
        thresholds.Add("x", xthresholds);
        thresholds.Add("z", zthresholds);

        // Enable brick reverb on startup
        brickReverb.SetActive(true);
        concreteReverb.SetActive(false);
        marbleReverb.SetActive(false);
        woodReverb.SetActive(false);
        metalReverb.SetActive(false);
    }

    void Update()
    {
        // Get player position
        Vector3 position = player.transform.position;

        // Get largest value between X and Z coordinates
        float max = Mathf.Max(Mathf.Abs(position.x), Mathf.Abs(position.z));
        string axis = "x";
        if (max == Mathf.Abs(position.x))
        {
            axis = "x";
        }

        else if (max == Mathf.Abs(position.z))
        {
            axis = "z";
        }

        // Choose reverb based on max coordinate
        if (max > thresholds[axis][3])
        {
            woodReverb.SetActive(false);
            metalReverb.SetActive(true);
        }

        else if (max > thresholds[axis][2])
        {
            marbleReverb.SetActive(false);
            woodReverb.SetActive(true);
            metalReverb.SetActive(false);
        }

        else if (max > thresholds[axis][1])
        {
            concreteReverb.SetActive(false);
            marbleReverb.SetActive(true);
            woodReverb.SetActive(false);
        }

        else if (max > thresholds[axis][0])
        {
            brickReverb.SetActive(false);
            concreteReverb.SetActive(true);
            marbleReverb.SetActive(false);
        }

        else if (max > 0)
        {
            brickReverb.SetActive(true);
            concreteReverb.SetActive(false);
        }
    }
}
