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

    //���õ� �̹���
    Texture2D selImage;

    IONNX iONNX;
    private void Awake()
    {
        iONNX = FindAnyObjectByType<IONNX>();

        btn_CalCul.onClick.AddListener(() => { Calculate(); });
    }

    private void OnEnable()
    {
        Debug.Log($"{name} Ȱ��ȭ �� Init");

        img_foodImage.enabled = false;
        go_notUploaded.SetActive(true);
        selImage = null;
    }

    public void Calculate()
    {
        if(selImage != null)
        {
            Debug.Log($"{name} ���� �м� �մϴ�. :: ");
            iONNX.Prediction(selImage);
        }
        else
        {
            Debug.LogWarning($"{name} ���õ� �̹����� ���� :: ");
        }
    }

    public void SelectImage(Texture2D img)
    {
        Debug.Log($"{name} �̹��� ���ÿϷ� ::");

        img_foodImage.enabled = true;
        go_notUploaded.SetActive(false);
        selImage = img;
        img_foodImage.texture = selImage;
    }
}
