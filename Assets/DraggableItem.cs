using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Image image;
    private Color originalColor;
    public int height;
    public int width;
    public Vector2Int home;

    // Add this to track the offset
    private Vector2 dragOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f); // Set transparency to 20%

        // Calculate and store the offset between mouse and object position
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint))
        {
            dragOffset = rectTransform.anchoredPosition - localPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint))
        {
            // Apply the offset to maintain relative positioning
            rectTransform.anchoredPosition = localPoint + dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryGridDrawer grid = GetComponentInParent<InventoryGridDrawer>();

        image.color = originalColor; // Reset color when dragging ends

        // Raycast to detect other InventoryGridDrawer objects under the mouse
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = eventData.position };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);


        foreach (var result in raycastResults)
        {
            InventoryGridDrawer potentialGrid = result.gameObject.GetComponent<InventoryGridDrawer>();
            if (potentialGrid != null)
            {
                if (potentialGrid.Equals(GetComponentInParent<InventoryGridDrawer>()))
                    break;
                // If the raycast hits an object with an InventoryGridDrawer, update the inventory
                if (potentialGrid.TryAddItem(this))
                {


                    grid.RemoveItem(this);
                    grid.UpdateInventory();
                    grid = GetComponentInParent<InventoryGridDrawer>();
                    grid.UpdateInventory();
                    return;
                }
            }
        }

        // If no new inventory was found, check the parent inventory (default behavior)

        if (grid != null)
        {
            grid.UpdateInventory(); // Snap item to grid
        }
        else
        {
            Debug.LogError("PARENTLESS ITEM, OBLITERATE IT");
        }

    }

}