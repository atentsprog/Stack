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
    public int level;
    float cubeHeight;
    public float distance = 2.75f;
    Color nextColor;
    public float colorChangeStep = 2f;
    public Renderer bgRenderer;
    Material bgMaterial;
    public float bgColorOffsetTop = 0.2f;
    public float bgColorOffsetBottom = 0.4f;


    void Start()
    {
        bgMaterial = bgRenderer.material;
        cubeHeight = item.transform.localScale.y;
        item.gameObject.SetActive(false);
        nextColor = item.GetComponent<Renderer>()
            .sharedMaterial.GetColor("_ColorTop");
        CreateNextCube();
        stackedCubeTr = item.transform;
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (isGameOver)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            DropCube();
            if (isGameOver)
            {
                return;
            }
            CreateNextCube();
        }
    }
    public bool isGameOver;
    private void DropCube()
    {
        if (movingCube == null) // 움직이는 큐브가 없다면 드랍하지 말자.
            return;

        float absOffsetX = Mathf.Abs(movingCube.localPosition.x - stackedCubeTr.localPosition.x);
        float absOffsetZ = Mathf.Abs(movingCube.localPosition.z - stackedCubeTr.localPosition.z);
       
        isGameOver = (stackedCubeTr.localScale.x < absOffsetX)
            || (stackedCubeTr.localScale.z < absOffsetZ);

        if (isGameOver)
        {
            OnGameOver();
            return;
        }

        Vector3 inCubeScale = new Vector3(
            stackedCubeTr.localScale.x - absOffsetX
            , movingCube.localScale.y
            , stackedCubeTr.localScale.z - absOffsetZ
            );

        // 짤린 큐브의 크기와 위치 구하기
        GetOutCubeScaleAndPosition(inCubeScale, out Vector3 outCubeScale, out Vector3 outCubePos);
        
        // 이전 큐브 영역안에 드랍된 큐브 생성
        CrateInCube(inCubeScale);

        // 이전 큐브 바깥에서 생긴 드랍될 큐브 생성하자.
        CreateOutCube(outCubeScale, outCubePos);


        // todo : 짤린 부분이 최소값보다 작다면 위치 스냅 시키고 콤보수치 올리자.
    }

    private void OnGameOver()
    {
        movingCube.GetComponent<MovingCube>().enabled = false;
        movingCube.gameObject.AddComponent<Rigidbody>();
        movingCube.GetComponent<Collider>().enabled = true;
        pointText.text = "Game Over";
    }

    private void GetOutCubeScaleAndPosition(Vector3 newCubeScale, out Vector3 dropCubeScale, out Vector3 dropCubePos)
    {
        dropCubeScale = new Vector3(
    movingCube.localScale.x - newCubeScale.x
    , movingCube.localScale.y
    , movingCube.localScale.z - newCubeScale.z
    );

        bool isNegativePositionX = movingCube.localPosition.x < stackedCubeTr.localPosition.x;
        bool isNegativePositionZ = movingCube.localPosition.z < stackedCubeTr.localPosition.z;
        float directionX = isNegativePositionX ? 1 : -1;
        float directionZ = isNegativePositionZ ? -1 : 1;

        bool moveLocalX = level % 2 == 0;

        if (moveLocalX)
        {
            dropCubePos = movingCube.localPosition - new Vector3(newCubeScale.x, 0, 0) * 0.5f * directionX;
            dropCubeScale.z = movingCube.localScale.z;
        }
        else
        {
            dropCubePos = movingCube.localPosition + new Vector3(0, 0, newCubeScale.z) * 0.5f * directionZ;
            dropCubeScale.x = movingCube.localScale.x;
        }
    }

    private void CrateInCube(Vector3 newCubeScale)
    {
        Vector3 newCubePos = Vector3.Lerp(movingCube.position, stackedCubeTr.position, 0.5f);
        newCubePos.y = movingCube.position.y;

        movingCube.localScale = newCubeScale;
        movingCube.position = newCubePos;
        movingCube.GetComponent<MovingCube>().enabled = false;
        movingCube.GetComponent<Collider>().enabled = true;
        movingCube.name = $"In:{level}";
    }

    private void CreateOutCube(Vector3 dropCubeScale, Vector3 dropCubePos)
    {
        var dropGo = Instantiate(movingCube.gameObject, dropCubePos, movingCube.rotation, movingCube.parent);
        dropGo.transform.localScale = dropCubeScale;
        dropGo.transform.localPosition = dropCubePos;
        dropGo.name = $"Out:{level}";
        dropGo.AddComponent<Rigidbody>();
        dropGo.GetComponent<Collider>().enabled = true;
    }

    Transform stackedCubeTr;
    Transform movingCube;
     
    private void CreateNextCube()
    {
        pointText.text = level.ToString();

        // 홀 수 일때는 오른쪽, 짝 수 일때는 왼쪽
        Vector3 startPos = new Vector3(distance, level * cubeHeight, distance);
        if (level % 2 != 0)
            startPos.x = -startPos.x;


        var newCube = Instantiate(item, startPos, item.transform.rotation);
        newCube.transform.parent = item.transform.parent;
        if (movingCube != null)
        {
            newCube.pivot = movingCube.transform.localPosition;
            newCube.transform.localScale = movingCube.transform.localScale;
        }

        newCube.gameObject.SetActive(true);
        newCube.name = level.ToString();

        // 다음 색 지정하자.
        ChangeColor(newCube);

        // 카메라 위로 이동하자.
        Camera.main.transform.Translate(0, cubeHeight, 0, Space.World);

        stackedCubeTr = movingCube;
        movingCube = newCube.transform;
        level++;
    }

    private void ChangeColor(MovingCube newCube)
    {
        Color.RGBToHSV(nextColor, out float h, out float s, out float v);
        float nextColorH = h + 1f / 256 * colorChangeStep;
        nextColor = Color.HSVToRGB(nextColorH, s, v);

        newCube.GetComponent<Renderer>().material.SetColor("_ColorTop", nextColor);
        newCube.GetComponent<Renderer>().material.SetColor("_ColorBottom", nextColor);
        float bgTopColorH = nextColorH + bgColorOffsetTop;
        if (bgTopColorH > 1)
            bgTopColorH = bgTopColorH - 1;
        float bgBottomColorH = nextColorH + bgColorOffsetBottom;
        if (bgBottomColorH > 1)
            bgBottomColorH = bgBottomColorH - 1;
        bgMaterial.SetColor("_ColorTop", Color.HSVToRGB(bgTopColorH, s, v));
        bgMaterial.SetColor("_ColorBottom", Color.HSVToRGB(bgBottomColorH, s, v));
    }
}
