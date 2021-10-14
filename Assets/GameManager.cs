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
    void Start()
    {
        cubeHeight = item.transform.localScale.y;
        item.gameObject.SetActive(false);
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
        // È¦ ¼ö ÀÏ¶§´Â ¿À¸¥ÂÊ, Â¦ ¼ö ÀÏ¶§´Â ¿ÞÂÊ
        Vector3 startPos;
        if(level % 2 == 1) // È¦¼ö
        {
            startPos = new Vector3(distance, level * cubeHeight, distance);
        }
        else
        {
            startPos = new Vector3(-distance, level * cubeHeight, distance);
        }
        var newCube = Instantiate(item, startPos, item.transform.rotation);
        newCube.gameObject.SetActive(true);
    }
}
