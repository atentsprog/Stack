using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour
{
    internal Vector3 pivot;
    Vector3 desPoint;    // 목표지점.
    Vector3 startPoint;  // 시작 지점.
    float startTime;
    void Start()
    {
        startPoint = new Vector3(transform.position.x + pivot.x, transform.position.y, transform.position.z + pivot.z);
        desPoint = new Vector3(-startPoint.x + pivot.x, startPoint.y, -startPoint.z + pivot.x);
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
