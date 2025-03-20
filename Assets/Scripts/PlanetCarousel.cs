using Assets.Scripts;

using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class PlanetCarousel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    [Header("Postion")]
    public float yOffset = 0;
    [Header("Carousel Settings")]
    [SerializeField] private float _selectedIndex = 0;
    public float SelectedIndex
    {
        get { return _selectedIndex; }
        set
        {
            _selectedIndex = value;
            KeepIndexInRange();
            lastChangeTime = Time.time;
            AutoSnap = false;
            
            TrueIndex = _selectedIndex;
        }
    }

    private void UpdateText()
    {
        int index = Mathf.RoundToInt(_selectedIndex);
        if (index == planets.Count)
            index = 0;
        planetDesc.text = planets[index].ToString();
    }

    [SerializeField] private float TrueIndex = 0f;
    [SerializeField] private bool AutoSnap = false;
    [SerializeField, Min(10f)] public float radiusX = 300f;
    [SerializeField, Min(10f)] public float radiusY = 150f;
    [SerializeField, Range(0.5f, 2.0f)] public float ellipseHeightFactor = 1.0f;
    [SerializeField, Range(0.1f, 2.0f)] public float maxScale = 2.0f;
    [SerializeField, Range(0.1f, 2.0f)] public float minScale = 0.1f;
    [SerializeField, Range(0f, 1f)] public float minAlpha = 0.3f;
    [SerializeField] private float snapDelay = 3f;
    [SerializeField, Range(0.01f, 1f)] private float snapSpeed = 0.5f;
    [SerializeField] private float dragSensitivity = 0.01f;
    [SerializeField] private float scrollSensitivity = 1f;
    [SerializeField] private float momentumDamping = 5f;
    public TextMeshProUGUI planetDesc;
    public bool UpdateInEditor = true;
    public bool spin;
    [Range(0, .05f)] public float spinSpeed = .01f;

    private List<RectTransform> planets = new List<RectTransform>();
    [SerializeField]
    private float lastChangeTime;
    [SerializeField]
    private float velocity = 0f; // Momentum velocity

    private float targetIndex;

    void OnEnable() => UpdatePlanets();
    void OnTransformChildrenChanged() => UpdatePlanets();
    void OnValidate() => UpdatePlanets();

    void Update()
    {
        // Spin behavior
        if (spin) SelectedIndex += spinSpeed;
        if (UpdateInEditor) SelectedIndex = _selectedIndex;

        // Handle momentum
        if (Mathf.Abs(velocity) > 0.01f)
        {
            SelectedIndex += velocity * Time.deltaTime;
            velocity *= Mathf.Exp(-momentumDamping * Time.deltaTime); // Smooth damping
            if (Mathf.Abs(velocity) < 1f)
            {
                velocity = 0f;
            }
        }
        UpdateText();// TODO THIS IS INEFFICIENT
        // Auto-snap logic
        if (!AutoSnap && Time.time - lastChangeTime > snapDelay)
        {
            AutoSnap = true;
            targetIndex = Mathf.Round(TrueIndex);

        }
        if (AutoSnap)
        {
            float step = snapSpeed * Time.deltaTime * 10f;

            if (Mathf.Abs(TrueIndex - targetIndex) > step)
            {
                TrueIndex += Mathf.Sign(targetIndex - TrueIndex) * step * Mathf.Max(1, 2 * Mathf.Sqrt(Mathf.Abs(targetIndex - TrueIndex)));
            }
            else
            {
                TrueIndex = targetIndex;
                SelectedIndex = targetIndex;
                AutoSnap = false;
            }
        }
        else
        {
            TrueIndex = _selectedIndex;
        }

        UpdatePlanets();
    }

    void UpdatePlanets()
    {
        planets.Clear();
        foreach (Transform child in transform)
        {
            if (child is RectTransform rt)
            {
                planets.Add(rt);
                LockTransform(rt);
            }
        }

        if (planets.Count == 0) return;

        int count = planets.Count;
        KeepIndexInRange();
        float angleOffset = (TrueIndex / count) * 360f;

        for (int i = 0; i < count; i++)
        {
            float angle = ((i * 360f / count) - angleOffset - 90f) * Mathf.Deg2Rad;
            float x = radiusX * Mathf.Cos(angle);
            float y = (radiusY * ellipseHeightFactor) * Mathf.Sin(angle);

            RectTransform planet = planets[i];
            planet.anchoredPosition = new Vector2(x, y + yOffset);

            float distanceToIndex = Mathf.Min(Mathf.Abs(TrueIndex - i), count - Mathf.Abs(TrueIndex - i));

            float scale = minScale;
            if (distanceToIndex == 0) scale = maxScale * 3f;
            else if (distanceToIndex <= 1) scale = Mathf.Lerp(maxScale * 1.2f, maxScale * 3f, Mathf.Pow(.5f * Mathf.Clamp01(1 - distanceToIndex / (count / 2f)), 2));
            else if (distanceToIndex <= 2) scale = Mathf.Lerp(maxScale * .8f, maxScale * 1.2f, Mathf.Pow(Mathf.Clamp01(1 - distanceToIndex / (count / 2f)), 2));
            else scale = Mathf.Lerp(minScale, maxScale, Mathf.Clamp01(1 - distanceToIndex / (count / 2f)));

            planet.localScale = Vector3.one * scale;
            float alpha = Mathf.Lerp(1f, minAlpha, Mathf.Clamp01(distanceToIndex / (count / 2f)));
            SetAlpha(planet, alpha);
        }
    }

    void KeepIndexInRange()
    {
        int count = planets.Count;
        if (count == 0) return;
        while (_selectedIndex < 0) _selectedIndex += count;
        while (_selectedIndex >= count) _selectedIndex -= count;
    }

    void LockTransform(RectTransform rt)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localRotation = Quaternion.identity;
    }

    void SetAlpha(RectTransform planet, float alpha)
    {
        CanvasGroup group = planet.GetComponent<CanvasGroup>() ?? planet.gameObject.AddComponent<CanvasGroup>();
        group.alpha = alpha;
    }
    public void ConfirmPlanet()
    {
        Globals.planetID = (int)Mathf.Round(TrueIndex);
        this.transform.parent.gameObject.SetActive(false);
    }
    public void SetPlanet(RectTransform planet)
    {
        velocity = 0;
        targetIndex = planets.IndexOf(planet);
        AutoSnap = true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        velocity = 0f; // Stop momentum
    }

    public void OnDrag(PointerEventData eventData)
    {
        SelectedIndex -= eventData.delta.x * dragSensitivity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        velocity = -eventData.delta.x * dragSensitivity * 10f;
    }

    public void OnScroll(PointerEventData eventData)
    {
        velocity -= eventData.scrollDelta.y * scrollSensitivity; // Adjust sensitivity as needed
    }

    public void IndexPlanet(bool forward = true)
    {
        velocity = 0;
        if (!AutoSnap)
            targetIndex = MathF.Round(TrueIndex + (forward ? 1 : -1));
        else
            targetIndex = MathF.Round(targetIndex + (forward ? 1 : -1));

        AutoSnap = true;
    }
}