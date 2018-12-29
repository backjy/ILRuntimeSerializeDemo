using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
//    NotificationCenter is used for handling messages between GameObjects.
//    GameObjects can register to receive specific notifications.  When another objects sends a notification of that type, all GameObjects that registered for it and implement the appropriate message will receive that notification.
//    Observing GameObjects must register to receive notifications with the AddObserver function, and pass their selves, and the name of the notification.  Observing GameObjects can also unregister themselves with the RemoveObserver function.  GameObjects must request to receive and remove notification types on a type by type basis.
//    Posting notifications is done by creating a Notification object and passing it to PostNotification.  All receiving GameObjects will accept that Notification object.  The Notification object contains the sender, the notification type name, and an option hashtable containing data.
//    To use NotificationCenter, either create and manage a unique instance of it somewhere, or use the static NotificationCenter.
// 修改成使用delegate 因为xlua 中使用不了
public class ObserverNode
{
    public object sender;
    public System.Action<String, object> callback;
    public bool hasInvoke;
    private WeakReference _observer;
    public System.Object observer
    {
        get
        {
            return _observer.IsAlive ? _observer.Target : null;
        }
    }
    public ObserverNode(System.Object _observer, System.Action<String, object> cb, object _sender)
    {
        _observer = new WeakReference(_observer);
        callback = cb; sender = _sender;
        hasInvoke = false;
    }
}

// We need a static method for objects to be able to obtain the default notification center.
// This default center is what all objects will use for most notifications.  We can of course create our own separate instances of NotificationCenter, but this is the static one used by all.
public class NotificationCenter : MonoBehaviour
{
    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
    private static NotificationCenter defaultCenter;
    public static NotificationCenter Instance()
    {
        // If the defaultCenter doesn't already exist, we need to create it
        if (!defaultCenter)
        {
            // Because the NotificationCenter is a component, we have to create a GameObject to attach it to.
            GameObject notificationObject = new GameObject("Default Notification Center");
            // Add the NotificationCenter component, and set it as the defaultCenter
            defaultCenter = notificationObject.AddComponent<NotificationCenter>();
            DontDestroyOnLoad(defaultCenter);
        }

        return defaultCenter;
    }
    
    //
    public void WaitAndDoClean()
    {
        StartCoroutine(GCAction());
    }

    IEnumerator GCAction()
    {
        yield return new WaitForEndOfFrame();
        // 移除不以及销毁的 observer
        foreach (string key in notifications.Keys)
        {
            List<ObserverNode> list = (List<ObserverNode>)notifications[key];
            int listcount = list.Count;
            for (int idx = 0; idx < listcount;)
            {
                ObserverNode curNode = list[idx];
                if (curNode.observer == null)
                {
                    curNode.hasInvoke = true;
                    list.RemoveAt(idx);
                    listcount--;
                }
                else
                {
                    idx++;
                }
            }
            if (list.Count == 0) { notifications.Remove(name); }
        }
    }

    // Our hashtable containing all the notifications.  Each notification in the hash table is an ArrayList that contains all the observers for that notification.
    Hashtable notifications = new Hashtable();

    // AddObserver includes a version where the observer can request to only receive notifications from a specific object.  We haven't implemented that yet, so the sender value is ignored for now.
    public void AddObserver(String name, System.Object observer, System.Action<String, object> cb) { AddObserver(name, observer, cb, null); }
    public void AddObserver(String name, System.Object observer, System.Action<String, object> cb, object sender)
    {
        // If the name isn't good, then throw an error and return.
        if (name == null || name == "") { Debug.Log("Null name specified for notification in AddObserver."); return; }
        // If this specific notification doens't exist yet, then create it.
        if (!notifications.ContainsKey(name))
        {
            notifications[name] = new List<ObserverNode>();
        }
        //	    if (!notifications[name]) {
        //	        notifications[name] = new List<Component>();
        //	    }

        List<ObserverNode> notifyList = (List<ObserverNode>)notifications[name];
        bool bContain = false;
        int observerCount = notifyList.Count;
        for ( int idx = 0; idx < observerCount; idx++)
        {
            ObserverNode curNode = notifyList[idx];
            if (curNode.observer == observer)
            {
                bContain = true; break;
            }
        }
        // If the list of observers doesn't already contain the one that's registering, then add it.
        if (!bContain)
        {
            ObserverNode curNode = new ObserverNode(observer, cb, sender);
            notifyList.Add(curNode);
        }
    }

