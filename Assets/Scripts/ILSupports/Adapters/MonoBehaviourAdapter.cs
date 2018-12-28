using UnityEngine;
using System.Collections.Generic;
using ILRuntime.Other;
using System;
using System.Collections;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;

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

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain Instance, ILTypeInstance instance)
    {
        return null;
        //return new Adaptor(ILAppDomain.Instance, instance);
    }

    //为了完整实现MonoBehaviour的所有特性，这个Adapter还得扩展，这里只抛砖引玉，只实现了最常用的Awake, Start和Update
    public class Adaptor : MonoBehaviour, CrossBindingAdaptorType
    {
        public string ilType;
        private bool _initialized = false;
        private ILTypeInstance instance;
        public ILTypeInstance ILInstance
        {
            get { return instance; }
            set { instance = value; }
        }

        public void Initialize(ILType type)
        {
            if ( _initialized == false && type != null)
            {
                ilType = type.FullName; _initialized = true; 
                instance = new ILTypeInstance(type as ILType, false);
                instance.CLRInstance = this;
            }
        }

        public void Initialize( string ilType)
        {
            if( !string.IsNullOrEmpty(ilType))
            {
                IType type;
                if (ILAppDomain.Instance.LoadedTypes.TryGetValue(ilType, out type))
                {
                    Initialize(type as ILType);
                }
            }
        }

        public void Awake()
        {
            Initialize( ilType);
            if( _initialized)
            {
                var awakeMethod = instance != null ? instance.Type.GetMethod("Awake", 0) : null;
                if (awakeMethod != null)
                {
                    ILAppDomain.Instance.Invoke(awakeMethod, instance, null);
                }
            }
        }
        
        void Start()
        {
            // 移除空的Behaviour
            if( instance == null || ILAppDomain.Instance == null)
            {
                Destroy(this); Debug.Log("Remove MonoBehaviourAdapter Behaviour!");
            }
            else
            {
                var startMethod = instance.Type.GetMethod("Start", 0);
                if (startMethod != null)
                    ILAppDomain.Instance.Invoke(startMethod, instance, null);
            }
        }

        void OnDestroy()
        {
            var destoryMethod = instance != null ? instance.Type.GetMethod("OnDestroy", 0): null;
            if (destoryMethod != null)
                ILAppDomain.Instance.Invoke(destoryMethod, instance, null);
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
                ILAppDomain.Instance.Invoke(enableMethod, instance, null);
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
                ILAppDomain.Instance.Invoke(disableMethod, instance, null);
            }
        }

        public override string ToString()
        {
            IMethod m = ILAppDomain.Instance.ObjectType.GetMethod("ToString", 0);
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
