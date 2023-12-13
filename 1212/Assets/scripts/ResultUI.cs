using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("음식 이미지")]
    public RawImage img_Food;
    [Header("음식이름")]
    public TMP_Text txt_FoodName;
    [Header("칼로리")]
    public TMP_Text txt_Cal;
    [Header("하루 섭취 칼로리")]
    public TMP_Text txt_DailyCal;
    public TMP_Text txt_DailyCal1;
    [Header("현재 섭취한 칼로리")]
    public TMP_Text txt_DailySumCal;
    public TMP_Text txt_DailySumCal1;

    [Header("단백질")]
    public TMP_Text txt_Protein;
    [Header("탄수화물")]
    public TMP_Text txt_Carbohydrate;
    [Header("지방")]
    public TMP_Text txt_Fat;
    [Header("당")]
    public TMP_Text txt_Sugar;


    [Header("확인 버튼")]
    public Button btn_Comfirm;


    void Start()
    {
        if ((txt_FoodName || txt_Cal || txt_DailyCal || txt_DailySumCal
            || txt_Protein || txt_Carbohydrate || txt_Fat || txt_Sugar))
        {
            Debug.LogWarning($"{name} :: 필수 UI 요소가 존재하지 않습니다.");
        }

        //PhotoUI 활성
        btn_Comfirm.onClick.AddListener((() => { CanvasManager.Instance.Active(UIState.Photo); }));

        Debug.LogWarning($"{name} :: 초기화.");
        gameObject.SetActive(false);

    }

    //결과를 보여줌
    public void Show(string Fname, string Fcal, string Dcal, string DScal,
                        string Protein, string Carbohydrate, string Fat, string Sugar, Texture2D img)
    {
        CanvasManager.Instance.Active(UIState.Result);

        Debug.Log($"{name} :: 결과값이 표시됩니다.");
        gameObject.SetActive(true);

        txt_FoodName.text = $"{Fname}";
        txt_Cal.text = $"{Fcal}";
        txt_DailyCal.text = $"{Dcal}";
        txt_DailyCal1.text = $"{Dcal}";
        txt_DailySumCal.text = $"{DScal}";
        txt_DailySumCal1.text = $"{DScal}";

        txt_Protein.text = $"{Protein}";
        txt_Carbohydrate.text = $"{Carbohydrate}";
        txt_Fat.text = $"{Fat}";
        txt_Sugar.text = $"{Sugar}";

        img_Food.texture = img;
    }
}