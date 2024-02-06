using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool playerActive = true;


    [SerializeField] public float stunCooldown;

    //**THIS DOES NOTHING RIGHT NOW.FIX THAT**
    public bool stunOnCooldown = false; //Checks to see if the cooldown is active or not

    [SerializeField] public float stunTimer; // How long the player should be stunned for
    private float stunTimerStored; // Gains the same value as stunTimer to be used later
    public bool playerStunned = false; // Boolean that decides when player loses control of their ship as a status effect


    //create the instance (exciting!)
    public static GameManager Instance { get; private set; }
        public void Awake()
        {

        playerActive = true;
        stunTimerStored = stunTimer;//Stores stunTimer value inputed in SerializeField so we can reset the stunTimer after it runs once

            //Make sure that any duplicate instances are destroyed on Awake()
            if (Instance != null && Instance != this) // make sure that any duplicate instances are destroyed on Awake()
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

        }

    private void Update()
    {

        //Only runs checkStun when another function makes playerStunned true, then runs on update until the player is no longer stunned.
        if (playerStunned) { 
        checkStun();
        }

        //Backspace restarts the game
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ReloadLevel();
        }

        //ESC quits the game
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
            Debug.Log("Quitting Game!");
        }

    }

    //Allows game to restart
    void ReloadLevel()
    {
        int activeSceneIndex =
            SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
    }

    //Disables player controls for a certain duration and sets stunOnCooldown() active so player cannot be stunned for short duration
    public void checkStun()
    {
        if (playerStunned)
        {
            playerActive = false;
            stunTimer -= Time.smoothDeltaTime;
            if (stunTimer >= 0)
            {
                Debug.Log("Still Stunned");
            }
            else
            {
                Debug.Log("Done");
                playerActive = true;
                playerStunned = false;
                stunTimer = stunTimerStored;
            }
        }
    }

}