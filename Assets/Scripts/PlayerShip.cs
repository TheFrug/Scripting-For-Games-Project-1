using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //ensures that the GetComponent will always find this object/s Rigidbody
public class PlayerShip : MonoBehaviour
{
    //[SerializeField]
    //private GameManager _gameController;

    //Set up movement speed variable
    [SerializeField]
    float _moveSpeed = 12f;

    //Set up turn speed variable
    [SerializeField]
    float _turnSpeed = 3f;

    //Maximum Health for Player
    public float playerHealth = 3;

    //Set up connections to GameObjects that have Particle Systems attached to them
    public ParticleSystem MainJet;
    public Color MainJetStartColor;

    public ParticleSystem _leftExhaust;
    public ParticleSystem _rightExhaust;

    Rigidbody _rb = null; //Will be stored on Awake()

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>(); //Find the Rigidbody and store it as the thing
        MainJet = GameObject.Find("jetEngine_PS").GetComponent<ParticleSystem>();
    }

    private void Start()
    {

        MainJet = GameObject.Find("jetEngine_PS").GetComponent<ParticleSystem>(); //Finds ParticleSystem from jetEngine_PS
        //There is a better way to do this that can allow designers to change the jet color without changing code
        MainJetStartColor = new Color(0.91f, 0.58f, 0.22f, 0.40f); //Saves starting color as MainJetStartColor
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) //Try out stun mechanics
        {
            GameManager.Instance.playerStunned = true;
        }



        //Activates a speed up buff
        if (Input.GetKey(KeyCode.LeftShift))
        {
            JetBoost(); //Changes particle color and increases speed
        }

    }


    private void FixedUpdate()
    {
        MoveShip();
        TurnShip();
    }

    //use forces to build momentum forward/backward
    void MoveShip()
    {
        if (GameManager.Instance.playerActive) {
            // S/Down = -1, W/Up = 1, None = 0.  Scale found direction with moveSpeed.
            // This is the final movement vector (force! like physics!!!!!!)
            float moveAmountThisFrame = Input.GetAxisRaw("Vertical") * _moveSpeed;
            // Combine direction with calculated amount
            Vector3 moveDirection = transform.forward * moveAmountThisFrame;
            // apply the movement to the physics object
            _rb.AddForce(moveDirection);

            // While holding W, particle effect plays
            if (Input.GetKey(KeyCode.W) && (GameManager.Instance.playerActive))
            {
                MainJet.Play();
            }
            // Anytime W is not held, the particle effect stops| There is probably a better way to do this
            if (!Input.GetKey(KeyCode.W))
            {
                MainJet.Stop();
            }
        }
    }
    
    // Don't use force for this, since we want turning to be precise as fuck
    void TurnShip()
    {
        if (GameManager.Instance.playerActive)
        {
            // A/Left = -1, D/Right = 1, None = 0.  Scale by turnSpeed
            // Store the vector of how the ship should turn within one frame
            float turnAmountThisFrame = Input.GetAxisRaw("Horizontal") * _turnSpeed;
            // Specify an axis to apply our turn amount (x,y,z) as a rotation
            Quaternion turnOffset = Quaternion.Euler(0, turnAmountThisFrame, 0);
            // spin the rigidbody
            _rb.MoveRotation(_rb.rotation * turnOffset);
        }
    }

    //**THIS STILL NEEDS FUNCTIONALITY**
    void JetBoost()
    {
        MainJet.GetComponent<Transform>().localScale = new Vector3(0.07f, 0.05f, 0.07f);
        MainJet.GetComponent<ParticleSystem>().startColor = new Color(.27f, .9f, .9f, .35f);
    }

    //Allows the player to be killed
    public void Kill()
    {
        this.gameObject.SetActive(false); //Delete Game Object
        GameManager.Instance.playerActive = false; //Remove Controls
    }

    public void Hurt(float damage)
    {
        playerHealth -= damage;
        Debug.Log("Player Hit.  Health Remaining: " + playerHealth);

        if (playerHealth <= 0)
        {
            Kill();
            GameManager.Instance.youLose();
        }
    }
}
