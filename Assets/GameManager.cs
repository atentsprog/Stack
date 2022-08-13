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
        CreateCube();
        topCubeTr = item.transform;
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
        if (lastInClub == null) // ù��°�� 
            return;

        //// ������, 
        //// ������,
        //currentCubeTr  �̹��� ������� ť�� -> �̰Ŷ� ���ϸ� �ȵ�.
        // lastCubeTr �� �̰����� ������� ť��� ���ؾ���.
        float absOffsetX = Mathf.Abs(lastInClub.localPosition.x - topCubeTr.localPosition.x);
        float absOffsetZ = Mathf.Abs(lastInClub.localPosition.z - topCubeTr.localPosition.z);

        isGameOver = (topCubeTr.localScale.x < absOffsetX)
            || (topCubeTr.localScale.z < absOffsetZ);

        if (isGameOver)
        {
            OnGameOver();
            return;
        }

        bool moveLocalX = absOffsetX > 0.0001f;

        Vector3 newCubeScale = new Vector3(
            topCubeTr.localScale.x - absOffsetX
            , lastInClub.localScale.y
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
        lastInClub.GetComponent<MovingCube>().enabled = false;
        lastInClub.gameObject.AddComponent<Rigidbody>();
        pointText.text = "Game Over";
    }

    private void GetDropCubeScaleAndPosition(Vector3 newCubeScale, bool moveLocalX, out Vector3 dropCubeScale, out Vector3 dropCubePos)
    {
        dropCubeScale = new Vector3(
    lastInClub.localScale.x - newCubeScale.x
    , lastInClub.localScale.y
    , lastInClub.localScale.z - newCubeScale.z
    );

        if (dropCubeScale.x < 0.001)
            dropCubeScale.x = lastInClub.localScale.x;

        if (dropCubeScale.z < 0.001)
            dropCubeScale.z = lastInClub.localScale.z;


        bool isNegativePositionX = lastInClub.localPosition.x < topCubeTr.localPosition.x;
        bool isNegativePositionZ = lastInClub.localPosition.z < topCubeTr.localPosition.z;
        float directionX = isNegativePositionX ? 1 : -1;
        float directionZ = isNegativePositionZ ? -1 : 1;
        if (moveLocalX)
            dropCubePos = lastInClub.localPosition - new Vector3(newCubeScale.x, 0, 0) * 0.5f * directionX;
        else
            dropCubePos = lastInClub.localPosition + new Vector3(0, 0, newCubeScale.z) * 0.5f * directionZ;
    }

    private void CrateInCube(Vector3 newCubeScale)
    {
        Vector3 newCubePos = Vector3.Lerp(lastInClub.position, topCubeTr.position, 0.5f);
        newCubePos.y = lastInClub.position.y;

        lastInClub.localScale = newCubeScale;
        lastInClub.position = newCubePos;
        lastInClub.GetComponent<MovingCube>().enabled = false;
        lastInClub.name = $"In:{level}";
    }

    private void CreateOutCube(Vector3 dropCubeScale, Vector3 dropCubePos)
    {
        var dropGo = Instantiate(lastInClub.gameObject, dropCubePos, lastInClub.rotation, lastInClub.parent);
        dropGo.transform.localScale = dropCubeScale;
        dropGo.transform.localPosition = dropCubePos;
        dropGo.name = $"Out:{level}";
        dropGo.AddComponent<Rigidbody>();
    }

    public float perfectMatchMaxDistance = 0.001f;
    public int comboCount;

    Transform topCubeTr;
    Transform lastInClub;

    private void CreateCube()
    {
        pointText.text = level.ToString();

        // Ȧ �� �϶��� ������, ¦ �� �϶��� ����
        Vector3 startPos;
        if(level % 2 == 0) // Ȧ��
        {
            startPos = new Vector3(distance, level * cubeHeight, distance);
        }
        else
        {
            startPos = new Vector3(-distance, level * cubeHeight, distance);
        }

        var newCube = Instantiate(item, startPos, item.transform.rotation);
        newCube.transform.parent = item.transform.parent;
        if (lastInClub != null)
        {
            newCube.pivot = lastInClub.transform.localPosition;
            newCube.transform.localScale = lastInClub.transform.localScale;
        }

        newCube.gameObject.SetActive(true);
        newCube.name = level.ToString();

        // ���� �� ��������.
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
        // ī�޶� ���� �̵�����.
        Camera.main.transform.Translate(0, cubeHeight, 0, Space.World);

        topCubeTr = lastInClub;
        lastInClub = newCube.transform;
        level++;
    }
    public Gradient gradient;
}
