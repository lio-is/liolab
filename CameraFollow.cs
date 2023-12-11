using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera follow script: attached to Camera1
/* Camera1: MainCamera, set as display1
 *   - Implementation: Script controls camera follow range, but is somewhat rigid
 * 
 * 
 * Camera2: CM vcam1 virtual camera under Player, set as display2
 *   - Implementation: Dynamic camera follow with better effects
 *   
 *   ------------------------------------
 *   - Issue 1-1: Visible white vertical lines appear during movement, especially in boss levels (multiple player collisions)
 *   - Problem Analysis: The white lines are the BackGround color under SolidClear, possibly caused by gaps between Tilemaps
 *   - Solution 1: Change the "pixels per unit" property of artistic resources to 99
 *   - Result 1: Ineffective, bug persists
 *   - Solution 2: Change ClearFlags mode in MainCamera to DontClear instead of SolidClear
 *   - Result 2: Indeed, no more visible white lines; DontClear mechanism: each frame is drawn on top of the next frame, creating a smear effect
 *   - Issue 1-2: After interpolating CameraFollow, white lines still appear; likely a Tile drawing issue, not a Camera problem
 *   
 *   ------------------------------------
 *   - Issue 2: Camera direction becomes -90 when the player dies and respawns
 *   - Problem Analysis: When the player dies, the rotation.z becomes -90, causing the following virtual camera to deviate
 *   - Solution: Place the virtual camera under the GameManager object
 */

public class CameraFollow : MonoBehaviour
{
    private Transform lookAt;           // Target for camera follow (Player)
    public float boundX = 0.3f;         // X-axis difference range
    public float boundY = 0.15f;        // Y-axis difference range

    private Vector3 delts = Vector3.zero;
    private Vector3 destination;

    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        // Movement difference (delts):
        // Prevents the camera from following when the player moves within a certain range, but follows when exceeding that range
        // In other words: based on the distance between the camera and the player, determine whether to follow the movement
        delts = Vector3.zero;

        // X-axis difference: If the distance between the camera and the player exceeds the range, assign a value to the difference (delts)
        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
                delts.x = deltaX - boundX;
            else
                delts.x = deltaX + boundX;
        }

        // Y-axis difference: If the distance between the camera and the player exceeds the range, assign a value to the difference (delts)
        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
                delts.y = deltaY - boundY;
            else
                delts.y = deltaY + boundY;
        }

        delts.z = 0f;

        // New method: Smooth transition with interpolation
        destination = Vector3.Lerp(transform.position, transform.position + delts, 0.2f);

        /* Potential issue here: camera rendering layer confusion
         * Problem Analysis: The following code should set z<0; otherwise, z=0 will be on the same plane as elements like the map, causing rendering order issues
         * Solution: Pay attention to camera rendering: transform.z<0 and depth=-1
         */
        destination.z = -1f;

        // Old method: Set the camera's new position without smooth interpolation
        // transform.position += new Vector3(delts.x, delts.y, 0);
        transform.position = destination;
    }

    private void LateUpdate()
    {
        // transform.position = destination;
    }
}


