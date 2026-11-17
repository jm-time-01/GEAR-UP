using System.Text.RegularExpressions;
using UnityEngine;

public class Carro : MonoBehaviour
{
    public WheelCollider[] guiar;
    public bool playerControlled = false;


    public Rigidbody rb;
    public Transform centerOfMass;

    // Estes, não aparecem visualmente mas
    // serão responsáveis pela aceleração, freio, direção, etc.
    [Header("Wheel Collider/Colisores das Rodas")]
    public WheelCollider WheelFE;
    public WheelCollider WheelFD;
    public WheelCollider WheelTE;
    public WheelCollider WheelTD;


    [Header("Rodas Visuais")]
    public Transform meshFE;
    public Transform meshFD;
    public Transform meshTE;
    public Transform meshTD;


    [Header("Motor")]
    public float motorTorque = 1500f;
    public float reverseTorque = 800f;

    // Ângulo máximo que as rodas da frente devem girar.
    [Header("Direção")]
    public float maxSteerAngle = 300f;

    [Header("Freios")]
    public float brakeTorque = 3500f;
    public float handBrakeTorque = 5000f;


    float gui = 0f;
    float acc = 0f;

    public bool HandbrakeEngaged { get; private set; }
    private float horizontalInput;

    public float forca;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);


    }

    private void Update()
    {
        if (playerControlled)
        {
            gui = Input.GetAxis("Horizontal");
            acc = Input.GetAxis("Vertical");

            horizontalInput = gui;

            HandbrakeEngaged = Input.GetKey(KeyCode.Space);
        }
        else
        {
            gui = 0f;
            acc = 0f;
            horizontalInput = 0f;
            HandbrakeEngaged = false;
        }

        UpdateWheelVisuals();
    }

    void FixedUpdate()
    {
/*        for (int i = 0; i < guiar.Length; i++) 
        {
            guiar[i].steerAngle = gui * 15f;
            guiar[i].motorTorque = 1f;
        }*/

        rb.AddForce(transform.forward * forca * acc);

        HandleSteering();
        HandleEngine();
        HandleHandbrake();


        UpdateWheelVisuals();

    }

    // Método que atribui ângulo para as rodas.
    void HandleSteering()
    {

        // Ciamos uma variável chamada steerAAngle para
        // pegar o valor do horizontalInput (eixo X) e
        // multiplicar por 30 (valor do maxSteerAngle).
        float steerAngle = horizontalInput * maxSteerAngle;

        // Atribui para cada collider o ângulo
        // de inclinação das rodas.
        WheelFE.steerAngle = steerAngle;
        WheelFD.steerAngle = steerAngle;

    }


    void HandleEngine()
    {
        // Zera os freios inicialmente
        WheelFE.brakeTorque = 0;
        WheelFD.brakeTorque = 0;
        WheelTE.brakeTorque = 0;
        WheelTD.brakeTorque = 0;

        // Aplica força nas rodas dianteiras
        WheelFE.motorTorque = acc * motorTorque;
        WheelFD.motorTorque = acc * motorTorque;

        // Se estiver andando para trás, o torque será negativo
    }




    void HandleHandbrake()
    {

        if (HandbrakeEngaged)
        {

            WheelTE.brakeTorque = handBrakeTorque;
            WheelTD.brakeTorque = handBrakeTorque;

        }

    }

    void UpdateWheelVisuals()
    {
        // Chamar um método chamado UpdateWheels.
        UpdateWheel(WheelFE, meshFE);
        UpdateWheel(WheelFD, meshFD);
        UpdateWheel(WheelTE, meshTE);
        UpdateWheel(WheelTD, meshTD);

    }


    // criando 2 variáveis que recebem valores do tipo WheelCollider
    // e Transform.
    void UpdateWheel(WheelCollider collider, Transform wheelMesh)
    {

        // GetWorldPode permite descobrir exatamente 
        // onde está esse collider no mundo físico da minha cena.
        collider.GetWorldPose(
            // "out"significa que a função irá preencher
            // essas variáveis (posição e rotação) para nós.
            out Vector3 position,
            // o Quaternion é um tipo usado pela Unity para representar rotações.
            out Quaternion rotation);

        // Atribuo a posição e rotação das rodas físicas para as do collider.
        wheelMesh.position = position;
        wheelMesh.rotation = rotation;




    }



}
