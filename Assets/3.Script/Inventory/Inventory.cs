using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Inventory : MonoBehaviour
{
    [Header("Maniplate")]
    public RectTransform inventoryPanel;
    public RectTransform slideEnableArea;
    public float slideDistance = 200f; 
    public float slideDuration = 0.5f;
    public float sensitivityDistance = 50f;
    private Vector2 startTouchPos;
    private bool isOpened = false;
    private bool isSwiping = false;

    [Header("Item")]
    [SerializeField] private Image[] itemSlot;
    private int itemCount = 0;

    void Update()
    {
#if UNITY_EDITOR
      
        if (Input.GetMouseButtonDown(0) && 
            IsTouchSlideImage(Input.mousePosition))
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
    private bool IsTouchSlideImage(Vector2 touchPos)
    {
        Vector2 localTouchPos = slideEnableArea.InverseTransformPoint(touchPos);
        return slideEnableArea.rect.Contains(localTouchPos);
    }
    #endregion

    #region Item
    public void AddItem(int itemCode)
    {

    }

    #endregion
}
