using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPoseView : MonoBehaviour
{
    [SerializeField] Transform player;
    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 povToPlayer = gameObject.transform.position - player.transform.position;
        gameObject.transform.forward = povToPlayer;
    }
}
