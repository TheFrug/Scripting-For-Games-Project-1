using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stunHitbox : MonoBehaviour
{

    public bool stunOnCooldown = false;

    //not detecting collision at all.  Need to fix this so that the debris can actually stun the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!stunOnCooldown)
            {
                GameManager.Instance.Stun();
                stunOnCooldown = true;

                Invoke("stunDelay", GameManager.Instance.stunCooldown);
            }
        }
            

    }

    private void stunDelay()
    {
        stunOnCooldown = false;
    }

}
