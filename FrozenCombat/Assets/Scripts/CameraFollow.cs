using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float mouseSensitivity = 100.0f;
    public float clampAngleUp = 50.0f;
    public float clampAngleDown = 30.0f;
    public float maxRotation = 45.0f;
    public float rotationSpeed = 30.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    public float zoomSpeed = 2.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    private bool isRotating = false;
    private Quaternion originalRotation;
    private float currentDistance;
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        originalRotation = transform.rotation;
        currentDistance = offset.magnitude;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            originalRotation = transform.rotation;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            rotY += mouseX * mouseSensitivity * Time.deltaTime;
            rotX += mouseY * mouseSensitivity * Time.deltaTime;
            rotX = Mathf.Clamp(rotX, -clampAngleDown, clampAngleUp);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            transform.rotation = localRotation;
        }
        else
        {
            //float horizontal = Input.GetAxis("Horizontal");
            //if (horizontal != 0)
            //{
            //    rotY += horizontal * rotationSpeed * Time.deltaTime;
            //    Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            //    transform.rotation = localRotation;
            //}
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            currentDistance = Mathf.Clamp(currentDistance - scroll * zoomSpeed, minDistance, maxDistance);
            offset = offset.normalized * currentDistance;
        }

        // Rotate the camera around the target based on the current rotation
        transform.position = target.position - (transform.rotation * Vector3.forward) * offset.magnitude;
    }
}
