using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunHitbox : MonoBehaviour
{
    //not detecting collision at all.  Need to fix this so that the debris can actually stun the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit the Player!");

            if (GameManager.Instance.stunOnCooldown == false)
            {
                GameManager.Instance.playerStunned = true;
                GameManager.Instance.stunOnCooldown = true;
                Debug.Log("Stun is on cooldown");

                Invoke("stunDelay", GameManager.Instance.stunCooldown);
            }
        }
            

    }

    private void stunDelay()
    {
        GameManager.Instance.stunOnCooldown = false;
        Debug.Log("Stun is no longer on cooldown");
    }

}
