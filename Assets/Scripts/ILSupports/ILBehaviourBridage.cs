
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
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
    // 对 HOT_Fixed 和正常模式采用不同的执行逻辑

    // instance
    protected ILTypeInstance ilInstance;
    protected IMethod startMethod;
    protected IMethod destoryMethod;
    protected IMethod enableMethod;
    protected IMethod disableMethod;
    private void Awake()
    {
        ILType ilType = ILAppDomain.GetInstance().LoadedTypes[fullType] as ILType;
        if (ilType != null)
        {
            ilInstance = ilType.Instantiate();
            // 设置属性
            int fieldlen = fields.Length;
            for( int i=0; i < fieldlen; i ++)
            {
                int idx = 0;
                if (ilType.FieldMapping.TryGetValue(fields[i].m, out idx))
                {
                    IType fieldType = ilType.FieldTypes[idx];
                    ConvertTo cv;
                    if (convertMap.TryGetValue(fieldType.Name, out cv))
                    {
                        ilInstance[idx] = cv.Invoke(ref fields[i]);
                    }
                    else
                    {
                        ilInstance[idx] = ConvertObject(ref fields[i]);
                    }
                }
            }
            startMethod     = ilType.GetMethod("Start", 0);
            destoryMethod   = ilType.GetMethod("OnDestroy", 0);
            enableMethod    = ilType.GetMethod("OnEnable", 0);
            disableMethod   = ilType.GetMethod("OnDisable", 0);
            var awakeMethod = ilType.GetMethod("Awake", 1);
            // 把自己传过去
            if (awakeMethod != null)
                ILAppDomain.GetInstance().Invoke(awakeMethod, ilInstance, new object[] { this});
        }
    }

    // Use this for initialization
    void Start ()
    {
        if (startMethod != null)
            ILAppDomain.GetInstance().Invoke(startMethod, ilInstance, null);
    }

    void OnDestroy()
    {
        if (destoryMethod != null)
            ILAppDomain.GetInstance().Invoke(destoryMethod, ilInstance, null);
    }

    void OnEnable()
    {
        if (enableMethod != null)
            ILAppDomain.GetInstance().Invoke(enableMethod, ilInstance, null);
    }

    void OnDisable()
    {
        if (disableMethod != null)
            ILAppDomain.GetInstance().Invoke(disableMethod, ilInstance, null);
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
    
    static public System.Object ConvertObject(ref ILBehaviourBridage.RefField source)
    {
        if( source.vo is ILBehaviourBridage)
        {
            return (source.vo as ILBehaviourBridage).ilInstance.CLRInstance;
        }
        return source.vo;
    }
}
