using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    //defaults to false
/*    public bool isGameOver;

    public int lives = 3;
    private int currentLives;
    public Image healthImage;*/

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    void Start() {
/*        Cursor.visible = false;
        currentLives = lives;
        UpdateHealthUI();*/
    }

    void Update() {
        //if (currentLives <= 0) isGameOver = true;
    }

    public void LoseLife() {
/*        currentLives--;
        UpdateHealthUI();*/
    }
    private void UpdateHealthUI() {
        // Update the fill amount of the health bar
        //healthImage.fillAmount = (float)currentLives / lives / 3.3f;
    }
}