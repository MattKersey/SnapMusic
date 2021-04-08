using UnityEngine;

public class MinimapRotator : MonoBehaviour
{
    //get main camera transform
    public Transform player;

    private void Start()
    {
    }

    //follow the player's y axis rotation
    void Update()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = player.eulerAngles.y;
        transform.rotation = Quaternion.Euler(euler);
    }
}
