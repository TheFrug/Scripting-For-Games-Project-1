using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // If we used GameObject instead of Transform, line 15 would have to say
    // "_objectToFollow.Transform.position;  Still works, but unnecessarily longer
    [SerializeField] Transform _objectToFollow = null;

    Vector3 _objectOffset;

    private void Awake()
    {
        // Create an offset between this position and object's position.  Now we know how far away these objects should be
        // this.transform.position is this object's position in relation to the objectToFollow on startup (can be changed in inspector)
        _objectOffset = this.transform.position - _objectToFollow.position;
    }

    // Happens after Update.  The Camera should move last
    private void LateUpdate()
    {
        // snaps it to a new position each LateUpdate call.  It uses the objectToFollow's continuous position and the Offset
        this.transform.position = _objectToFollow.position + _objectOffset;
    }
}
