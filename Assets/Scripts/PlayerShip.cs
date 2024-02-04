using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //ensures that the GetComponent will always find this object/s Rigidbody
public class PlayerShip : MonoBehaviour
{
    [SerializeField]
    private GameInput _gameController;

    //Set up movement speed variable
    [SerializeField]
    float _moveSpeed = 12f;

    //Set up turn speed variable
    [SerializeField]
    float _turnSpeed = 3f;

    Rigidbody _rb = null; //Will be stored on Awake()

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>(); //Find the Rigidbody and store it as the thing
    }

    private void FixedUpdate()
    {
        MoveShip();
        TurnShip();
    }

    //use forces to build momentum forward/backward
    void MoveShip()
    {
        // S/Down = -1, W/Up = 1, None = 0.  Scale found direction with moveSpeed.
        // This is the final movement vector (force! like physics!!!!!!)
        float moveAmountThisFrame = Input.GetAxisRaw("Vertical") * _moveSpeed;
        // Combine direction with calculated amount
        Vector3 moveDirection = transform.forward * moveAmountThisFrame;
        // apply the movement to the physics object
        _rb.AddForce(moveDirection);
    }
    
    // Don't use force for this, since we want turning to be precise as fuck
    void TurnShip()
    {
        // A/Left = -1, D/Right = 1, None = 0.  Scale by turnSpeed
        // Store the vector of how the ship should turn within one frame
        float turnAmountThisFrame = Input.GetAxisRaw("Horizontal") * _turnSpeed;
        // Specify an axis to apply our turn amount (x,y,z) as a rotation
        Quaternion turnOffset = Quaternion.Euler(0, turnAmountThisFrame, 0);
        // spin the rigidbody
        _rb.MoveRotation(_rb.rotation * turnOffset);
    }

    public void Kill()
    {
        Debug.Log("Player has been killed!");
        this.gameObject.SetActive(false);
        _gameController.playerAlive = false;
    }
}
