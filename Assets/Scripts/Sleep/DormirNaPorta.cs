
using UnityEngine;
using TMPro;
using System.Collections;

public class DormirNaPorta : MonoBehaviour
{
    public TextMeshProUGUI mensagem;
    public CanvasGroup telaPreta;
    public DayNightCycle cicloDiaNoite;

    private bool jogadorPerto = false;
    private bool dormindo = false;

    void Start()
    {
        mensagem.gameObject.SetActive(false);
        telaPreta.alpha = 0;


        mensagem.gameObject.SetActive(false);

        telaPreta.alpha = 0;

           RectTransform rt = telaPreta.GetComponent<RectTransform>();

            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            telaPreta.transform.SetAsLastSibling();

    }

    void Update()
    {
        // Mostra a mensagem somente durante a noite
        mensagem.gameObject.SetActive(
            jogadorPerto &&
            !dormindo &&
            cicloDiaNoite.EstaDeNoite
        );

        if (jogadorPerto && !dormindo)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (cicloDiaNoite.EstaDeNoite)
                {
                    StartCoroutine(FinalizarDia());
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            mensagem.gameObject.SetActive(false);
        }
    }

    IEnumerator FinalizarDia()
    {
        dormindo = true;
        mensagem.gameObject.SetActive(false);

        // Escurece a tela
        while (telaPreta.alpha < 1)
        {
            telaPreta.alpha += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        // Avança para a manhã
        cicloDiaNoite.AvancarParaManha();

        yield return new WaitForSeconds(1f);

        // Clareia a tela
        while (telaPreta.alpha > 0)
        {
            telaPreta.alpha -= Time.deltaTime;
            yield return null;
        }

        dormindo = false;
    }
}
