using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winCondition : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        PlayerShip playerShip
            = other.gameObject.GetComponent<PlayerShip>();
        if (playerShip != null)
        {
            GameManager.Instance.winPoints += 1; //Increments win condition
            this.gameObject.SetActive(false); //Makes the object disappear
            Debug.Log("Points: " + GameManager.Instance.winPoints);

            if (GameManager.Instance.winPoints >= 5)
            {
                GameManager.Instance.youWin();
            }
        }
    }

}
