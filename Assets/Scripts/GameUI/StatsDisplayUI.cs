using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatsDisplayUI : MonoBehaviour
{
    [Header("UI Элементы")]
    public GameObject statisticsPanel;
    public TextMeshProUGUI tomatoSoldText;
    public TextMeshProUGUI cabbageSoldText;
    public TextMeshProUGUI potatoSoldText;
    public TextMeshProUGUI eggplantSoldText;
    public TextMeshProUGUI pepperSoldText;
    public TextMeshProUGUI totalEarningsText;
    
    public delegate void StatisticsUIAction();
    public event StatisticsUIAction OnStatisticsOpened;
    public event StatisticsUIAction OnStatisticsClosed;
    
    [Header("Настройки")]
    public bool pauseGameWhenOpen = false;
    
    private bool isOpen = false;
    private float previousTimeScale;
    private PlayerController playerController;
    
    private void Start()
    {
        if (statisticsPanel != null)
        {
            statisticsPanel.SetActive(false);
        }
        
        playerController = FindObjectOfType<PlayerController>();
        

        MysticalBox[] mysticalBoxes = FindObjectsOfType<MysticalBox>();
        foreach (var box in mysticalBoxes)
        {
            box.statsDisplayUI = this;
        }
    }
    
    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Q))
        {
            Hide();
        }
    }
    
    public void Show(Dictionary<SeedBag.PlantType, int> soldCrops, int totalEarnings)
    {
        if (statisticsPanel != null)
        {
            UpdateStatisticsDisplay(soldCrops, totalEarnings);
            
            statisticsPanel.SetActive(true);
            isOpen = true;
            
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            
            if (pauseGameWhenOpen)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            
            OnStatisticsOpened?.Invoke();
        }
    }
    
    public void Hide()
    {
        if (statisticsPanel != null)
        {
            statisticsPanel.SetActive(false);
            isOpen = false;
            
            if (playerController != null)
            {
                playerController.enabled = true;
            }
            
            if (pauseGameWhenOpen && Time.timeScale == 0f)
            {
                Time.timeScale = previousTimeScale;
            }
            
            OnStatisticsClosed?.Invoke();
        }
    }
    
    public bool IsOpen()
    {
        return isOpen;
    }
    
    private void UpdateStatisticsDisplay(Dictionary<SeedBag.PlantType, int> soldCrops, int totalEarnings)
    {
        if (tomatoSoldText != null)
            tomatoSoldText.text = $"Продано помидор: {soldCrops[SeedBag.PlantType.Tomato]}";
        
        if (cabbageSoldText != null)
            cabbageSoldText.text = $"Продано капусты: {soldCrops[SeedBag.PlantType.Cabage]}";
        
        if (potatoSoldText != null)
            potatoSoldText.text = $"Продано картофеля: {soldCrops[SeedBag.PlantType.Potato]}";
        
        if (eggplantSoldText != null)
            eggplantSoldText.text = $"Продано баклажанов: {soldCrops[SeedBag.PlantType.Eggplant]}";
        
        if (pepperSoldText != null)
        {
            int totalPeppers = 
                soldCrops[SeedBag.PlantType.PepperRed] + 
                soldCrops[SeedBag.PlantType.PepperGreen] + 
                soldCrops[SeedBag.PlantType.PepperYellow];
            
            pepperSoldText.text = $"Продано перцев: {totalPeppers}";
        }
        
        if (totalEarningsText != null)
            totalEarningsText.text = $"Всего заработано: {totalEarnings} тёмуксов";
    }
}
