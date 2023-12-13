using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Photo : MonoBehaviour
{
    public RawImage img_foodImage;
    public GameObject go_notUploaded;

    //public Button btn_Upload;
    public Button btn_CalCul;

    //선택된 이미지
    Texture2D selImage;

    IONNX iONNX;
    private void Awake()
    {
        iONNX = FindAnyObjectByType<IONNX>();

        btn_CalCul.onClick.AddListener(() => { Calculate(); });
    }

    private void OnEnable()
    {
        Debug.Log($"{name} 활성화 및 Init");

        img_foodImage.enabled = false;
        go_notUploaded.SetActive(true);
        selImage = null;
    }

    public void Calculate()
    {
        if(selImage != null)
        {
            Debug.Log($"{name} 사진 분석 합니다. :: ");
            iONNX.Prediction(selImage);
        }
        else
        {
            Debug.LogWarning($"{name} 선택된 이미지가 없음 :: ");
        }
    }

    public void SelectImage(Texture2D img)
    {
        Debug.Log($"{name} 이미지 선택완료 ::");

        img_foodImage.enabled = true;
        go_notUploaded.SetActive(false);
        selImage = img;
        img_foodImage.texture = selImage;
    }
}
