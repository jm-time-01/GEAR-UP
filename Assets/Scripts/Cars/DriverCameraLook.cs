
using UnityEngine;

public class DriverCameraLook : MonoBehaviour
{
    [Header("Configurações da câmera")]
    public float distanciaCamera = 6f;
    public float alturaCamera = 3f;
    public float alturaOlhar = 1.5f;
    public float deslocamentoLateral = 0f;

    [Header("Sensibilidade do mouse")]
    public float sensibilidadeX = 120f;
    public float sensibilidadeY = 100f;

    [Header("Limites verticais")]
    public float limiteVerticalMin = -20f;
    public float limiteVerticalMax = 60f;

    private Transform carro;
    private float yaw;
    private float pitch = 15f;

    void Awake()
    {
        carro = transform.parent;
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (carro == null) return;

        float mouseX = Input.GetAxis("Mouse X") *
                       sensibilidadeX * Time.deltaTime;

        float mouseY = Input.GetAxis("Mouse Y") *
                       sensibilidadeY * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(
            pitch,
            limiteVerticalMin,
            limiteVerticalMax
        );

        Quaternion rotacao = carro.rotation * Quaternion.Euler(pitch, yaw, 0f);

        Vector3 deslocamento =
            rotacao * Vector3.back * distanciaCamera;

        Vector3 foco =
            carro.position +
            Vector3.up * alturaOlhar -
            carro.right * deslocamentoLateral;

        transform.position =
            carro.position +
            deslocamento +
            Vector3.up * alturaCamera -
            carro.right * deslocamentoLateral;

        transform.LookAt(foco);

        transform.LookAt(
            carro.position + Vector3.up * alturaOlhar
        );
    }
}
