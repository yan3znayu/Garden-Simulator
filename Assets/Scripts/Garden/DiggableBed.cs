using UnityEngine;
using System.Collections;

public class DiggableBed : MonoBehaviour, IInteractable
{
    public GameObject realBedPrefab;
    public float fadeInDuration = 1.0f;
    public float transitionDelay = 0.2f;

    private Renderer bedRenderer;
    private Collider bedCollider;
    private Material bedMaterial;
    private bool isTransitioning = false;

    void Start()
    {
        bedRenderer = GetComponent<Renderer>();
        bedCollider = GetComponent<Collider>();

        if (bedRenderer == null)
        {
            return;
        }

        bedMaterial = bedRenderer.material;
        Color invisibleColor = bedMaterial.color;
        invisibleColor.a = 0f;
        bedMaterial.color = invisibleColor;

        if (bedCollider != null)
        {
            bedCollider.isTrigger = true;
        }
    }

    public string GetInteractionHint()
    {
        if (isTransitioning) return "";

        var interactor = FindObjectOfType<Interactor>();
        if (interactor.HeldItem == null) return "";

        if (interactor.HeldItem.CompareTag("Shovel"))
        {
            return "Нажмите [E] чтобы вскопать грядку";
        }

        return "";
    }

    public void Interact()
    {
        if (isTransitioning) return;

        var interactor = FindObjectOfType<Interactor>();
        var heldItem = interactor.HeldItem;

        if (heldItem != null && heldItem.CompareTag("Shovel"))
        {
            StartCoroutine(DigBedAnimation());
        }
    }

    private IEnumerator DigBedAnimation()
    {
        isTransitioning = true;

        float elapsedTime = 0f;
        Color startColor = bedMaterial.color;
        Color targetColor = startColor;
        targetColor.a = 1f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeInDuration);

            bedMaterial.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        bedMaterial.color = targetColor;

        yield return new WaitForSeconds(transitionDelay);

        if (realBedPrefab != null)
        {
            Instantiate(realBedPrefab, transform.position, transform.rotation);

            Destroy(gameObject);
        }
        else
        {
            isTransitioning = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}
