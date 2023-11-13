using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 10.0f;

    public Camera cam;

    private Vector3 normalViewPosition = new Vector3(6, 10, 6);
    private Quaternion normalViewRotation;

    private Vector3 orbitPosition = new Vector3(18, -8, 6);
    private Quaternion orbitRotation;
    private bool isOrbiting = false;

    void Start()
    {
        normalViewPosition = transform.position;
        normalViewRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOrbiting = !isOrbiting;

            if (isOrbiting)
            {
                transform.position = orbitPosition;
                transform.rotation = orbitRotation;
                cam.orthographic = false;
            }
            else
            {
                transform.position = normalViewPosition;
                transform.rotation = normalViewRotation;
                cam.orthographic = true;
            }
        }

        if (isOrbiting)
        {
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);

            transform.LookAt(target);

            orbitPosition = transform.position;
            orbitRotation = transform.rotation;
        }
    }
}
