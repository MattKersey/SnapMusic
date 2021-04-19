using System;
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
    private float timer = 0f;
    private int numOfSwaps = 0;
    private bool gameOver = false;

    public GameObject finalBitesPositions;
    public GameObject validatePodium;
    public GameObject playPodium;
    public Material invalidMaterial;
    public Material validMaterial;
    public bool validateOn = false;
    public GameObject thePlayerObject;
    public GameObject thePlayerControllerL;
    public GameObject ThePlayerControllerR;
    public GameObject hud;
    public GameObject celebrateObj;
    public GameObject watchTextObj;
    public GameObject ghostBitesParent;
    public OVRScreenFade cameraFader;
    public TeleportSound teled;

    private GameObject directionsParent;
    private GameObject directions;
    private GameObject arrows;


    /**
    Upon start, get all the bite gameobjects (children), calculate their quantity,
    and randomize their song-bite index (i.e. give them a random sample of the song
    and set the order-index as a variable).
    **/
    private void Start()
    {
        //get the screen fader object from the main camera
        cameraFader = GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>();

        //get the audio script
        teled = transform.GetComponent<TeleportSound>();

        directionsParent = GameObject.Find("Stage Directions");
        arrows = directionsParent.transform.GetChild(1).gameObject;
        arrows.SetActive(false);
        directions = directionsParent.transform.GetChild(0).gameObject;

        songBites = GameObject.FindGameObjectsWithTag("Sound Bite");

        numOfTotalBites = songBites.Length;
        orderFound = new int[songBites.Length];
        RandomizeBitIdxs();
    }

    // Every fixed update, check if gameover and update the time
    private void FixedUpdate()
    {
        if (!gameOver)
        {
            timer += Time.deltaTime;
            watchTextObj.GetComponent<WristText>().UpdateTime(timer);
            //if (timer > 3f)
            //{       // Testing: DELETE ALL UNDERNEATH
            //    gameOver = true;
            //    Celebrate();
            //}
        }
    }

    // Adds another swap to the counter
    public void AddSwap()
    {
        if (!gameOver)
        {
            numOfSwaps += 1;
        }
    }

    // Activates the celebration-related objects
    private void Celebrate()
    {
        Debug.Log("Celebrate");
        celebrateObj.SetActive(true);
        celebrateObj.GetComponent<Celebrate>().ShowMessages(timer, numOfSwaps);
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
        //foreach (int i in numberList) { Debug.Log("numberList: " + i); }
        for (int i=0; i<numOfTotalBites; i++)
        {
            BiteSelf _biteSelf = songBites[i].GetComponent<BiteSelf>();
            _biteSelf.SetBiteIdx(numberList[i]);
            //_biteSelf.SetRandomPitch();
            //FoundBite(songBites[i], _biteSelf.GetBiteIdx()); // debug test purposes
        }
    }

    // Return true if the user has found all the sound bites
    private bool FoundAllBites()
    {
        return numOfFoundBites == numOfTotalBites;
    }

    // Places the bite on the WIM by setting layer to 'minimap'
    private void ChangeLayer(GameObject bite)
    {
        bite.layer = 12; // 9 is minimap
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
        // ChangeLayer(bite);
        numOfFoundBites += 1;
        SayHowManyFound();
        if (FoundAllBites())
        {
            validatePodium.SetActive(true);
            playPodium.SetActive(true);

            //teleport playrer to center
            cameraFader.FadeOut();
            StartCoroutine(TelepHome());

            //turn off hud and hud toggle in controller script
            hud.SetActive(false);
            thePlayerControllerL.GetComponent<CustomController>().inEditMode = true;
            ThePlayerControllerR.GetComponent<CustomController>().inEditMode = true;
            thePlayerObject.GetComponent<AdditionalControls>().UpdateState(AdditionalControls.States.EDIT);

            //turn on song bite direction arrows
            arrows.SetActive(true);
            directions.SetActive(false);
        }
    }

    // (Debug) Log how many bites have been found so far. 
    private void SayHowManyFound()
    {
        string text = numOfFoundBites.ToString() + " out of " + numOfTotalBites.ToString() + " found.";
        Debug.Log(text);
    }

    // Activates the Particle Effect
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
        bite.GetComponent<BiteAudio>().StopBite();
    }

    private int count = 0;
    private void SwitchWithGhost(Vector3 originalPosition)
    {
        Transform ghost = ghostBitesParent.transform.GetChild(count);
        ghost.position = originalPosition;
        ghost.GetChild(0).gameObject.layer = 9;
        Debug.Log("ghost: " + ghost.gameObject.name);
        count += 1;
        ghost.transform.GetChild(0).GetComponent<Collider>().enabled = false;
    }

    /**
    With the currently found bite, attach it as a child to the container of found bites (gameobject)
    and transport it to it's destined location on the stage according to how many bites have been 
    found so far.
    **/
    private void PlaceBiteInStage(GameObject bite)
    {
        BiteSelf _biteSelf = bite.GetComponent<BiteSelf>(); // add bite to parent obj
        SwitchWithGhost(bite.transform.position);
        bite.transform.parent = finalBitesPositions.transform; // add to finalBitesPositions group
        Vector3 target = new Vector3(
            minX + stepSize,
            finalBitesPositions.transform.position.y + floatHeight,
            finalBitesPositions.transform.position.z
        );
        float duration = 3f;
        ParticleEffect(bite, true);
        StartCoroutine(AnimateMove(bite, target, duration));
        // bite.transform.position = finalBitesPositions.transform.position; // reset position
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
        bool correct = false;
        if (CorrectBiteOrder())
        {
            for (int idx = 0; idx < numOfTotalBites; idx++)
            {

                GameObject myChild = finalBitesPositions.transform.GetChild(idx).gameObject;
                myChild.GetComponent<Renderer>().material = validMaterial;
            }
            correct = true;
        }
        if (correct)
        {
            gameOver = true;
            Celebrate();
        }
    } 

    /**
    Iterate on every bite within the stage and return false if any bite is out of place
    or in the wrong playback order, meaning if idx != bite/song-index or pitch != 1.
    Otherwise return true. 
    **/
    private bool CorrectBiteOrder()
    {
        bool correctOrder = true;

        for (int idx = 0; idx<numOfTotalBites; idx++)
        {
            GameObject child = finalBitesPositions.transform.GetChild(idx).gameObject;
            BiteSelf _biteSelf = child.GetComponent<BiteSelf>();
            if (idx != _biteSelf.GetBiteIdx() || _biteSelf.GetPlayBackOrder() != 1)
            {
                child.GetComponent<Renderer>().material = invalidMaterial;
                child.GetComponent<Renderer>().material = invalidMaterial;
                correctOrder = false;
            }
        }
        return correctOrder;
    }

    /**
    Teleports the player back to the center of the world in 1 second. 
    **/
    IEnumerator TelepHome()
    {
        yield return new WaitForSeconds(1f);

        thePlayerObject.SetActive(false);
        thePlayerObject.transform.position = Vector3.zero;
        thePlayerObject.transform.rotation = Quaternion.identity;
        teled.PlaySound();
        thePlayerObject.SetActive(true);

        cameraFader.FadeIn();
    }
}
