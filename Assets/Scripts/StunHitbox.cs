using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunHitbox : MonoBehaviour
{
    public ParticleSystem mainJet;

    private PlayerShip playerShip; //Establishes internal player
    public AudioSource audioSource;
    public AudioClip stunSound;

    private void Awake()
    {
         playerShip = FindObjectOfType<PlayerShip>();
         mainJet = GameObject.Find("jetThruster_PS").GetComponent<ParticleSystem>();
    }

    // Stuns player if they run into this
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (playerShip.ShieldActive == false) //If player has shield power up, doesn't trigger
            {
                if (GameManager.Instance.stunOnCooldown == false)
                {
                    Debug.Log("Hit the Player!");
                    audioSource.PlayOneShot(stunSound);

                    GameManager.Instance.playerStunned = true;
                    GameManager.Instance.stunOnCooldown = true;
                    Debug.Log("Stun is on cooldown");

                    Invoke("stunDelay", GameManager.Instance.stunCooldown);

                    playerShip.Hurt(1);
                    playerShip.MainThrusterEnd(); //Tells animation to stop because we just collided
                    playerShip.LeftThrusterEnd();
                    playerShip.RightThrusterEnd();
                }
            }
        }
            

    }



    private void stunDelay()
    {
        GameManager.Instance.stunOnCooldown = false;
        Debug.Log("Stun is no longer on cooldown");
    }

}
