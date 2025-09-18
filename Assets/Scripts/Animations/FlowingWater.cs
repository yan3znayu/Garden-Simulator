using UnityEngine;
using System.Collections;

public class FlowingWater : MonoBehaviour
{
    [Header("Flow Settings")]
    public bool isFlowing = false;
    public float flowSpeed = 0.3f;
    public float fadeDuration = 0.5f;
    public float maxWidth = 0.3f; 

    [Header("References")]
    public Renderer waterRenderer;
    public Transform waterStream; 

    private Material waterMaterial;
    private Coroutine flowCoroutine;
    private Vector3 initialScale;
    private float currentWidth;

    void Start()
    {
        waterMaterial = waterRenderer.material;
        initialScale = waterStream.localScale;

        currentWidth = 0f;
        UpdateStreamSize();

        SetAlpha(0f);
        waterRenderer.enabled = false;
    }

    void Update()
    {
        if (waterMaterial.color.a > 0.01f)
        {
            float offset = Time.time * flowSpeed;
            waterMaterial.mainTextureOffset = new Vector2(0, offset);
        }
    }

    public void ToggleWater(bool turnOn)
    {
        isFlowing = turnOn;

        if (flowCoroutine != null)
            StopCoroutine(flowCoroutine);

        flowCoroutine = StartCoroutine(FlowWater());
    }

    private IEnumerator FlowWater()
    {
        float targetAlpha = isFlowing ? 1f : 0f; //
        float targetWidth = isFlowing ? maxWidth : 0f;

        if (isFlowing)
        {
            waterRenderer.enabled = true;
        }

        float elapsedTime = 0f;
        float startAlpha = waterMaterial.color.a;
        float startWidth = currentWidth;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetAlpha(alpha);

            currentWidth = Mathf.Lerp(startWidth, targetWidth, t);
            UpdateStreamSize();

            yield return null;
        }

        SetAlpha(targetAlpha);
        currentWidth = targetWidth;
        UpdateStreamSize();

        if (!isFlowing)
        {
            yield return new WaitForSeconds(0.5f);
            waterRenderer.enabled = false;
        }
    }

    private void UpdateStreamSize()
    {
        waterStream.localScale = new Vector3(
            Mathf.Lerp(initialScale.x, maxWidth, currentWidth / maxWidth),
            initialScale.y, 
            Mathf.Lerp(initialScale.z, maxWidth, currentWidth / maxWidth)
        );
    }

    private void SetAlpha(float a)
    {
        Color c = waterMaterial.color;
        c.a = a;
        waterMaterial.color = c;
    }
}
