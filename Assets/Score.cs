using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText; 
    public int Currentscore = 0; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "Kills: 0"; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateScore(int score)
    {
        Currentscore += score;
        scoreText.text = "Kills: " + Currentscore.ToString();
    }
}
