using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MovingCube item;
    public int level;
    float cubeHeight;
    public float distance = 2.75f;
    Color nextColor;
    public float colorChangeStep = 2f;

    void Start()
    {
        cubeHeight = item.transform.localScale.y;
        item.gameObject.SetActive(false);
        nextColor = item.GetComponent<Renderer>()
            .material.GetColor("_ColorTop");
        CreateCube();
    }
    void Update()
    {
        if(Input.anyKeyDown)
            CreateCube();
    }
    private void CreateCube()
    {
        level++;
        // Ȧ �� �϶��� ������, ¦ �� �϶��� ����
        Vector3 startPos;
        if(level % 2 == 1) // Ȧ��
        {
            startPos = new Vector3(distance, level * cubeHeight, distance);
        }
        else
        {
            startPos = new Vector3(-distance, level * cubeHeight, distance);
        }
        var newCube = Instantiate(item, startPos, item.transform.rotation);
        newCube.gameObject.SetActive(true);

        // ���� �� ��������.
        Color.RGBToHSV(nextColor, out float h, out float s, out float v);
        nextColor = Color.HSVToRGB(h + 1f/256 * colorChangeStep, s, v);

        //// �׶��̼� ����Ҳ��� �Ʒ� ���� ���
        //nextColor = gradient.Evaluate( level % 100f * 0.01f);

        newCube.GetComponent<Renderer>().material.SetColor("_ColorTop", nextColor);
        newCube.GetComponent<Renderer>().material.SetColor("_ColorBottom", nextColor);

        // ī�޶� ���� �̵�����.
        Camera.main.transform.Translate(0, cubeHeight, 0, Space.World);

    }
    public Gradient gradient;
}
