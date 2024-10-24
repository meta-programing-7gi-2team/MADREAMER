using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour,IPointerClickHandler ,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemView;
    public Canvas canvas;
    public RectTransform rect;
    public Image slotImage;
    public Image itemImage;
    public int itemCode;
    public bool isEmpty;
    public bool isSelected;
    public bool isDragging;

    private Vector2 defaultPos;

    public void SetSlot(Sprite itemImage, int itemCode)
    {
        this.itemImage.sprite = itemImage;
        this.itemCode = itemCode;
        isEmpty = false;
    }
    public void InitSlot()
    {
        slotImage.color = Color.grey;
        itemImage.sprite = null;
        itemCode = -1;
        isEmpty = true;
    }
    
	public void OnBeginDrag(PointerEventData eventData)
	{
        if (isEmpty || isSelected) return;
        Debug.Log("Drag Start");
        isDragging = true;
        canvas.sortingOrder = 2;
        defaultPos = rect.anchoredPosition;
        rect.localScale *= 1.3f;
	}
	public void OnDrag(PointerEventData eventData)
	{
        if (isEmpty || isSelected) return;
        Debug.Log("Dragging");
        rect.anchoredPosition += eventData.delta;
	}
	public void OnEndDrag(PointerEventData eventData)
	{
        if (isEmpty || isSelected) return;
        Debug.Log("Drag End");
        isDragging = false;
        canvas.sortingOrder = 1;
        rect.anchoredPosition = defaultPos;
        rect.localScale = Vector2.one;
	}
	public void OnPointerClick(PointerEventData eventData)
	{
        if (isDragging || isEmpty) return;
        Debug.Log("Touch Start");
        isSelected = true;
        //slotImage.color = Color.green;
        itemView.SetActive(true);
        itemView.GetComponent<ItemView>().ShowItem(itemCode);
    }
}
