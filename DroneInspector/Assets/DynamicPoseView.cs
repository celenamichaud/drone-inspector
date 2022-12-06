using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPoseView : MonoBehaviour
{
    // public Transform player;
    Vector3 playerPosition;
    bool updatedPosition = false;
    GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        // Required to ensure drone pose prefab can find VR player to face quad view towards
        //GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        //Debug.Log("there are " + cameras.Length);
        //GameObject camera = GameObject.Find("Main Camera");
        //player = cameras[0].transform;
        
        //if (player == null && cameras != null)
        //{
        //    player = camera.transform;
        //    Debug.Log("Set player to " + player.position.x + " " + player.position.y + " " + player.position.z);
        //}
    }

    // Update is called once per frame
    void Update()
    {        
        if (updatedPosition == false)
        {
            GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
            camera = cameras[0];
            
            updatedPosition = true;
        }
        playerPosition = camera.transform.position;
        Vector3 povToPlayer = gameObject.transform.position - playerPosition;
        gameObject.transform.forward = povToPlayer;
    }
}
