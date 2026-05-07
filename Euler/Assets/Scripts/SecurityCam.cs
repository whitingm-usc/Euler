using UnityEngine;
using System.Collections.Generic;

public class SecurityCam : MonoBehaviour
{
    public List<Transform> cameras;
    public float lerpSpeed = 0.01f;
    public float rotSpeed = 360.0f;
    public float lerpValue = 0.0f;

    int currentCameraIndex = 0;
    int previousCameraIndex = 0;
    Vector3 currentPos = Vector3.zero;
    Quaternion currentRot = Quaternion.identity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPos = transform.position;
        currentRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cameras.Count > 0)
            {
                previousCameraIndex = currentCameraIndex;
                currentCameraIndex = (currentCameraIndex + 1) % cameras.Count;
            }
        }
        currentPos = Vector3.Lerp(cameras[previousCameraIndex].position, cameras[currentCameraIndex].position, lerpValue);
        currentRot = Quaternion.Slerp(cameras[previousCameraIndex].rotation, cameras[currentCameraIndex].rotation, lerpValue);
//        currentRot = Quaternion.RotateTowards(currentRot, cameras[currentCameraIndex].rotation, rotSpeed * Time.deltaTime);
        transform.position = currentPos;
        transform.rotation = currentRot;
    }
}
