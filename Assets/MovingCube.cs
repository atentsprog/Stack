using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour
{
    Vector3 desPoint;    // 목표지점.
    Vector3 startPoint;  // 시작 지점.
    float startTime;
    void Start()
    {
        startPoint = transform.position;
        desPoint = new Vector3(-startPoint.x, startPoint.y, -startPoint.z);
        startTime = Time.time;
    }
    

    void Update()
    {
        float elapsTime = Time.time - startTime;
        float time = Mathf.Abs(elapsTime % 2 - 1f);
        Vector3 pos = Vector3.Lerp(desPoint, startPoint, time);
        transform.position = pos;
    }
}
