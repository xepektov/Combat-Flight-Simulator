using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonBallRoll : MonoBehaviour
{
    public Transform planeTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(-planeTransform.localEulerAngles.z-90f,90f, 0f);
    }
}
