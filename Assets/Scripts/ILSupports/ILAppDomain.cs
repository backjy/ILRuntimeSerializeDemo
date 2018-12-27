using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;

public class ILAppDomain {
    public static string ILScriptPath = "Library/ScriptAssemblies/ILScripts.dll";
    static AppDomain _instance;
    static public AppDomain GetInstance()
    {
        if(_instance == null)
        {
            _instance = new AppDomain();
            _instance.LoadAssemblyFile(ILScriptPath);
            InitializeDomain();
        }
        return _instance;
    }

    static private void InitializeDomain()
    {
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(_instance);
        _instance.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        _instance.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
    }
}
