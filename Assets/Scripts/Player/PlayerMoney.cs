using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class PlayerMoney : MonoBehaviour
{
    public int currentMoney = 0; 
    public TextMeshProUGUI moneyText; 
    public Image coinIcon;      

    public static PlayerMoney Instance; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateMoneyUI(); 
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        moneyText = null;

        GameObject moneyDisplay = GameObject.Find("money");
        if (moneyDisplay != null)
        {
            moneyText = moneyDisplay.GetComponent<TextMeshProUGUI>();
            currentMoney = 100;
            UpdateMoneyUI();
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public bool TryRemoveMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    public void UpdateMoneyUI()
    {

        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }

        if (coinIcon != null)
        {
            coinIcon.enabled = currentMoney > 0;
        }
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}