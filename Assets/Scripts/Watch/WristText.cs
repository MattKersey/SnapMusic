using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristText : MonoBehaviour
{

    private SimpleHelvetica _simpleHelvetica;

    private void Start()
    {
        _simpleHelvetica = GetComponent<SimpleHelvetica>();
    }

    public void UpdateTime(float time)
    {
        _simpleHelvetica.Text = TimeToString(time);
        _simpleHelvetica.GenerateText();
    }

    private string TimeToString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
