using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour
{
	public InputField IPInput, PortInput, NickInput;
	public string clientName;

    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
	StreamWriter writer;
    StreamReader reader;

	public ClientManager theCM;
	public void ConnectToServer()
	{
		// 이미 연결되었다면 함수 무시
		if (socketReady) return;

		// 기본 호스트/ 포트번호
		string ip = IPInput.text == "" ? "127.0.0.1" : IPInput.text;
		int port = PortInput.text == "" ? 7777 : int.Parse(PortInput.text);

		// 소켓 생성
		try
		{
			socket = new TcpClient(ip, port);
			stream = socket.GetStream();
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);
			socketReady = true;
		}
		catch (Exception e) 
		{
			Chat.instance.ShowMessage($"소켓에러 : {e.Message}");
		}
	}

	void Update()
	{
		if (socketReady && stream.DataAvailable) 
		{
			string data = reader.ReadLine();
			if (data != null)
				OnIncomingData(data);
		}
	}

	void OnIncomingData(string data)
	{
		if (data == "%NAME") 
		{
			clientName = NickInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : NickInput.text;
			Send($"&NAME|{clientName}");
			return;
		}

		DateTime currentTime = DateTime.Now;
		string formattedTime = currentTime.ToString("HH:mm:ss"); // 시:분:초 형식

		if (data.Contains("$"))
		{
			if (data.Contains(":") && !data.Contains("movieticon"))
			{
				string[] parts = data.Split(new string[] { " : " }, StringSplitOptions.None);
				string extractedString = parts[1];
				Debug.Log(extractedString);
				string[] delimiter = { "***" };
				string[] result = extractedString.Split(delimiter, System.StringSplitOptions.None);
				string message = result[0];
				string videoInfo = result[1];
				string videoIndex = result[2];
				string userInfo = result[3];
				
				if (userInfo == clientName)
				{
					if (message != "^NONE^")
					{
						Chat.instance.ShowMessage(message);						
						theCM.chatLog = theCM.AddItemToArray(theCM.chatLog, "AAA%%" + message + "   %%" + formattedTime);
					}
					PythonRunner.instance.VideoPlay(videoInfo, true);
				}
				else
				{
					if (message != "^NONE^")
					{
						Chat.instance.ShowMessage_u(message);
						theCM.chatLog = theCM.AddItemToArray(theCM.chatLog, "BBB%%" + message + "   %%" + formattedTime);
					}
					PythonRunner.instance.StartCoroutine(PythonRunner.instance.VideoPlayByOtherPerson(videoInfo));
					//PythonRunner.instance.VideoPlay(videoIndex, false);
				}
				//PythonRunner.instance.OtherClientThumbnail(videoInfo);
			}
			else if (data.Contains("movieticon"))
			{
				Debug.Log("movieticon");
				// ************수정 필요***************
				//PythonRunner.instance.VideoPlaySimSwap();
				//Invoke("PlayVideoDelay", 30f);
			}
			//else PythonRunner.instance.VideoPlay(data);

		}
		else
		{
			if (data.Contains(clientName))
			{
				Chat.instance.ShowMessage(data);
				theCM.chatLog = theCM.AddItemToArray(theCM.chatLog, "AAA%%" + data + "   %%" + formattedTime);
			}
			else
			{
				Chat.instance.ShowMessage_u(data);
				theCM.chatLog = theCM.AddItemToArray(theCM.chatLog, "BBB%%" + data + "   %%" + formattedTime);
			}
			
		}
	}


	public void Send(string data)
	{
		if (!socketReady) return;
		Debug.Log(data);
		writer.WriteLine(data);
		writer.Flush();
	}

	public void OnSendButton(InputField SendInput) 
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		if (!Input.GetButtonDown("Submit")) return;
		SendInput.ActivateInputField();
#endif
		if (SendInput.text.Trim() == "") return;

		string message = SendInput.text;
		SendInput.text = "";
		Send(message);
		//Debug.Log("test");
	}

	public void SendMessageToServer(string message)
    {
		Send(message);
	}

	void OnApplicationQuit()
	{
		CloseSocket();
	}

	void CloseSocket()
	{
		if (!socketReady) return;

		writer.Close();
		reader.Close();
		socket.Close();
		socketReady = false;
	}
}
