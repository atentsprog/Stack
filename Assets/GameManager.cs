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
        topCubeTr = item.transform;
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
        if (lastInCube == null) // ù��°�� 
            return;

        //// ������, 
        //// ������,
        //currentCubeTr  �̹��� ������� ť�� -> �̰Ŷ� ���ϸ� �ȵ�.
        // lastCubeTr �� �̰����� ������� ť��� ���ؾ���.
        float absOffsetX = Mathf.Abs(lastInCube.localPosition.x - topCubeTr.localPosition.x);
        float absOffsetZ = Mathf.Abs(lastInCube.localPosition.z - topCubeTr.localPosition.z);
        //absOffsetZ = absOffsetX = 0;
        isGameOver = (topCubeTr.localScale.x < absOffsetX)
            || (topCubeTr.localScale.z < absOffsetZ);

        if (isGameOver)
        {
            OnGameOver();
            return;
        }

        bool moveLocalX = level % 2 == 0;// absOffsetX > 0.0001f;

        Vector3 newCubeScale = new Vector3(
            topCubeTr.localScale.x - absOffsetX
            , lastInCube.localScale.y
            , topCubeTr.localScale.z - absOffsetZ
            );

        // ©�� ť���� ũ��� ��ġ ���ϱ�
        GetDropCubeScaleAndPosition(newCubeScale, moveLocalX, out Vector3 dropCubeScale, out Vector3 dropCubePos);
        //print($"moveLocalX:{moveLocalX}, ũ�� ���� ť�� LocalPos :{scaleModifyCubeTr.localPosition.x}, dropCubePos:{dropCubePos.x}, newCubeScale.x:{newCubeScale.x}");
        //Debug.Break();

        // ����� ����� ť�� ����
        CrateInCube(newCubeScale);


        // ©�� ť�� ��������.
        CreateOutCube(dropCubeScale, dropCubePos);


        // todo : ©�� �κ��� �ּҰ����� �۴ٸ� ��ġ ���� ��Ű�� �޺���ġ �ø���.
    }

    private void OnGameOver()
    {
        lastInCube.GetComponent<MovingCube>().enabled = false;
        lastInCube.gameObject.AddComponent<Rigidbody>();
        lastInCube.GetComponent<Collider>().enabled = true;
        pointText.text = "Game Over";
    }

    private void GetDropCubeScaleAndPosition(Vector3 newCubeScale, bool moveLocalX, out Vector3 dropCubeScale, out Vector3 dropCubePos)
    {
        dropCubeScale = new Vector3(
    lastInCube.localScale.x - newCubeScale.x
    , lastInCube.localScale.y
    , lastInCube.localScale.z - newCubeScale.z
    );

        bool isNegativePositionX = lastInCube.localPosition.x < topCubeTr.localPosition.x;
        bool isNegativePositionZ = lastInCube.localPosition.z < topCubeTr.localPosition.z;
        float directionX = isNegativePositionX ? 1 : -1;
        float directionZ = isNegativePositionZ ? -1 : 1;
        if (moveLocalX)
        {
            dropCubePos = lastInCube.localPosition - new Vector3(newCubeScale.x, 0, 0) * 0.5f * directionX;
            dropCubeScale.z = lastInCube.localScale.z;
        }
        else
        {
            dropCubePos = lastInCube.localPosition + new Vector3(0, 0, newCubeScale.z) * 0.5f * directionZ;
            dropCubeScale.x = lastInCube.localScale.x;
        }
    }

    private void CrateInCube(Vector3 newCubeScale)
    {
        Vector3 newCubePos = Vector3.Lerp(lastInCube.position, topCubeTr.position, 0.5f);
        newCubePos.y = lastInCube.position.y;

        lastInCube.localScale = newCubeScale;
        lastInCube.position = newCubePos;
        lastInCube.GetComponent<MovingCube>().enabled = false;
        lastInCube.GetComponent<Collider>().enabled = true;
        lastInCube.name = $"In:{level}";
    }

    private void CreateOutCube(Vector3 dropCubeScale, Vector3 dropCubePos)
    {
        var dropGo = Instantiate(lastInCube.gameObject, dropCubePos, lastInCube.rotation, lastInCube.parent);
        dropGo.transform.localScale = dropCubeScale;
        dropGo.transform.localPosition = dropCubePos;
        dropGo.name = $"Out:{level}";
        dropGo.AddComponent<Rigidbody>();
        dropGo.GetComponent<Collider>().enabled = true;
    }

    public float perfectMatchMaxDistance = 0.001f;
    public int comboCount;

    Transform topCubeTr;
    Transform lastInCube;

    private void CreateNextCube()
    {
        pointText.text = level.ToString();

        // Ȧ �� �϶��� ������, ¦ �� �϶��� ����
        Vector3 startPos;
        if (level % 2 == 0) // Ȧ��
        {
            startPos = new Vector3(distance, level * cubeHeight, distance);
        }
        else
        {
            startPos = new Vector3(-distance, level * cubeHeight, distance);
        }

        var newCube = Instantiate(item, startPos, item.transform.rotation);
        newCube.transform.parent = item.transform.parent;
        if (lastInCube != null)
        {
            newCube.pivot = lastInCube.transform.localPosition;
            newCube.transform.localScale = lastInCube.transform.localScale;
        }

        newCube.gameObject.SetActive(true);
        newCube.name = level.ToString();

        // ���� �� ��������.
        changeColor(newCube);

        // ī�޶� ���� �̵�����.
        Camera.main.transform.Translate(0, cubeHeight, 0, Space.World);

        topCubeTr = lastInCube;
        lastInCube = newCube.transform;
        level++;
    }

    private void changeColor(MovingCube newCube)
    {
        Color.RGBToHSV(nextColor, out float h, out float s, out float v);
        float nextColorH = h + 1f / 256 * colorChangeStep;
        nextColor = Color.HSVToRGB(nextColorH, s, v);

        //// �׶��̼� ����Ҳ��� �Ʒ� ���� ���
        //nextColor = gradient.Evaluate( level % 100f * 0.01f);

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

    public Gradient gradient;
}
