using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Video;
using System.IO;
using System.Collections.Generic;
using TMPro;

public class PythonRunner : MonoBehaviour
{
    public string serverIP = "127.0.0.1"; // 파이썬 서버 IP 주소
    public int serverPort = 8888; // 파이썬 서버 포트
    public string receivedText; // 파이썬에서 받은 텍스트
    public Client clientScript;

    private TcpClient client;
    private NetworkStream stream;

    public Text message;
    public VideoPlayer videoPlayer;
    public VideoClip videoClip; // 동영상 파일을 할당할 변수

    public Text[] movieLines;
    public Text[] movieTitles;
    public Image[] movieThumbnail;
    public Sprite noneImg;

    public bool faceSwap;
    public Text faceSwapText;
    public static PythonRunner instance;
    public ClientManager theCM;

    public GameObject movieCon;
    public GameObject movieThumb;
    public GameObject movieThumb_u;

    public GameObject ComfirmPopupWindow;
    public GameObject PreviewWindow;
    public GameObject LoadingWindow;
    public GameObject warningMessageWindow;

    bool movieconReady = false;
    public GameObject movieconReadyWindow;

    public GameObject MovieSendButtonObject;

    public Server theServer;

    void Awake() => instance = this;


    public bool forBuild = false;
    public string path;

    bool textOnly = false;
    int MovieCount = 0;
    List<int> sendMovieList = new List<int>();
    List<int> receiveMovieList = new List<int>();
    public Text sendMovieSelect;
    public Text receiveMovieSelect;


    public GameObject onlyTextCheck;
    public GameObject withMovieCheck;
    public int TextCount = 0;
    public List<int> sendTextList = new List<int>();
    public List<int> receiveTextList = new List<int>();

    private void Start()
    {
        client = new TcpClient(serverIP, serverPort);
        stream = client.GetStream();

    }

    public static string RemoveSpecialCharacters(string inputString)
    {
        // 정규식을 사용하여 괄호 및 줄바꿈 기호를 제거
        string cleanedString = Regex.Replace(inputString, @"[\[\]\n\'\'\,]", "");
        return cleanedString;
    }

    public void MessageSendButton()
    {
        if (movieconReady == true)
        {
            movieIndexButton(tmpi);
            //clientScript.SendMessageToServer(message.text);
            movieconReady = false;
        }
        else clientScript.SendMessageToServer(message.text);
    }

    int countable;
    public void MovieSendButton()
    {
        //string message = inputField.text;
        // Unity에서 파이썬으로 텍스트 보내기
        if (message.text != "")
        {
            Debug.Log(message.text);

            for (int i = 0; i < 5; i++)
            {
                movieThumbnail[i].sprite = noneImg;
                movieLines[i].text = "";
                movieTitles[i].text = "";
            }


            //clientScript.SendMessageToServer(message.text);
            byte[] data = Encoding.UTF8.GetBytes(message.text);
            stream.Write(data, 0, data.Length);

            // 파이썬에서 받은 텍스트를 읽어 Unity 변수에 저장
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log("Received from Python: " + receivedText);
            receivedText = RemoveSpecialCharacters(receivedText);
            string[] parts = receivedText.Split('@');
            for (countable = 0; countable < 5; countable++)
            {
                if (parts[countable] == "-NONE: NONE" && countable == 0)
                {
                    warningMessageWindow.SetActive(true);
                    Invoke("CloseWarningWindow", 1.5f);
                    break;
                }
                else if (parts[countable] == " -NONE: NONE") break;
                else 
                { 
                    string[] script = parts[countable].ToString().Split(':');
                    Debug.Log(parts[countable]);
                    Debug.Log(countable);
                    movieLines[countable].text = '"' + script[1].Substring(1).ToString() + '"';
                    movieTitles[countable].text = script[0].ToString();
                }
            }
            Debug.Log(countable);
            if (countable != 0) StartCoroutine(GetThumbnail(countable));
        }
    }

    void CloseWarningWindow()
    {
        warningMessageWindow.SetActive(false);
    }

    IEnumerator GetThumbnail(int maxi)
    {
        LoadingWindow.SetActive(true);

        if (forBuild == false) path = "C://Users//user//Desktop//build//python//thumbnail.png";
        else path = "python//thumbnail.png";

        for (int i = 0; i < maxi; i++)
        {
            string movieIndex = "thumbnail" + i.ToString();
            byte[] data = Encoding.UTF8.GetBytes(movieIndex);
            stream.Write(data, 0, data.Length);

            yield return new WaitForSeconds(1f);

            // 이미지 파일을 바이트 배열로 읽기
            // 개발용 코드
            byte[] fileData = System.IO.File.ReadAllBytes(path);


            // Texture2D를 생성하고 이미지를 로드
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            // Image 컴포넌트에 Texture2D 적용
            movieThumbnail[i].sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            
        }
        LoadingWindow.SetActive(false);

    }

