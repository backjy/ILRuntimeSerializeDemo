using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate RectTransform InitializeCell(RectTransform transform, int idx);
[DisallowMultipleComponent]
public class UIListView : ScrollRect
{
    // 循环池
    public Transform cycleParent;
    // 是否分页 每个page cell 的宽高定
    public bool pageEnable = false;
    [NonSerialized]
    public InitializeCell initializeCell;
    // 当前显示的第一个索引
    private int iCurrentIdx = 0;
    public int CurrentIndex {  get { return iCurrentIdx; } }
    // 当前显示的最后一个索引
    private int iLastIndex = 0;
    public int LastIndex { get { return iLastIndex; } }
    //
    private int totalCells = 0;
    private Vector2 anchorMax = Vector2.zero;
    private Vector2 anchorMin = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        if( cycleParent == null)
        {
            var obj = new GameObject("recycle node!", typeof(RectTransform));
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            cycleParent = obj.transform;
        }
        content.pivot = Vector2.up;
        // 避免content anchor设置错误 造成计算错误
        if (horizontal)
        {
            anchorMin.y = 1;anchorMax.y = 1;
            content.anchorMin = Vector2.zero; content.anchorMax = Vector2.up;
            content.offsetMin = Vector2.zero; content.offsetMax = Vector2.zero;
        }
        if (vertical)
        {
            horizontal = false;
            anchorMin.y = 1; anchorMax.y = 1;
            content.anchorMin = Vector2.up; content.anchorMax = Vector2.one;
            content.offsetMin = Vector2.zero; content.offsetMax = Vector2.zero;
        }
    }

    public virtual void Refresh( int tCount)
    {
        totalCells = tCount;
        content.RemoveAllChildren();
        cycleParent.RemoveAllChildren();
        iCurrentIdx = 0;
        if( horizontal) {
            FillRightContnet(null, 0);
        } else {
            FillBottomContent(null, 0);
        }
    }

    private void PushRecycleCell( RectTransform cell)
    {
        cell.SetParent(cycleParent, false);
    }

    private RectTransform GetCycleCellAt( int index)
    {
        var cyclecell = cycleParent.childCount > 0 ? (RectTransform)cycleParent.GetChild(0) : null;
        var cell = initializeCell != null? initializeCell(cyclecell, index): null;
        return cell;
    }

    private int GetPreviewIndex(int childCount)
    {
        int preview = 0;
        if(childCount > 0)
        {
            preview = iCurrentIdx - 1;
            if( preview < 0)
            {
                if (movementType == MovementType.Unrestricted)
                {
                    preview = totalCells + preview;
                }
                else preview = -1;
            }
        }
        return preview;
    }

    private int GetNextIndex( int childCount)
    {
        int next = 0;
        if( childCount > 0) {
            next = iLastIndex + 1;
            next = (movementType == MovementType.Unrestricted)? next % totalCells : next;
        }
        return next;
    }

    // todo: 需要修复回滚高度如果超出顶部 那么要重新调整整个窗口大小以及位置
    protected virtual void FillLeftContent( RectTransform fristCell, float positionx)
    {
        int childCount = content.childCount;
        Vector2 anchpos = fristCell != null ? fristCell.anchoredPosition : Vector2.zero;
        while( (anchpos.x + positionx) > 0)
        {
            int index = GetPreviewIndex(childCount);
            if (index < 0 || index >= totalCells) break;
            fristCell = GetCycleCellAt(index);
            if (fristCell == null) break;
            fristCell.SetParent(content, false);
            fristCell.SetAsFirstSibling();
            anchorMin.y = fristCell.anchorMin.y; anchorMax.y = fristCell.anchorMax.y;
            fristCell.anchorMin = anchorMin; fristCell.anchorMax = anchorMax;
            anchpos.x = anchpos.x - fristCell.rect.width; anchpos.y = fristCell.anchoredPosition.y;
            fristCell.anchoredPosition = anchpos;
            iCurrentIdx = index; childCount++;
        }
    }

    protected virtual void FillRightContnet( RectTransform lastCell, float positionx)
    {
        float contentWidth = content.rect.width;
        int childCount = content.childCount;
        Vector2 anchpos = Vector2.zero;
        float right = lastCell != null ? lastCell.anchoredPosition.x + lastCell.rect.width : 0;
        float emptyWidth = viewRect.rect.width - (right + content.anchoredPosition.x);
        while( emptyWidth > 0)
        {
            int index = GetNextIndex(childCount);
            if (index < 0 || index >= totalCells) break;
            lastCell = GetCycleCellAt(index);
            if (lastCell == null) break;
            lastCell.SetParent(content, false);
            lastCell.SetAsLastSibling();
            anchorMin.y = lastCell.anchorMin.y; anchorMax.y = lastCell.anchorMax.y;
            lastCell.anchorMin = anchorMin; lastCell.anchorMax = anchorMax;
            anchpos.x = right; anchpos.y = lastCell.anchoredPosition.y;
            lastCell.anchoredPosition = anchpos;
            // state
            right = anchpos.x + lastCell.rect.width;
            contentWidth = Mathf.Max(contentWidth, right);
            emptyWidth = viewRect.rect.width - (right + content.anchoredPosition.x);
            childCount++;
            iLastIndex = index;
        }
        // 是否重设高度
        if (!Mathf.Approximately(contentWidth, content.rect.width))
        {
            content.SetWidth(contentWidth);
        }
    }
    // todo: 需要修复回滚高度如果超出顶部 那么要重新调整整个窗口大小以及位置
    protected virtual void FillTopContent( RectTransform fristCell, float positiony)
    {
        int childCount = content.childCount;
        Vector2 anchpos = fristCell != null ? fristCell.anchoredPosition : Vector2.zero;
        while( (anchpos.y + positiony) < 0)
        {
            int index = GetPreviewIndex(childCount);
            if (index < 0 || index >= totalCells) break;
            fristCell = GetCycleCellAt(index);
            if (fristCell == null) break;
            fristCell.SetParent(content, false);
            fristCell.SetAsFirstSibling();
            anchorMin.x = fristCell.anchorMin.x; anchorMax.x = fristCell.anchorMax.x;
            fristCell.anchorMin = anchorMin; fristCell.anchorMax = anchorMax;
            anchpos.x = fristCell.anchoredPosition.x; anchpos.y = anchpos.y + fristCell.rect.height;
            fristCell.anchoredPosition = anchpos;
            iCurrentIdx = index; childCount++;
        }
    }

    protected virtual void FillBottomContent( RectTransform lastCell, float positiony)
    {
        int childCount = content.childCount;
        float contentHeight = content.rect.height;
        Vector2 anchpos = Vector2.zero;
        //
        float cellbottom = lastCell ? lastCell.anchoredPosition.y  - lastCell.rect.height: 0;
        float emptyheight = viewRect.rect.height - (-cellbottom - positiony);
        while( emptyheight > 0)
        {
            int index = GetNextIndex(childCount);
            if (index < 0 || index >= totalCells) break;
            lastCell = GetCycleCellAt(index);
            if (lastCell == null) break;
            lastCell.SetParent(content, false);
            lastCell.SetAsLastSibling();
            // set anchor & anchposition
            anchorMin.x = lastCell.anchorMin.x; anchorMax.x = lastCell.anchorMax.x;
            lastCell.anchorMin = anchorMin; lastCell.anchorMax = anchorMax;
            anchpos.x = lastCell.anchoredPosition.x; anchpos.y = cellbottom;
            lastCell.anchoredPosition = anchpos;
            // record state values
            cellbottom = cellbottom - lastCell.rect.height;
            emptyheight = viewRect.rect.height - (-cellbottom - positiony);
            contentHeight = Mathf.Max( Mathf.Abs(cellbottom), contentHeight);
            childCount++;
            iLastIndex = index;
        }
        // 是否重设高度
        if( !Mathf.Approximately(contentHeight, content.rect.height))
        {
            content.SetHeight(contentHeight);
        }
    }
    
    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        RectTransform tempContent = content;
        int childCount = tempContent.childCount;
        RectTransform frist = childCount > 0 ? (RectTransform)tempContent.GetChild(0) : null;
        RectTransform last = childCount > 0 ? (RectTransform)tempContent.GetChild(childCount - 1) : null;
        if( childCount > 0)
        {
            if(vertical && !Mathf.Approximately(position.y, tempContent.anchoredPosition.y))
            {
                bool bMoveUp = position.y > tempContent.anchoredPosition.y;
                if (bMoveUp) {
                    float fristbottom = frist.anchoredPosition.y - frist.rect.height;
                    if( frist != last && (iLastIndex + 1 != totalCells) && fristbottom + position.y > 0)
                    {
                        PushRecycleCell(frist); iCurrentIdx++;
                        iCurrentIdx = iCurrentIdx >= totalCells ? iCurrentIdx - totalCells : iCurrentIdx;
                    }
                    FillBottomContent(last, position.y);
                } else {
                    if (frist != last && iCurrentIdx != 0 && last.anchoredPosition.y + position.y < viewRect.rect.height * -1)
                    {
                        PushRecycleCell(last); iLastIndex--;
                        iLastIndex = iLastIndex < 0 ? totalCells + iLastIndex : iLastIndex;
                    }
                    FillTopContent(frist, position.y);
                }
            }

            if( horizontal && !Mathf.Approximately( position.x, tempContent.anchoredPosition.x))
            {
                bool bMoveLeft = position.x < tempContent.anchoredPosition.x;
                if( bMoveLeft)
                {
                    float fristRight = frist.anchoredPosition.x + frist.rect.width;
                    if( frist != last && (iLastIndex + 1 != totalCells) && fristRight + position.x < 0)
                    {
                        PushRecycleCell(frist); iCurrentIdx++;
                        iCurrentIdx = iCurrentIdx >= totalCells ? iCurrentIdx - totalCells : iCurrentIdx;
                    }
                    FillRightContnet(last, position.x);
                }
                else
                {
                    if (frist != last && iCurrentIdx != 0 && last.anchoredPosition.x + position.x > viewRect.rect.width)
                    {
                        PushRecycleCell(last); iLastIndex--;
                        iLastIndex = iLastIndex < 0? totalCells + iLastIndex: iLastIndex;
                    }
                    FillLeftContent(frist, position.x);
                }
            }
        }
        base.SetContentAnchoredPosition(position);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        // 移动结束
        if (pageEnable)
        {
            // clear m_velocity values
            StopMovement();
            // do move to
            RectTransform frist = content.childCount > 0 ? (RectTransform)content.GetChild(0) : null;
            if(frist != null && horizontal)
            {
                float halfWidth = frist.rect.width * 0.33f;
            }
            else if( frist != null)
            {
                float halfHeight = frist.rect.height * 0.33f;
            }
        }
    }
    
    public override void OnScroll(PointerEventData data)
    {
        // 移动结束
        if (!pageEnable)
        {
            base.OnScroll(data);
        }
    }

    //
    IEnumerator ScrollToPage( Vector2 position)
    {
        yield return null;
    }
}
