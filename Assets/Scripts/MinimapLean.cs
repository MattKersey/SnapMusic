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

        transform.localPosition = new Vector3(transform.localPosition.x, 30 + euler.x, -100 + euler.x);
    }
}
