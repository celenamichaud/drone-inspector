using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayer : MonoBehaviour
{
    // Gripping Variables
    enum GRIP_STATE {OPEN, OBJECT, AIR}
    GRIP_STATE[] gripStates = new GRIP_STATE[2] {GRIP_STATE.OPEN, GRIP_STATE.OPEN};
    float[] gripValues = new float[2] { 0, 0 };
    [SerializeField] float gripThresholdActivate;
    [SerializeField] float gripThresholdDeactivate;
    Vector3[] gripLocations = new Vector3[2];
    public VRHand[] hands = new VRHand[2]; // controller prefabs; element 0 is left hand, 1 is right
    VRGrabbable[] grabbedObjects = new VRGrabbable[2] { null, null };
    Vector3[] cameraRigGripLocation = new Vector3[2];

    // Snap rotation variables
    enum SNAP_STATE {ACTIVE, WAITING}
    SNAP_STATE[] snapStates = new SNAP_STATE[] {SNAP_STATE.WAITING, SNAP_STATE.WAITING};
    Vector2[] joyValues = new Vector2[2];
    [SerializeField] float snapActivate;
    [SerializeField] float snapDeactivate;
    [SerializeField] float snapDegree;
    [SerializeField] Transform head; //the vr camera

    // Teleportation variables
    enum TELEPORT_STATE { ACTIVE, WAITING }
    TELEPORT_STATE[] teleportStates = new TELEPORT_STATE[] { TELEPORT_STATE.WAITING, TELEPORT_STATE.WAITING };
    //public Transform[] teleporterStartPoses = new Transform[2];
    //public Transform[] teleporterTargetPoses = new Transform[2];
    [SerializeField] float teleportThresholdActivate;
    [SerializeField] float teleportThresholdDeactivate;
    bool[] teleporterValid = new bool[2];
    [SerializeField] float teleporterStartSpeed;
    [SerializeField] float teleporterMaxDistance;
    //[SerializeField] GameObject teleporterArcPointPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        setGripAndJoyValues();

        Vector3[] displacements = new Vector3[2];
        for (int i = 0; i < 2; i++)
        { // one loop for each of the hands
            handleGripping(displacements, i);
            handleSnapRotation(i);
            handleTeleport(i);
        }

        // make sure avg of two stick movements is how far you are moving
        if (gripStates[0] == GRIP_STATE.AIR && gripStates[1] == GRIP_STATE.AIR)
        {
            this.transform.position = (cameraRigGripLocation[0] + displacements[0] + cameraRigGripLocation[1] + displacements[1]) / 2.0f;
        }
        else if (gripStates[0] == GRIP_STATE.AIR)
        {
            this.transform.position = cameraRigGripLocation[0] + displacements[0];
        }
        else if (gripStates[1] == GRIP_STATE.AIR)
        {
            this.transform.position = cameraRigGripLocation[1] + displacements[1];
        }

    }

    void setGripAndJoyValues()
    {
        gripValues[0] = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        gripValues[1] = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        joyValues[0] = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        joyValues[1] = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
    }

    void handleGripping(Vector3[] displacements, int i)
    {
        displacements[i] = Vector3.zero;
        if (gripStates[i] == GRIP_STATE.AIR) // if gripping air, handle movement/release
        {
            if (gripValues[i] < gripThresholdDeactivate) // end movement & release
            {
                gripStates[i] = GRIP_STATE.OPEN;
            }
            else // move the player
            {
                // addresses jittering by using local space offset from rig to get hands local pos
                Vector3 handInTracking = transform.worldToLocalMatrix.MultiplyPoint(hands[i].transform.position);
                Vector3 between = handInTracking - gripLocations[i];
                displacements[i] = transform.TransformVector(-between);
            }
        } // [end] if grip state == air
        else if (gripStates[i] == GRIP_STATE.OBJECT)
        {
            // the object is not a kinematic rigidbody,
            // so we have to use physics to make the object to follow our hand
            if (gripValues[i] < gripThresholdDeactivate)
            {
                //release it, set velocities to zero
                VRGrabbable g = grabbedObjects[i];
                Rigidbody rb = g.GetComponent<Rigidbody>();

                rb.velocity = new Vector3(0.0f, 0.0f);
                rb.angularVelocity = new Vector3(0.0f, 0.0f);

                gripStates[i] = GRIP_STATE.OPEN;
            }
            else
            {
                //cause it to move
                VRGrabbable g = grabbedObjects[i];
                Rigidbody rb = g.GetComponent<Rigidbody>();

                Vector3 between = hands[i].grabOffset.position - g.transform.position;
                Vector3 direction = between.normalized;

                rb.velocity = between / Time.deltaTime;

                // Setting the angular velocity of the object according to physics
                Quaternion betweenRot = hands[i].grabOffset.rotation * Quaternion.Inverse(g.transform.rotation);
                Vector3 axis;
                float angle;
                betweenRot.ToAngleAxis(out angle, out axis);

                rb.angularVelocity = angle * Mathf.Deg2Rad * axis / Time.deltaTime;

            }
        } // [end] if grip state == object
        else // grip state is open
        {
            if (gripValues[i] > gripThresholdActivate) // handle movement or pickup
                if (hands[i].grabbables.Count == 0) // grabbing the air to move
                {
                    gripStates[i] = GRIP_STATE.AIR;
                    Vector3 handInTracking = transform.worldToLocalMatrix.MultiplyPoint(hands[i].transform.position);

                    gripLocations[i] = handInTracking;
                    cameraRigGripLocation[i] = this.transform.position;
                }
                else // grabbing an object to carry it
                {
                    gripStates[i] = GRIP_STATE.OBJECT;
                    grabbedObjects[i] = hands[i].grabbables[0]; //just grab the first objecct
                    hands[i].grabOffset.transform.position = grabbedObjects[i].transform.position;
                    hands[i].grabOffset.transform.rotation = grabbedObjects[i].transform.rotation;
                }
        } // [end] if grip state == open
        
    } // handleGripping

    Vector3 getFootPositionWorld()
    {
        Vector3 headInWorld = head.position;
        Vector3 playCenter = transform.position;
        Vector3 feetInWorld = headInWorld;
        feetInWorld.y = playCenter.y;

        return feetInWorld;
    }

    void handleSnapRotation(int i)
    {
        if (snapStates[i] == SNAP_STATE.WAITING)
        {
            float lr = joyValues[i].x;

            if (Mathf.Abs(lr) > snapActivate)
            {
                snapStates[i] = SNAP_STATE.ACTIVE;
                float rotateAmount = lr > 0 ? snapDegree : -snapDegree;
                Vector3 currentFootPosition = getFootPositionWorld();

                transform.Rotate(0, rotateAmount, 0, Space.Self);
                doTeleport(currentFootPosition); //moves back to where we were
            }
        }
        else if (snapStates[i] == SNAP_STATE.ACTIVE)
        {
            float lr = joyValues[i].x;

            if (Mathf.Abs(lr) < snapDeactivate)
            {
                snapStates[i] = SNAP_STATE.WAITING;
            }
        }
    }
    public void doTeleport(Vector3 targetFootPosWorld)
    {
        // Debug.Log("teleported to location (" + loc.x + ", " + loc.y + ", " + loc.z + ").");
        Vector3 offset = targetFootPosWorld - getFootPositionWorld();
        transform.position = transform.position + offset;
    }

    void handleTeleport(int i)
    {
        ////destroy the teleporter arc so we get a new one each update
        //foreach (Transform t in teleporterStartPoses[i])
        //{
        //    GameObject.Destroy(t.gameObject);
        //}

        if (teleportStates[i] == TELEPORT_STATE.WAITING)
        {
            if (joyValues[i].y > teleportThresholdActivate)
            {
                teleportStates[i] = TELEPORT_STATE.ACTIVE;

            }
        }
        else if (teleportStates[i] == TELEPORT_STATE.ACTIVE)
        {
            if (joyValues[i].y < teleportThresholdDeactivate)
            {
                //do the teleport
                Vector3 teleportOffset = hands[i].transform.forward;
                teleportOffset.y = 0;
                teleportOffset.Normalize();
                this.transform.position += teleportOffset;
                teleportStates[i] = TELEPORT_STATE.WAITING;
            }
            //if (joyValues[i].y < teleportThresholdDeactivate)
            //{
            //    if (teleporterValid[i])
            //    {
            //        //do the teleport
            //        doTeleport(teleporterTargetPoses[i].position);
            //    }
            //    teleportStates[i] = TELEPORT_STATE.WAITING;
            //    teleporterTargetPoses[i].gameObject.SetActive(false);
            //}
            //else
            //{
            //    //adjust the teleporter visualization
            //    //shoot a projectile out from the start point, in the direction of the start point forward at a velocity
            //    Vector3 currentPosition = teleporterStartPoses[i].position;
            //    Vector3 currentVelocity = teleporterStartPoses[i].forward * teleporterStartSpeed;
            //    float currentDistance = 0;
            //    float deltaTime = .02f; // how many points are on the ray
            //    teleporterValid[i] = false;
            //    while (currentDistance < teleporterMaxDistance && !teleporterValid[i])
            //    { 
            //        // simulating a clock and calculating points of the arc
            //        Vector3 nextPosition = currentPosition + currentVelocity * deltaTime;
            //        Vector3 nextVelocity = currentVelocity + -9.81f * Vector3.up * deltaTime;

            //        Vector3 between = nextPosition - currentPosition;
            //        RaycastHit[] hits = Physics.RaycastAll(currentPosition, between.normalized, between.magnitude);

            //        teleporterTargetPoses[i].gameObject.SetActive(false); //deactivate every frame
            //        foreach (RaycastHit h in hits)
            //        {
            //            // determine if the landing spot of the arc is acceptable
            //            if (h.normal.y > .9f) //partially broken, will go through slanted surfaces
            //            {
            //                teleporterTargetPoses[i].position = h.point;
            //                teleporterTargetPoses[i].up = h.normal;
            //                teleporterValid[i] = true;
            //                teleporterTargetPoses[i].gameObject.SetActive(true); //deactivate every frame
            //                break;
            //            }

            //        }

            //        // create the arc point
            //        GameObject point = GameObject.Instantiate(teleporterArcPointPrefab);
            //        point.transform.parent = teleporterStartPoses[i]; // make sure the point gets deleted on next update
            //        point.transform.position = nextPosition;
            //        point.transform.forward = nextVelocity.normalized; // make sure boxes curve down in the direction of the arc
                    
            //        // update current distance, position, and velocity
            //        currentDistance += between.magnitude;
            //        currentPosition = nextPosition;
            //        currentVelocity = nextVelocity;
            //    }
            //}


        }
    } // [end] handleTeleport
}
