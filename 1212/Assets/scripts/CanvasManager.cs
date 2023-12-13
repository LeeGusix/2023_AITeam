using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//각 캔버스 활성화 상태를 판단합니다.
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
        else { Debug.LogWarning($"{name} :: 전역 인스턴스가 존재합니다."); }

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

    //결과값을 보여줍니다.
    public void ShowResult(string Fname, string Fcal, string Dcal, string DScal,
                        string Protein, string Carbohydrate, string Fat, string Sugar, Texture2D Fimg)
    {
        
        resultUI.Show(Fname, Fcal, Dcal, DScal, Protein, Carbohydrate, Fat, Sugar, Fimg);
    }

    //지정한 UI를 활성화합니다.
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
