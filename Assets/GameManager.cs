using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists.

public class GameManager : MonoBehaviour
{   void Start()
    {
        
    }

    //Update is called every frame.
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    void Awake()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}