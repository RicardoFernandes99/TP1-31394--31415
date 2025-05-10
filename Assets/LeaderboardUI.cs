using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct ScoreEntry {
    public string playerName;
    public int score;
}

[System.Serializable]
public class ScoreData {
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

public class LeaderboardUI : MonoBehaviour
{
    [Header("Prefab & Container")]
    public GameObject entryPrefab;      
    public Transform entriesContainer;
    
    [Header("Name Input UI")]
    public InputField nameInputField; 
    public Image TryagainImage;  
    public Text scoreDisplay;
    
    [Header("Save Settings")]
    public Button saveButton;
      [Header("Play Again")]
    public Button playAgain;         
    public int maxEntries = 10;        
    private string SavePath => Path.Combine(Application.persistentDataPath, "leaderboard.json");
    private List<ScoreEntry> leaderboardData = new List<ScoreEntry>();
    private int currentSessionScore = 0;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        nameInputField.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);

        LoadScores();
        
        if (PlayerPrefs.HasKey("PlayerFinalScore"))
        {
            currentSessionScore = PlayerPrefs.GetInt("PlayerFinalScore");
            
            if (scoreDisplay != null)
            {
                scoreDisplay.text = currentSessionScore.ToString();
            }
            else
            {
                playAgain.gameObject.SetActive(true);
                TryagainImage.gameObject.SetActive(true);
                nameInputField.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(false);
                scoreDisplay.gameObject.SetActive(false);
                Debug.LogWarning("Score Display Text is not assigned in the inspector");
            }
        }
        else
        {
            playAgain.gameObject.SetActive(true);

            TryagainImage.gameObject.SetActive(true);
            nameInputField.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(false);
            scoreDisplay.gameObject.SetActive(false);
        }
        
        Populate(leaderboardData);
    }
    
    public void SaveCurrentScore()
    {
        if (currentSessionScore <= 0)
        {
                playAgain.gameObject.SetActive(true);

                TryagainImage.gameObject.SetActive(true);
                nameInputField.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(false);
                scoreDisplay.gameObject.SetActive(false);
            return;
        }
        
        string playerName = "Player";
        if (nameInputField != null)
        {
            playerName = nameInputField.text;
        }
        
        if (IsHighScore(currentSessionScore))
        {
            AddScore(playerName, currentSessionScore);
                playAgain.gameObject.SetActive(true);

                TryagainImage.gameObject.SetActive(true);
                nameInputField.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(false);
                scoreDisplay.gameObject.SetActive(false);
            Debug.Log($"Added high score: {currentSessionScore} for player: {playerName}");
        }
        else
        {

            Debug.Log($"Score {currentSessionScore} not high enough to be added to leaderboard");
        }
        
        currentSessionScore = 0;
                playAgain.gameObject.SetActive(true);

                TryagainImage.gameObject.SetActive(true);
                nameInputField.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(false);
                scoreDisplay.gameObject.SetActive(false);

        PlayerPrefs.DeleteKey("PlayerFinalScore");
        PlayerPrefs.Save();
    }

    public void LoadScores()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                ScoreData data = JsonUtility.FromJson<ScoreData>(json);
                leaderboardData = data.scores;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading leaderboard: {e.Message}");
                leaderboardData = new List<ScoreEntry>();
            }
        }
        else
        {
            leaderboardData = new List<ScoreEntry>();
        }
    }
    
    public void SaveScores()
    {
        try
        {
            ScoreData data = new ScoreData
            {
                scores = leaderboardData
            };
            
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving leaderboard: {e.Message}");
        }
    }
    
    public void AddScore(string playerName, int score)
    {
        ScoreEntry newEntry = new ScoreEntry
        {
            playerName = playerName,
            score = score
        };
        
        leaderboardData.Add(newEntry);
        
        leaderboardData = leaderboardData
            .OrderByDescending(entry => entry.score)
            .ToList();
        
        if (leaderboardData.Count > maxEntries)
        {
            leaderboardData = leaderboardData.Take(maxEntries).ToList();
        }
        
        SaveScores();
        
        Populate(leaderboardData);
    }
    
    public bool IsHighScore(int score)
    {
        if (leaderboardData.Count < maxEntries)
            return true;
            
        return score > leaderboardData.Min(entry => entry.score);
    }

    void Populate(List<ScoreEntry> scores)
    {
        foreach (Transform t in entriesContainer)
            Destroy(t.gameObject);

        for (int i = 0; i < scores.Count; i++)
        {
            var go = Instantiate(entryPrefab, entriesContainer);
            go.transform.localScale = Vector3.one;

            var rankText = go.transform.Find("rank").GetComponent<Text>();
            var nameText = go.transform.Find("username").GetComponent<Text>();
            var scoreText = go.transform.Find("score").GetComponent<Text>();

            rankText.text = $"{i+1}.";
            nameText.text = scores[i].playerName;
            scoreText.text = scores[i].score.ToString();
        }
        
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}