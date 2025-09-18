using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    [SerializeField] private int requiredMoney = 100;
    [SerializeField] private string gateName = "Загадочные врата";

    [Header("UI References")]
    public TextMeshProUGUI gateHintText;
    public Image gateLockedImage;

    private bool playerIsInTrigger = false;

    void Start()
    {
        if (gateHintText != null)
        {
            gateHintText.text = "";
            gateHintText.gameObject.SetActive(false);
        }

        if (gateLockedImage != null)
        {
            gateLockedImage.gameObject.SetActive(false);
        }

        Collider gateCollider = GetComponent<Collider>();
        if (gateCollider != null && !gateCollider.isTrigger)
        {
            gateCollider.isTrigger = true;
        }
    }

    void Update()
    {
        if (playerIsInTrigger && Input.GetKeyDown(KeyCode.Q))
        {
            AttemptToEnterGate();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInTrigger = true;
            UpdateGateHintUI();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            HideGateHintUI();
        }
    }

    private void UpdateGateHintUI()
    {
        int playerCurrentMoney = PlayerMoney.Instance.currentMoney;

        if (playerCurrentMoney >= requiredMoney)
        {
            if (gateHintText != null)
            {
                gateHintText.text = $"Нажмите [Q], чтобы войти в \"{gateName}\"";
                gateHintText.color = Color.white;
                gateHintText.gameObject.SetActive(true);
            }
            if (gateLockedImage != null)
            {
                gateLockedImage.gameObject.SetActive(false);
            }
        }
        else
        {
            if (gateHintText != null)
            {
                gateHintText.text = "";
                gateHintText.gameObject.SetActive(false);
            }
            if (gateLockedImage != null)
            {
                gateLockedImage.gameObject.SetActive(true);
            }
        }
    }

    private void HideGateHintUI()
    {
        if (gateHintText != null)
        {
            gateHintText.text = "";
            gateHintText.gameObject.SetActive(false);
        }
        if (gateLockedImage != null)
        {
            gateLockedImage.gameObject.SetActive(false);
        }
    }

    private void AttemptToEnterGate()
    {
        if (PlayerMoney.Instance.currentMoney >= requiredMoney)
        {
            SceneManager.LoadScene("MenuScene");

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            HideGateHintUI();
        }
        else
        {
            UpdateGateHintUI();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 0, 1, 0.3f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}