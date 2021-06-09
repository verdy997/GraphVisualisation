using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private float posX;
    private float posY;
    [SerializeField] private GameObject sphere;
    [SerializeField] private GUI_EL el;
    private float mouseSensitiviti = 8;
    private bool stop = true;
    
    void Update()
    {
        if ( Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

            if (Physics.Raycast (ray, out hit, 1500.0f))
            {
                el.ShowInfo(hit.transform.gameObject);
            }
        }
        
        if (!stop)
        {
            posX -= Input.GetAxis("Mouse Y") * mouseSensitiviti;
            posY += Input.GetAxis("Mouse X") * mouseSensitiviti;


            transform.localEulerAngles = new Vector3(posX, 0, 0);
            sphere.transform.localEulerAngles = new Vector3(0, posY, 0);
        }
        if (Input.GetKey(KeyCode.C))
        {
            if (stop)
            {
                stop = false;
            }
            else
            {
                stop = true;
            }
        }
    }

}
