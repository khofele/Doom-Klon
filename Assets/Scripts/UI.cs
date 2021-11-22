using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Image healthBar = null;
    [SerializeField] private GameObject deathMessage = null;
    [SerializeField] private GameObject counter = null;
    [SerializeField] private GameObject gameWonMessage = null;

    private static int zombieCounter = 0;

    public static int ZombieCounter
    {
        get => zombieCounter;
        set
        {
            zombieCounter = value;
        }
    }

    private void Start()
    {
        healthBar.color = Color.green;
        counter.GetComponentInChildren<TextMeshProUGUI>().text = "Zombies killed: " + 0;
    }

    private void Update()
    {
        counter.GetComponentInChildren<TextMeshProUGUI>().text = "Zombies killed: " + zombieCounter;
        CheckGameWon();
    }

    public void SetColor(float value)
    {
        healthBar.fillAmount = value;
        
        if(healthBar.fillAmount < 0.2f)
        {
            healthBar.color = Color.red;
        }
        else if(healthBar.fillAmount  < 0.5f)
        {
            healthBar.color = Color.yellow;
        }
        else
        {
            healthBar.color = Color.green;
        }
    }

    public void DisplayDeathMessage()
    {
        deathMessage.SetActive(true);
    }

    private void CheckGameWon()
    {
        if(zombieCounter >= 7)
        {
            DisplayGameWonMessage();
        }
    }

    private void DisplayGameWonMessage()
    {
        gameWonMessage.SetActive(true);
    }
}
