using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;

using pred = YOLOv3MLNet.DataStructures;
using YOLOv3MLNet.DataStructures;
using System.Collections.Generic;
using System.Collections;
using TMPro;

/// <summary>
/// 이미지를 넣으면 예측 결과값을 추측함
/// </summary>
public class IONNX : MonoBehaviour
{
    //ONNX의 값을 참고
    static int CategoriesCount = 403 + 4;

    //바라쿠다 모델
    public NNModel Model;
    private Model m_RunTimeModel; //모델을 불러오기 위함

    //변환한 이미지
    public RawImage image_result;

    //결과값 도출 컴포넌트ㄴ
    YoloV3Prediction predict = null;

    //임시 카테고리
    string[] catecories = new string[CategoriesCount];

    //음식 이름 저장 변수
    int FoodName;

    /*
    //음식 정보 텍스트
    public TMP_Text FoodName_Text;
    public TMP_Text kcal_Text;
    public TMP_Text Protein_Text;
    public TMP_Text Carbohydrate_Text;
    public TMP_Text fat_Text;
    public TMP_Text sugar_Text;
    */

    [Header("테스트를 위한 값")]
    public bool isTesting;

    //임시 분류 이미지
    public Texture2D image;

    //Road_Calori 스크립트 불러오기
    Road_Calori RC;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < CategoriesCount; i++)
        {
            catecories[i] = i.ToString();
        }

        //Importing
        m_RunTimeModel = ModelLoader.Load(Model);
        predict = GetComponent<pred.YoloV3Prediction>();
        RC = FindObjectOfType<Road_Calori>();
    }

    IEnumerator Start()
    {
        if (isTesting)
        {
            yield return new WaitForSeconds(2.0f);
            Prediction(image);
        }
        else
        {
            image_result.enabled = false;
        }
    }

    public IReadOnlyList<YoloV3Result> Prediction(Texture2D img)
    {
        Texture2D texture = img;
        Debug.Log(texture.format);
        texture = ResizeTexture(image, 256, 256);
        Debug.Log(texture.format);

        texture.name = "NEW RESIZE IMAGE_" + texture.name;
        image_result.texture = texture;

        //추론 엔진 생성 // GPU 작업 예약
        var worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, m_RunTimeModel);

        //#인풋 인자값인 텍스쳐 지정
        TensorShape shape = new TensorShape(1, 256, 256, 3);
        Tensor input = new Tensor(texture);//.Reshape(shape);
        Debug.Log("Shape : " + input.shape + "Length : " + input.length);
        //값을 넘겨 작업 실행
        worker.Execute(input);

        //출력 결과 값을 도출함
        string[] output = m_RunTimeModel.outputs.ToArray();

        Tensor output_classe = worker.PeekOutput(output[0]);

        Tensor output_boxes = worker.PeekOutput(output[1]);

        //boxes
        Debug.Log($"BOXES : {output_boxes.ToReadOnlyArray().Length}");

        //classe
        Debug.Log($"CLASSE : {output_classe.ToReadOnlyArray().Length}");

        //setting
        predict.BBoxes = output_boxes.ToReadOnlyArray();
        predict.Classes = output_classe.ToReadOnlyArray();
        predict.ImageWidth = texture.width;
        predict.ImageHeight = texture.height;

        Debug.Log($"BOXES.COUNT : {predict.BBoxes.Length} / Classes.COUNT : {predict.Classes.Length} / ImageWidth: {predict.ImageWidth} / ImageHeight : {predict.ImageHeight}");

        //결과값을 가져옵니다.
        IReadOnlyList<YoloV3Result> result = null;
        result = predict.GetResults(catecories);

        // 0번이 라벨로 발생하면 
        if (result.Count > 0)
        {
            Debug.Log("도출된 결과 개수 : " + result.Count);
            foreach (var item in result)
            {
                //    Debug.Log(RC.Get(0, 0));
                //    Debug.Log(RC.Get(1, 0));
                    Debug.Log(RC.Get(2, 0));
                FoodName = int.Parse(item.Label);
                Debug.Log("라벨 : " + item.Label + "  :: " + RC.Get(FoodName, 0)
                    + " 정확도 : " + item.Confidence);
                //FoodName = 150;
                
            }

            //값이 올바른지 판단
            int index = int.Parse(result[result.Count - 1].Label);
            index = index == 0? 1 : index;

            //최종 1개에 대해 값을 지정해서 출력
            FoodName = index;
            Food_Data_Set(img);
        }

        //종료
        worker.Dispose();
        output_boxes.Dispose();
        output_classe.Dispose();
        input.Dispose();

        predict.BBoxes = null;
        predict.Classes = null;

        //분류된 결과값이 없다면 return NULL
        //Label = 0일 경우 분류되 되지 않은 것과 같음
        //Confidence = 정확도
        return result;
    }
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = new RenderTexture(newWidth, newHeight, 0);
        Graphics.Blit(source, rt);
        Texture2D newTexture = new Texture2D(newWidth, newHeight);

        //포맷변경
        newTexture.Reinitialize(newWidth, newHeight, TextureFormat.RGB24, false);
        //픽셀을 읽어와 지정
        newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        newTexture.Apply();
        RenderTexture.active = null;
        rt.Release();
        return newTexture;
    }

    public void Food_Data_Set(Texture2D img)
    {
        Debug.Log(RC.Get(FoodName, 0));
        string fName = RC.Get(FoodName, 0);
        string fCal = RC.Get(FoodName, 2);
        string fProtein = RC.Get(FoodName, 6);
        string fCarbohydrate = RC.Get(FoodName, 3);
        string fFat = RC.Get(FoodName, 5);
        string fSugar = RC.Get(FoodName, 4);
        string DCal = "2000";

        float _cal = float.Parse(fCal);
        string DSCal = $"{2000.0f - _cal}";


        CanvasManager.Instance.ShowResult(
            fName,
            fCal,
            DCal,
            DSCal,
            fProtein,
            fCarbohydrate,
            fFat,
            fSugar,
            img
         );

        /*
        FoodName_Text.text = RC.Get(FoodName, 0);
        if (kcal_Text)
        {
           

            /*
            Protein_Text.text = RC.Get(FoodName, 6);

            Carbohydrate_Text.text = RC.Get(FoodName, 3);

            fat_Text.text = RC.Get(FoodName, 5);

            sugar_Text.text = RC.Get(FoodName, 4);*/
    }
}


