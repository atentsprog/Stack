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
        startPoint = new Vector3(transform.localPosition.x, transform.position.y, transform.localPosition.z);
        desPoint = new Vector3(-startPoint.x, startPoint.y, -startPoint.z);
        pivot.y = 0;
        startPoint += pivot;
        desPoint += pivot;
        startTime = Time.time;
    }

    public float elapsTime;
    public float ����������1;
    public float time;
    void Update()
    {
        elapsTime = Time.time - startTime;
        ����������1 = elapsTime % 2 - 1f;
        time = Mathf.Abs(����������1);
        Vector3 pos = Vector3.Lerp(desPoint, startPoint, time);
        transform.localPosition = pos;
    }
}
