using UnityEngine;
using TMPro;
using System.Collections;

public class WaterTapInteraction : MonoBehaviour, IInteractable
{
    public FlowingWater flowingWater;
    public Transform waterOutputPosition;
    public float moveSpeed = 2f;
    public float fillRate = 10f;
    public float maxWaterCapacity = 100f;
    public string interactionHint = "Набрать воды [E]";
    public float detectionRadius = 1.5f;

    private PickableItem heldItem;
    private bool isFilling = false;
    private float currentFillAmount = 0f;
    private Coroutine fillCoroutine;
    private bool tapIsOn = false;
    private WaterContainer nearbyContainer;

    [Header("Tap Handle Animation")]
    public Transform tapHandle;
    public float handleRotationSpeed = 90f;
    private Quaternion handleClosedRotation;
    private Quaternion handleOpenRotation;
    private Coroutine handleRotationCoroutine;

    void Start()
    {
        handleClosedRotation = tapHandle.localRotation;
        handleOpenRotation = handleClosedRotation * Quaternion.Euler(0, 90, 0);
    }

    private void Update()
    {
        if (!isFilling)
        {
            CheckForNearbyContainer();
        }
    }

    private void CheckForNearbyContainer()
    {
        nearbyContainer = null;

        Collider[] colliders = Physics.OverlapSphere(waterOutputPosition.position, detectionRadius);
        float closestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            WaterContainer container = collider.GetComponent<WaterContainer>();
            if (container != null)
            {
                float distance = Vector3.Distance(waterOutputPosition.position, container.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearbyContainer = container;
                }
            }
        }
    }

    public string GetInteractionHint()
    {
        if (tapIsOn)
        {
            return "Выключить кран [E]";
        }
        else if (nearbyContainer != null)
        {
            return "Включить кран [E]";
        }

        return interactionHint;
    }

    public void Interact()
    {
        if (tapIsOn)
        {
            StopFilling();
            if (flowingWater != null)
                flowingWater.ToggleWater(false);
        }
        else
        {
            Interactor interactor = FindObjectOfType<Interactor>();
            heldItem = interactor.HeldItem;

            if (heldItem != null && heldItem.TryGetComponent<WaterContainer>(out var container))
            {
                StartFilling(interactor, container.gameObject);
                if (flowingWater != null)
                    flowingWater.ToggleWater(true);
            }
            else if (nearbyContainer != null)
            {
                StartFillingNearbyContainer(nearbyContainer.gameObject);
                if (flowingWater != null)
                    flowingWater.ToggleWater(true);
            }
            else
            {
                tapIsOn = true;
                interactionHint = "Выключить кран [E]";
                if (flowingWater != null)
                    flowingWater.ToggleWater(true);
                RotateHandle(true);
            }
        }
    }

    private void StartFilling(Interactor interactor, GameObject containerObject)
    {
        isFilling = true;
        tapIsOn = true;
        interactionHint = "Выключить кран [E]";

        Rigidbody rb = containerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }

        heldItem.enabled = false;
        interactor.HeldItem = null;
        heldItem.GetComponent<PickableItem>().SetPickupParent(null);
        heldItem.Drop(Vector3.zero);

        containerObject.transform.position = waterOutputPosition.position;
        containerObject.transform.rotation = waterOutputPosition.rotation;

        WaterContainer waterContainer = containerObject.GetComponent<WaterContainer>();
        if (waterContainer != null)
        {
            currentFillAmount = waterContainer.GetWaterAmount();

            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
            }

            fillCoroutine = StartCoroutine(FillWaterContainer(containerObject));
        }

        RotateHandle(true);
    }

    private void StartFillingNearbyContainer(GameObject containerObject)
    {
        isFilling = true;
        tapIsOn = true;
        interactionHint = "Выключить кран [E]";

        Rigidbody rb = containerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }

        PickableItem pickableItem = containerObject.GetComponent<PickableItem>();
        if (pickableItem != null)
        {
            pickableItem.enabled = false;
        }

        WaterContainer waterContainer = containerObject.GetComponent<WaterContainer>();
        if (waterContainer != null)
        {
            currentFillAmount = waterContainer.GetWaterAmount();

            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
            }

            fillCoroutine = StartCoroutine(FillWaterContainer(containerObject));
        }

        RotateHandle(true);
    }

    private IEnumerator FillWaterContainer(GameObject containerObject)
    {
        WaterContainer waterContainer = containerObject.GetComponent<WaterContainer>();
        if (waterContainer == null) yield break;

        Vector3 targetPosition = waterOutputPosition.position;
        Quaternion targetRotation = waterOutputPosition.rotation;

        while (containerObject != null && Vector3.Distance(containerObject.transform.position, targetPosition) > 0.1f)
        {
            containerObject.transform.position = Vector3.MoveTowards(containerObject.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            containerObject.transform.rotation = Quaternion.Slerp(containerObject.transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
            yield return null;
        }

        while (containerObject != null && currentFillAmount < maxWaterCapacity && isFilling)
        {
            currentFillAmount += fillRate * Time.deltaTime;
            waterContainer.SetWaterAmount(currentFillAmount);
            yield return null;
        }

        if (currentFillAmount >= maxWaterCapacity)
        {
            StopFilling();
        }
    }

    public void StopFilling()
    {
        if (!tapIsOn) return;

        isFilling = false;
        tapIsOn = false;
        interactionHint = "Включить кран [E]";

        if (flowingWater != null)
            flowingWater.ToggleWater(false);

        if (heldItem != null)
        {
            heldItem.enabled = true;
            heldItem = null;
        }

        if (nearbyContainer != null)
        {
            var pickableItem = nearbyContainer.GetComponent<PickableItem>();
            if (pickableItem != null)
            {
                pickableItem.enabled = true;
            }

            var rb = nearbyContainer.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }

        RotateHandle(false);
    }

    private void RotateHandle(bool open)
    {
        if (handleRotationCoroutine != null)
            StopCoroutine(handleRotationCoroutine);

        handleRotationCoroutine = StartCoroutine(AnimateHandleRotation(open));
    }

    private IEnumerator AnimateHandleRotation(bool open)
    {
        Quaternion startRotation = tapHandle.localRotation;
        Quaternion targetRotation = open ? handleOpenRotation : handleClosedRotation;

        float duration = Quaternion.Angle(startRotation, targetRotation) / handleRotationSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            tapHandle.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        tapHandle.localRotation = targetRotation;
        handleRotationCoroutine = null;
    }
}