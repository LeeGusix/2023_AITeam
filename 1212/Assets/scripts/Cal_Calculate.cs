using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cal_Calculate : MonoBehaviour
{
    [SerializeField] TMP_InputField user_height, user_weight;
    [SerializeField] TextMeshProUGUI textview;

    public void Calc()
    {
        double height = double.Parse(user_height.text);
        double weight = double.Parse(user_weight.text);
        double Standard_Weight = 0;
        double Calories_Needed = 0;
        string User_staus;

        //표준 체중 계산
        if (height < 150)
        {
            Standard_Weight = height - 100;
        }
        else if(height >= 150 && height < 160)
        {
            Standard_Weight = (height - 150) / 2 + 50;
        }
        else
        {
            Standard_Weight = (double)((height - 100) * 0.9);
        }

        if (Standard_Weight < weight)
        {
            User_staus = "체중 감소";
        }
        else if (Standard_Weight > weight)
        {
            User_staus = "체중 증가";
        }
        else
        {
            User_staus = "체중 유지";
        }

        //일당 필요 칼로리 계산
        Calories_Needed = Standard_Weight * 30 - 35;

        //텍스트 출력
        textview.text = "표준 체중<color=#11f3af><b>" + Standard_Weight + "</b></color>입니다.\n<color=#11f3af><b>"
            + User_staus + "</b></color>를 위해 하루 <color=#11f3af><b>" + Calories_Needed + "</b></color>의 영량 섭취가 권장됩니다.";
    }
}