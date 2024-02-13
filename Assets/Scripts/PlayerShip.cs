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
    public Color _jetStartColor;

    //Jet color when JetBoost is active
    [SerializeField]
    public Color _jetBoostColor;

    //Manage size of visual effects for each jet
    [SerializeField]
    Vector3 _mainJetBoostSize; //Size when boosted (decided by designer)
    [SerializeField]
    Vector3 mainJetStartSize; //Main Jet Size by default
    [SerializeField]
    Vector3 _sideThrusterBoostSize; //Side Thruster size when boosted
    [SerializeField]
    Vector3 sideThrusterStartSize; //Side Thruster size by default
    
    //Set up Particle Systems for each jet
    public GameObject MainJet;
    public GameObject LeftJet;
    public GameObject RightJet;

    Rigidbody _rb = null; //Will be stored on Awake()

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>(); //Find the Rigidbody and store it as the thing
        MainJet = GameObject.Find("jetThruster_PS"); //Finds ParticleSystem on jetThruster_PS and rembers it
        LeftJet = GameObject.Find("leftThruster_PS"); //Finds leftThruster GameObject
        RightJet = GameObject.Find("rightThruster_PS"); //Finds rightThruster GameObject
        sideThrusterStartSize = LeftJet.GetComponent<Transform>().localScale;
        checkSpeed();
    }

    public void Update()
    {
        //transform.Rotate(0f, 1, 0f, Space.Self);  //This rotates my man to test particle animations

        if (GameManager.Instance.playerActive)
        {
            //Activates a speed up buff
            if (Input.GetKey(KeyCode.LeftShift)) {
                JetBoost(); //Changes particle color and increases speed
            }
            //Ends jet boost
            if (Input.GetKeyUp(KeyCode.LeftShift)) {
                JetBoostEnd(); //Returns particle effect to how it was
            }

            //Right Thruster Controls
            if (Input.GetKey(KeyCode.A)) {
                RightThruster(); //Activator
            }
            if (Input.GetKeyUp(KeyCode.A)) {
                RightThrusterEnd(); //Deactivator
            }

            //Left Thruster Controls
            if (Input.GetKey(KeyCode.D)) {
                LeftThruster(); //Activator
            }
            if (Input.GetKeyUp(KeyCode.D)) {
                LeftThrusterEnd(); //Deactivator
            }
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
                MainThruster();
            }
            // Anytime W is not held, the particle effect stops| There is probably a better way to do this
            if (!Input.GetKey(KeyCode.W))
            {
                MainThrusterEnd();
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

    void JetBoost()
    {
        jetBoostActive = true; //Used by other systems that check the state of jetBoost
        //Sets each jet to their respective boosted sizes
        MainJet.GetComponent<Transform>().localScale = _mainJetBoostSize;
        //THESE ARE ONLY SUPPOSED TO BE THE SIZES OF THE SIDE JETS WHILE A DIRECTIONAL KEY IS HELD
        RightJet.GetComponent<Transform>().localScale = _sideThrusterBoostSize;
        LeftJet.GetComponent<Transform>().localScale = _sideThrusterBoostSize;
        //Sets each jet to their respective boosted sizes
        MainJet.GetComponent<ParticleSystem>().startColor = _jetBoostColor;
        RightJet.GetComponent<ParticleSystem>().startColor = _jetBoostColor;
        LeftJet.GetComponent<ParticleSystem>().startColor = _jetBoostColor;
        RightThruster(); //Turns on right thruster
        LeftThruster(); //Turns on left thruster
        checkSpeed();
    }

    void JetBoostEnd()
    {
        jetBoostActive = false;
        //Sets jets back to original sizes
        MainJet.GetComponent<Transform>().localScale = mainJetStartSize;
        RightJet.GetComponent<Transform>().localScale = sideThrusterStartSize;
        LeftJet.GetComponent<Transform>().localScale = sideThrusterStartSize;
        //Sets jets back to original color
        MainJet.GetComponent<ParticleSystem>().startColor = _jetStartColor; 
        RightJet.GetComponent<ParticleSystem>().startColor = _jetStartColor;
        LeftJet.GetComponent<ParticleSystem>().startColor = _jetStartColor;

        //Turns side jets off
        if (!Input.GetKey(KeyCode.A)) {
            RightThrusterEnd();
        }
        if (!Input.GetKey(KeyCode.D)) {
            LeftThrusterEnd();
        }

        checkSpeed(); //Recalculate player speed
    }

    void MainThruster()
    {
        MainJet.GetComponent<ParticleSystem>().Play();
    }

    public void MainThrusterEnd()
    {
        MainJet.GetComponent<ParticleSystem>().Stop();
    }

    //Function that turns right thruster on
    void RightThruster() {
        RightJet.GetComponent<ParticleSystem>().Play();
    }

    //Function that turns right thruster off
    public void RightThrusterEnd() {
        RightJet.GetComponent<ParticleSystem>().Stop();
    }

    //Function that turns left thruster on
    void LeftThruster() {
        LeftJet.GetComponent<ParticleSystem>().Play();
    }
    //Function that turns left thruster off
    public void LeftThrusterEnd() {
        LeftJet.GetComponent<ParticleSystem>().Stop();
    }

    //Allows the player to be killed
    public void Kill()
    {
        this.gameObject.SetActive(false); //Delete Game Object
        GameManager.Instance.playerActive = false; //Remove Controls
    }

    //Called when player is hit by something
    public void Hurt(float damage)
    {
        playerHealth -= damage; //Reduce playerHealth by amount of damage the source deals
        Debug.Log("Player Hit.  Health Remaining: " + playerHealth);

        if (playerHealth <= 0) {
            Kill(); //Runs kill as a lose state
            GameManager.Instance.youLose(); //Informs player they lost.  Lmao
        }
    }

    //Runs calculation for movement speed based on multiple calculation
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
