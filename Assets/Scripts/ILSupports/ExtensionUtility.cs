using ILRuntime.CLR.TypeSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionUtility
{
    public static Component AddILComponent(this GameObject go, string component)
    {
        if (string.IsNullOrEmpty(component) == false)
        {
            var com = go.AddComponent<ILBehaviourBridage>();
            com.fullType = component;
            com.Initialize();
            return com;
        }
        return null;
    }

    public static UnityEngine.Object GetILComponent(this GameObject go, string type)
    {
        ILBehaviourBridage[] components = go.GetComponents<ILBehaviourBridage>();
        foreach( ILBehaviourBridage beh in components)
        {
            if( beh.fullType == type)
            {
                return beh;
            }
        }
        return null;
    }

    public static UnityEngine.Object GetILComponent(this Component go, string type)
    {
        ILBehaviourBridage[] components = go.GetComponents<ILBehaviourBridage>();
        foreach (ILBehaviourBridage beh in components)
        {
            if (beh.fullType == type)
            {
                return beh;
            }
        }
        return null;
    }
    
    public static GameObject GetChild(this GameObject go, string path)
    {
        Transform child = go.transform.Find(path);
        return child != null ? child.gameObject : null;
    }

    public static T GetChild<T>(this Component go, string path)
    {
        Transform child = go.transform.Find(path);
        return child!= null? child.GetComponent<T>() : default(T);
    }

    public static T GetChild<T>(this GameObject go, string path)
    {
        Transform child = go.transform.Find(path);
        return child != null ? child.GetComponent<T>() : default(T);
    }

    public static void StringToVector2( string value, out Vector2 v2)
    {
        v2.x = 0; v2.y = 0;
        string[] values = string.IsNullOrEmpty(value)? null: value.Split('|');
        if(values != null && values.Length == 2)
        {
            float.TryParse(values[0], out v2.x);
            float.TryParse(values[1], out v2.y);
        }
    }

    public static string Vector2ToString(Vector2 value)
    {
        return string.Format("{0}|{1}", value.x, value.y);
    }

    public static void StringToVector3(string value, out Vector3 v3)
    {
        v3.x = 0; v3.y = 0; v3.z = 0;
        string[] values = string.IsNullOrEmpty(value) ? null : value.Split('|');
        if (values != null && values.Length == 3)
        {
            float.TryParse(values[0], out v3.x);
            float.TryParse(values[1], out v3.y);
            float.TryParse(values[2], out v3.z);
        }
    }
    
    public static string Vector3ToString(Vector3 value)
    {
        return string.Format("{0}|{1}|{2}", value.x, value.y, value.z);
    }

    public static void StringToVector4(string value, out Vector4 v4)
    {
        v4.x = 0; v4.y = 0; v4.z = 0; v4.w = 0;
        string[] values = string.IsNullOrEmpty(value) ? null : value.Split('|');
        if (values != null && values.Length == 4)
        {
            float.TryParse(values[0], out v4.x);
            float.TryParse(values[1], out v4.y);
            float.TryParse(values[2], out v4.z);
            float.TryParse(values[3], out v4.w);
        }
    }

    public static string Vector4ToString(Vector4 value)
    {
        return string.Format("{0}|{1}|{2}|{3}", value.x, value.y, value.z, value.w);
    }

    public static void StringToRect(string value, out Rect rect)
    {
        rect = Rect.zero;
        string[] values = string.IsNullOrEmpty(value) ? null : value.Split('|');
        if (values != null && values.Length == 4)
        {
            float x, y, w, h;
            float.TryParse(values[0], out x);
            float.TryParse(values[1], out y);
            float.TryParse(values[2], out w);
            float.TryParse(values[3], out h);
            rect.x = x; rect.y = y; rect.width = w; rect.height = h;
        }
    }

    public static string RectToString(Rect value)
    {
        return string.Format("{0}|{1}|{2}|{3}", value.x, value.y, value.width, value.height);
    }
}