using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;

using pred = YOLOv3MLNet.DataStructures;
using YOLOv3MLNet.DataStructures;
using System.Collections.Generic;
using System.Collections;

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

    [Header("For Testing")]
    public bool isTesting;

    //임시 분류 이미지
    public Texture2D image;

    //결과를 출력해줄 UI 이미지
    public RawImage image_result;

    //결과값 도출 컴포넌트ㄴ
    YoloV3Prediction predict = null;

    //임시 카테고리
    string[] catecories = new string[CategoriesCount];

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
                //    Debug.Log(FindObjectOfType<Road_Calori>().Get(0, 0));
                //    Debug.Log(FindObjectOfType<Road_Calori>().Get(1, 0));
                //    Debug.Log(FindObjectOfType<Road_Calori>().Get(2, 0));
                int index = int.Parse(item.Label);
                Debug.Log("라벨 : " + item.Label + "  :: " + FindObjectOfType<Road_Calori>().Get(index, 0)
                    + " 정확도 : " + item.Confidence);
                //.Data[index][0];
            }
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
}


