using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
    private Transform pickupParent;
    private bool isHeld = false;
    public string itemName = "Item";
    private Rigidbody rb;
    private Collider itemCollider;
    private Quaternion initialWorldRotation;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }

    public void Interact()
    {
        if (!isHeld)
        {
            PickUp();
        }
        else
        {
            Drop(Camera.main.transform.forward * FindObjectOfType<Interactor>().throwForce);
        }
    }

    public string GetInteractionHint()
    {
        return isHeld ? "Бросить " + itemName : "Взять " + itemName;
    }

    public void PickUp()
    {
        isHeld = true;
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        initialWorldRotation = transform.rotation;

        Interactor interactor = FindObjectOfType<Interactor>();
        if (interactor != null)
        {
            pickupParent = interactor.heldItemParent;
            transform.SetParent(pickupParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.rotation = initialWorldRotation;
            interactor.SetHeldItem(this);
        }
    }

    public void Drop(Vector3 force)
    {
        isHeld = false;
        transform.SetParent(null);
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
        }
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }
    }

    public void Drop()
    {
        Drop(transform.forward * FindObjectOfType<Interactor>().throwForce);
    }

    public void SetPickupParent(Transform parent)
    {
        pickupParent = parent;
    }
}