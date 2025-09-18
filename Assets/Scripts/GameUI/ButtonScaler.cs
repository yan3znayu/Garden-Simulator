using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки масштабирования")]
    [Tooltip("Начальный размер кнопки (1 = оригинальный размер).")]
    [SerializeField] private float originalScale = 1f;

    [Tooltip("Размер кнопки при наведении (1.1 = на 10% больше).")]
    [SerializeField] private float hoveredScale = 1.1f;

    [Tooltip("Скорость анимации масштабирования.")]
    [SerializeField] private float scaleSpeed = 10f;

    private Vector3 targetScale;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            enabled = false;
        }

        rectTransform.localScale = new Vector3(originalScale, originalScale, originalScale);
        targetScale = rectTransform.localScale;
    }

    void Update()
    {
        if (rectTransform.localScale != targetScale)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = new Vector3(hoveredScale, hoveredScale, hoveredScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = new Vector3(originalScale, originalScale, originalScale);
    }
}