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

    void Start()
    {
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
        if (scaleModifyCubeTr == null) // ù��°�� 
            return;

        //// ������, 
        //// ������,
        //currentCubeTr  �̹��� ������� ť�� -> �̰Ŷ� ���ϸ� �ȵ�.
        // lastCubeTr �� �̰����� ������� ť��� ���ؾ���.
        float absOffsetX = Mathf.Abs(scaleModifyCubeTr.localPosition.x - topCubeTr.localPosition.x);
        float absOffsetZ = Mathf.Abs(scaleModifyCubeTr.localPosition.z - topCubeTr.localPosition.z);

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
            , scaleModifyCubeTr.localScale.y
            , topCubeTr.localScale.z - absOffsetZ
            );

        // ©�� ť���� ũ��� ��ġ ���ϱ�
        GetDropCubeScaleAndPosition(newCubeScale, moveLocalX, out Vector3 dropCubeScale, out Vector3 dropCubePos);
        //print($"moveLocalX:{moveLocalX}, ũ�� ���� ť�� LocalPos :{scaleModifyCubeTr.localPosition.x}, dropCubePos:{dropCubePos.x}, newCubeScale.x:{newCubeScale.x}");
        //Debug.Break();

        // ����� ����� ť�� ����
        CrateRemainCube(newCubeScale);


        // ©�� ť�� ��������.
        CreateDropCube(dropCubeScale, dropCubePos);


        // todo : ©�� �κ��� �ּҰ����� �۴ٸ� ��ġ ���� ��Ű�� �޺���ġ �ø���.
    }

    private void OnGameOver()
    {
        scaleModifyCubeTr.GetComponent<MovingCube>().enabled = false;
        scaleModifyCubeTr.gameObject.AddComponent<Rigidbody>();
        pointText.text = "Game Over";
    }

    private void GetDropCubeScaleAndPosition(Vector3 newCubeScale, bool moveLocalX, out Vector3 dropCubeScale, out Vector3 dropCubePos)
    {
        dropCubeScale = new Vector3(
    scaleModifyCubeTr.localScale.x - newCubeScale.x
    , scaleModifyCubeTr.localScale.y
    , scaleModifyCubeTr.localScale.z - newCubeScale.z
    );

        if (dropCubeScale.x < 0.001)
            dropCubeScale.x = scaleModifyCubeTr.localScale.x;

        if (dropCubeScale.z < 0.001)
            dropCubeScale.z = scaleModifyCubeTr.localScale.z;


        bool isNegativePositionX = scaleModifyCubeTr.localPosition.x < topCubeTr.localPosition.x;
        bool isNegativePositionZ = scaleModifyCubeTr.localPosition.z < topCubeTr.localPosition.z;
        float directionX = isNegativePositionX ? 1 : -1;
        float directionZ = isNegativePositionZ ? -1 : 1;
        if (moveLocalX)
            dropCubePos = scaleModifyCubeTr.localPosition - new Vector3(newCubeScale.x, 0, 0) * 0.5f * directionX;
        else
            dropCubePos = scaleModifyCubeTr.localPosition + new Vector3(0, 0, newCubeScale.z) * 0.5f * directionZ;
    }

    private void CrateRemainCube(Vector3 newCubeScale)
    {
        Vector3 newCubePos = Vector3.Lerp(scaleModifyCubeTr.position, topCubeTr.position, 0.5f);
        newCubePos.y = scaleModifyCubeTr.position.y;

        scaleModifyCubeTr.localScale = newCubeScale;
        scaleModifyCubeTr.position = newCubePos;
        scaleModifyCubeTr.GetComponent<MovingCube>().enabled = false;
        scaleModifyCubeTr.name = "ũ�� ���� ť��";
    }

    private void CreateDropCube(Vector3 dropCubeScale, Vector3 dropCubePos)
    {
        var dropGo = Instantiate(scaleModifyCubeTr.gameObject, dropCubePos, scaleModifyCubeTr.rotation, scaleModifyCubeTr.parent);
        dropGo.transform.localScale = dropCubeScale;
        dropGo.transform.localPosition = dropCubePos;
        dropGo.name = "����� ť��";
        dropGo.AddComponent<Rigidbody>();
    }

    public float perfectMatchMaxDistance = 0.001f;
    public int comboCount;

    Transform topCubeTr;
    Transform scaleModifyCubeTr;

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
        if (scaleModifyCubeTr != null)
        {
            newCube.pivot = scaleModifyCubeTr.transform.localPosition;
            newCube.transform.localScale = scaleModifyCubeTr.transform.localScale;
        }

        newCube.gameObject.SetActive(true);
        newCube.name = level.ToString();

        // ���� �� ��������.
        Color.RGBToHSV(nextColor, out float h, out float s, out float v);
        nextColor = Color.HSVToRGB(h + 1f/256 * colorChangeStep, s, v);

        //// �׶��̼� ����Ҳ��� �Ʒ� ���� ���
        //nextColor = gradient.Evaluate( level % 100f * 0.01f);

        newCube.GetComponent<Renderer>().material.SetColor("_ColorTop", nextColor);
        newCube.GetComponent<Renderer>().material.SetColor("_ColorBottom", nextColor);
        // ī�޶� ���� �̵�����.
        Camera.main.transform.Translate(0, cubeHeight, 0, Space.World);

        topCubeTr = scaleModifyCubeTr;
        scaleModifyCubeTr = newCube.transform;
        level++;
    }
    public Gradient gradient;
}
