using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Collections;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;


public class MonoBehaviourAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(MonoBehaviour);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    //为了完整实现MonoBehaviour的所有特性，这个Adapter还得扩展，这里只抛砖引玉，只实现了最常用的Awake, Start和Update
    public class Adaptor : MonoBehaviour, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain {
            get
            {
                return appdomain;
            }
            set
            {
                appdomain = value;
            }
        }

        bool hasAwake = false;
        public void Awake()
        {
            if (hasAwake == true) return;
            var awakeMethod = instance != null? instance.Type.GetMethod("Awake", 0): null;
            if( awakeMethod != null)
            {
                appdomain.Invoke(awakeMethod, instance, null);
            }
        }
        
        void Start()
        {
            var startMethod = instance != null? instance.Type.GetMethod("Start", 0): null;
            if( startMethod != null)
                appdomain.Invoke(startMethod, instance, null);
        }

        void OnDestroy()
        {
            var destoryMethod = instance != null ? instance.Type.GetMethod("OnDestroy", 0): null;
            if (destoryMethod != null)
                appdomain.Invoke(destoryMethod, instance, null);
        }

        IMethod enableMethod;
        private void OnEnable()
        {
            if( instance != null && enableMethod == null)
            {
                enableMethod = instance.Type.GetMethod("OnEnable", 0);
            }
            if( enableMethod != null)
            {
                appdomain.Invoke(enableMethod, instance, null);
            }
        }

        IMethod disableMethod;
        private void OnDisable()
        {
            if (instance != null && disableMethod == null)
            {
                disableMethod = instance.Type.GetMethod("OnDisable", 0);
            }
            if (enableMethod != null)
            {
                appdomain.Invoke(disableMethod, instance, null);
            }
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance != null ? instance.Type.GetVirtualMethod(m): null;
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }
}
