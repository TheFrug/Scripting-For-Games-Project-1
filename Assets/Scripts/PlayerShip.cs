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
    float _baseMoveSpeed = 12f;
    private float adjustedSpeed;
    
    //JetBoost Management
    private bool jetBoostActive = false;
    [SerializeField]
    private float _jetBoostSpeed;
    


    //Set up turn speed variable
    [SerializeField]
    float _turnSpeed = 3f;

    //Maximum Health for Player
    public float playerHealth = 3;

    //Set up connections to GameObjects that have Particle Systems attached to them

    //Default color Set by designer
    [SerializeField]
    public Color mainJetStartColor;

    //Jet color when JetBoost is active
    [SerializeField]
    public Color jetBoostColor;

    //Manage size of visual effects for each jet
    [SerializeField]
    Vector3 _mainJetBoostSize; //Size when boosted (decided by designer)
    Vector3 mainJetStartSize;

    // **THIS DOES NOT WORK YET. FIX THIS FIRST**

    [SerializeField]
    Vector3 _leftThrusterBoostSize;
    Vector3 leftThrusterStartSize;
    [SerializeField]
    Vector3 _rightThrusterBoostSize;
    public Vector3 rightThrusterStartSize;







    //Set up Particle Systems for each jet
    public ParticleSystem MainJet;
    public ParticleSystem _leftThruster;
    public ParticleSystem _rightThruster;

    Rigidbody _rb = null; //Will be stored on Awake()

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>(); //Find the Rigidbody and store it as the thing
        MainJet = GameObject.Find("jetThruster_PS").GetComponent<ParticleSystem>(); //Finds ParticleSystem on jetThruster_PS and rembers it
        Vector3 rightThrusterStartSize = _rightThruster.GetComponent<Transform>().localScale; //This is not working
        checkSpeed();
    }

    private void Start()
    {
        MainJet = GameObject.Find("jetThruster_PS").GetComponent<ParticleSystem>(); //Finds ParticleSystem from jetThruster_PS
        //There is a better way to do this that can allow designers to change the jet color without changing code
    }

    public void Update()
    {
        //transform.Rotate(0f, 1, 0f, Space.Self);

        if (Input.GetKeyDown(KeyCode.KeypadEnter)) //Try out stun mechanics
        {
            GameManager.Instance.playerStunned = true; //This being true makes the GameManager trigger checkStun()
        }

        //Activates a speed up buff
        if (Input.GetKey(KeyCode.LeftShift)) {
            JetBoost(); //Changes particle color and increases speed
        }

        //Ends jet boost
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            JetBoostEnd(); //Returns particle effect to how it was
        }

        if (Input.GetKey(KeyCode.A)) {
            RightThruster();
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            RightThrusterEnd();
        }
    

    }

    private void FixedUpdate()
    {
        //This happens in FixedUpdate()
        MoveShip();
        TurnShip();
    }

    //use forces to build momentum forward/backward
    void MoveShip()
    {
        if (GameManager.Instance.playerActive) {
            // S/Down = -1, W/Up = 1, None = 0.  Scale found direction with moveSpeed.
            // This is the final movement vector (force! like physics!!!!!!)
            float moveAmountThisFrame = Input.GetAxisRaw("Vertical") * adjustedSpeed;
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
        if (GameManager.Instance.playerActive) {
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
        jetBoostActive = true;
        MainJet.GetComponent<Transform>().localScale = _mainJetBoostSize;
        MainJet.startColor = jetBoostColor;
        _rightThruster.gameObject.SetActive(true);
        _leftThruster.gameObject.SetActive(true);
        checkSpeed();
    }

    void JetBoostEnd()
    {
        jetBoostActive = false;
        MainJet.GetComponent<Transform>().localScale = mainJetStartSize;
        MainJet.startColor = mainJetStartColor;
        _rightThruster.gameObject.SetActive(false);
        _leftThruster.gameObject.SetActive(false);
        checkSpeed();
    }
    void RightThruster()
    {
        _rightThruster.gameObject.SetActive(true);
        if (jetBoostActive)
        {
            _rightThruster.GetComponent<Transform>().localScale = new Vector3(0.035f, 0.04f, 0.035f);
            //Needs to also change color
        }
    }

    void RightThrusterEnd()
    {
        _rightThruster.gameObject.SetActive(false);
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

        if (playerHealth <= 0) {
            Kill();
            GameManager.Instance.youLose();
        }
    }

    public void checkSpeed()
    {
        if (jetBoostActive) {
            adjustedSpeed = _baseMoveSpeed + _jetBoostSpeed;
        }
        else {
            adjustedSpeed = _baseMoveSpeed;
        }
    }
}
