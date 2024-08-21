using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    public GameObject[] trackedObjects;

    public List<GameObject> targetList;
    public GameObject CurTarget;

    float startTime;
    float elapsedTime;
    public float timeInterval;
    public float targetTimeInterval;
    public float targetTimeIntervalSum;

    public float track_range;

    public Camera mainCamera;

    Transform targetBox;
    GameObject targetBoxGO;
    Plane plane;

    Dictionary<GameObject, GameObject> targetBoxDict; //enemy, targetBox

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        // launchStartTime = Time.time;
        
        plane = GetComponent<Plane>();

        
        // trackedObjects = trackTargets();
        // targetBoxGO = targetBox.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;
        // launchElapsedTime = Time.time - launchStartTime;
        if(elapsedTime > timeInterval){
            trackedObjects = trackTargets();
            UpdateList();
            UnityEngine.Debug.Log("update list");
            startTime = Time.time;
            targetTimeIntervalSum += elapsedTime;

            if(targetTimeIntervalSum > targetTimeInterval){
                // trackedObjects = trackTargets();
                targetTimeIntervalSum = 0;
            }
        }
    }

    

    GameObject[] trackTargets(){
        GameObject[] trackedObjects = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject trackedObject in trackedObjects){
            Target tgt = trackedObject.GetComponent<Target>();
            tgt.OnDeath += (sender, e) => {
                RemoveTarget(e.dead_guy);
            };
        }

        return trackedObjects;
    }

    void UpdateList(){

        foreach(GameObject trackedObject in targetList){
            if(!trackedObjects.Contains(trackedObject)){
                targetList.Remove(trackedObject);
            }
        }

        foreach (GameObject trackedObject in trackedObjects){
            if(isInView(trackedObject)){
                if(!targetList.Contains(trackedObject)){
                    targetList.Add(trackedObject);
                }
            }
            else{
                if(targetList.Contains(trackedObject)){
                    targetList.Remove(trackedObject);
                }
            }
        
        }

        //get get gameobject with lest angle from forward vector
        
        if(targetList.Count > 0){
            CurTarget = targetList[0];
            float minAngle = Vector3.Angle(transform.forward, CurTarget.transform.position - transform.position);
            foreach(GameObject trackedObject in targetList){
                float angle = Vector3.Angle(transform.forward, trackedObject.transform.position - transform.position);
                if(angle < minAngle){
                    CurTarget = trackedObject;
                    minAngle = angle;
                }
            }
            plane.target = CurTarget.GetComponent<Target>();
        }
        else{
            //get random object from trackedObjects
            CurTarget = trackedObjects[Random.Range(0,trackedObjects.Length)];
            plane.target = CurTarget.GetComponent<Target>();
        }

        // foreach (GameObject Object in targetList){
        //     Vector3 targetPos = TransformToHUDSpace(Object.transform.position);
        //     targetBoxGO.transform.localPosition = targetPos;
        // }
    }



    // bool isInView(GameObject trackedObject){
    //     //check if trackedObject is in view frustrum of camera
    //     Vector3 viewPos = mainCamera.WorldToViewportPoint(trackedObject.transform.position);
    //     if(viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1 && viewPos.z > 0){
    //         return true;
    //     }
    //     else{
    //         return false;
    //     }
    // }

    bool isInView(GameObject trackedObject){
        //check if trackedObject is in view frustrum of camera
        Renderer renderer = trackedObject.GetComponentInChildren<Renderer>();
        if(renderer.isVisible){
            //do raycast from camera to trackedObject and check if it hits object
            // return true;
            if(Vector3.Distance(trackedObject.transform.position, transform.position) < track_range){
                return true;
            }
        }
        return false;
    }

    void RemoveTarget(GameObject target){
        List<GameObject> newTrackedObjects = new List<GameObject>();
        for(int i = 0; i < trackedObjects.Length; i++){
            newTrackedObjects.Add(trackedObjects[i]);
        }
        newTrackedObjects.Remove(target);
        trackedObjects = newTrackedObjects.ToArray();
    }

    // void initializeTargetBox(GameObject TB){
        
    // }

    
}


