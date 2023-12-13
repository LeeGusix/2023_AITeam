using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;

using pred = YOLOv3MLNet.DataStructures;
using YOLOv3MLNet.DataStructures;
using System.Collections.Generic;
using System.Collections;
using TMPro;

/// <summary>
/// �̹����� ������ ���� ������� ������
/// </summary>
public class IONNX : MonoBehaviour
{
    //ONNX�� ���� ����
    static int CategoriesCount = 403 + 4;

    //�ٶ���� ��
    public NNModel Model;
    private Model m_RunTimeModel; //���� �ҷ����� ����

    //��ȯ�� �̹���
    public RawImage image_result;

    //����� ���� ������Ʈ��
    YoloV3Prediction predict = null;

    //�ӽ� ī�װ�
    string[] catecories = new string[CategoriesCount];

    //���� �̸� ���� ����
    int FoodName;

    /*
    //���� ���� �ؽ�Ʈ
    public TMP_Text FoodName_Text;
    public TMP_Text kcal_Text;
    public TMP_Text Protein_Text;
    public TMP_Text Carbohydrate_Text;
    public TMP_Text fat_Text;
    public TMP_Text sugar_Text;
    */

    [Header("�׽�Ʈ�� ���� ��")]
    public bool isTesting;

    //�ӽ� �з� �̹���
    public Texture2D image;

    //Road_Calori ��ũ��Ʈ �ҷ�����
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

        //�߷� ���� ���� // GPU �۾� ����
        var worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, m_RunTimeModel);

        //#��ǲ ���ڰ��� �ؽ��� ����
        TensorShape shape = new TensorShape(1, 256, 256, 3);
        Tensor input = new Tensor(texture);//.Reshape(shape);
        Debug.Log("Shape : " + input.shape + "Length : " + input.length);
        //���� �Ѱ� �۾� ����
        worker.Execute(input);

        //��� ��� ���� ������
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

        //������� �����ɴϴ�.
        IReadOnlyList<YoloV3Result> result = null;
        result = predict.GetResults(catecories);

        // 0���� �󺧷� �߻��ϸ� 
        if (result.Count > 0)
        {
            Debug.Log("����� ��� ���� : " + result.Count);
            foreach (var item in result)
            {
                //    Debug.Log(RC.Get(0, 0));
                //    Debug.Log(RC.Get(1, 0));
                    Debug.Log(RC.Get(2, 0));
                FoodName = int.Parse(item.Label);
                Debug.Log("�� : " + item.Label + "  :: " + RC.Get(FoodName, 0)
                    + " ��Ȯ�� : " + item.Confidence);
                //FoodName = 150;
                
            }

            //���� �ùٸ��� �Ǵ�
            int index = int.Parse(result[result.Count - 1].Label);
            index = index == 0? 1 : index;

            //���� 1���� ���� ���� �����ؼ� ���
            FoodName = index;
            Food_Data_Set(img);
        }

        //����
        worker.Dispose();
        output_boxes.Dispose();
        output_classe.Dispose();
        input.Dispose();

        predict.BBoxes = null;
        predict.Classes = null;

        //�з��� ������� ���ٸ� return NULL
        //Label = 0�� ��� �з��� ���� ���� �Ͱ� ����
        //Confidence = ��Ȯ��
        return result;
    }
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = new RenderTexture(newWidth, newHeight, 0);
        Graphics.Blit(source, rt);
        Texture2D newTexture = new Texture2D(newWidth, newHeight);

        //���˺���
        newTexture.Reinitialize(newWidth, newHeight, TextureFormat.RGB24, false);
        //�ȼ��� �о�� ����
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