    public void movieIndexButton(int i)
    {
        string movieIndex = "selectMovie_" + i.ToString();

        if (faceSwap == true)
            movieIndex = movieIndex + "face_";

        byte[] data = Encoding.UTF8.GetBytes(movieIndex);
        stream.Write(data, 0, data.Length);

        // 파이썬에서 받은 텍스트를 읽어 Unity 변수에 저장
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Debug.Log(receivedText);
        if (message.text != "")
        {
            clientScript.Send(message.text + "***" + receivedText + "***" + movieIndex + "***" + clientScript.clientName);
        }
        else clientScript.Send("^NONE^" + "***" + receivedText + "***" + movieIndex + "***" + clientScript.clientName);


    }

    public void moviePreview(int i)
    {
        string movieIndex = "preview_" + i.ToString();
        byte[] data = Encoding.UTF8.GetBytes(movieIndex);
        stream.Write(data, 0, data.Length);

        // 파이썬에서 받은 텍스트를 읽어 Unity 변수에 저장
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        VideoPreviewPlay(receivedText);
    }

    int tmpi = 0;
    public void ConfirmPopupWindow(int i)
    {
        //Debug.Log("button number: " + i);
        if (i <= countable - 1)
        {
            // 여기서 영상은 미리보기로 재생되어야 함
            // 공유창에서 미리 재생 or Instantiate로 목록 옆에 새 창을 생성하고 재생
            //ComfirmPopupWindow.SetActive(true);
            movieconReady = true;
            tmpi = i;
            moviePreview(tmpi);
        }
    }
    public RenderTexture renderTexture;

    public void movieconReadyNot()
    {
        renderTexture.Release();
        movieconReady = false;
        movieconReadyWindow.SetActive(true);
        Invoke("movieconReadyWindowCloise", 1f);
    }
    public void movieconReadyWindowCloise()
    {
        movieconReadyWindow.SetActive(false);
    }

    public void YesButton()
    {
        movieIndexButton(tmpi);
        ComfirmPopupWindow.SetActive(false);
        if (message.text != "")
        {
            //clientScript.SendMessageToServer(message.text);
        }
    }
    public void NoButton()
    {
        ComfirmPopupWindow.SetActive(false);
    }

    public void OtherClientThumbnail(string info)
    {
        string videoName_; // 영상 이름
        float startTime_; // 시작 시간
        float endTime_; // 끝 시간

        string[] parts = info.Split('$');
        if (parts.Length == 3)
        {
            videoName_ = parts[0];
            startTime_ = float.Parse(parts[1]);
            endTime_ = float.Parse(parts[2]);
            string sendMessage = '&' + videoName_ + '&' + startTime_;
            byte[] data = Encoding.UTF8.GetBytes(sendMessage);
            stream.Write(data, 0, data.Length);
        }
    }

    string videoName; // 영상 이름
    float startTime; // 시작 시간
    float endTime; // 끝 시간
    int movieCounter = 0;
    public void VideoPlay(string movieinfo, bool me)
    {

        string[] parts = movieinfo.Split('$');
        if (parts.Length == 3)
        {
            videoName = parts[0];
            startTime = float.Parse(parts[1]);
            endTime = float.Parse(parts[2]);

            //videoPlayer.url = "C://Users//USERK//Desktop//project_G - 복사본//" + videoName + ".mp4";

            // 비디오 준비 (clip으로 사용시 아래 코드 모두 활성화)
            //videoPlayer.prepareCompleted += OnVideoPrepared;
            //videoPlayer.Prepare();
            //Invoke("StopVideo", endTime - startTime);

            //videoPlayer.Play();
            StartCoroutine(ShowThumbnailInChat(me));
            /*
            // 동영상 클립 이름으로 Resources 폴더에서 VideoClip을 동적으로 로드
            videoClip = Resources.Load<VideoClip>(videoName);
            
            if (videoClip != null)
            {
                // VideoClip에 동영상 파일 할당
                videoPlayer.clip = videoClip;

                // 동영상 재생 범위 설정
                videoPlayer.time = startTime;
                videoPlayer.skipOnDrop = true;

                // 동영상 재생
                videoPlayer.Play();

                Invoke("StopVideo", endTime - startTime);

                ShowThumbnailInChat();
                //SendVideoInChatWindow();
            }
            */
            //movieCounter += 1;
        }
    }

