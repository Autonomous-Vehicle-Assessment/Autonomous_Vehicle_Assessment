﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EngineModel))]
public class CarUserControl : MonoBehaviour
{
    private EngineModel Engine;               // Engine model
    private DataLogging dataLogger;

    [SerializeField] public TransferCase m_CurrentTransfercase;
    [SerializeField] public bool trackPoints;
    [SerializeField] public bool active;
    [SerializeField] public int renderTime = 1;

    private LineRenderer[] lineRenderers;
    private List<Vector3[]> points;

    private void Awake()
    {
        // get the controller
        Engine = GetComponent<EngineModel>();

        // DataLogger
        dataLogger = GetComponent<DataLogging>();
    }


    private void LateUpdate()
    {
        // pass the input to the car!
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float footbrake = Input.GetAxis("FootBrake");
        float handbrake = Input.GetAxis("HandBrake");

        if (Input.GetKeyDown(KeyCode.E)||Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (m_CurrentTransfercase == TransferCase.High)
            {
                m_CurrentTransfercase = TransferCase.Low;
            }
            else
            {
                m_CurrentTransfercase = TransferCase.High;
            }
        }
            
        Engine.currentTransferCase = m_CurrentTransfercase;

        Engine.UpdateState();

        if (Engine != null)
        {
            if (active) Engine.Move(h, v, footbrake, handbrake);
        }

        string s_Time = Time.time.ToString();
        string s_Velocity = Engine.speed.ToString();
        string s_EngineRPM = Engine.engineRPM.ToString();
        string s_EngineTorque = Engine.engineTorque.ToString();

        float m_TransmissionTorque = 0;
        float m_WheelForce = 0;

        for (int i = 0; i < Engine.wheels.Count; i++)
        {
            m_TransmissionTorque += Engine.wheels[i].collider.motorTorque;
            m_WheelForce += Engine.wheels[i].collider.motorTorque * Engine.wheels[i].collider.radius;
        }

        string s_WheelForce = m_WheelForce.ToString();
        string s_TransmissionTorque = m_TransmissionTorque.ToString();

        string s_CurrentGear = (Engine.currentGear + 1).ToString();

        // Log data
        if(dataLogger.isActiveAndEnabled) dataLogger.WriteToFile(s_Time + ";" + s_Velocity + ";" + s_WheelForce + ";" + s_TransmissionTorque + ";" + s_CurrentGear + ";" + s_EngineRPM + ";" + s_EngineTorque + "\n");


        
    }

    /// <summary>
    /// Draw lines from wheel centers.
    /// </summary>
    private void UpdateLineRenderer()
    {
        if (lineRenderers == null || points == null && trackPoints)
        {
            Color[] ColorArray = { new Color(1, 0, 0), new Color(1, 0, 0), new Color(0, 0, 1), new Color(0, 0, 1) };

            lineRenderers = new LineRenderer[4];
            points = new List<Vector3[]>();
            for (int i = 0; i < 4; i++)
            {
                lineRenderers[i] = Engine.wheels[i].collider.gameObject.AddComponent<LineRenderer>();
                lineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
                lineRenderers[i].material.color = ColorArray[i];
                lineRenderers[i].widthMultiplier = 0.02f;
                lineRenderers[i].positionCount = (int)(renderTime / Time.fixedDeltaTime);

                points.Add(new Vector3[(int)(renderTime / Time.fixedDeltaTime)]);
                for (int k = 0; k < points[i].Length; k++)
                {
                    points[i][k] = Engine.wheels[i].mesh.transform.position;
                }
                lineRenderers[i].SetPositions(points[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < points[i].Length - 1; k++)
                {
                    points[i][k] = points[i][k + 1];
                }
                points[i][points[i].Length - 1] = Engine.wheels[i].mesh.transform.position;
                lineRenderers[i].SetPositions(points[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                lineRenderers[i].enabled = trackPoints;
            }

        }        
    }
}