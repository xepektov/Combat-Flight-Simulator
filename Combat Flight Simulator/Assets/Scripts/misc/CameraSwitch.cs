using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera mainCamera;
    public Camera cockpitCamera;

    public Camera currentCamera;

    public PlaneHUD planeHUD;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera.enabled = true;
        cockpitCamera.enabled = false;
        currentCamera = mainCamera;
    }

    public void SwitchCamera()
    {
        mainCamera.enabled = !mainCamera.enabled;
        cockpitCamera.enabled = !cockpitCamera.enabled;
        if (mainCamera.enabled)
        {
            currentCamera = mainCamera;
            planeHUD.SetCamera(mainCamera,false);
        }
        else
        {
            currentCamera = cockpitCamera;
            planeHUD.SetCamera(cockpitCamera,true);
        }
    }

    public Camera getCurrentCamera()
    {
        return currentCamera;
    }

    
}
