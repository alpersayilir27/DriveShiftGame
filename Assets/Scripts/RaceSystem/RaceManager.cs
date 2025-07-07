using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instansce;

    [SerializeField] private TextMeshProUGUI currentLapTimeText;
    [SerializeField] private TextMeshProUGUI overallRaceTimeText;
    [SerializeField] private TextMeshProUGUI bestLapTimeText;
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI checkpointMissedText;

    [SerializeField] private GameObject[] checkpoints;
    [SerializeField] private int lastCheckpointIndex = -1;
    [SerializeField] private bool isCircuit = false;
    [SerializeField] private int totalLaps = 3;
    [SerializeField] private int currentLap = 0;

    [SerializeField] private GameObject raceEndPanel;
    [SerializeField] private CanvasGroup raceEndPanelCanvasGroup;  // Panel için CanvasGroup (fade için)
    [SerializeField] private TextMeshProUGUI resultsText;

    [SerializeField] private AudioSource backgroundMusicAudioSource; // Yarış boyunca çalacak müzik

    [SerializeField] private GameObject mobileCanvas;  // Mobil direksiyon vb. UI
    [SerializeField] private GameObject speedMeterCanvas; // Speed meter UI

    private bool raceStarted = false;
    private bool raceFinished = false;
    private bool ifCheckpointMissed = false;

    private float currentLapTime = 0f;
    private float overallRaceTime = 0f;
    private float bestLapTime = Mathf.Infinity;

    private void Awake()
    {
        if (Instansce == null)
        {
            Instansce = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (raceEndPanel != null)
        {
            raceEndPanel.SetActive(false);
        }
        if (raceEndPanelCanvasGroup != null)
        {
            raceEndPanelCanvasGroup.alpha = 0f;
        }

        if (backgroundMusicAudioSource != null)
        {
            backgroundMusicAudioSource.Play();
        }
    }

    private void Update()
    {
        if (raceStarted)
        {
            UpdateTimers();
        }
        UpdateUI();
    }

    public void CheckpointReached(int checkpointIndex)
    {
        if ((!raceStarted && checkpointIndex != 0) || raceFinished) return;

        bool validLapFinish = isCircuit && raceStarted && lastCheckpointIndex == checkpoints.Length - 1 && checkpointIndex == 0;

        if (checkpointIndex == 0 && !raceStarted)
        {
            StartRace();
            lastCheckpointIndex = checkpointIndex;
            HideCheckpointMissedText();
            return;
        }

        if (checkpointIndex == lastCheckpointIndex + 1 || validLapFinish)
        {
            UpdateCheckpoint(checkpointIndex);
            HideCheckpointMissedText();
        }
        else
        {
            ShowCheckpointMissedText();
        }
    }

    private void UpdateCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == 0)
        {
            if (!raceStarted)
            {
                StartRace();
            }
            else
            {
                OnLapFinished();
            }
        }
        else if (!isCircuit && checkpointIndex == checkpoints.Length - 1)
        {
            OnLapFinished();
        }

        lastCheckpointIndex = checkpointIndex;
    }

    private void OnLapFinished()
    {
        currentLap++;

        if (currentLapTime < bestLapTime)
        {
            bestLapTime = currentLapTime;
        }

        if (currentLap >= totalLaps)
        {
            StartCoroutine(SmoothStopAndShowPanel());
        }
        else
        {
            currentLapTime = 0f;
            lastCheckpointIndex = isCircuit ? 0 : -1;
        }
    }

    private void StartRace()
    {
        raceStarted = true;
        raceFinished = false;

        if (raceEndPanel != null)
            raceEndPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private IEnumerator SmoothStopAndShowPanel()
    {
        raceFinished = true;
        raceStarted = false;

        if (mobileCanvas != null)
        {
            mobileCanvas.SetActive(false);
        }

        if (speedMeterCanvas != null)
        {
            speedMeterCanvas.SetActive(false);
        }

        if (raceEndPanel != null)
        {
            raceEndPanel.SetActive(true);
        }
        if (raceEndPanelCanvasGroup != null)
        {
            raceEndPanelCanvasGroup.alpha = 0f;
        }

        if (resultsText != null)
        {
            string bestLapStr = FormatTime(bestLapTime);
            string overallStr = FormatTime(overallRaceTime);
            resultsText.text = $"Best Lap Time: {bestLapStr}\nOverall Race Time: {overallStr}";
        }

        float fadeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (raceEndPanelCanvasGroup != null)
                raceEndPanelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        if (raceEndPanelCanvasGroup != null)
            raceEndPanelCanvasGroup.alpha = 1f;

        float stopDuration = 1f;
        elapsed = 0f;
        float startTimeScale = Time.timeScale;

        while (elapsed < stopDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startTimeScale, 0f, elapsed / stopDuration);
            yield return null;
        }
        Time.timeScale = 0f;
    }

    private void UpdateTimers()
    {
        currentLapTime += Time.deltaTime;
        overallRaceTime += Time.deltaTime;
    }

    private void UpdateUI()
    {
        currentLapTimeText.text = "Current Lap Time: " + FormatTime(currentLapTime);
        overallRaceTimeText.text = "Overall Race Time: " + FormatTime(overallRaceTime);
        lapText.text = $"Lap {currentLap}/{totalLaps}";
        bestLapTimeText.text = "Best Lap Time: " + FormatTime(bestLapTime);

        UpdateCheckpointMissedText();
    }

    private void UpdateCheckpointMissedText()
    {
        if (ifCheckpointMissed)
        {
            float alpha = Mathf.PingPong(Time.time * 2, 1);
            Color newColor = checkpointMissedText.color;
            newColor.a = alpha;
            checkpointMissedText.color = newColor;
        }
    }

    private void ShowCheckpointMissedText()
    {
        if (!ifCheckpointMissed)
        {
            checkpointMissedText.gameObject.SetActive(true);
            ifCheckpointMissed = true;
        }
    }

    private void HideCheckpointMissedText()
    {
        if (ifCheckpointMissed)
        {
            checkpointMissedText.gameObject.SetActive(false);
            ifCheckpointMissed = false;
        }
    }

    private string FormatTime(float time)
    {
        if (float.IsInfinity(time) || time < 0) return "--:--";

        int minutes = (int)time / 60;
        float seconds = time % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Butonlara bağlamak için public fonksiyonlar

    public void LoadNextLevel()
    {
        Time.timeScale = 1f; // Oyunu tekrar başlat
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Oyunu tekrar başlat
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
