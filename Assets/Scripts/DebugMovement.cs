using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Vector3 rotation = transform.localEulerAngles;
                rotation.y += -1.0f;
                transform.localEulerAngles = rotation;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Vector3 rotation = transform.localEulerAngles;
                rotation.y += 1.0f;
                transform.localEulerAngles = rotation;
            }
        }

        Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement = new Vector3(0.0f, 0.0f, 2.0f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement = new Vector3(0.0f, 0.0f, -2.0f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement = new Vector3(-2.0f * Time.deltaTime, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement = new Vector3(2.0f * Time.deltaTime, 0.0f, 0.0f);
        }


        transform.Translate(movement);
    }
}
