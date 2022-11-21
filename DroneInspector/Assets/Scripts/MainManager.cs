using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainManager : MonoBehaviour
{
    public GameObject subject; // the object of interest to the drone
    List<DronePose> poses = new List<DronePose>(); // to maintain a list of poses
    public DronePose posePrefab; // to add new poses
    public InputActionReference buttonA = null; // button used to add a pose
    [SerializeField] Transform player; // used to set location/direction of pose

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
        Vector3 playerInWorld = player.transform.localToWorldMatrix.MultiplyPoint(player.transform.position);
        dp.transform.position = playerInWorld;
        //dp.transform.forward = player.transform.forward; // direction
        dp.transform.parent = subject.transform; // child of subject
        dp.name = "DronePose " + poses.Count;
        dp.actionType = "Photo"; // default action = photo; todo: add UI to choose action
        poses.Add(dp);
        // todo: adjust spawn location of target position
    }

    public List<DronePose> GetPoses()
    {
        return poses;
    }

    public void ClearPoses()
    {
        foreach (DronePose dp in poses)
        {
            Destroy(dp.gameObject);
        }
        poses.Clear();
    }
}
