﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BiteController : MonoBehaviour
{
    private GameObject[] songBites;
    private int[] orderFound;
    private int numOfFoundBites = 0;
    private float floatHeight = 2;
    private float minX = -7;
    private float stepSize = 2;
    private int numOfTotalBites;

    public GameObject placeholderParent;
    public GameObject validatePodium;
    public GameObject playPodium;
    public bool validateOn = false;

    /**
    Upon start, get all the bite gameobjects (children), calculate their quantity,
    and randomize their song-bite index (i.e. give them a random sample of the song
    and set the order-index as a variable).
    **/
    private void Start()
    {
        songBites = GameObject.FindGameObjectsWithTag("Sound Bite");
        numOfTotalBites = songBites.Length;
        orderFound = new int[songBites.Length];
        RandomizeBitIdxs();
    }

    // Public method to get all the sound bite gameobjects
    public GameObject[] GetSoundBites()
    {
        return songBites;
    }

    /**
    Create a integer list of length equal to the number of sound bites that's filled
    with numbers from [1...num_sbites] and shuffle the order. Iterate of every sound bite
    and attach one of the shuffled integers. The shuffled integer represents the position
    of the full length song that has been split into `num_sbites` parts.
    **/
    private void RandomizeBitIdxs()
    {
        System.Random random = new System.Random();
        List<int> numberList = Enumerable.Range(0, numOfTotalBites).ToList();
        numberList = numberList.OrderBy(item => random.Next()).ToList<int>();
        foreach (int i in numberList) { Debug.Log("numberList: " + i); }
        for (int i=0; i<numOfTotalBites; i++)
        {
            BiteSelf _biteSelf = songBites[i].GetComponent<BiteSelf>();
            _biteSelf.SetBiteIdx(numberList[i]);
            // FoundBite(songBites[i], _biteSelf.GetBiteIdx()); // debug test purposes
        }
    }

    // Return true if the user has found all the sound bites
    private bool FoundAllBites()
    {
        return numOfFoundBites == numOfTotalBites;
    }

    /**
    If the user has found a bite, place it onto the stage, say how many have been 
    found so far (debugging), and if they've found all bites, then activate the two
    podium gameobjects
    **/
    public void FoundBite(GameObject bite, int biteIdx)
    {
        orderFound[numOfFoundBites] = biteIdx;
        PlaceBiteInStage(bite);
        numOfFoundBites += 1;
        SayHowManyFound();
        if (FoundAllBites())
        {
            validatePodium.SetActive(true);
            playPodium.SetActive(true);
            // TODO: Teleport the player to the center of world.
        }
    }

    // (Debug) Log how many bites have been found so far. 
    private void SayHowManyFound()
    {
        string text = numOfFoundBites.ToString() + " out of " + numOfTotalBites.ToString() + " found.";
        Debug.Log(text);
    }

    private void ParticleEffect(GameObject bite, bool status){
        bite.transform.GetChild(0).gameObject.SetActive(status);
    }


    // Lerp to the target destination from the bite's current position within duration (seconds)
    IEnumerator AnimateMove(GameObject bite, Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = bite.transform.position;
        while (time < duration)
        {
            bite.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        bite.transform.position = targetPosition;
        ParticleEffect(bite, false);
    }

    /**
    With the currently found bite, attach it as a child to the container of found bites (gameobject)
    and transport it to it's destined location on the stage according to how many bites have been 
    found so far.
    **/
    private void PlaceBiteInStage(GameObject bite)
    {
        BiteSelf _biteSelf = bite.GetComponent<BiteSelf>(); // add bite to parent obj 
        bite.transform.parent = placeholderParent.transform; // add to placeholder group
        Vector3 target = new Vector3(
            minX + stepSize,
            placeholderParent.transform.position.y + floatHeight,
            placeholderParent.transform.position.z
        );
        float duration = 3f;
        ParticleEffect(bite, true);
        StartCoroutine(AnimateMove(bite, target, duration));
        // bite.transform.position = placeholderParent.transform.position; // reset position
        // bite.transform.position = new Vector3( // change position afterwards to avoid conflict
        //     minX + stepSize,
        //     bite.transform.position.y + floatHeight,
        //     bite.transform.position.z
        //     );
        _biteSelf.SetFoundPosition(target);
        minX += stepSize;
    }

    // If all bites are in order, then turn every bite green. Otherwise, turn them red.
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

    /**
    Iterate on every bite within the stage and return false if any bite is out of place, meaning
    if idx != bite/song-index. Otherwise return true. 
    **/
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
