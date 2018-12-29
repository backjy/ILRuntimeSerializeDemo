//#define USE_HOT_FIX 
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ILScriptAttribute: System.Attribute { }
delegate object ConvertTo(ref ILBehaviourBridage.RefField source);
// 桥接Behaviour
public class ILBehaviourBridage : MonoBehaviour
{
    [Serializable]
    public struct RefField
    {
        public string t;
        public string m; // 成员名字
        public string vs;// 成员string 值 
        public UnityEngine.Object vo; // 成员Unity.object 值
        //public AnimationCurve ak; // 成员AnimationCurve 值
    };
#if UNITY_EDITOR
    public TextAsset scriptFile;
#endif
    public string fullType;
    // 不建议使用过多的字段属性用于序列化, 需要序列化的字段请添加 [System.SerializeField]
    public RefField[] fields;
    // runtime instance 自动控制不能在编辑器中设置 该instance 会在
    public UnityEngine.Component instance;

    void Awake()
    {
        if (ILAppDomain.UsingILMode)
            ILRuntimeInitialize();
        else
            NattiveInitialize();
    }

    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            if (ILAppDomain.UsingILMode)
                ILRuntimeInitialize();
            else
                NattiveInitialize();
        }
    }

    // IL runtime 
    public void ILRuntimeInitialize()
    {
        if ( string.IsNullOrEmpty(fullType) ) return;
        // 如果不是MonoBehaviourAdaptor  
        if(instance == null || !(instance is MonoBehaviourAdapter.Adaptor))
        {
            // 就要删了重新处理
            if (instance != null) Destroy(instance);
            // 创建新的MonoBehaviourAdapter
            MonoBehaviourAdapter.Adaptor adaptor = gameObject.AddComponent<MonoBehaviourAdapter.Adaptor>();
            //  初始化脚本类型
            adaptor.Initialize(fullType);
            // 赋值到instance 上
            instance = adaptor;
            // 序列化属性
            ILRuntimeSerializeFields();
            // 补充awake 调用
            adaptor.Awake();
        }
    }

    public void ILRuntimeSerializeFields()
    {
        ILTypeInstance ilInstance = (instance as MonoBehaviourAdapter.Adaptor).ILInstance;
        if (ilInstance != null)
        {
            ILType ilType = ilInstance.Type;
            // 设置属性
            int fieldlen = fields.Length;
            for (int i = 0; i < fieldlen; i++)
            {
                int idx = 0;
                if (ilType.FieldMapping.TryGetValue(fields[i].m, out idx))
                {
                    ConvertTo cv;
                    if (convertMap.TryGetValue(fields[i].t, out cv))
                    {
                        ilInstance[idx] = cv(ref fields[i]);
                    }
                    else
                    {
                        ilInstance[idx] = ILConvertObject(ref fields[i]);
                    }
                }
            }
        }
    }

    // Native C# code
    public void NattiveInitialize()
    {
        if (string.IsNullOrEmpty(fullType) || instance != null) return;
        foreach (var type in (from assmbly in AppDomain.CurrentDomain.GetAssemblies()
                              from type in assmbly.GetTypes()
                              where (type.FullName == fullType)
                              select type))
        {
            instance = gameObject.AddComponent(type);
            NativeSerializeFields();
        }
    }

    public void NativeSerializeFields()
    {
        if (instance != null)
        {
            Type ilType = instance.GetType();
            // 设置属性
            int fieldlen = fields.Length;
            for (int i = 0; i < fieldlen; i++)
            {
                FieldInfo info = ilType.GetField(fields[i].m);
                if (info != null)
                {
                    ConvertTo cv;
                    if (convertMap.TryGetValue(info.FieldType.Name, out cv))
                    {
                        info.SetValue(instance, cv(ref fields[i]));
                    }
                    else
                    {
                        info.SetValue(instance, NTConvertObject(ref fields[i]));
                    }
                }
            }
        }
    }

    static Dictionary<string, ConvertTo> convertMap = new Dictionary<string, ConvertTo> {
        { "Int32", ILBehaviourBridage.ConvertInt32},
        { "Int64", ILBehaviourBridage.ConvertInt64},
        { "Boolean", ILBehaviourBridage.ConvertBoolean},
        { "Single", ILBehaviourBridage.ConvertFloat},
        { "Double", ILBehaviourBridage.ConvertDouble},
        { "String", ILBehaviourBridage.ConvertString},
        { "Color", ILBehaviourBridage.ConvertColor},
        { "Color32", ILBehaviourBridage.ConvertColor32},
        { "Vector2", ILBehaviourBridage.ConvertVector2},
        { "Vector3", ILBehaviourBridage.ConvertVector3},
        { "Vector4", ILBehaviourBridage.ConvertVector4},
        { "Rect", ILBehaviourBridage.ConvertRect},
    };

    // 类型转换函数
    static public System.Object ConvertInt32(ref ILBehaviourBridage.RefField source)
    {
        int value = 0;
        int.TryParse(source.vs, out value);
        return value;
    }

    static public System.Object ConvertInt64(ref ILBehaviourBridage.RefField source)
    {
        long value = 0;
        long.TryParse(source.vs, out value);
        return value;
    }

    static public System.Object ConvertBoolean(ref ILBehaviourBridage.RefField source)
    {
        Boolean value;
        Boolean.TryParse(source.vs, out value);
        return value;
    }

    static public System.Object ConvertFloat(ref ILBehaviourBridage.RefField source)
    {
        float value;
        float.TryParse(source.vs, out value);
        return value;
    }

    static public System.Object ConvertDouble(ref ILBehaviourBridage.RefField source)
    {
        double value;
        double.TryParse(source.vs, out value);
        return value;
    }

    static public System.Object ConvertString(ref ILBehaviourBridage.RefField source)
    {
        return source.vs;
    }
    
    static public System.Object ConvertColor(ref ILBehaviourBridage.RefField source)
    {
        Color value;
        ColorUtility.TryParseHtmlString(source.vs, out value);
        return value;
    }

    static public System.Object ConvertColor32(ref ILBehaviourBridage.RefField source)
    {
        Color value;
        ColorUtility.TryParseHtmlString(source.vs, out value);
        Color32 c32 = value;
        return c32;
    }

    static public System.Object ConvertVector2(ref ILBehaviourBridage.RefField source)
    {
        Vector2 value;
        ExtensionUtility.StringToVector2(source.vs, out value);
        return value;
    }

    static public System.Object ConvertVector3(ref ILBehaviourBridage.RefField source)
    {
        Vector3 value;
        ExtensionUtility.StringToVector3(source.vs, out value);
        return value;
    }

    static public System.Object ConvertVector4(ref ILBehaviourBridage.RefField source)
    {
        Vector4 value;
        ExtensionUtility.StringToVector4(source.vs, out value);
        return value;
    }

    static public System.Object ConvertRect(ref ILBehaviourBridage.RefField source)
    {
        Rect value;
        ExtensionUtility.StringToRect(source.vs, out value);
        return value;
    }
    
    static public System.Object ILConvertObject(ref ILBehaviourBridage.RefField source)
    {
        if (source.vo is ILBehaviourBridage)
        {
            var ilinstance = (((ILBehaviourBridage)source.vo).instance as MonoBehaviourAdapter.Adaptor).ILInstance;
            return ilinstance != null ? ilinstance.CLRInstance : null;
        }
        return source.vo;
    }

    static public System.Object NTConvertObject(ref ILBehaviourBridage.RefField source)
    {
        if (source.vo is ILBehaviourBridage)
        {
            var ilinstance = (source.vo as ILBehaviourBridage).instance;
            return ilinstance;
        }
        return source.vo;
    }
}
