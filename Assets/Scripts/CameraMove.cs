using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private float posX;
    private float posY;
    public GameObject sphere;
    private float mouseSensitiviti = 8;
    
    void Start()
    {
        
    }

    void Update()
    {
        posX -= Input.GetAxis("Mouse Y") * mouseSensitiviti;
        posY += Input.GetAxis("Mouse X") * mouseSensitiviti;

        posX = Mathf.Clamp(posX, 335, 420);

        transform.localEulerAngles = new Vector3(posX, 0, 0);
        sphere.transform.localEulerAngles = new Vector3(0, posY, 0);
    }
}
