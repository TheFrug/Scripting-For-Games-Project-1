using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //ensures that the GetComponent will always find this object/s Rigidbody
public class PlayerShip : MonoBehaviour
{
    //Maximum Health for Player
    public float playerHealth = 3;
    public GameObject explosionPrefab;

    //Set up movement speed variable
    [SerializeField]
    float _baseMoveSpeed = 12f;
    public float adjustedSpeed;
    //Set up turn speed variable
    [SerializeField]
    float _turnSpeed = 3f;
    float incumberence;

    //Set up connections to GameObjects that have Particle Systems attached to them

    //JetBoost Management
    private bool jetBoostActive = false;
    [SerializeField]
    private float _jetBoostSpeed;

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
    
    //Set up GameObjects for each jet. They must be transforms in order to access their localScale variables
    [SerializeField]
    public GameObject MainJet;
    [SerializeField]
    public GameObject LeftJet;
    [SerializeField]
    public GameObject RightJet;
    [SerializeField]
    public Renderer shieldVisual;

    //Set up particle systems for each jet
    ParticleSystem.MainModule MainJet_PS;
    ParticleSystem.MainModule LeftJet_PS;
    ParticleSystem.MainModule RightJet_PS;

    //PowerUp Manager
    //[HideInInspector]
    public float JetBoostRemaining = 0;

    public bool ShieldActive = false;
    [HideInInspector]
    public float ShieldTimeRemaining = 0;

    //Audio Setup
    public AudioSource audioSource;
    float thrusterBaseVolume = 0.25f;
    float thrusterBoostedVolume = 0.4f;

    public AudioClip Stunned;
    public AudioClip StunEnd;
    public AudioClip ShieldEnd;
    public AudioClip youWin;

    Rigidbody _rb = null; //Will be stored on Awake()

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();

        _rb = GetComponent<Rigidbody>(); //Find the Rigidbody and store it as the thing
        MainJet_PS = MainJet.GetComponent<ParticleSystem>().main;
        LeftJet_PS = LeftJet.GetComponent<ParticleSystem>().main;
        RightJet_PS = RightJet.GetComponent<ParticleSystem>().main;
        sideThrusterStartSize = LeftJet.GetComponent<Transform>().localScale;
        checkSpeed();
    }

    public void Update()
    {
        //transform.Rotate(0f, 1, 0f, Space.Self);  //This rotates my man to test particle animations
        
        if (ShieldActive)
        {
            checkShield();
        }

        if (GameManager.Instance.playerActive)
        {
            if (JetBoostRemaining > 0)
            {
                //Activates a speed up buff
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    JetBoost(); //Changes particle color and increases speed
                }
                //Ends jet boost
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    JetBoostEnd(); //Returns particle effect to how it was
                    RightThrusterEnd();
                    LeftThrusterEnd();
                }
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
        MainJet_PS.startColor = _jetBoostColor;
        RightJet_PS.startColor = _jetBoostColor;
        LeftJet_PS.startColor = _jetBoostColor;

        audioSource.volume = thrusterBoostedVolume;
        RightThruster(); //Turns on right thruster
        LeftThruster(); //Turns on left thruster
        checkSpeed(); //Adjust speed appropriately

        JetBoostRemaining -= Time.smoothDeltaTime;
        if (JetBoostRemaining <= 0)
        {
            Debug.Log("JetBoost Is Out");
            JetBoostEnd();
        }
    }

    void JetBoostEnd()
    {
        jetBoostActive = false;

        //Set jets back to orginial size and color
        MainJet.GetComponent<Transform>().localScale = mainJetStartSize;
        RightJet.GetComponent<Transform>().localScale = sideThrusterStartSize;
        LeftJet.GetComponent<Transform>().localScale = sideThrusterStartSize;

        MainJet_PS.startColor = _jetStartColor; 
        RightJet_PS.startColor = _jetStartColor;
        LeftJet_PS.startColor = _jetStartColor;

        //Turns side jets off
        if (Input.GetKeyUp(KeyCode.A)) {
            RightThrusterEnd();
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            LeftThrusterEnd();
        }

        audioSource.volume = thrusterBaseVolume; //Sets jet to be quieter
        checkSpeed(); //Recalculate player speed
    }

    void MainThruster()
    {
        MainJet.GetComponent<ParticleSystem>().Play();

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void MainThrusterEnd()
    {
        MainJet.GetComponent<ParticleSystem>().Stop();
        audioSource.Stop();
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
        Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
        foreach (Renderer renderer in gameObject.GetComponentsInChildren(typeof(Renderer)))
        {
            renderer.enabled = false;
        }
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
        incumberence = GameManager.Instance.winPoints * 0.5f;

        if (jetBoostActive) {
            adjustedSpeed = _baseMoveSpeed + _jetBoostSpeed - incumberence;
        }
        else {
            adjustedSpeed = _baseMoveSpeed - incumberence;
        }
    }

    //Runs every frame.  Might be scuffed.
    public void checkShield()
    {
        ShieldTimeRemaining -= Time.smoothDeltaTime;
        if (ShieldTimeRemaining <= 0)
        {
            ShieldActive = false;
            shieldVisual.enabled = false;
            audioSource.PlayOneShot(ShieldEnd);
        }
    }
}
