using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GPSManager : MonoBehaviour {

    bool gpsInit = false;
    public LocationInfo currentGPSPosition;
    int gps_connect = 0;
    double detailed_num = 1.0;
    public double latitude = 0;
    public double longitude = 0;

    void Start()
    {
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

        latitude = currentGPSPosition.latitude * detailed_num;
        longitude = currentGPSPosition.longitude * detailed_num;

        Debug.Log((currentGPSPosition.latitude).ToString());
        Debug.Log((currentGPSPosition.longitude).ToString());
    }
}