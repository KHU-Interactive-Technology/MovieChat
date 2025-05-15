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

    // 문자열 배열에 항목 추가하는 함수
    public string[] AddItemToArray(string[] originalArray, string newItem)
    {
        // 새로운 크기의 배열 생성 (기존 배열 + 1)
        string[] newArray = new string[originalArray.Length + 1];

        // 기존 배열 복사
        for (int i = 0; i < originalArray.Length; i++)
        {
            newArray[i] = originalArray[i];
        }

        // 새로운 항목 추가
        newArray[newArray.Length - 1] = newItem;

        return newArray;
    }

    public void SaveCSVFile()
    {
        // "%%" 구분자를 기준으로 각 채팅 로그 항목 분리
        List<string[]> splitChatLog = new List<string[]>();
        foreach (string entry in chatLog)
        {
            string[] parts = entry.Split(new string[] { "%%" }, StringSplitOptions.None);
            splitChatLog.Add(parts);
        }

        // CSV 출력을 위해 나눠진 채팅 로그를 행을 기준으로 저장
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

        // CSV 내용을 생성합니다.
        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }

        // 파일 경로를 설정합니다.
        if (thePR.forBuild == false) filepath = "C://Users//user//Desktop//";

        // 디렉터리가 존재하지 않으면 생성합니다.
        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        // 타임스탬프와 함께 파일 이름을 생성합니다.
        string timestamp = DateTime.Now.ToString("MMdd_HH_mm");
        string newFileName = fileName + "_" + timestamp + ".csv";

        // CSV 파일에 쓰기 작업을 수행합니다.
        StreamWriter outStream = new StreamWriter(filepath + newFileName, false, Encoding.UTF8);
        outStream.Write(sb);
        outStream.Close();

        // 시스템 팝업 메시지를 표시합니다.
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
                    // 각 라인을 읽어가며 인덱스 증가
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
