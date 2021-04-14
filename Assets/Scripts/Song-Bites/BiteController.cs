using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BiteController : MonoBehaviour
{
    private GameObject[] songBites;
    private int[] orderFound;
    public int numOfTotalBites;
    private int numOfFoundBites = 0;

    private float floatHeight = 2;
    private float minX = -7;
    private float stepSize = 2;

    public GameObject placeholderParent;
    public GameObject validatePodium;
    public GameObject playPodium;
    public bool validateOn = false;

    private void Start()
    {
        songBites = GameObject.FindGameObjectsWithTag("Sound Bite");
        numOfTotalBites = songBites.Length;
        orderFound = new int[songBites.Length];
        RandomizeBitIdxs();
    }

    public GameObject[] GetSoundBites()
    {
        return songBites;
    }

    // Testing Purposes
    //private void FixedUpdate()
    //{
    //    if (validateOn) { AllInOrder(); }
    //}

    private void RandomizeBitIdxs()
    {
        System.Random random = new System.Random();
        List<int> numberList = Enumerable.Range(0, numOfTotalBites).ToList();
        numberList = numberList.OrderBy(item => random.Next()).ToList<int>();
        // foreach (int i in numberList) { Debug.Log("numberList: " + i); }

        for (int i=0; i<numOfTotalBites; i++)
        {
            BiteSelf _biteSelf = songBites[i].GetComponent<BiteSelf>();
            _biteSelf.SetBiteIdx(numberList[i]);
            PlaceBiteInStage(songBites[i]); // Testing Purposes
        }
    }

    private bool FoundAllBites()
    {
        return numOfFoundBites == numOfTotalBites;
    }

    public void FoundBite(GameObject bite, int biteIdx)
    {
        orderFound[numOfFoundBites] = biteIdx;
        PlaceBiteInStage(bite);
        numOfFoundBites += 1;
        // SayHowManyFound();
        if (FoundAllBites())
        {
            // activate both podiums
            validatePodium.SetActive(true);
            playPodium.SetActive(true);

            // alternative: PlaceBitesInStage(); // will need to interate on all bites
            // Teleport the player to the center of world.
        }
    }

    private void SayHowManyFound()
    {
        string text = numOfFoundBites.ToString() + " out of " + numOfTotalBites.ToString() + " found.";
        Debug.Log(text);
    }

    private void PlaceBiteInStage(GameObject bite)
    {
        BiteSelf _biteSelf = bite.GetComponent<BiteSelf>(); // add bite to parent obj 
        //_biteSelf.StopBouncing();
        bite.transform.parent = placeholderParent.transform; // add to placeholder group
        bite.transform.position = placeholderParent.transform.position; // reset position
        bite.transform.position = new Vector3( // change position afterwards to avoid conflict
            minX + stepSize,
            bite.transform.position.y + floatHeight,
            bite.transform.position.z
            );
        _biteSelf.SetFoundPosition(bite.transform.position);
        minX += stepSize;
    }

    public void AllInOrder()
    {
        Color finalColor;
        if (CorrectBiteOrder())
        {
            finalColor = Color.green;
        }
        else
        {
            finalColor = Color.red;
        }
        for (int idx = 0; idx < numOfTotalBites; idx++)
        {
            GameObject myChild = placeholderParent.transform.GetChild(idx).gameObject;
            // both: used to be child
            myChild.GetComponent<Renderer>().material.SetColor("_Color", finalColor);
            myChild.GetComponent<Renderer>().material.SetColor("_EmissionColor", finalColor);
        }

    } 

    private bool CorrectBiteOrder()
    {
        for (int idx = 0; idx<numOfTotalBites; idx++)
        {
            GameObject child = placeholderParent.transform.GetChild(idx).gameObject;
            BiteSelf _biteSelf = child.GetComponent<BiteSelf>();
            if (idx != _biteSelf.GetBiteIdx())
            {
                return false;
            }
        }
        return true;
    }

}
