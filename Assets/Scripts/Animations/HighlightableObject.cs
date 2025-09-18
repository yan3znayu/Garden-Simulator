using UnityEngine;
using System.Collections.Generic;

public class HighlightableObject : MonoBehaviour
{
    [Tooltip("Материал, который будет использоваться для подсветки.")]
    public Material highlightMaterial;

    private Renderer targetRenderer;
    private List<Material> originalMaterials = new List<Material>();
    private bool isHighlighted = false;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            enabled = false;
            return;
        }

        if (highlightMaterial == null)
        {
            enabled = false;
            return;
        }

        foreach (Material mat in targetRenderer.sharedMaterials)
        {
            originalMaterials.Add(mat);
        }

        Unhighlight();
    }

    public void Highlight()
    {
        if (!isHighlighted)
        {
            Material[] highlightMaterialsArray = new Material[originalMaterials.Count];
            for (int i = 0; i < highlightMaterialsArray.Length; i++)
            {
                highlightMaterialsArray[i] = highlightMaterial;
            }
            targetRenderer.materials = highlightMaterialsArray;
            isHighlighted = true;
        }
    }

    public void Unhighlight()
    {
        if (isHighlighted)
        {
            targetRenderer.materials = originalMaterials.ToArray();
            isHighlighted = false;
        }
    }

    void OnApplicationQuit()
    {
        if (targetRenderer != null && originalMaterials.Count > 0)
        {
            targetRenderer.materials = originalMaterials.ToArray();
        }
    }
}