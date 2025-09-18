using UnityEngine;
using TMPro;

public class Interactor : MonoBehaviour
{
    private IInteractable currentInteractable;
    public TextMeshProUGUI interactionHintText;

    public Transform heldItemParent;
    private PickableItem heldItem;
    public PickableItem HeldItem
    {
        get { return heldItem; }
        set { heldItem = value; }
    }
    public float throwForce = 10f;

    private SeedTrade lastHoveredSeedShelf;
    private WaterContainer lastHoveredContainer;
    private GardenBed lastHoveredGardenBed;
    private PlantableBed lastHoveredPlantableBed;

    private HighlightableObject lastHighlightedObject; 
    void Start()
    {
        heldItemParent = new GameObject("HeldItemParent").transform;
        heldItemParent.SetParent(Camera.main.transform);
        heldItemParent.localPosition = new Vector3(3f, -2.2f, 10f);
        heldItemParent.localRotation = Quaternion.identity;
    }

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        HighlightableObject currentHighlightable = null;

        if (Physics.Raycast(ray, out hit, 21f))
        {
            MysticalBox mysticalBox = hit.collider.GetComponent<MysticalBox>();
            
            currentHighlightable = hit.collider.GetComponent<HighlightableObject>();
            DiggableBed diggableBed = hit.collider.GetComponent<DiggableBed>();
            PickableItem hoveredItem = hit.collider.GetComponent<PickableItem>();
            GardenBed gardenBed = hit.collider.GetComponent<GardenBed>();
            PlantableBed plantableBed = hit.collider.GetComponent<PlantableBed>();
            WaterTapInteraction waterTap = hit.collider.GetComponent<WaterTapInteraction>();
            SeedTrade seedShelf = hit.collider.GetComponent<SeedTrade>();

            lastHoveredContainer = hoveredItem ? hoveredItem.GetComponent<WaterContainer>() : null;
            lastHoveredGardenBed = gardenBed;
            lastHoveredPlantableBed = plantableBed;

            if (mysticalBox != null)
            {
                currentInteractable = mysticalBox;
            }
            else if (plantableBed != null)
            {
                currentInteractable = plantableBed;
            }
            else if (gardenBed != null)
            {
                currentInteractable = gardenBed;
            }
            else if (waterTap != null)
            {
                currentInteractable = waterTap;
            }
            else if (diggableBed != null)
            {
                currentInteractable = diggableBed;
            }
            else if (seedShelf != null)
            {
                currentInteractable = seedShelf;
                lastHoveredSeedShelf = seedShelf;
            }
            else if (hoveredItem != null) 
            {
                currentInteractable = hoveredItem; 
            }
            else
            {
                currentInteractable = null;
            }

            if (lastHighlightedObject != null && lastHighlightedObject != currentHighlightable)
            {
                lastHighlightedObject.Unhighlight();
                lastHighlightedObject = null;
            }

            if (currentHighlightable != null && currentHighlightable != lastHighlightedObject)
            {
                currentHighlightable.Highlight();
                lastHighlightedObject = currentHighlightable;
            }

            if (interactionHintText != null && currentInteractable != null)
            {
                interactionHintText.text = currentInteractable.GetInteractionHint();
            }
            else if (interactionHintText != null)
            {
                interactionHintText.text = "";
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (mysticalBox != null)
                {
                    mysticalBox.Interact();
                    return;
                }
                
                if (heldItem != null)
                {
                    if (plantableBed != null && heldItem.GetComponent<SeedBag>() != null)
                    {
                        plantableBed.Interact();
                        return;
                    }
                    else if (gardenBed != null && heldItem.GetComponent<WaterContainer>() != null)
                    {
                        gardenBed.Interact();
                        return;
                    }
                    else if (diggableBed != null && heldItem.GetComponent<Shovel>() != null)
                    {
                        diggableBed.Interact();
                        return;
                    }
                }

                if (waterTap != null)
                {
                    waterTap.Interact();
                }
                else if (currentInteractable != null && !(currentInteractable is PickableItem))
                {
                    currentInteractable.Interact();
                }
            }

           

            if (Input.GetKeyDown(KeyCode.B) && lastHoveredSeedShelf != null)
            {
                lastHoveredSeedShelf.Interact();
            }

            if (hoveredItem != null && !(currentInteractable is WaterTapInteraction) && Input.GetMouseButtonDown(0))
            {
                if (heldItem == null)
                {
                    PlantGrowth plant = hit.collider.GetComponentInParent<PlantGrowth>();
                    if (plant != null && plant.isReadyToHarvest)
                    {
                        PickableItem harvestedItem = plant.Harvest();
                        if (harvestedItem != null)
                        {
                            harvestedItem.Interact();
                        }
                        return;
                    }

                    hoveredItem.Interact();
                }
            }


        }
        else
        {
            currentInteractable = null;
            lastHoveredContainer = null;
            lastHoveredGardenBed = null;
            lastHoveredPlantableBed = null;
            if (interactionHintText != null)
            {
                interactionHintText.text = "";
            }
            lastHoveredSeedShelf = null;

            currentHighlightable = null;
            if (lastHighlightedObject != null)
            {
                lastHighlightedObject.Unhighlight();
                lastHighlightedObject = null;  

            }


        }

        if (heldItem != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                heldItem.transform.Rotate(Vector3.up, scrollInput * 50f);
            }

            if (Input.GetMouseButtonDown(1))
            {
                heldItem.Drop(Camera.main.transform.forward * throwForce);
                heldItem = null;
            }
        }
    }

    public void SetHeldItem(PickableItem item)
    {
        WaterTapInteraction[] taps = FindObjectsOfType<WaterTapInteraction>();
        foreach (WaterTapInteraction tap in taps)
        {
            tap.StopFilling();
        }

        heldItem = item;
        if (heldItem != null)
        {
            item.SetPickupParent(heldItemParent);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
    }

    void OnGUI()
    {
        if (lastHoveredContainer != null)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(lastHoveredContainer.transform.position);
            GUI.Label(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 50, 200, 30),
                     lastHoveredContainer.GetWaterLevelText(), style);
        }

        if (lastHoveredGardenBed != null)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(lastHoveredGardenBed.transform.position);
            GUI.Label(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 70, 200, 30),
                     lastHoveredGardenBed.GetBedStatus(), style);
        }

        if (lastHoveredPlantableBed != null && heldItem != null && heldItem.GetComponent<SeedBag>() != null)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(lastHoveredPlantableBed.transform.position);
            GUI.Label(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 90, 200, 30),
                     lastHoveredPlantableBed.GetInteractionHint(), style);
        }
    }

    public void PickUpItem(PickableItem item)
    {
        if (heldItem != null)
        {
            heldItem.Drop(Camera.main.transform.forward * throwForce);
        }
        heldItem = item;
        heldItem.PickUp();
    }
}