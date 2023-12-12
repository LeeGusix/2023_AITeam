using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;

using pred = YOLOv3MLNet.DataStructures;
using YOLOv3MLNet.DataStructures;
using System.Collections.Generic;
using System.Collections;

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

    [Header("For Testing")]
    public bool isTesting;

    //�ӽ� �з� �̹���
    public Texture2D image;

    //����� ������� UI �̹���
    public RawImage image_result;

    //����� ���� ������Ʈ��
    YoloV3Prediction predict = null;

    //�ӽ� ī�װ�
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
                //    Debug.Log(FindObjectOfType<Road_Calori>().Get(0, 0));
                //    Debug.Log(FindObjectOfType<Road_Calori>().Get(1, 0));
                //    Debug.Log(FindObjectOfType<Road_Calori>().Get(2, 0));
                int index = int.Parse(item.Label);
                Debug.Log("�� : " + item.Label + "  :: " + FindObjectOfType<Road_Calori>().Get(index, 0)
                    + " ��Ȯ�� : " + item.Confidence);
                //.Data[index][0];
            }
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
}


