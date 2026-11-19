
using UnityEngine;

public class ControleCarro : MonoBehaviour
{
    [Header("Referências")]
    public Rigidbody rb;
    public Transform centerOfMass;

    [Header("Wheel Colliders")]
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public WheelCollider WheelRL;
    public WheelCollider WheelRR;

    [Header("Rodas Visuais")]
    public Transform meshFL;
    public Transform meshFR;
    public Transform meshRL;
    public Transform meshRR;

    [Header("Motor")]
    public float motorTorque = 1500f;
    public float reverseTorque = 800f;

    [Header("Direção")]
    public float maxSteerAngle = 30f;

    [Header("Freios")]
    public float brakeTorque = 3500f;
    public float handBrakeTorque = 5000f;

    [Header("Velocidade")]
    public float maxSpeed = 120f;

    public float CurrentSpeedKmh { get; private set; }
    public bool HandbrakeEngaged { get; private set; }

    private float verticalInput;
    private float horizontalInput;
    private bool playerInside = false;

    void Start()
    {
        if (rb != null && centerOfMass != null)
            rb.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
    }

    void Update()
    {
        if (playerInside)
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
            HandbrakeEngaged = Input.GetKey(KeyCode.Space);
        }
        else
        {
            verticalInput = 0f;
            horizontalInput = 0f;
            HandbrakeEngaged = false;
        }

        UpdateWheelVisuals();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (!playerInside)
        {
            WheelFL.motorTorque = 0f;
            WheelFR.motorTorque = 0f;
            WheelRL.motorTorque = 0f;
            WheelRR.motorTorque = 0f;

            WheelFL.brakeTorque = brakeTorque;
            WheelFR.brakeTorque = brakeTorque;
            WheelRL.brakeTorque = brakeTorque;
            WheelRR.brakeTorque = brakeTorque;

            return;
        }

        CalculateSpeed();
        HandleSteering();
        HandleEngine();
        HandleHandbrake();
    }

    public void SetPlayerInside(bool estado)
    {
        playerInside = estado;

        if (estado)
        {
            WheelFL.brakeTorque = 0f;
            WheelFR.brakeTorque = 0f;
            WheelRL.brakeTorque = 0f;
            WheelRR.brakeTorque = 0f;
        }
        else
        {
            StopCar();
        }
    }

    public void StopCar()
    {
        playerInside = false;
        verticalInput = 0f;
        horizontalInput = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        WheelFL.motorTorque = 0f;
        WheelFR.motorTorque = 0f;
        WheelRL.motorTorque = 0f;
        WheelRR.motorTorque = 0f;

        WheelFL.brakeTorque = brakeTorque;
        WheelFR.brakeTorque = brakeTorque;
        WheelRL.brakeTorque = brakeTorque;
        WheelRR.brakeTorque = brakeTorque;
    }

    void CalculateSpeed()
    {
        CurrentSpeedKmh = rb.linearVelocity.magnitude * 3.6f;
    }

    void HandleSteering()
    {
        float steerAngle = horizontalInput * maxSteerAngle;

        WheelFL.steerAngle = steerAngle;
        WheelFR.steerAngle = steerAngle;
    }

    void HandleEngine()
    {
        float localSpeed = transform.InverseTransformDirection(rb.linearVelocity).z;

        WheelFL.motorTorque = 0f;
        WheelFR.motorTorque = 0f;

        WheelFL.brakeTorque = 0f;
        WheelFR.brakeTorque = 0f;
        WheelRL.brakeTorque = 0f;
        WheelRR.brakeTorque = 0f;

        if (verticalInput > 0 && CurrentSpeedKmh < maxSpeed)
        {
            WheelFL.motorTorque = verticalInput * motorTorque;
            WheelFR.motorTorque = verticalInput * motorTorque;
        }

        if (verticalInput < 0)
        {
            if (localSpeed > 1f)
            {
                ApplyBrake();
            }
            else
            {
                WheelFL.motorTorque = verticalInput * reverseTorque;
                WheelFR.motorTorque = verticalInput * reverseTorque;
            }
        }
    }

    void ApplyBrake()
    {
        WheelFL.brakeTorque = brakeTorque;
        WheelFR.brakeTorque = brakeTorque;
        WheelRL.brakeTorque = brakeTorque;
        WheelRR.brakeTorque = brakeTorque;
    }

    void HandleHandbrake()
    {
        if (HandbrakeEngaged)
        {
            WheelRL.brakeTorque = handBrakeTorque;
            WheelRR.brakeTorque = handBrakeTorque;
        }
    }

    void UpdateWheelVisuals()
    {
        UpdateWheel(WheelFL, meshFL);
        UpdateWheel(WheelFR, meshFR);
        UpdateWheel(WheelRL, meshRL);
        UpdateWheel(WheelRR, meshRR);
    }

    void UpdateWheel(WheelCollider collider, Transform wheelMesh)
    {
        if (collider == null || wheelMesh == null) return;

        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }
}
