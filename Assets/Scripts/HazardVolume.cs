using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardVolume : MonoBehaviour
{

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        PlayerShip playerShip
            = other.gameObject.GetComponent<PlayerShip>();
        //If we found something valid, continue
        if (playerShip != null)
        {
            // Do something!
            playerShip.Kill();
            audioSource.Play();
            GameManager.Instance.youLose();
        }
    }
}
