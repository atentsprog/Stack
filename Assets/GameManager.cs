using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text pointText;
    public MovingCube item;
    public Transform baseCube;
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
            .sharedMaterial.GetColor("_ColorTop");
        CreateCube();
        topCubeTr = baseCube;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGameOver)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            BreakCube();
            if (isGameOver)
            {
                return;
            }
            CreateCube();
        }
    }
    public bool isGameOver;
    private void BreakCube()
    {
        if (scaleModifyCubeTr == null) // 첫번째는 
            return;

        //// 스케일, 
        //// 포지션,
        Vector3 newCubeScale, newCubePos;
        //currentCubeTr  이번에 만들어진 큐브 -> 이거랑 비교하면 안됨.
        // lastCubeTr 와 이거전에 만들어진 큐브와 비교해야함.
        isGameOver = (topCubeTr.localScale.x < Mathf.Abs(scaleModifyCubeTr.localPosition.x - topCubeTr.localPosition.x))
            || (topCubeTr.localScale.z < Mathf.Abs(scaleModifyCubeTr.localPosition.z - topCubeTr.localPosition.z));

        if(isGameOver)
        {
            scaleModifyCubeTr.GetComponent<MovingCube>().enabled = false;
            scaleModifyCubeTr.gameObject.AddComponent<Rigidbody>();
            pointText.text = "Game Over";
            return;
        }

        newCubeScale = new Vector3(
            topCubeTr.localScale.x - Mathf.Abs(scaleModifyCubeTr.localPosition.x - topCubeTr.localPosition.x)
            , scaleModifyCubeTr.localScale.y
            , topCubeTr.localScale.z - Mathf.Abs(scaleModifyCubeTr.localPosition.z - topCubeTr.localPosition.z)
            );

        newCubePos = Vector3.Lerp(scaleModifyCubeTr.position, topCubeTr.position, 0.5f) + Vector3.up * cubeHeight * 0.5f;

        scaleModifyCubeTr.localScale = newCubeScale;
        scaleModifyCubeTr.position = newCubePos;
        scaleModifyCubeTr.GetComponent<MovingCube>().enabled = false;
        scaleModifyCubeTr.name = "크기 줄인 큐브";
    }

    Transform topCubeTr;
    Transform scaleModifyCubeTr;

    private void CreateCube()
    {
        pointText.text = level.ToString();

        // 홀 수 일때는 오른쪽, 짝 수 일때는 왼쪽
        Vector3 startPos;
        if(level % 2 == 1) // 홀수
        {
            startPos = new Vector3(distance, level * cubeHeight, distance);
        }
        else
        {
            startPos = new Vector3(-distance, level * cubeHeight, distance);
        }

        var newCube = Instantiate(item, startPos, item.transform.rotation);
        newCube.transform.parent = item.transform.parent;
        if (scaleModifyCubeTr != null)
            newCube.pivot = scaleModifyCubeTr.transform.localPosition;

        newCube.gameObject.SetActive(true);
        newCube.name = level.ToString();

        // 다음 색 지정하자.
        Color.RGBToHSV(nextColor, out float h, out float s, out float v);
        nextColor = Color.HSVToRGB(h + 1f/256 * colorChangeStep, s, v);

        //// 그라데이션 사용할꺼면 아래 로직 사용
        //nextColor = gradient.Evaluate( level % 100f * 0.01f);

        newCube.GetComponent<Renderer>().material.SetColor("_ColorTop", nextColor);
        newCube.GetComponent<Renderer>().material.SetColor("_ColorBottom", nextColor);
        // 카메라 위로 이동하자.
        Camera.main.transform.Translate(0, cubeHeight, 0, Space.World);

        topCubeTr = scaleModifyCubeTr;
        scaleModifyCubeTr = newCube.transform;
        level++;
    }
    public Gradient gradient;
}
