using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainManager : MonoBehaviour
{
    public GameObject subject; // the object of interest to the drone
    public DronePose posePrefab; // to add new poses
    public InputActionReference buttonA = null; // button used to add a pose
    [SerializeField] Transform player; // used to set location/direction of pose
    [SerializeField] GameObject VRRig;

    private void Awake()
    {
        buttonA.action.performed += AddPose;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AddPose(InputAction.CallbackContext context)
    {
        // Get current position, forward, name, and use default action
        DronePose dp = Instantiate(posePrefab);
        dp.transform.position = player.transform.position;// + player.transform.forward; // spawn in front of user
        dp.transform.forward = player.transform.up; // direction
        dp.transform.parent = subject.transform; // child of subject
        dp.name = "DronePose " + GetPoses().Count;
        dp.actionType = "Photo"; // default action = photo; todo: add UI to choose action
        // Move VR player to location behind drone when drone is added
        Vector3 newPosition = player.transform.forward.normalized;
        float newX = VRRig.transform.position.x - newPosition.x;
        float newY = VRRig.transform.position.y - newPosition.y;
        float newZ = VRRig.transform.position.z - newPosition.z;
        
        VRRig.transform.position = new Vector3(newX, newY, newZ);
    }


    public List<DronePose> GetPoses()
    {
        List<DronePose> ps = new List<DronePose>(FindObjectsOfType<DronePose>());
        return ps;
    }

    public void ClearPoses()
    {
        foreach (DronePose dp in GetPoses())
        {
            Destroy(dp.gameObject);
        }
    }
}