    // RemoveObserver removes the observer from the notification list for the specified notification type
    public void RemoveObserver(System.Object observer, String name)
    {
        List<ObserverNode> notifyList = (List<ObserverNode>)notifications[name]; //change from original
        // Assuming that this is a valid notification type, remove the observer from the list.
        // If the list of observers is now empty, then remove that notification type from the notifications hash.  This is for housekeeping purposes.
        if (notifyList != null)
        {
            int observerCount = notifyList.Count;
            for (int idx = 0; idx < observerCount; idx++)
            {
                ObserverNode curNode = notifyList[idx];
                if (curNode.observer == observer)
                {
                    curNode.hasInvoke = true;
                    notifyList.RemoveAt(idx);
                    break;
                }
            }
            if (notifyList.Count == 0) { notifications.Remove(name); }
        }
    }

    // Post with Delay End Frame
    public void PostDelayFrame( string aName, object aData = null, object aSender = null)
    {
        StartCoroutine(coroutinePostFrame( aName, aData, aSender));
    }

    IEnumerator coroutinePostFrame(string aName, object aData, object aSender)
    {
        yield return new WaitForEndOfFrame();
        this.PostData(aName, aData, aSender);
    }

    // Post with Delay Time
    public void PostDelayTime( float second,string aName, object aData = null, object aSender = null)
    {
        StartCoroutine(coroutinePostTime( second, aName, aData, aSender));
    }

    IEnumerator coroutinePostTime(float second, string aName, object aData, object aSender)
    {
        yield return new WaitForSecondsRealtime( second);
        this.PostData(aName, aData, aSender);
    }

    // PostNotification sends a notification object to all objects that have requested to receive this type of notification.
    // A notification can either be posted with a notification object or by just sending the individual components.
    public void Post(String aName, object aData = null ) { PostData( aName, aData); }
    public void PostData( String aName, object aData, object aSender = null)
    {
        // First make sure that the name of the notification is valid.
        if ( aName == null || aName == "") {
            //Debug.Log("Null name sent to PostNotification." + aName); 
            return;
        }
        // Obtain the notification list, and make sure that it is valid as well
        List<ObserverNode> notifyList = (List<ObserverNode>)notifications[aName]; //change from original
        if (notifyList == null) {
            //Debug.Log("Notify list not found in PostNotification." + aName);
            return;
        }

        // Clone list, so there won't be an issue if an observer is added or removed while notifications are being sent
        notifyList = new List<ObserverNode>(notifyList);

        // Create an array to keep track of invalid observers that we need to remove
        List<ObserverNode> observersToRemove = new List<ObserverNode>(); //change from original

        // Itterate through all the objects that have signed up to be notified by this type of notification.
        int observerCount = notifyList.Count;
        for (int idx = 0; idx < observerCount; idx++)
        {
            ObserverNode curNode = notifyList[idx];
            if (curNode.hasInvoke == true) continue; 
            // If the observer isn't valid, then keep track of it so we can remove it later.
            // We can't remove it right now, or it will mess the for loop up.
            if ( curNode.observer == null || (curNode.callback == null))
            {
                observersToRemove.Add(curNode);
            }
            else
            {
                // If the observer is valid, then send it the notification.  The message that's sent is the name of the notification.
                if ( curNode.sender == null || ( aSender == curNode.sender))
                {
                    try
                    {
                        curNode.callback(aName, aData);
                    }
                    catch (System.Exception exp)
                    {
                        Debug.LogError(exp);
                    }
                }
            }
        }

        // Remove all the invalid observers
        int deleationCount = observersToRemove.Count;
        for (int idx = 0; idx < deleationCount; idx++)
        {
            ObserverNode curNode = observersToRemove[idx];
            notifyList.Remove(curNode);
        }
    }
}