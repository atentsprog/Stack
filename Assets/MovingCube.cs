using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour
{
    public Vector3 pivot;
    public Vector3 desPoint;    // ��ǥ����.
    public Vector3 startPoint;  // ���� ����.
    float startTime;
    void Start()
    {
        startPoint = new Vector3(transform.localPosition.x + pivot.x, transform.position.y, transform.localPosition.z + pivot.z);
        desPoint = new Vector3(-startPoint.x + pivot.x, startPoint.y, -startPoint.z + pivot.z);
        startTime = Time.time;
    }
    

    void Update()
    {
        float elapsTime = Time.time - startTime;
        float time = Mathf.Abs(elapsTime % 2 - 1f);
        Vector3 pos = Vector3.Lerp(desPoint, startPoint, time);
        transform.localPosition = pos;
    }
}
