using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour
{
    public static Chat instance;
	void Awake() => instance = this;

	public InputField SendInput;
	public RectTransform ChatContent;
	public Text ChatText;
	public ScrollRect ChatScrollRect;
	public GameObject ChatLine;
	public GameObject ChatLine_u;
	public PythonRunner thePR;

	public void ShowMessage(string data)
	{
		//ChatText.text += ChatText.text == "" ? data : "\n" + data;
		GameObject tmpObject = Instantiate(ChatLine, ChatContent);
		ChatText = tmpObject.transform.GetChild(0).GetComponent<Text>();
		//tmpObject.transform.parent = ChatContent.transform;
		tmpObject.transform.GetChild(0).GetComponent<Text>().text = ChatText.text == "" ? data : "\n" + data;
		//Fit(ChatText.GetComponent<RectTransform>());
		//Fit(ChatContent);
		Invoke("ScrollDelay", 0.03f);
		SendInput.text = "";
		thePR.TextCount += 1;
		thePR.sendTextList.Add(thePR.TextCount);
		tmpObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = thePR.TextCount.ToString();
	}

	public void ShowMessage_u(string data)
	{
		//ChatText.text += ChatText.text == "" ? data : "\n" + data;
		GameObject tmpObject = Instantiate(ChatLine_u, ChatContent);
		ChatText = tmpObject.transform.GetChild(0).GetComponent<Text>();
		//tmpObject.transform.parent = ChatContent.transform;
		tmpObject.transform.GetChild(0).GetComponent<Text>().text = ChatText.text == "" ? data : "\n" + data;
		//Fit(ChatText.GetComponent<RectTransform>());
		//Fit(ChatContent);
		Invoke("ScrollDelay", 0.03f);
		thePR.TextCount += 1;
		thePR.receiveTextList.Add(thePR.TextCount);
		tmpObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = thePR.TextCount.ToString();
	}

	void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

	void ScrollDelay() => ChatScrollRect.verticalScrollbar.value = 0;
}
