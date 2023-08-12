using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoordinateFunctions
{
    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public static Vector3 LatitudeLongitudeToCartesianCoordinates(float latitude, float longitude)
    {
        latitude *= Mathf.Deg2Rad;
       float radius = 6371;
        float x = radius * Mathf.Cos(latitude) * Mathf.Cos(longitude);
 
        float y = radius * Mathf.Cos(latitude) * Mathf.Sin(longitude);
        float z = radius * Mathf.Sin(latitude);

        Vector3 Cartesian = new Vector3(x, y, z);
        return Cartesian;
    }
    public static float Remap01(this float value, float zero, float one)
    {
        return (value - zero) / (one - zero);
    }


}
