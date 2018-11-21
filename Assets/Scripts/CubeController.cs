using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour 
{
    Transform blueFace;
    void Start()
    {
        blueFace = transform.GetChild(2);
    }
    
    void Update()
    {
        blueFace.Rotate(new Vector3(0, 0, 1), 20.0f * Time.deltaTime);
        transform.Rotate(new Vector3(1, 1, 1), 30.0f*Time.deltaTime);
    }

}
