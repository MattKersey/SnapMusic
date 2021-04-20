using UnityEngine;

public class MinimapLean : MonoBehaviour
{
    //get main camera transform
    public Transform player;

    void Start()
    {
    }

    //follow the player's x axis rotation and move transform into view when leaning down
    void Update()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x = player.eulerAngles.x;
        transform.rotation = Quaternion.Euler(euler);

        if (euler.x < 90)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 60 + euler.x * .5f, -200 + euler.x * 2.5f);
        }
        else if (euler.x > 280)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 60 + (euler.x-360) * .5f, -200 + (euler.x-360) * 2.5f);
        }

        Debug.Log(transform.localPosition);
    }
}
