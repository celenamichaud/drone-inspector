using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JSONReader : MonoBehaviour
{
    // Attribution: Destined to Learn on YouTube

    public TextAsset jsonText;
    public GameObject subject;

    // public PoseList poseList = new PoseList(); // idea: the object assigned here should be the parent
    // under which all the children (poses) live, then this script will pull poses from there
    
    [System.Serializable]
    public class Pose
    {
        public Transform transform;

        // Question: can we just make a public transform here instead? is that serializable?
        // another thought: instead of making a dronePose class here could we just
        // create objects by searching through the objects in the hierarchy?
        // is that even a good idea since we don't know the number of drone poses a user would want?
        // -- maybe we can search for only prefabs (of a drone pose) as children?

        // new idea: create DronePose objects from children of GameObject subject
        // if we're doing this in one file, to create children, you parse the JSON text into a list of
        // drone poses, then create a child object (prefab?) with transform of those in some function
        
        // to create json output from children, you go through the children of subject and create
        // drone poses from them, and create a list of drone poses to be serialized (i think)
        
        // at the launch of the project, you would need the user to select whether they want to upload
        // data from a file or create data, right? or how would you address that when the program runs
        // it's immediately going to run start() and want to read a file [that is empty or doesnt exist]
        // or read a file [when the user actually doesn't want those points?]

        // another addition: make a mesh collider for the table so that if user tries to place a pose
        // in the table, the table will turn red and if the user places a pose, they will get a prompt
        // stating they cannot do so (idea for later)

    }

    [System.Serializable]
    public class PoseList
    {
        public Pose[] poses;
        // instead of making this a DronePose[], make it Transform and serialize info from transform
        // this assumes transform can be serialized
    }

    

    void Start()
    {   
        List<Pose> children = new(); // using a List because number of child poses is unknown
        foreach (Transform child in subject.transform)
            if (child.name.Contains("Pose")) {
                Debug.Log("found child " + child.name);
                Pose obj = new();
                obj.transform = child.transform;
                children.Add(obj);
            }
        PoseList poseList = new();
        poseList.poses = children.ToArray();
        string strOutput = JsonUtility.ToJson(poseList);
        File.WriteAllText(Application.dataPath + "/json-data.txt", strOutput);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
