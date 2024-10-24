using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject[] itemList;
    [SerializeField] private RectTransform swipeEnableArea;
    private int curItem = -1;
    private Vector2 startTouchPos;
    private bool isDragging;
    public void ShowItem(int itemCode) 
    {
        if (curItem == -1)
        {
            itemList[itemCode].transform.rotation = Quaternion.identity;
            itemList[itemCode].SetActive(true);
        }
        else 
        {
            itemList[curItem].transform.rotation = Quaternion.identity;
            itemList[curItem].SetActive(false);
            itemList[itemCode].transform.rotation = Quaternion.identity;
            itemList[itemCode].SetActive(true);
        }
        curItem = itemCode;
    }
    public void CloseView() 
    {
        itemList[curItem].SetActive(false);
        curItem = -1;
    }
    private bool IsTouchEnableArea(Vector2 touchPos)
    {
        Vector2 localTouchPos = swipeEnableArea.InverseTransformPoint(touchPos);
        return swipeEnableArea.rect.Contains(localTouchPos);
    }
    public void OnBeginDrag(PointerEventData eventData)
	{
        startTouchPos = eventData.position;
        if (IsTouchEnableArea(startTouchPos)) isDragging = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
        if (!isDragging) return;
        Vector2 delta = eventData.delta;
        itemList[curItem].transform.Rotate(-delta.x, -delta.y, 0);
	}
	public void OnEndDrag(PointerEventData eventData)
	{
        if (!isDragging) return;
        isDragging = false;
	}
}
