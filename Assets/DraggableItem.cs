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
    private Vector2Int home;

    private Vector2 dragOffset;
    private bool isFromShop = false;
    private Transform originalParent;
    private Vector2Int originalHome;

    private bool init = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // ✅ Moved this here for safety
        image = GetComponent<Image>();
    }

    public void Init(Vector2Int size, Color color, Vector2Int home)
    {
        canvas = GetComponentInParent<Canvas>();

        init = true;
        this.width = size.x;
        this.height = size.y;
        originalColor = color;
        image.color = color;
        this.home = home;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!init)
        {
            Debug.LogError("Moving an uninitialized object!");
            return;
        }

        InventoryGridDrawer grid = transform.parent.GetComponentInParent<InventoryGridDrawer>();
        if (grid == null)
        {
            Debug.LogError("Item not in an inventory!");
            return;
        }

        if (grid.shop)
        {
            if (!Buy())
            {
                eventData.pointerDrag = null;
                return;
            }

            isFromShop = true;
        }

        originalParent = transform.parent;
        originalHome = home;
        grid.RemoveItem(this);
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            dragOffset = rectTransform.anchoredPosition - localPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint + dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.color = originalColor;

        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = eventData.position };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        InventoryGridDrawer potentialGrid = null;

        foreach (var result in raycastResults)
        {
            potentialGrid = result.gameObject.GetComponent<InventoryGridDrawer>();
            if (potentialGrid != null) break;
        }

        if (potentialGrid == null || potentialGrid.shop)
        {
            if (isFromShop)
            {
                ReturnOriginal();
            }
            Sell();
            return;
        }

        // ✅ Get the local mouse position inside the potential grid
        RectTransform gridRect = potentialGrid.GetComponent<RectTransform>();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRect,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localMousePos))
        {
            ReturnOriginal();
            Destroy(gameObject);
            return;
        }

        // ✅ Convert local position to grid coordinates
        Vector2Int newHome = potentialGrid.GetGridPositionFromLocal(localMousePos);

        var check = potentialGrid.TryAddItem(this, newHome);
        if (check != null)
        {
            home = (Vector2Int)check;
            if (isFromShop)
            {
                ReturnOriginal();
                isFromShop = false;
            }
        }
        else
        {
            ReturnOriginal();
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
        Debug.Log($"Item sold. it was {isFromShop}");
        Destroy(gameObject);
    }

    private void ReturnOriginal()
    {
        if (originalParent == null)
        {
            Debug.LogError("Returned an original without an original");
            return;
        }

        GameObject replacement = Instantiate(gameObject, originalParent);
        replacement.name = gameObject.name;

        DraggableItem replacementItem = replacement.GetComponent<DraggableItem>();
        replacementItem.Init(new Vector2Int(width, height), originalColor, originalHome);
        replacementItem.transform.SetParent(originalParent);

        var check = replacementItem.transform.parent
            .GetComponentInParent<InventoryGridDrawer>()
            .TryAddItem(replacementItem, originalHome);

        if (check == null || check != originalHome)
        {
            Debug.LogError("couldn't return object to original position");
        }

        replacementItem.isFromShop = false;
    }
}