    public IEnumerator VideoPlayByOtherPerson(string movieInfo)
    {
        Debug.Log("get : " + movieInfo);
        int prevNumber = movieCounter;
        // 영화이름$시작시간$끝시간 으로 이루어진 텍스트를 파이썬에 보내 영상을 자른다.
        byte[] data = Encoding.UTF8.GetBytes("other_" + movieInfo);
        stream.Write(data, 0, data.Length);

        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        // 컴퓨터 성능에 따라 달라질 수 있음
        //yield return new WaitForSeconds(2f);
        // 파이썬으로 영상이 생성되고 아래 코드가 실행되어야 함. 순서가 맞게 동작하는지 체크해야 함
        Debug.Log("show movie" + receivedText);
        StartCoroutine(ShowThumbnailInChat(false));
        yield return null;

    }

    public void VideoPreviewPlay(string movieinfo)
    {
        string[] parts = movieinfo.Split('$');
        if (parts.Length == 3)
        {
            videoName = parts[0];
            startTime = float.Parse(parts[1]);
            endTime = float.Parse(parts[2]);

            //videoPlayer.url = "C://Users//USERK//Desktop//project_G - 복사본//" + videoName + ".mp4";
            if (forBuild == false) videoPlayer.url = "C://Users//user//Desktop//build//python//" + videoName + ".mp4";
            else videoPlayer.url = "file:///python//" + videoName + ".mp4";
            // 비디오 준비 (clip으로 사용시 아래 코드 모두 활성화)
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
            Invoke("StopVideo", endTime - startTime);

            videoPlayer.Play();
           
            /*
            // 동영상 클립 이름으로 Resources 폴더에서 VideoClip을 동적으로 로드
            videoClip = Resources.Load<VideoClip>(videoName);
            
            if (videoClip != null)
            {
                // VideoClip에 동영상 파일 할당
                videoPlayer.clip = videoClip;

                // 동영상 재생 범위 설정
                videoPlayer.time = startTime;
                videoPlayer.skipOnDrop = true;

                // 동영상 재생
                videoPlayer.Play();

                Invoke("StopVideo", endTime - startTime);

                ShowThumbnailInChat();
                //SendVideoInChatWindow();
            }
            */
            //movieCounter += 1;
        }
    }

    void OnVideoPrepared(VideoPlayer source)
    {
        // 비디오가 준비되면 호출되는 함수
        // 비디오를 재생하고 특정 시간으로 이동
        videoPlayer.Play();
        videoPlayer.time = startTime;
    }

    public void VideoPlaySimSwap(bool me)
    {
        if (forBuild == false) path = "C://Users//user//Desktop//build//python//SimSwap//output//movieticon.mp4";
        else path = "python//SimSwap//output//movieticon.mp4";
        Debug.Log("simswap");
        // 동영상 파일 경로 설정
        // 개발용 코드
        videoPlayer.url = path;
        // 빌드용 코드
        //videoPlayer.url = "python//SimSwap//output//movieticon.mp4";

        // 동영상 준비
        videoPlayer.Prepare();

        // 준비 완료 시 이벤트 등록
        videoPlayer.prepareCompleted += VideoPrepared;

        StartCoroutine(ShowThumbnailInChat(me));
        //SendVideoInChatWindow();
    }

    void VideoPrepared(VideoPlayer source)
    {
        // 동영상이 준비되면 재생
        source.Play();
    }

    void StopVideo()
    {
        videoPlayer.Stop();
    }

