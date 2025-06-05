using System;
using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
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
        customPositions[0] = new Vector3(0, 1, 2);
        customPositions[1] = new Vector3(0, 1, 3.5f);
        customPositions[2] = new Vector3(0, 1, 8);
        customPositions[3] = new Vector3(0, 1, 12);
    }


    // Test rotate vars
    // float radius = 5f;
    float angle = 90f;
    float rotationSpeed = 45f;

    // Update is called once per frame
    void Update()
    {
        // determine current state
        zoneState currentState = userActive ? activeState : inactiveState;
        Vector3 centerPoint = centerPlane.transform.position;
        centerPoint.y = this.transform.position.y;
        float radius = CalculateHypotenuse(transform.position.x, transform.position.z);

        // rotate the agent 
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            angle += rotationSpeed * Time.deltaTime;
        }

        if (angle < 0) angle += 360;
        if (angle > 360f) angle -= 360f;

        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;


        // customPositions[(int)currentState] = centerPoint + offset;
        transform.position = customPositions[(int)currentState];
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

        // reset current position
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            customPositions[(int)currentState] = positions[(int)currentState];
        }


        // get the object to look at the center point no matter where we move it in zones
        // gravity is off and y-position locked so we don't move at all in y-axis
        this.transform.LookAt(centerPoint);
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
        transform.position = customPositions[(int)currentState];
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

    float CalculateHypotenuse(float x, float z)
    {
        return Mathf.Sqrt(x * x + z * z);
    }
}
