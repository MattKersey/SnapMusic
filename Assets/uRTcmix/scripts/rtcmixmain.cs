using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Runtime.InteropServices;


// you only need this class defined once in any given unity project
// these are the RTcmix functions needed to run and control RTcmix
public static class DLL
{
	[DllImport ("librtcmix_embedded")]
	public static extern int RTcmix_init(int objno);

	[DllImport ("librtcmix_embedded")]
	public static extern int RTcmix_setparams(float sr, int nchans, int vecsize, int recording, int bus_count, int objno);

	[DllImport ("librtcmix_embedded")]
	public static extern int RTcmix_setAudioBufferFormat(int format, int nchans, int objno);

	[DllImport("librtcmix_embedded")]
	unsafe public static extern int RTcmix_runAudio(void *k, void *outAudioBuffer, int nframes, int objno);

	[DllImport ("librtcmix_embedded")]
	unsafe public static extern int RTcmix_destroy(int objno);

	[DllImport ("librtcmix_embedded")]
	unsafe public static extern int unity_parse_score(char *buf, int len, int objno);

	[DllImport ("librtcmix_embedded")]
	unsafe public static extern int  unity_checkForBang(int objno);

	[DllImport ("librtcmix_embedded")]
	unsafe public static extern int  unity_checkForVals(float *thevals, int objno);

	[DllImport ("librtcmix_embedded")]
	unsafe public static extern int unity_checkForPrint(char *pptr, int objno);

	[DllImport ("librtcmix_embedded")]
	unsafe public static extern void RTcmix_setPField(int inlet, float pval, int objno);

	[DllImport ("librtcmix_embedded")]
	unsafe public static extern int check_context();

    [DllImport("librtcmix_embedded")]
    unsafe public static extern void RTcmix_flushScore(int objno);
}

public class rtcmixmain : MonoBehaviour {

    // Use this for initialization
    void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public void initRTcmix(int objno) {
        // RTcmix
        while (DLL.check_context() == 1) System.Threading.Thread.Sleep (2);
		DLL.RTcmix_init(objno);

		// set it for single-precision float samples (see RTcmix_API.h), scaled between -1.0 and 1.0, 2 chans;
		while (DLL.check_context() == 1) System.Threading.Thread.Sleep (2);
		DLL.RTcmix_setAudioBufferFormat(8, 2, objno);

		// note the "1024" here, vs. "2048" in the Unity code
		// this is the difference between 'frames' and 'nsamples'
		while (DLL.check_context() == 1) System.Threading.Thread.Sleep (2);
		DLL.RTcmix_setparams(AudioSettings.outputSampleRate, 2, 1024, 1, 32, objno); // '32' is the nmber of RTcmix busses
    }

	public void runRTcmix(float [] data, int objno, int inputflag) {
		unsafe{
			fixed (float* b = data) {
				while (DLL.check_context() == 1) System.Threading.Thread.Sleep (2);
				if (inputflag == 0)
					DLL.RTcmix_runAudio((void *)null, (void *)b, 1024, objno);
				else
					DLL.RTcmix_runAudio((void *)b, (void *)b, 1024, objno);
			}
		}
	}

	public int checkbangRTcmix(int objno) {
		while (DLL.check_context () == 1) System.Threading.Thread.Sleep (2);
		return DLL.unity_checkForBang (objno);
	}

	public int checkvalsRTcmix(out float[] farray, int objno) {
		farray = new float[1024];
		int nvals;

		unsafe {
			fixed (float *vptr = farray) {
				while (DLL.check_context () == 1) System.Threading.Thread.Sleep (2);
				nvals = DLL.unity_checkForVals (vptr, objno);
			}
		}
		return nvals;
	}
	/*  BGG here's how you call checkvalsRTcmix()
	float[] vals;
	int nvals = RTcmix.checkvalsRTcmix (out vals, objno);
	if (nvals > 0) {
		Debug.Log (vals [0]);
		Debug.Log (vals [2]);
	}
	*/

	public void printRTcmix(int objno) {
		char[] printvals = new char[8192];
		unsafe {
			fixed (char *pptr = printvals) {
				while (DLL.check_context () == 1) System.Threading.Thread.Sleep (2);
				int pcheck = DLL.unity_checkForPrint(pptr, objno);
				while (pcheck > 0) {
					string s = new string(printvals);
					Debug.Log(s);
					Array.Clear(printvals, 0, printvals.Length);
					pcheck = DLL.unity_checkForPrint(pptr, objno);
				}
			}
		}
	}

	public void setpfieldRTcmix(int inlet, float val, int objno) {
		while (DLL.check_context () == 1) System.Threading.Thread.Sleep (2);
		DLL.RTcmix_setPField (inlet, val, objno);
	}

	// this will replace any "$n" vars used in the score with values from Unity
	public String setscorevalsRTcmix(String s, params float[] list) {
		for (int i = 0; i < list.Length; i++) {
			String rpl = "$X".Replace("X", Convert.ToString(i+1));
			s = s.Replace(rpl, Convert.ToString(list[i]));
		}
		return (s);
	}

	public void SendScore(String score, int objno) {
		IntPtr sptr = Marshal.StringToHGlobalAnsi(score);
		unsafe {
			while (DLL.check_context() == 1) System.Threading.Thread.Sleep (2);
			DLL.unity_parse_score ((char*)sptr.ToPointer (), score.Length, objno);
		}
		Marshal.FreeHGlobal(sptr);
	}

    public void flushScore(int objno)
    {
        while (DLL.check_context() == 1) System.Threading.Thread.Sleep(2);
        DLL.RTcmix_flushScore(objno);
    }

    public void destroy(int objno) {
		while (DLL.check_context() == 1) System.Threading.Thread.Sleep (2);
		DLL.RTcmix_destroy (objno); // reset RTcmix
	}
}
