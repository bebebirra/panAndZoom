using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanZoom : MonoBehaviour, IDragHandler
{
    //Vector3 touchStart;
    [SerializeField] float zoomOutMin = 1;
    [SerializeField] float zoomOutMax = 8;
    [SerializeField] GameObject window;

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount == 2) return;

        RectTransform rectTransform = this.transform as RectTransform;

        Vector2 newposition = rectTransform.anchoredPosition + (eventData.delta);
        NormalizePosition(ref newposition);

        rectTransform.anchoredPosition = newposition;
    }

    //Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //}
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.01f);
        }

        if (Input.mouseScrollDelta.y != 0f)
        {
            Debug.Log(Input.mouseScrollDelta.y);
        }
        //else if (Input.GetMouseButton(0))
        //{
        //    Vector3 inputPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector3 localPoint = this.transform.InverseTransformPoint(inputPoint);

        //    Vector3 direction = touchStart - localPoint;

        //    Vector3 resultantPosition = this.transform.position + direction;
        //    checkPosition(ref resultantPosition);
        //    this.transform.position = resultantPosition;
        //}
        //zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void NormalizePosition(ref Vector2 newPosition)
    {
        RectTransform wdwRect = window.transform as RectTransform;
        RectTransform ownRect = this.transform as RectTransform;

        //Get the apparent width and Height
        float ownWidth = ownRect.rect.width * ownRect.localScale.x;
        float ownHeight = ownRect.rect.height * ownRect.localScale.y;

        //Get the window width and Height
        float wdwWidth = wdwRect.rect.width;
        float wdwHeight = wdwRect.rect.height;

        Debug.Log($"Own: ({ownWidth}; {ownHeight}), Wdw: ({wdwWidth}; {wdwHeight})");

        //obtain limits
        float maxLeft = (ownWidth / 2) - (wdwWidth/2);
        if(maxLeft < 0) maxLeft = 0;
        float minRight = -maxLeft;

        float maxBottom = ownHeight/2 - (wdwHeight/2); 
        if(maxBottom < 0) maxBottom = 0;
        float minTop= -maxBottom;

        Debug.Log($"MinTop: {minTop}, MaxLeft: {maxLeft}, MinRight: {minRight}, MaxBottom: {maxBottom}");

        string orig = $"x:{newPosition.x}, y: {newPosition.y}";

        //apply corrections
        if (maxLeft < newPosition.x)
            newPosition.x = maxLeft;

        if (minRight > newPosition.x)
            newPosition.x = minRight;

        if (minTop > newPosition.y)
            newPosition.y = minTop;

        if (maxBottom < newPosition.y)
            newPosition.y = maxBottom;

        Debug.Log($"old: {orig}, new: x:{newPosition.x}, y: {newPosition.y}");
    }

    public void zoom(float increment)
    {
        if (increment == 0) return;

        this.transform.localScale = new Vector3(
                Mathf.Clamp(this.transform.localScale.x + increment, zoomOutMin, zoomOutMax),
                Mathf.Clamp(this.transform.localScale.y + increment, zoomOutMin, zoomOutMax),
                this.transform.localScale.z);

        RectTransform rect = this.transform as RectTransform;
        Vector2 newPosition = new(rect.anchoredPosition.x, rect.anchoredPosition.y);
        NormalizePosition(ref newPosition);
        rect.anchoredPosition = newPosition;

        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);

    }
}
