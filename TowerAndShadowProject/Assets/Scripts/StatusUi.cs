using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUi : MonoBehaviour
{
    private Camera uiCamera; //UI ī�޶� ���� ����
    private Canvas canvas; //ĵ������ ���� ����
    private RectTransform rectParent; //�θ��� rectTransform ������ ������ ����
    private RectTransform rectHp; //�ڽ��� rectTransform ������ ����

    //HideInInspector�� �ش� ���� �����, ���� ������ �ʿ䰡 ���� �� 
    public Vector3 offset = Vector3.zero; //HpBar ��ġ ������, offset�� ��� HpBar�� ��ġ �������
    public Transform enemyTr; //�� ĳ������ ��ġ
    public float enemyHeight;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>(); //�θ� �������ִ� canvas ��������, Enemy HpBar canvas��
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
        //rectHp = transform.Find("HpBar").GetComponent<RectTransform>();
    }

    //LateUpdate�� update ���� ������, ���� �������� Update���� ����Ǵ� ������ ���Ŀ� HpBar�� �����
    private void LateUpdate()
    {

        var screenPos = (Camera.main.WorldToScreenPoint ( enemyTr.position + new Vector3(0.0f, enemyHeight, 0.0f) ) ); // //������ǥ(3D)�� ��ũ����ǥ(2D)�� ����, offset�� ������Ʈ �Ӹ� ��ġ
        
        /*
        if(screenPos.z < 0.0f)
        {
            screenPos *= -1.0f;
            //x, y, (z) ����ī�޶󿡼� XY�������� �Ÿ�
        }
        ������������ �ڵ� ��� HpBar�� ���̴� ������ �־ ���� �ڵ�� �Ⱥ��̰� ������, ���� ���ͺ� �����̶� �ʿ����
        */

        var localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, null, out localPos); //��ũ����ǥ���� ĵ�������� ����� �� �ִ� ��ǥ�� ����?
        localPos *= 1.0f;
        rectHp.localPosition = localPos; //�� ��ǥ�� localPos�� ����, �ű⿡ hpbar�� ���
    }

}
