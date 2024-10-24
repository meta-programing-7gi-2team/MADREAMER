using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class Inventory : MonoBehaviour
{
    [Header("Control")]
    public RectTransform inventoryPanel;
    public RectTransform swipeEnableArea;
    public float slideDistance = 200f; 
    public float slideDuration = 0.5f;
    public float sensitivityDistance = 50f;

    private Vector2 startTouchPos;
    private bool isOpened = false;
    private bool isSwiping = false;

    [Header("Item")]
    public Sprite[] itemImages;
    public ItemSlot[] slotList;
    private int itemCount = 0;

    void Update()
    {
#if UNITY_EDITOR
      
        if (Input.GetMouseButtonDown(0) && 
            IsTouchEnableArea(Input.mousePosition))
        {
            startTouchPos = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 endTouchPos = Input.mousePosition;
            float swipeDistanceY = endTouchPos.y - startTouchPos.y;

            ManipulateUI(swipeDistanceY);

            isSwiping = false;
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && 
                IsTouchSlideImage(Input.mousePosition))
            {
                startTouchPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                Vector2 endTouchPos = touch.position;
                float swipeDistanceY = endTouchPosition.y - startTouchPos.y;

                ManipulateUI(swipeDistanceY);

                isSwiping = false;
            }
        }
#endif
    }

    #region Control
    private void ManipulateUI(float swipeDistanceY)
    {
        if (swipeDistanceY > sensitivityDistance)
        {
            if (isOpened) return;
            inventoryPanel.DOAnchorPosY(inventoryPanel.anchoredPosition.y + slideDistance, slideDuration);
            isOpened = true;
        }
        else if (swipeDistanceY < -sensitivityDistance)
        {
            if (!isOpened) return;
            inventoryPanel.DOAnchorPosY(inventoryPanel.anchoredPosition.y - slideDistance, slideDuration);
            isOpened = false;
        }
    }
    private bool IsTouchEnableArea(Vector2 touchPos)
    {
        Vector2 localTouchPos = swipeEnableArea.InverseTransformPoint(touchPos);
        return swipeEnableArea.rect.Contains(localTouchPos);
    }
    #endregion

    #region Item
    public void ItemTest()
    {
        int rand = Random.Range(0, itemImages.Length);
        AddItem(rand);
    }
    public void AddItem(int itemCode)
    {
        if(itemCount == 6)
        {
            Debug.Log("Inventory Full");
            return;
        }
        for(int i = 0; i < slotList.Length; i++)
        {
            if (slotList[i].isEmpty)
            {
                slotList[i].SetSlot(itemImages[itemCode] ,itemCode);
                itemCount++;
                break;
            }
        }
    }
    public void RemoveItem(int slotIndex)
    {
        slotList[slotIndex].InitSlot();
        itemCount--;
    }
    #endregion
}
