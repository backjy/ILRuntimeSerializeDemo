using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ILComponent
{
    public  Transform transform
    {
        get {
            if( _bridage) { return _bridage.transform; }
            return null;
        }
    }

    public GameObject gameObject
    {
        get {
            if (_bridage) return _bridage.gameObject;
            return null;
        }
    }

    protected ILBehaviourBridage _bridage;
    // override后需要base.Awake()
    public virtual void Awake(ILBehaviourBridage bridage)
    {
        _bridage = bridage;
    }

    public T GetComponent<T>()
    {
        return _bridage.GetComponent<T>();
    }

    public Component GetComponent(string type)
    {
        return _bridage.GetComponent(type);
    }

    public System.Object GetILComponent( string type)
    {
        return _bridage.GetILComponent(type);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return _bridage.StartCoroutine(routine);
    }
    
    public void StopAllCoroutines()
    {
        _bridage.StopAllCoroutines();
    }

    public void StopCoroutine(IEnumerator routine)
    {
        _bridage.StopCoroutine(routine);
    }

    public void StopCoroutine(Coroutine routine)
    {
        _bridage.StopCoroutine(routine);
    }
}