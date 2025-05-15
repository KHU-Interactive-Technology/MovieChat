using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{

    [SerializeField] GameObject OptionWindow;
    public string[] chatLog;
    public string fileName = "ChatLog.csv";
    List<string[]> data = new List<string[]>();
    public PythonRunner thePR;
    string filepath = "chatLog//";
    public GameObject SystemPopupMessage;


    public void OptionButton()
    {
        bool buttonState = OptionWindow.activeSelf;
        if (buttonState == true)
            OptionWindow.SetActive(false);
        else OptionWindow.SetActive(true);
    }

    // ���ڿ� �迭�� �׸� �߰��ϴ� �Լ�
    public string[] AddItemToArray(string[] originalArray, string newItem)
    {
        // ���ο� ũ���� �迭 ���� (���� �迭 + 1)
        string[] newArray = new string[originalArray.Length + 1];

        // ���� �迭 ����
        for (int i = 0; i < originalArray.Length; i++)
        {
            newArray[i] = originalArray[i];
        }

        // ���ο� �׸� �߰�
        newArray[newArray.Length - 1] = newItem;

        return newArray;
    }

    public void SaveCSVFile()
    {
        // "%%" �����ڸ� �������� �� ä�� �α� �׸� �и�
        List<string[]> splitChatLog = new List<string[]>();
        foreach (string entry in chatLog)
        {
            string[] parts = entry.Split(new string[] { "%%" }, StringSplitOptions.None);
            splitChatLog.Add(parts);
        }

        // CSV ����� ���� ������ ä�� �α׸� ���� �������� ����
        int rows = splitChatLog.Count;
        int cols = 3; 

        string[][] output = new string[rows][];
        for (int i = 0; i < rows; i++)
        {
            output[i] = new string[cols];

            // Check speaker and store message content accordingly in the first or second column
            if (splitChatLog[i][0] == "AAA")
            {
                output[i][0] = splitChatLog[i][1]; // AAA speaker, store in the first column
            }
            else if (splitChatLog[i][0] == "BBB")
            {
                output[i][1] = splitChatLog[i][1]; // BBB speaker, store in the first column
            }

            output[i][2] = splitChatLog[i][2];



        }

        // CSV ������ �����մϴ�.
        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }

        // ���� ��θ� �����մϴ�.
        if (thePR.forBuild == false) filepath = "C://Users//user//Desktop//";

        // ���͸��� �������� ������ �����մϴ�.
        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        // Ÿ�ӽ������� �Բ� ���� �̸��� �����մϴ�.
        string timestamp = DateTime.Now.ToString("MMdd_HH_mm");
        string newFileName = fileName + "_" + timestamp + ".csv";

        // CSV ���Ͽ� ���� �۾��� �����մϴ�.
        StreamWriter outStream = new StreamWriter(filepath + newFileName, false, Encoding.UTF8);
        outStream.Write(sb);
        outStream.Close();

        // �ý��� �˾� �޽����� ǥ���մϴ�.
        SystemPopupMessage.SetActive(true);
        Invoke("PopupMessageClose", 2f);
    }





    private int GetLastLineIndex(string filePath)
    {
        int lastLineIndex = 0;

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    // �� ������ �о�� �ε��� ����
                    reader.ReadLine();
                    lastLineIndex++;
                }
            }
        }

        return lastLineIndex;
    }

    void PopupMessageClose()
    {
        SystemPopupMessage.SetActive(false);
    }


    private void OnApplicationQuit()
    {
        SaveCSVFile();
    }
}
