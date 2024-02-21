using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    //Allow designer to easily change what a power up does
    public MyEnum powerUps = new MyEnum();
    private MeshRenderer mRenderer;
    private bool powerUpAvailable = true;

    //Color of each power up's pickup item
    private Color jetBoostPowerUpColor = new Color(0.718f, 0.0784f, 0.165f); //Red
    private Color shieldPowerUp = new Color(0.078f, 0.72f, 0.087f, 0.808f); //Green

    //Audio stuff
    public AudioSource audioSource;

    //Power up sounds
    public AudioClip JetBoostPowerUp;
    public AudioClip ShieldPowerUp;

    public enum MyEnum
    {
        JetBoost,
        Shield
    }
    private void Awake()
    {
        mRenderer = this.gameObject.GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (powerUps == (MyEnum)0)
        {
            this.gameObject.GetComponent<Renderer>().material.SetColor("_Color", jetBoostPowerUpColor);
        }

        if (powerUps == (MyEnum)1)
        {
            this.gameObject.GetComponent<Renderer>().material.SetColor("_Color", shieldPowerUp);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerShip playerShip
            = other.gameObject.GetComponent<PlayerShip>();
        if (playerShip != null)
        {
            mRenderer.enabled = false; //Turn off model in game

            if (powerUpAvailable)
            {
                if (powerUps == (MyEnum)0) //Gives player five seconds of jet boost
                {
                    audioSource.PlayOneShot(JetBoostPowerUp, 0.6f);
                    playerShip.JetBoostRemaining += 5;
                    Debug.Log("Thanks, Sam!");
                }

                if (powerUps == (MyEnum)1) //Shield Power up **NEEDS VISUAL FEEDBACK**  
                {
                    audioSource.PlayOneShot(ShieldPowerUp, 0.6f);
                    playerShip.ShieldActive = true;
                    playerShip.shieldVisual.enabled = true;
                    playerShip.ShieldTimeRemaining = 5;
                }

                powerUpAvailable = false;
            }
        }
    }
}
