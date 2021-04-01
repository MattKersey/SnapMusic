using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // this is needed if you use any String operations with uRTcmix


// outline of basic RTcmix C# script
// the necessary functions are in place
// various other RTcmix functions for use are commented out below

public class uRTcmixScriptTemplate : MonoBehaviour {
    int objno = 0; // set this to the RTcmix instance you are using for this script
    rtcmixmain RTcmix;
    private bool did_start = false;


    private void Awake()
    {
        // find the RTcmixmain object with the RTcmix function definitions
        RTcmix = GameObject.Find("RTcmixmain").GetComponent<rtcmixmain>();
    }


    // Use this for initialization
    void Start ()
    {
        // initialize RTcmix
        RTcmix.initRTcmix(objno);

        did_start = true;
    }


    /* utility functions used for controlling RTcmix
     * 

    RTcmix.SendScore(score, objno); // send a (string) score
    RTcmix.setpfieldRTcmix(inlet, value, objno); // set a PField to a value
    RTcmix.setscorevalsRTcmix(score, list); // replace $n variables with values from list
    RTcmix.flushScore(objno); // flush everything in the scheduling queue
    
    *
    */


	// Update is called once per frame
	void Update ()
    {
    }


    // called once for each sample-buffer. data[] contains incoming and outgoing samples
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!did_start) return;

        // compute sound samples
        RTcmix.runRTcmix(data, objno, 0); // set "0" to "1" for input processing


        /* utility functions for controlling script execution and checking values
         * uncomment what you need
         * 

        if (RTcmix.checkbangRTcmix(objno) == 1) {
            // a MAXBANG was received
        }


        float[] vals;
        int nvals = RTcmix.checkvalsRTcmix(out vals, objno);
        if (nvals > 0) // one or more values from MAXMESSAGE was received
        {
            Debug.Log(vals[0]);
            Debug.Log(vals[2]);
            // etc.
        }


        RTcmix.printRTcmix(objno); // print messages if print_on() is set in score
        
        *
        */
    }


    void OnApplicationQuit()
    {
        did_start = false;
        RTcmix = null;
    }
}
