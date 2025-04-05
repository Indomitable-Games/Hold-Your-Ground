using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
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

    private Vector2 dragOffset;
    private bool isFromShop = false;
    private Transform originalParent;

    private bool init = false;

    private void Awake()
    {
        
    }
    public void Init(Vector2Int size, Color color)
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();


        init = true;
        this.width = size.x;
        this.height = size.y;
        originalColor = color;
        image.color = color;

        
    }
    public void ReInit()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!init)
            Debug.LogError("Moving an unit object is a big nono");

        InventoryGridDrawer grid = transform.parent.GetComponentInParent<InventoryGridDrawer>();


        if (grid == null)
            Debug.LogError("Item not in an inventory");


        if (grid != null && grid.shop)
        {
            if (Buy()) // Check if purchase is allowed
            {
                isFromShop = true;  // Mark as originally from a shop
                
            }
            else
            {
                isFromShop = false;
                eventData.pointerDrag = null; // Cancel drag
                return;
            }
        }
        grid.RemoveItem(this); //its being moved. remove it from where it was

        //store where it was so it can either be replaced or returned home if it is moved to an invalid position
        originalParent = transform.parent;

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f); // make the item semi transparent

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
            rectTransform.anchoredPosition = localPoint + dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.color = originalColor;

        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = eventData.position };
        List<RaycastResult> raycastResults = new List<RaycastResult>(); //all objects under mouse when drag ends
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        InventoryGridDrawer potentialGrid = null;

        foreach (var result in raycastResults)
        {
            potentialGrid = result.gameObject.GetComponent<InventoryGridDrawer>();
            if (potentialGrid != null) break; // Stop at the first valid grid
        }


        // If dropped into a shop or no inventory grid is found, sell it (if it's from the shop or not)
        if (potentialGrid == null || potentialGrid.shop)
        {
            if (isFromShop) // If the item came from the shop, re-add it after selling it
            {
                ReturnOriginal();  // Replace item in shop with a new one
            }
            Sell();
            return;
        }

        // If moving to a valid normal inventory
        if (potentialGrid.TryAddItem(this))
        {

            if (isFromShop)
            {
                ReturnOriginal(); // Replace only if the item was originally from the shop
                isFromShop = false;  // Reset flag to prevent further replacements
            }
        }
        else
        {
            ReturnOriginal(); //put it back a copy, inventory rejected
            Destroy(gameObject);
        }

    }

    private bool Buy()
    {
        Debug.Log("Attempting to buy item...");
        return true;
    }

    private void Sell()
    {
        Debug.Log("Item sold.");
        Destroy(gameObject);
    }

    private void ReturnOriginal()
    {
        if (originalParent == null)
        {
            Debug.LogError("Returned an original without an original");
            return; // Safety check
        }

        GameObject replacement = Instantiate(gameObject, originalParent);
        replacement.name = gameObject.name; // Prevent "(Clone)" from being appended

        DraggableItem replacementItem = replacement.GetComponent<DraggableItem>();
        replacementItem.transform.SetParent(originalParent);
        replacementItem.transform.parent.GetComponentInParent<InventoryGridDrawer>().TryAddItem(replacementItem,home);


        replacementItem.isFromShop = false; // Reset so the new item doesn't duplicate again
    }

}
