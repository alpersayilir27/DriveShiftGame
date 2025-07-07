using UnityEngine;
using TMPro;
using System.Collections;

public class RaceCountdown : MonoBehaviour
{
    public TextMeshProUGUI countdownTMP;
    public AudioSource audioSource;
    public AudioClip countdownClip; 

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        
        countdownTMP.gameObject.SetActive(false);  
        yield return StartCoroutine(WaitForRealSeconds(1.5f));

        
        Time.timeScale = 0f;

        countdownTMP.gameObject.SetActive(true);

        
        audioSource.PlayOneShot(countdownClip);

        
        countdownTMP.text = "3";
        countdownTMP.color = Color.red;
        countdownTMP.transform.localScale = Vector3.one * 1.5f;

        yield return StartCoroutine(WaitForRealSeconds(1f));

        countdownTMP.text = "2";
        countdownTMP.color = new Color(1f, 0.65f, 0f);
        countdownTMP.transform.localScale = Vector3.one * 1.5f;

        yield return StartCoroutine(WaitForRealSeconds(1f));

        countdownTMP.text = "1";
        countdownTMP.color = Color.yellow;
        countdownTMP.transform.localScale = Vector3.one * 1.5f;

        yield return StartCoroutine(WaitForRealSeconds(1f));

        countdownTMP.text = "GO!";
        countdownTMP.color = Color.green;
        countdownTMP.transform.localScale = Vector3.one * 1.5f;

        yield return StartCoroutine(WaitForRealSeconds(1f));

        countdownTMP.gameObject.SetActive(false);

        
        Time.timeScale = 1f;
    }

    IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
