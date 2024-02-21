using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winCondition : MonoBehaviour
{
    public AudioClip CoinGet;
    private bool pickedUp = false;

    ParticleSystem ps_winNugget;

    private void Awake()
    {
        ps_winNugget = this.GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerShip playerShip
            = other.gameObject.GetComponent<PlayerShip>();
        if (playerShip != null)
        {
            if (!pickedUp)
            {
                GameManager.Instance.winPoints += 1; //Increments win condition
                this.GetComponent<Renderer>().enabled = false; //Makes the object disappear
                Debug.Log("Points: " + GameManager.Instance.winPoints);
                pickedUp = true;
                ps_winNugget.Play();
                this.GetComponent<AudioSource>().PlayOneShot(CoinGet);

                if (GameManager.Instance.winPoints >= 5)
                {
                    GameManager.Instance.youWin();
                }
            }
        }
    }

}
