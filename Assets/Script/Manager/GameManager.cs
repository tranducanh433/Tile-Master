using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject winText;
    [SerializeField] GameObject loseText;

    public static GameManager Instance;


    void Start()
    {
        Instance = this;
    }

    public void DisplayWinText()
    {
        winText.SetActive(true);
    }
    public void DisplayLoseText()
    {
        loseText.SetActive(true);
    }
}
