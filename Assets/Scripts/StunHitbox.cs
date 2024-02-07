using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunHitbox : MonoBehaviour
{
    public ParticleSystem mainJet;

    private PlayerShip playerShip; //Establishes internal player


    private void Awake()
    {
         playerShip = FindObjectOfType<PlayerShip>();
    }

    // Stuns player if they run into this
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            mainJet = GameObject.Find("jetEngine_PS").GetComponent<ParticleSystem>();

            if (GameManager.Instance.stunOnCooldown == false)
            {
                Debug.Log("Hit the Player!");

                GameManager.Instance.playerStunned = true;
                GameManager.Instance.stunOnCooldown = true;
                Debug.Log("Stun is on cooldown");

                Invoke("stunDelay", GameManager.Instance.stunCooldown);

                playerShip.Hurt(1);
                mainJet.Stop(); //Tells animation to stop because we just collided
            }
        }
            

    }

    private void stunDelay()
    {
        GameManager.Instance.stunOnCooldown = false;
        Debug.Log("Stun is no longer on cooldown");
    }

}
