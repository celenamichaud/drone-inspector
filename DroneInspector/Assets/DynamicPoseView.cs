using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPoseView : MonoBehaviour
{
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        // Required to ensure drone pose prefab can find VR player to face quad view towards
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("Main Camera");
        if (player == null && cameras != null)
        {
            player = cameras[0].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 povToPlayer = gameObject.transform.position - player.transform.position;
        gameObject.transform.forward = povToPlayer;
    }
}
