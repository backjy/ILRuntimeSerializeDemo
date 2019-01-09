using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate RectTransform InitializeCell(RectTransform transform, int idx);
[DisallowMultipleComponent]
public class UIListView : ScrollRect
{
    // 循环池
    public Transform cycleParent;
    public InitializeCell initializeCell;
    private int iCurrentIdx = 0;
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
        if (horizontal)
        {
            anchorMin.y = 1;anchorMax.y = 1;
            // 避免content anchor设置错误 造成计算错误
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
        cell.SetParent(cycleParent);
    }

    private RectTransform GetCycleCellAt( int index)
    {
        var cyclecell = cycleParent.childCount > 0 ? (RectTransform)cycleParent.GetChild(0) : null;
        var cell = initializeCell != null? initializeCell(cyclecell, index): null;
        return cell;
    }

    private int GetPreviewIndex()
    {
        var priview = iCurrentIdx - 1;
        if( priview < 0)
        {
            return movementType == MovementType.Unrestricted ? totalCells - 1 : -1;
        }
        return priview;
    }

    private int GetNextIndex( int childCount)
    {
        int next = childCount + iCurrentIdx;
        if ( next >= totalCells)
        {
            return movementType == MovementType.Unrestricted? next % totalCells : next;
        }
        return next;
    }

    protected virtual void FillLeftContent( RectTransform fristCell, float positionx)
    {
        Vector2 anchpos = fristCell != null ? fristCell.anchoredPosition : Vector2.zero;
        while( (anchpos.x + positionx) > 0)
        {
            int index = GetPreviewIndex();
            if (index < 0 || index >= totalCells) break;
            fristCell = GetCycleCellAt(index);
            if (fristCell == null) break;
            fristCell.SetParent(content);
            fristCell.SetAsFirstSibling();
            anchorMin.y = fristCell.anchorMin.y; anchorMax.y = fristCell.anchorMax.y;
            fristCell.anchorMin = anchorMin; fristCell.anchorMax = anchorMax;
            anchpos.x = anchpos.x - fristCell.rect.width; anchpos.y = 0;
            fristCell.anchoredPosition = anchpos;
            iCurrentIdx = index;
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
            lastCell.SetParent(content);
            lastCell.SetAsLastSibling();
            anchorMin.y = lastCell.anchorMin.y; anchorMax.y = lastCell.anchorMax.y;
            lastCell.anchorMin = anchorMin; lastCell.anchorMax = anchorMax;
            anchpos.x = right; anchpos.y = 0;
            lastCell.anchoredPosition = anchpos;
            // state
            right = anchpos.x + lastCell.rect.width;
            contentWidth = Mathf.Max(contentWidth, right);
            emptyWidth = viewRect.rect.width - (right + content.anchoredPosition.x);
            childCount++;
        }
        // 是否重设高度
        if (!Mathf.Approximately(contentWidth, content.rect.width))
        {
            content.SetWidth(contentWidth);
        }
    }

    protected virtual void FillTopContent( RectTransform fristCell, float positiony)
    {
        Vector2 anchpos = fristCell != null ? fristCell.anchoredPosition : Vector2.zero;
        while( (anchpos.y + positiony) < 0)
        {
            int index = GetPreviewIndex();
            if (index < 0 || index >= totalCells) break;
            fristCell = GetCycleCellAt(index);
            if (fristCell == null) break;
            fristCell.SetParent(content);
            fristCell.SetAsFirstSibling();
            anchorMin.x = fristCell.anchorMin.x; anchorMax.x = fristCell.anchorMax.x;
            fristCell.anchorMin = anchorMin; fristCell.anchorMax = anchorMax;
            anchpos.x = 0; anchpos.y = anchpos.y + fristCell.rect.height;
            fristCell.anchoredPosition = anchpos;
            iCurrentIdx = index;
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
            lastCell.SetParent(content);
            lastCell.SetAsLastSibling();
            // set anchor & anchposition
            anchorMin.x = lastCell.anchorMin.x; anchorMax.x = lastCell.anchorMax.x;
            lastCell.anchorMin = anchorMin; lastCell.anchorMax = anchorMax;
            anchpos.x = 0; anchpos.y = cellbottom;
            lastCell.anchoredPosition = anchpos;
            // record state values
            cellbottom = cellbottom - lastCell.rect.height;
            emptyheight = viewRect.rect.height - (-cellbottom - positiony);
            contentHeight = Mathf.Max( Mathf.Abs(cellbottom), contentHeight);
            childCount++;
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
                    FillBottomContent(last, position.y);
                    float fristbottom = frist.anchoredPosition.y - frist.rect.height;
                    if( frist != last && (iCurrentIdx + tempContent.childCount != totalCells) && fristbottom + position.y > 0)
                    {
                        PushRecycleCell(frist);
                        iCurrentIdx++;
                    }
                } else {
                    FillTopContent(frist, position.y);
                    if (frist != last && iCurrentIdx != 0 && last.anchoredPosition.y + position.y < viewRect.rect.height * -1)
                    {
                        PushRecycleCell(last);
                    }
                }
            }

            if( horizontal && !Mathf.Approximately( position.x, tempContent.anchoredPosition.x))
            {
                bool bMoveLeft = position.x < tempContent.anchoredPosition.x;
                if( bMoveLeft)
                {
                    FillRightContnet(last, position.x);
                    float fristRight = frist.anchoredPosition.x + frist.rect.width;
                    if( frist != last && (iCurrentIdx + tempContent.childCount != totalCells) && fristRight + position.x < 0)
                    {
                        PushRecycleCell(frist);
                        iCurrentIdx++;
                    }
                }
                else
                {
                    FillLeftContent(frist, position.x);
                    if (frist != last && iCurrentIdx != 0 && last.anchoredPosition.x + position.x > viewRect.rect.width)
                    {
                        PushRecycleCell(last);
                    }
                }
            }
        }
        base.SetContentAnchoredPosition(position);
    }

}
