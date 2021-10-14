using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper S;
    public static int highScore = 1000;
    int currentScore;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text currScoreText;
    [SerializeField] float scoreTextDuration;
    [SerializeField] TMP_Text worldTextPrefab;
    [SerializeField] bool resetHighScoreOnLoad = false;
    readonly List<GameObject> worldTexts = new List<GameObject>();
    public GameEvent OnPointGained;

    void Awake()
    {
        S = this;
        if (!resetHighScoreOnLoad)
        {
            if (PlayerPrefs.HasKey("HighScore"))
                highScore = PlayerPrefs.GetInt("HighScore");
        }
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    void Update()
    {
        highScoreText.text = $"High Score: {highScore}";
        currScoreText.text = $"Current Score: {currentScore}";
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    public void AddToScore(GameObject obj, int amount)
    {
        GameObject txt = GetWorldText();
        txt.GetComponent<TMP_Text>().text =  $"+ {amount}";
        txt.transform.position = obj.transform.position;
        txt.SetActive(true);
        StartCoroutine(HideScoreText(txt));
        currentScore += amount;
        if (OnPointGained != null)
            OnPointGained.Invoke(obj);
    }

    public void DisplayMessage(string message, float duration, Vector3 location) => StartCoroutine(ShowMessage(message, duration, location));

    IEnumerator<WaitForSeconds> ShowMessage(string message, float duration, Vector3 location)
    {
        GameObject newText = GetWorldText();
        newText.GetComponent<TMP_Text>().text = message;
        newText.transform.position = location;
        newText.SetActive(true);
        yield return new WaitForSeconds(duration);
        newText.SetActive(false);
    }

    IEnumerator<WaitForSeconds> HideScoreText(GameObject obj)
    {
        Vector3 objPos = obj.transform.position;
        obj.transform.position = Vector3.MoveTowards(objPos, objPos + Vector3.up, Time.deltaTime);
        yield return new WaitForSeconds(scoreTextDuration);
        obj.SetActive(false);
    }

    public GameObject GetWorldText()
    {
        if (worldTexts.Count > 0)
        {
            for (int i = 0; i < worldTexts.Count; i++)
                if (!worldTexts[i].activeSelf)
                    return worldTexts[i];
        }

        GameObject newText = Instantiate(worldTextPrefab.gameObject, Vector3.down, Quaternion.identity, transform);
        newText.SetActive(false);
        worldTexts.Add(newText);
        return newText;
    }
}