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
    static float AGENT_HEIGHT_OFFSET = 1f;

    // Variables that Govern Agent Movement
    private bool userActive = true;
    public enum zoneState
    {
        intimateZone = 0,
        casualZone = 1,
        socialZone = 2,
        publicZone = 3
    };
    public struct ZonePolarCoord
    {
        public float angle;
        public float radius;
        public float minRad;
        public float maxRad;

        public ZonePolarCoord(float angle, float radius, float minRad, float maxRad)
        {
            this.angle = angle;
            this.radius = radius;
            this.minRad = minRad;
            this.maxRad = maxRad;
        }
    }
    Dictionary<zoneState, ZonePolarCoord> activeZoneCoords = new();
    Dictionary<zoneState, ZonePolarCoord> inactiveZoneCoords = new();
    private zoneState inactiveState = zoneState.socialZone;
    private zoneState activeState = zoneState.intimateZone;
    private bool canMove = true;

    // Start is called before the first frame update
    void Awake()
    {
        agentBody = GetComponent<Rigidbody>();

        activeZoneCoords[zoneState.intimateZone] = new ZonePolarCoord(90f, 2f, 1.5f, 2.5f);
        activeZoneCoords[zoneState.casualZone] = new ZonePolarCoord(90f, 3.5f, 2.5f, 4.5f);
        activeZoneCoords[zoneState.socialZone] = new ZonePolarCoord(90f, 8f, 4.5f, 12f);
        activeZoneCoords[zoneState.publicZone] = new ZonePolarCoord(90f, 12f, 12f, 20f);

        inactiveZoneCoords[zoneState.intimateZone] = new ZonePolarCoord(90f, 2f, 1.5f, 2.5f);
        inactiveZoneCoords[zoneState.casualZone] = new ZonePolarCoord(90f, 3.5f, 2.5f, 4.5f);
        inactiveZoneCoords[zoneState.socialZone] = new ZonePolarCoord(90f, 8f, 4.5f, 12f);
        inactiveZoneCoords[zoneState.publicZone] = new ZonePolarCoord(90f, 12f, 12f, 20f);
    }


    // Test rotate vars
    // float radius = 5f;
    // float angle = 90f;
    float rotationSpeed = 45f;

    // Update is called once per frame
    void Update()
    {
        // determine current state, get the current dictionary, & then polar coords
        zoneState currentState = userActive ? activeState : inactiveState;
        Dictionary<zoneState, ZonePolarCoord> currentDic = userActive ? activeZoneCoords : inactiveZoneCoords;
        ZonePolarCoord polar = currentDic[currentState];

        // get a center point to reference 
        Vector3 centerPoint = centerPlane.transform.position;
        centerPoint.y = AGENT_HEIGHT_OFFSET;


        // rotate the agent 
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            polar.angle -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            polar.angle += rotationSpeed * Time.deltaTime;
        }

        // prevents looping over/under 360 degrees
        polar.angle = (polar.angle + 360f) % 360;
        currentDic[currentState] = polar;

        // float rad = angle * Mathf.Deg2Rad;
        float rad = polar.angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * polar.radius;


        // positions[(int)currentState] = centerPoint + offset;
        transform.position = centerPoint + offset;
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
            // positions[(int)currentState] = positions[(int)currentState];
        }

        // get the object to look at the center point no matter where we move it in zones
        // gravity is off and y-position locked so we don't move at all in y-axis
        this.transform.LookAt(centerPoint);
    }

    void FixedUpdate()
    {
        // no need to use (I think)
    }

    // take hand gesture input and change state from active -> inactive OR inactive -> active
    void changeState()
    {
        userActive = !userActive;
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
