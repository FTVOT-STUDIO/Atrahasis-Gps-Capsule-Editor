using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

public class Scroll : MonoBehaviour
{
    public GameObject scrollBar;
    private float scroll_pos = 0;
    private float[] pos;
    private Scrollbar scrollbarComponent;

    void Start()
    {
        scrollbarComponent = scrollBar.GetComponent<Scrollbar>();
        UpdatePositions();
        ResetToFirstButton();
    }

    void OnEnable()
    {
        if (scrollbarComponent == null)
        {
            scrollbarComponent = scrollBar.GetComponent<Scrollbar>();
        }
        UpdatePositions();
        ResetToFirstButton();
    }

    void OnDisable()
    {
        ResetToFirstButton();
    }

    void UpdatePositions()
    {
        int childCount = transform.childCount;
        pos = new float[childCount];
        float distance = 1f / (childCount - 1f);
        for (int i = 0; i < childCount; i++)
        {
            pos[i] = distance * i;
        }
    }

    void ResetToFirstButton()
    {
        scroll_pos = 0f;
        scrollbarComponent.value = 0f;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.localScale = (i == 0) ? new Vector2(1f, 1f) : new Vector2(0.8f, 0.8f);
        }
    }

    void Update()
    {
        if (transform.childCount != pos.Length)
        {
            UpdatePositions();
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbarComponent.value;
        }
        else
        {
            float closestPos = FindClosestPos();
            scrollbarComponent.value = Mathf.Lerp(scrollbarComponent.value, closestPos, 0.1f);
        }

        UpdateScales();
    }

    float FindClosestPos()
    {
        float closestDistance = Mathf.Infinity;
        float closestPos = 0f;
        for (int i = 0; i < pos.Length; i++)
        {
            float distance = Mathf.Abs(scroll_pos - pos[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPos = pos[i];
            }
        }
        return closestPos;
    }

    void UpdateScales()
    {
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            Transform child = transform.GetChild(i);
            float targetScale = (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2)) ? 1f : 0.8f;
            if (Mathf.Abs(child.localScale.x - targetScale) > 0.01f)
            {
                child.localScale = Vector2.Lerp(child.localScale, new Vector2(targetScale, targetScale), 0.1f);
            }
        }
    }
}
