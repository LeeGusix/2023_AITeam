using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using TreeEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class Road_Calori : MonoBehaviour
{
    const string langURL = "https://docs.google.com/spreadsheets/d/1vq5bOnVS79-hKQDKxzsCbdrCMc8oVq3R/export?format=tsv";
    string SheetData;
    public List<List<string>> Data = new List<List<string>>();

    void Start()
    {
        StartCoroutine(LoadData());
    }

    IEnumerator LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(langURL);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        SheetData = www.downloadHandler.text;

        SetRowList();
    }

    void SetRowList()
    {
        string[] rows = SheetData.Split('\n');
        for (int i = 0; i < rows.Length; i++)
        {
            string[] columns = rows[i].Split('\t');
            List<string> ColumnData = new List<string>();
            for (int j = 0; j < columns.Length; j++)
            {
                ColumnData.Add(columns[j]);
            }
            Data.Add(ColumnData);
        }
        //Debug.Log(Data[215-1][1-1]);
    }

    public string Get(int x, int y)
    {
        return Data[x][y];
    }
}
