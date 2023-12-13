using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//�� ĵ���� Ȱ��ȭ ���¸� �Ǵ��մϴ�.
public enum UIState
{
    Photo,
    Result,
    Exit
}

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public GameObject PhotoCanvas;
    public GameObject ResultCanvas;
    public GameObject ExitNotice;

    ResultUI resultUI;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Debug.LogWarning($"{name} :: ���� �ν��Ͻ��� �����մϴ�."); }

        resultUI = FindObjectOfType<ResultUI>();
    }
    void Start()
    {
        PhotoCanvas.gameObject.SetActive(true);
        //ResultCanvas.gameObject.SetActive(false);
        ExitNotice.gameObject.SetActive(false);
    }

    public void OnNextButtonClicked()
    {
        if (PhotoCanvas.activeSelf == true)
        {
            PhotoCanvas.SetActive(false);
            ResultCanvas.SetActive(true);
        }
        else if (ResultCanvas.activeSelf == true)
        {
            PhotoCanvas.SetActive(false);
            ExitNotice.SetActive(true);
        }
    }

    //������� �����ݴϴ�.
    public void ShowResult(string Fname, string Fcal, string Dcal, string DScal,
                        string Protein, string Carbohydrate, string Fat, string Sugar, Texture2D Fimg)
    {
        
        resultUI.Show(Fname, Fcal, Dcal, DScal, Protein, Carbohydrate, Fat, Sugar, Fimg);
    }

    //������ UI�� Ȱ��ȭ�մϴ�.
    public void Active(UIState state)
    {
        switch (state)
        {
            case UIState.Photo:
                PhotoCanvas.SetActive(true);
                ExitNotice.SetActive(false);
                ResultCanvas.SetActive(false);
                break;
            case UIState.Result:
                PhotoCanvas.SetActive(false);
                ExitNotice.SetActive(false);
                ResultCanvas.SetActive(true);
                break;
            case UIState.Exit:
                PhotoCanvas.SetActive(true);
                ExitNotice.SetActive(true);
                ResultCanvas.SetActive(false);
                break;
        }
    }
}
