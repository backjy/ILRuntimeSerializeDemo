using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// event system 
public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    static public EventTriggerListener Require(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null)
            listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    static public EventTriggerListener Require(Component go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null)
            listener = go.gameObject.AddComponent<EventTriggerListener>();
        return listener;
    }

    static public bool HasListener(GameObject go)
    {
        return go.GetComponent<EventTriggerListener>() == null;
    }

    static public bool HasListener(Component go)
    {
        return go.GetComponent<EventTriggerListener>() == null;
    }
    //
    // 摘要:
    //     Called before a drag is started.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onBeginDrag = null;
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) { onBeginDrag(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a Cancel event occurs.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<BaseEventData> onCancel = null;
    public override void OnCancel(BaseEventData eventData)
    {
        if (onCancel != null) { onCancel(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a new object is being selected.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<BaseEventData> onDeselect = null;
    public override void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null) { onDeselect(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem every time the pointer is moved during dragging.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onDrag = null;
    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) { onDrag(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when an object accepts a drop.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onDrop = null;
    public override void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null) { onDrop(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem once dragging ends.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onEndDrag = null;
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) { onEndDrag(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a drag has been found, but before it is valid
    //     to begin the drag.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onInitializePotentialDrag = null;
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (onInitializePotentialDrag != null) { onInitializePotentialDrag(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a Click event occurs.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onPointerClick = null;
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onPointerClick != null) { onPointerClick(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a PointerDown event occurs.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onPointerDown = null;
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null) { onPointerDown(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when the pointer enters the object associated with
    //     this EventTrigger.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onPointerEnter = null;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null) { onPointerEnter(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when the pointer exits the object associated with this
    //     EventTrigger.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onPointerExit = null;
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onPointerExit != null) { onPointerExit(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a PointerUp event occurs.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onPointerUp = null;
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null) { onPointerUp(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a Scroll event occurs.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<PointerEventData> onScroll = null;
    public override void OnScroll(PointerEventData eventData)
    {
        if (onScroll != null) { onScroll(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a Select event occurs.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<BaseEventData> onSelect = null;
    public override void OnSelect(BaseEventData eventData)
    {
        if(onSelect != null) { onSelect(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a Submit event occurs.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<BaseEventData> onSubmit = null;
    public override void OnSubmit(BaseEventData eventData)
    {
        if(onSubmit != null) { onSubmit(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when the object associated with this EventTrigger is
    //     updated.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<BaseEventData> onUpdateSelected = null;
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if(onUpdateSelected != null) { onUpdateSelected(eventData); }
    }
    //
    // 摘要:
    //     Called by the EventSystem when a Move event occurs.
    //
    // 参数:
    //   eventData:
    //     Current event data.
    public System.Action<AxisEventData> onMove = null;
    public override void OnMove(AxisEventData eventData)
    {
        if (onMove != null) { onMove(eventData); }
    }
}