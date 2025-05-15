using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survey : MonoBehaviour
{
    public void OpenSurvey()
    {
        string surveyUrl = "https://docs.google.com/forms/d/e/1FAIpQLSfKdiNkr4Qqt5kn6KH5fuZ0JESvywgB0b3JyQOECewvyC0X5A/viewform?usp=sf_link"; // 여기에 구글 설문지 URL을 넣어주세요.

        // Google 설문지 URL을 엽니다.
        Application.OpenURL(surveyUrl);
    }
}
