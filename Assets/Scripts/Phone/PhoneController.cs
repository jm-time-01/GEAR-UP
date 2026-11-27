
using System;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
    [Header("Configuração do celular")]
    public RectTransform phone;

    public KeyCode phoneKey = KeyCode.Z;

    public float animationSpeed = 8f;

    private bool phoneOpen = false;

    private Vector2 closedPosition;
    private Vector2 openPosition;

    [Header("Painel de aplicativos")]
    public GameObject appsPanel;

    [Header("Telas dos aplicativos")]
    // Ordem: Shop, Sound, Phone, Minigame, Messages, Map
    public GameObject[] appScreens = new GameObject[6];

    private GameObject currentApp;

    public bool IsPhoneOpen => phoneOpen;

    public event Action<bool> OnPhoneStateChanged;

    private void Start()
    {
        if (phone == null)
        {
            Debug.LogError("RectTransform do celular não configurado!");
            return;
        }

        openPosition = phone.anchoredPosition;

        closedPosition = openPosition + new Vector2(0f, -700f);

        phone.anchoredPosition = closedPosition;

        LockCursor();
    }

    private void Update()
    {
        if (phone == null)
            return;

        if (Input.GetKeyDown(phoneKey))
        {
            TogglePhone();
        }

        Vector2 targetPosition;

        if (phoneOpen)
        {
            targetPosition = openPosition;
        }
        else
        {
            targetPosition = closedPosition;
        }

        phone.anchoredPosition = Vector2.Lerp(
            phone.anchoredPosition,
            targetPosition,
            animationSpeed * Time.deltaTime
        );
    }

    public void TogglePhone()
    {
        phoneOpen = !phoneOpen;

        if (phoneOpen)
        {
            UnlockCursor();
            ShowApps();
        }
        else
        {
            LockCursor();
            CloseAllApps();
        }

        OnPhoneStateChanged?.Invoke(phoneOpen);
    }

    public void ShowApps()
    {
        if (appsPanel != null)
            appsPanel.SetActive(true);

        CloseAllApps();
    }

    public void CloseAllApps()
    {
        if (appScreens == null)
            return;

        for (int i = 0; i < appScreens.Length; i++)
        {
            if (appScreens[i] != null)
                appScreens[i].SetActive(false);
        }

        currentApp = null;
    }

    public void OpenAppByIndex(int index)
    {
        if (appScreens == null)
            return;

        if (index < 0 || index >= appScreens.Length)
            return;

        if (appsPanel != null)
            appsPanel.SetActive(false);

        CloseAllApps();

        if (appScreens[index] != null)
        {
            appScreens[index].SetActive(true);
            currentApp = appScreens[index];
        }
    }

    public void OpenShop() => OpenAppByIndex(0);

    public void OpenSound() => OpenAppByIndex(1);

    public void OpenPhone() => OpenAppByIndex(2);

    public void OpenMinigame() => OpenAppByIndex(3);

    public void OpenMessages() => OpenAppByIndex(4);

    public void OpenMap() => OpenAppByIndex(5);

    public void BackToApps()
    {
        if (currentApp != null)
        {
            currentApp.SetActive(false);
            currentApp = null;
        }

        if (appsPanel != null)
            appsPanel.SetActive(true);
    }

    public void ClosePhone()
    {
        CloseAllApps();

        phoneOpen = false;

        if (phone != null)
            phone.anchoredPosition = closedPosition;

        LockCursor();

        OnPhoneStateChanged?.Invoke(phoneOpen);
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