    GameObject tmp;
    IEnumerator ShowThumbnailInChat(bool me)
    {
        //if (forBuild == false) path = "C://Users//user//Desktop//build//python//thumbnail.png";
        //else path = "python//thumbnail.png";
        DateTime currentTime = DateTime.Now;
        string formattedTime = currentTime.ToString("HH:mm:ss"); // 시:분:초 형식

        if (me == true)
        {
            tmp = Instantiate(movieThumb, Chat.instance.ChatContent);
            theCM.chatLog = theCM.AddItemToArray(theCM.chatLog, "AAA%%movieticon" + movieCounter.ToString() + "   %%" + formattedTime);          
            MovieCount += 1;
            sendMovieList.Add(MovieCount);
            tmp.transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = MovieCount.ToString();
        }
        else
        {
            tmp = Instantiate(movieThumb_u, Chat.instance.ChatContent);
            theCM.chatLog = theCM.AddItemToArray(theCM.chatLog, "BBB%%movieticon" + movieCounter.ToString() + "   %%" + formattedTime);
            MovieCount += 1;
            receiveMovieList.Add(MovieCount);
            tmp.transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = MovieCount.ToString();
        }

        //byte[] fileData = System.IO.File.ReadAllBytes(path);
        //Texture2D texture = new Texture2D(2, 2);
        //texture.LoadImage(fileData);
        //tmp.transform.GetChild(2).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        VideoPlayer newVideoPlayer = tmp.transform.GetChild(1).GetChild(0).GetComponent<VideoPlayer>();
        RenderTexture renderTexture = new RenderTexture(256, 256, 24);
        renderTexture.Create();
        newVideoPlayer.targetTexture = renderTexture;
        tmp.transform.GetChild(1).GetComponent<RawImage>().texture = renderTexture;
        newVideoPlayer.source = VideoSource.Url;
        yield return new WaitForSeconds(1f);
        if (forBuild == false) newVideoPlayer.url = "C://Users//user//Desktop//build//python//" + "movieticon" + movieCounter.ToString() + ".mp4";
        else newVideoPlayer.url = "file:///python//" + "movieticon" + movieCounter.ToString() + ".mp4";
        if (File.Exists(newVideoPlayer.url))
            Debug.Log(newVideoPlayer.url);
        else Debug.Log("File Not exists");
        newVideoPlayer.Prepare();
        newVideoPlayer.Play();
        Debug.Log("after");
        movieCounter += 1;

        
    }


    public void SimSwapCheck()
    {
        if (faceSwap == true)
        {
            faceSwap = false;
            faceSwapText.text = "얼굴 합성 OFF";
        }
        else
        {
            faceSwap = true;
            faceSwapText.text = "얼굴 합성 ON";
        }
    }

    private void OnDestroy()
    {
        stream.Close();
        client.Close();
    }




    public void FinishExperiment()
    {
        if (textOnly == false)
        {
            if (sendMovieList.Count >= 3 && receiveMovieList.Count >= 3)
            {
                List<int> tmp1 = new List<int>();
                List<int> tmp2 = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    int rand = UnityEngine.Random.Range(0, sendMovieList.Count);
                    tmp1.Add(sendMovieList[rand]);                  
                    sendMovieList.RemoveAt(rand);

                    int rand2 = UnityEngine.Random.Range(0, receiveMovieList.Count);
                    tmp2.Add(receiveMovieList[rand2]);
                    receiveMovieList.RemoveAt(rand2);
                }
                tmp1.Sort();
                sendMovieSelect.text += tmp1[0] + ", " + tmp1[1] + ", " + tmp1[2];
                tmp2.Sort();
                receiveMovieSelect.text += tmp2[0] + ", " + tmp2[1] + ", " + tmp2[2];
            }
        }
        else
        {
            if (sendTextList.Count >= 5 && receiveTextList.Count >= 5)
            {
                List<int> tmp1 = new List<int>();
                List<int> tmp2 = new List<int>();
                if (theServer.serverStarted)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int rand = UnityEngine.Random.Range(2, sendTextList.Count);
                        tmp1.Add(sendTextList[rand]);
                        sendTextList.RemoveAt(rand);

                        int rand2 = UnityEngine.Random.Range(1, receiveTextList.Count);
                        tmp2.Add(receiveTextList[rand2]);
                        receiveTextList.RemoveAt(rand2);
                    }
                    tmp1.Sort();
                    sendMovieSelect.text += tmp1[0] + ", " + tmp1[1] + ", " + tmp1[2];
                    tmp2.Sort();
                    receiveMovieSelect.text += tmp2[0] + ", " + tmp2[1] + ", " + tmp2[2];
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int rand = UnityEngine.Random.Range(1, sendTextList.Count);
                        tmp1.Add(sendTextList[rand]);
                        sendTextList.RemoveAt(rand);

                        int rand2 = UnityEngine.Random.Range(0, receiveTextList.Count);
                        tmp2.Add(receiveTextList[rand2]);
                        receiveTextList.RemoveAt(rand2);
                    }
                    tmp1.Sort();
                    sendMovieSelect.text += tmp1[0] + ", " + tmp1[1] + ", " + tmp1[2];
                    tmp2.Sort();
                    receiveMovieSelect.text += tmp2[0] + ", " + tmp2[1] + ", " + tmp2[2];
                }
            }
        }
    }

    public void OnlyTextButton()
    {
        textOnly = true;
        onlyTextCheck.SetActive(true);
        withMovieCheck.SetActive(false);
        MovieSendButtonObject.SetActive(false);
    }
    public void WithMovieButton()
    {
        textOnly = false;
        onlyTextCheck.SetActive(false);
        withMovieCheck.SetActive(true);
        MovieSendButtonObject.SetActive(true);
    }
}
