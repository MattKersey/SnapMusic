using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BiteController : MonoBehaviour
{
    private GameObject[] songBites;
    private int numOfTotalBites;
    private int numOfFoundBites = 0;

    private void Start()
    {
        songBites = GameObject.FindGameObjectsWithTag("Bite");
        numOfTotalBites = songBites.Length;
        RandomizeBitIdxs();
    }

    private void RandomizeBitIdxs()
    {
        System.Random rand = new System.Random();
        List<int> numberList = Enumerable.Range(1, numOfTotalBites).ToList();
        numberList.OrderBy(item => rand.Next());
        for(int i=0; i<numOfTotalBites; i++)
        {
            BiteSelf _biteSelf = songBites[i].GetComponent<BiteSelf>();
            _biteSelf.SetBiteIdx(numberList[i]);
        }
    }

    private bool FoundAllBites()
    {
        return numOfFoundBites == numOfTotalBites;
    }

    public void FoundBite()
    {
        numOfFoundBites += 1;
        SayHowManyFound();
        if (FoundAllBites())
        {
            PlaceBitesInCenter();
            // Teleport the player to the center of world.
        }
    }

    private void SayHowManyFound()
    {
        string text = numOfFoundBites.ToString() + " out of " + numOfTotalBites.ToString() + " found.";
        Debug.Log(text);
    }

    private void PlaceBitesInCenter()
    {

    }

    public void ValidateBiteOrder()
    {

    }

}
