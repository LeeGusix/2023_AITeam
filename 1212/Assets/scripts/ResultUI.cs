using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("���� �̹���")]
    public RawImage img_Food;
    [Header("�����̸�")]
    public TMP_Text txt_FoodName;
    [Header("Į�θ�")]
    public TMP_Text txt_Cal;
    [Header("�Ϸ� ���� Į�θ�")]
    public TMP_Text txt_DailyCal;
    [Header("���� ������ Į�θ�")]
    public TMP_Text txt_DailySumCal;

    [Header("�ܹ���")]
    public TMP_Text txt_Protein;
    [Header("ź��ȭ��")]
    public TMP_Text txt_Carbohydrate;
    [Header("����")]
    public TMP_Text txt_Fat;
    [Header("��")]
    public TMP_Text txt_Sugar;


    [Header("Ȯ�� ��ư")]
    public Button btn_Comfirm;


    void Start()
    {
        if ((txt_FoodName || txt_Cal || txt_DailyCal || txt_DailySumCal 
            || txt_Protein || txt_Carbohydrate || txt_Fat || txt_Sugar))
        {
            Debug.LogWarning($"{name} :: �ʼ� UI ��Ұ� �������� �ʽ��ϴ�.");
        }

        //PhotoUI Ȱ��
        btn_Comfirm.onClick.AddListener((() => { CanvasManager.Instance.Active(UIState.Photo); }));

        Debug.LogWarning($"{name} :: �ʱ�ȭ.");
        gameObject.SetActive(false);

    }

    //����� ������
    public void Show(string Fname, string Fcal, string Dcal, string DScal,
                        string Protein, string Carbohydrate, string Fat, string Sugar , Texture2D img)
    {
        CanvasManager.Instance.Active(UIState.Result);

        Debug.Log($"{name} :: ������� ǥ�õ˴ϴ�.");
        gameObject.SetActive (true);

        txt_FoodName.text = $"{Fname}";
        txt_Cal.text = $"{Fcal}";
        txt_DailyCal.text = $"{Dcal}";
        txt_DailySumCal.text = $"{DScal}";

        txt_Protein.text = $"{Protein}";
        txt_Carbohydrate.text = $"{Carbohydrate}";
        txt_Fat.text = $"{Fat}";
        txt_Sugar.text = $"{Sugar}";

        img_Food.texture = img;
    }
}
