using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionZones : MonoBehaviour
{
    // GameObject Agent;
    Rigidbody agentBody;
    public GameObject centerPlane;

    // Variables that Govern Agent Movement
    private bool userActive = true;
    public enum zoneState
    {
        intimateZone = 0,
        casualZone = 1,
        socialZone = 2,
        publicZone = 3
    };
    private zoneState inactiveState = zoneState.socialZone;
    private zoneState activeState = zoneState.intimateZone;
    Vector3[] positions = new Vector3[4];
    Vector3[] customPositions = new Vector3[4];
    
    private bool canMove = true;
    // Start is called before the first frame update
    void Awake()
    {
        agentBody = GetComponent<Rigidbody>();
        positions[0] = new Vector3(0, 1, 2);
        positions[1] = new Vector3(0, 1, 3.5f);
        positions[2] = new Vector3(0, 1, 8);
        positions[3] = new Vector3(0, 1, 12);
        customPositions = positions;
    }

    // Update is called once per frame
    void Update()
    {
        // get the object to look at the center point no matter where we move it in zones
        // gravity is off and y-position locked so we don't move at all in y-axis
        Vector3 centerPoint = centerPlane.transform.position;
        centerPoint.y = this.transform.position.y;
        this.transform.LookAt(centerPoint);

        // // if we do four hand poses then we can do this easily 
        // // up down zones would be much harder or use way more if's
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     agentBody.detectCollisions = false;
        //     agentBody.position = customPositions[0];
        //     agentBody.detectCollisions = true;
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     agentBody.detectCollisions = false;
        //     agentBody.position = customPositions[1];
        //     agentBody.detectCollisions = true;
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     agentBody.detectCollisions = false;
        //     agentBody.position = customPositions[2];
        //     agentBody.detectCollisions = true;
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     agentBody.detectCollisions = false;
        //     agentBody.position = customPositions[3];
        //     agentBody.detectCollisions = true;
        // }

        zoneState currentState = userActive ? activeState : inactiveState;
        transform.position = positions[(int)currentState];  
        // above will seemingly stay since key inputs will be phased out by hand inputs

        // Cycle up
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (userActive)
                IncrementZone(ref activeState);
            else
                IncrementZone(ref inactiveState);
        }

        // Cycle down
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (userActive)
                DecrementZone(ref activeState);
            else
                DecrementZone(ref inactiveState);
        }

        // Toggle active/inactive with spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            userActive = !userActive;
        }

    }

    void FixedUpdate()
    {
        // used for physics movements
        // Vector3 center = centerPlane.transform.position;
        // Vector3 direction = (transform.position - center).normalized; // away from plane center
        // agentBody.velocity = direction * forceStrength; 
    }

    // take hand gesture input and change state from active -> inactive OR inactive -> active
    void changeState()
    {
        userActive = !userActive;
        
        // intantly parse current state after change then move to that position
        zoneState currentState = userActive ? activeState : inactiveState;
        transform.position = positions[(int)currentState];
    }

    
    // On Hand Input Make the Agent Change Zone Up/Down
    void changeZoneUp()
    { 
        if (userActive) 
            IncrementZone(ref activeState);
        else
            IncrementZone(ref inactiveState);
    }

    void changeZoneDown()
    { 
        if (userActive)
            DecrementZone(ref activeState);
        else
            DecrementZone(ref inactiveState);
    }

    // will be used to toggle agent move mode
    // either two hand signs to enter or one to toggle
    void allowMove(bool allowMove)
    {
        canMove = allowMove;
    }

    void directionMove(string directionMove)
    {

    }

    // Helpers to change Enums up or Down
    void IncrementZone(ref zoneState state)
    {
        int next = (int)state + 1;
        int max = System.Enum.GetValues(typeof(zoneState)).Length - 1;

        if (next <= max)
        {
            state = (zoneState)next;
        }
    }

    void DecrementZone(ref zoneState state)
    {
        int prev = (int)state - 1;

        if (prev >= 0)
        {
            state = (zoneState)prev;
        }
    }
}
