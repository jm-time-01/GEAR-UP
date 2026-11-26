
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sol;
    public Material skyboxMaterial;

    public float duracaoDoDia = 120f;

    public bool EstaDeNoite = false;

    private float tempo;
    private Material skyboxInstancia;

    void Start()
    {
        // Cria uma cópia do Skybox para não alterar o material original
        if (skyboxMaterial != null)
        {
            skyboxInstancia = new Material(skyboxMaterial);
            RenderSettings.skybox = skyboxInstancia;
        }
    }

    void Update()
    {
        tempo += Time.deltaTime;
        tempo %= duracaoDoDia;

        float angulo = (tempo / duracaoDoDia) * 360f;

        // Identifica se é dia ou noite
        EstaDeNoite = angulo >= 180f;

        sol.transform.rotation = Quaternion.Euler(
            angulo, 170f, 0f
        );

        // Intensidade da iluminação
        float intensidade = Mathf.Clamp01(
            Mathf.Sin(angulo * Mathf.Deg2Rad)
        );

        sol.intensity = intensidade * 1.2f;

        // Iluminação ambiente
        RenderSettings.ambientLight = Color.Lerp(
            new Color(0.04f, 0.05f, 0.12f),
            new Color(0.8f, 0.8f, 0.8f),
            intensidade
        );

        // Transição das cores do céu
        if (skyboxInstancia != null)
        {
            Color corDia = Color.white;
            Color corNoite = new Color(0.12f, 0.16f, 0.35f);

            Color corAtual = Color.Lerp(
                corNoite,
                corDia,
                intensidade
            );

            skyboxInstancia.SetColor("_Tint", corAtual);

            // Controla o brilho do céu
            float exposicao = Mathf.Lerp(
                0.2f,
                1f,
                intensidade
            );

            skyboxInstancia.SetFloat("_Exposure", exposicao);
        }
    }

    public void AvancarParaManha()
    {
        tempo = duracaoDoDia * 0.125f;
        EstaDeNoite = false;
    }
}
