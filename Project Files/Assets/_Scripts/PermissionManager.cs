using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PermissionManager : MonoBehaviour
{
    bool gpsInit = false;
    public LocationInfo currentGPSPosition;
    int gps_connect = 0;

    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);
        
        Input.location.Start(0.5f);

        int wait = 1000;
        if (Input.location.isEnabledByUser)
        {
            while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
            {
                wait--;
            }
            if (Input.location.status != LocationServiceStatus.Failed)
            {
                gpsInit = true;
                InvokeRepeating("RetrieveGPSData", 0.0001f, 1.0f);
            }
        }
        else
        {
            Debug.Log("GPS is not available");
        }
    }
    
    public void RetrieveGPSData()
    {
        currentGPSPosition = Input.location.lastData;

        Debug.Log((currentGPSPosition.latitude).ToString());
        Debug.Log((currentGPSPosition.longitude).ToString());
    }
}
