using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool playerActive = false;


    [SerializeField] public float stunCooldown;
    [SerializeField] float stunTimer;
    float stunTimerStored;

    public bool playerStunned = false;

    //create the instance (exciting!)
    public static GameManager Instance { get; private set; }
        public void Awake()
        {

        stunTimerStored = stunTimer;
        playerActive = true;

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

        Stun();

        if (Input.GetKeyDown(KeyCode.Backspace)) // use backspace to restart
        {
            ReloadLevel();
        }
    }

    void ReloadLevel()
    {
        int activeSceneIndex =
            SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
    }

    public void Stun()
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