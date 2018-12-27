using System;
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
    public virtual void Awake(ILBehaviourBridage bridage)
    {
        _bridage = bridage;
    }
}