//-----------------------------------------
//   ScoreTextController.cs
//
//   Bobby Yang
//   http://dragonberri.me
//   @dragonberryyang
//
//   last edited on 4/25/2015
//-----------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreTextController : MonoBehaviour 
{
    private Text txt;

    void OnDisable ()
    {
        // clear the text
        txt.text = string.Empty;
    }

    void OnEnable ()
    {
        // cache components
        txt = GetComponent<Text>();

        if (this.gameObject.name == "MainMenu_Text_HighScore")
        {
            float highScore = GameController.gameHighScore;
            txt.text = "Highscore " + highScore;
        }
        else if (this.gameObject.name == "GameOver_Text_Score_Time")
        {
            float score = GameController.gameScore;
            txt.text = "You scored " + score + " points!";
        }
    }

    void Start ()
    {
        if (this.gameObject.name == "MainMenu_Text_HighScore")
        {
            float highScore = GameController.gameHighScore;
            txt.text = "Highscore " + highScore;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // update the score based on following object names
        if (this.gameObject.name == "GamePlay_Text_Score_Time")
        {
            float score = GameController.gameScore;
            txt.text = "Score " + score;
        }
	}
}
