using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
// using UnityEngine.Time;

public class Gunner : MonoBehaviour
{

    public GameObject bulletPrefab;
    public GameObject[] trackedObjects;

    public List<GameObject> targetList;
    public GameObject CurTarget;
    // public Transform launchPos;
    // public float launchSpeed;
    // public float nextLaunchDelay;
    public bool isLocked;
    // public bool isLoaded;
    // public int launcher_Capacity;

    float startTime;
    float elapsedTime;
    public float timeInterval;

    // float launchStartTime;
    // float launchElapsedTime;
    public float launchTimeInterval;

    public bool cannonFiring;
    // public float effectiveRange;

    [SerializeField]
    float cannonDebounceTimer;
    [SerializeField]
    float cannonFiringTimer;
    public float cannonFireRate;
    public float cannonSpread;
    public float cannonRange;
    public float cannonMaxFireAngle;
    public float cannonBurstLength;
    public float cannonBurstCooldown;
    public float bulletSpeed;
    public float cannonCooldownTimer;
    public float cannonBurstTimer;

    public Transform cannonSpawnPoint;

    // public GameObject targetloc;



    // Start is called before the first frame update
    void Start()
    {
        // isLoaded=true;
        isLocked=false;
        startTime = Time.time;
        // launchStartTime = Time.time;
        trackedObjects = trackTargets();

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;
        // launchElapsedTime = Time.time - launchStartTime;
        UpdateWeaponCooldown();
        if(elapsedTime > timeInterval){
            // trackedObjects = trackTargets();
            UpdateList();
            // UnityEngine.Debug.Log("update list");
            startTime = Time.time;
        }

        if((CurTarget!=null) && inRange(CurTarget)){
            Vector3 targetPos = CalculateCannon(Time.fixedDeltaTime);
            if(targetPos != Vector3.zero){

                // Vector3 extend = -(targetPos - CurTarget.transform.position).normalized;
                // targetPos = CurTarget.transform.position - extend * 10f;
                // targetloc.transform.position = targetPos;
                isLocked = true;
                Vector3 dir = (targetPos - cannonSpawnPoint.position).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                // transform.rotation = rot;
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            }
        }
        else{
            CurTarget = null;
            isLocked = false;
        }

        if(cannonFiring){
            FireCannon(Time.fixedDeltaTime);
        }

    }

    void UpdateWeaponCooldown() {
        float dt = Time.fixedDeltaTime;
        cannonDebounceTimer = Mathf.Max(0, cannonDebounceTimer - dt);
        cannonFiringTimer = Mathf.Max(0, cannonFiringTimer - dt);
    }

    void FireCannon(float dt) {
        if (cannonFiring && cannonFiringTimer == 0) {
            cannonFiringTimer = 60f / cannonFireRate;

            var spread = Random.insideUnitCircle * cannonSpread;

            var bulletGO = Instantiate(bulletPrefab, cannonSpawnPoint.position, cannonSpawnPoint.rotation * Quaternion.Euler(spread.x, spread.y, 0));
            var bullet = bulletGO.GetComponent<Bullet>();
            bullet.Fire(this);
        }
    }

    Vector3 CalculateCannon(float dt) {

        Vector3 targetPosition = Vector3.zero;

        if (cannonFiring) {
            cannonBurstTimer = Mathf.Max(0, cannonBurstTimer - dt);

            if (cannonBurstTimer == 0) {
                cannonFiring = false;
                cannonCooldownTimer = cannonBurstCooldown;
                // plane.SetCannonInput(false);
            }
        } else {
            cannonCooldownTimer = Mathf.Max(0, cannonCooldownTimer - dt);

            targetPosition = Utilities.FirstOrderIntercept(cannonSpawnPoint.position, Vector3.zero, bulletSpeed, CurTarget.transform.position, CurTarget.GetComponent<Rigidbody>().velocity);

            var error = targetPosition - cannonSpawnPoint.position;
            var range = error.magnitude;
            var targetDir = error.normalized;
            var targetAngle = Vector3.Angle(targetDir, cannonSpawnPoint.rotation * Vector3.forward);

            if (range < cannonRange && targetAngle < cannonMaxFireAngle && cannonCooldownTimer == 0) {
                cannonFiring = true;
                cannonBurstTimer = cannonBurstLength;
                // SetCannonInput(true);
            }
        }

        return targetPosition;
    }

    GameObject[] trackTargets(){
        GameObject[] trackedObjects = GameObject.FindGameObjectsWithTag("friendly");
        trackedObjects = trackedObjects.Concat(GameObject.FindGameObjectsWithTag("player")).ToArray();

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
            if(inRange(trackedObject)){
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
        //get random gamobject from targetList
        if(targetList.Count > 0){
            CurTarget = targetList[Random.Range(0,targetList.Count)];
        }
        else{
            CurTarget = null;
        }
        UnityEngine.Debug.Log(targetList.Count);
    }

    bool inRange(GameObject target){
        if(Vector3.Distance(target.transform.position, transform.position) < cannonRange){
            return true;
        }
        else{
            return false;
        }
    }

    void RemoveTarget(GameObject target){
        List<GameObject> newTrackedObjects = new List<GameObject>();
        for(int i = 0; i < trackedObjects.Length; i++){
            newTrackedObjects.Add(trackedObjects[i]);
        }
        newTrackedObjects.Remove(target);
        trackedObjects = newTrackedObjects.ToArray();
    }
}
