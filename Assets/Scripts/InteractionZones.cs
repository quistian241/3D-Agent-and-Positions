using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionZones : MonoBehaviour
{
    // GameObject Agent;
    Rigidbody agentBody;
    public GameObject centerPlane;
    static float AGENT_HEIGHT_OFFSET = 1f;

    // movement constants
    float rotationSpeed = 45f;
    float moveSpeed = 2f;

    // Variables that Govern Agent Movement
    private bool userActive = false;
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
        public float zoneSpeed;

        public ZonePolarCoord(float angle, float radius, float minRad, float maxRad, float zoneSpeed)
        {
            this.angle = angle;
            this.radius = radius;
            this.minRad = minRad;
            this.maxRad = maxRad;
            this.zoneSpeed = zoneSpeed;
        }
    }
    Dictionary<zoneState, ZonePolarCoord> activeZoneCoords = new();
    Dictionary<zoneState, ZonePolarCoord> inactiveZoneCoords = new();
    private zoneState inactiveState = zoneState.socialZone;
    private zoneState activeState = zoneState.intimateZone;
    private bool canChangeZone = true;
    private bool canMove = false;
    private bool isMoving = false;

    Animator agentAnimator;

    // Start is called before the first frame update
    void Awake()
    {
        agentBody = GetComponent<Rigidbody>();

        activeZoneCoords[zoneState.intimateZone] = new ZonePolarCoord(90f, 2f, 1.5f, 2.5f, 1f);
        activeZoneCoords[zoneState.casualZone] = new ZonePolarCoord(90f, 3.5f, 2.5f, 4.5f, 1.5f);
        activeZoneCoords[zoneState.socialZone] = new ZonePolarCoord(90f, 8f, 4.5f, 12f, 2.5f);
        activeZoneCoords[zoneState.publicZone] = new ZonePolarCoord(90f, 12f, 12f, 20f, 4f);

        inactiveZoneCoords[zoneState.intimateZone] = new ZonePolarCoord(90f, 2f, 1.5f, 2.5f, 1f);
        inactiveZoneCoords[zoneState.casualZone] = new ZonePolarCoord(90f, 3.5f, 2.5f, 4.5f, 1.5f);
        inactiveZoneCoords[zoneState.socialZone] = new ZonePolarCoord(90f, 8f, 4.5f, 12f, 2.5f);
        inactiveZoneCoords[zoneState.publicZone] = new ZonePolarCoord(90f, 12f, 12f, 20f, 4f);

        agentAnimator = GetComponentInChildren<Animator>();
    }

    private Vector3 previousPosition;
    void Start()
    {
        previousPosition = transform.position;
    }

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

        // default unless moving
        isMoving = false;

        // rotate the agent 
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            polar.angle -= rotationSpeed * Time.deltaTime;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            polar.angle += rotationSpeed * Time.deltaTime;
            isMoving = true;
        }

        //move agent
        if (Input.GetKey(KeyCode.RightControl) && (polar.minRad < polar.radius))
        {
            polar.radius -= polar.zoneSpeed * Time.deltaTime;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.RightShift) && (polar.radius < polar.maxRad))
        {
            polar.radius += polar.zoneSpeed * Time.deltaTime;
            isMoving = true;
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

        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - previousPosition;

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
        if (isMoving)
        {
            movement.y = 0f;
            transform.rotation = Quaternion.LookRotation(movement.normalized);
            agentAnimator.SetFloat("isMoving", 1);
        }
        // nope, find another way like with a timer or something
        else
        {
            this.transform.LookAt(centerPoint);
            agentAnimator.SetFloat("isMoving", 0);

            if (userActive)
                agentAnimator.SetFloat("isActive", 1);
            else
                agentAnimator.SetFloat("isActive", 0);
        }

        previousPosition = currentPosition;
    }

    void FixedUpdate()
    {
        // no need to use (I think)
    }

    // take hand gesture input and change state from active -> inactive OR inactive -> active
    public void changeState()
    {
        userActive = !userActive;
        Debug.Log("User Active: " + userActive);
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
    public void toggleZoneMovement()
    {
        canMove = !canMove;
        canChangeZone = !canChangeZone;
        Debug.Log("Can Move: " + canMove + ", Can Change Zone: " + canChangeZone);
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