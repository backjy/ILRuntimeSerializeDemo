using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;

public class ILAppDomain {
    // ILRuntime 脚本加载路径
    public static string ILScriptPath = "Library/ScriptAssemblies/ILScripts.dll";
    // ILRuntime 脚本PDB加载路径
    public static string ILScriptPDBPath = "Temp/UnityVS_bin/Debug/ILScripts.pdb";
    static AppDomain _instance;
    // 获取和设置ILRuntimeDomain
    static public AppDomain Instance
    {
        set
        {
            _instance = value;
        }
        get
        {
            if (_instance == null)
            {
                _instance = new AppDomain();
                _instance.LoadAssemblyFile(ILScriptPath);
                InitializeDomain();
            }
            return _instance;
        }
    }

    // 初始化ILRintime相关的 CLRBinding & 添加对应跨域继承
    static private void InitializeDomain()
    {
        if (!UsingILMode) return;
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(_instance);
        _instance.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        _instance.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
    }

    // 是否使用ILRuntime 进行脚本运行
    static public bool UsingILMode = true;
}
