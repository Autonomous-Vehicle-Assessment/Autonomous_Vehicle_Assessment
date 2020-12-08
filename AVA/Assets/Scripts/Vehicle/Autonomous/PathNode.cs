﻿using UnityEngine;
using System;

public enum LeaderFollowerMode
{
    Column,
    Diamond,
    SideBySide
}

public class PathNode : MonoBehaviour
{
    public float targetVelocity;
    [HideInInspector]
    public bool activeNode;
    [HideInInspector]
    public float targetHeight = 1;
    private float deltaHeight;
    public SpeedType speedType;

    [Header("Events")]
    public Transform eventObject;
    public bool objectState = true;
    public Vector3 movePosition;
    public Vector3 moveOrientation;


    [Header("Leader Follower TBD")]
    public WaypointGenerator[] leaderFollowers;
    public EngineModel[] leaders;
    public bool leaderActive;
    public LeaderFollowerMode leaderFollowerMode;

    [Header("Waypoint Driver TBD")]
    public WaypointController driver;
    public bool driverActive;
    public bool skipPath;
    public bool uAVThreat;

    public void SetHeight()
    {
        LayerMask mask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 250, mask))
        {
            deltaHeight = targetHeight + transform.InverseTransformPoint(hit.point).y;

            Vector3 newPos = new Vector3(transform.position.x, transform.position.y + deltaHeight, transform.position.z);
            transform.position = newPos;

            //Gizmos.DrawLine(transform.position, hit.point);
        }
    }

    public void RecalculateSpeed()
    {
        targetVelocity = targetVelocity * GenericFunctions.SpeedCoefficient(speedType);
    }

    public void WayPointEvent()
    {
        if (eventObject != null)
        {
            eventObject.gameObject.SetActive(objectState);
            eventObject.position = eventObject.TransformPoint(movePosition);
            eventObject.rotation = eventObject.rotation * Quaternion.Euler(moveOrientation);
        }

        if (leaderFollowers != null)
        {
            // Set leader follower state and mode
            switch (leaderFollowerMode)
            {
                case LeaderFollowerMode.Column:
                    break;
                case LeaderFollowerMode.Diamond:
                    Vector3[] diamondFormation = { new Vector3(-5,0,-1), new Vector3(5, 0, -1), new Vector3(0, 0, -5) };
                    int index = 0;
                    foreach (WaypointGenerator waypointGenerator in leaderFollowers)
                    {
                        waypointGenerator.leader = leaders[index];
                        waypointGenerator.offset = diamondFormation[index];
                        index++;
                    }
                    break;
                case LeaderFollowerMode.SideBySide:
                    break;
                default:
                    break;
            }

        }
        if (driver != null)
        {
            if (uAVThreat)
            {
                // Skip to UAV alternate return path   
            }

            if (skipPath)
            {
                // Skip to next path
                // Set current node to 0
                // Update active waypoint
                // Deactivate previous point
            }
        }


    }

    public void Activate()
    {
        activeNode = true;
        WayPointEvent();
    }

}
