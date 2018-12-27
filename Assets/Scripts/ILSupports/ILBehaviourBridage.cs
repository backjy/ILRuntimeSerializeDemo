
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
    // 对 HOT_Fixed 和正常模式采用不同的执行逻辑
    private bool hasInitilized = false;
#if USE_HOT_FIX
    // instance
    protected ILTypeInstance ilInstance;
    protected IMethod startMethod;
    protected IMethod destoryMethod;
    protected IMethod enableMethod;
    protected IMethod disableMethod;
    
    public void Initialize()
    {
        IType ilType;
        if (string.IsNullOrEmpty(fullType) == false && ILAppDomain.GetInstance().LoadedTypes.TryGetValue(fullType, out ilType))
        {
            ilInstance = (ilType as ILType).Instantiate();
            startMethod = ilType.GetMethod("Start", 0);
            destoryMethod = ilType.GetMethod("OnDestroy", 0);
            enableMethod = ilType.GetMethod("OnEnable", 0);
            disableMethod = ilType.GetMethod("OnDisable", 0);
            var awakeMethod = ilType.GetMethod("Awake", 1);
            hasInitilized = true;
            // 把自己传过去
            // SerializeFields(); start 的时候调用 避免ILruntime 中的instance 为null
            if (awakeMethod != null)
                ILAppDomain.GetInstance().Invoke(awakeMethod, ilInstance, new object[] { this });
        }
    }
    
    public void SerializeFields()
    {
        if(ilInstance != null)
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
                        ilInstance[idx] = cv.Invoke(ref fields[i]);
                    }
                    else
                    {
                        ilInstance[idx] = ConvertObject(ref fields[i]);
                    }
                }
            }
        }
    }

    public ILTypeInstance GetILInstance()
    {
        return ilInstance;
    }

    void Awake()
    {
        ilInstance = null;
        Initialize();
    }

    // Use this for initialization
    void Start ()
    {
        if (!hasInitilized) Initialize();
        SerializeFields();
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
        
    
    static public System.Object ConvertObject(ref ILBehaviourBridage.RefField source)
    {
        if( source.vo is ILBehaviourBridage)
        {
            var ilinstance = (source.vo as ILBehaviourBridage).ilInstance;
            if (ilinstance != null) return ilinstance.CLRInstance;
            return null;
        }
        return source.vo;
    }
#else

    // instance
    protected System.Object ilInstance;
    protected MethodInfo startMethod;
    protected MethodInfo destoryMethod;
    protected MethodInfo enableMethod;
    protected MethodInfo disableMethod;

    public void Initialize()
    {
        foreach (var type in (from assmbly in AppDomain.CurrentDomain.GetAssemblies()
                              from type in assmbly.GetTypes()
                              where (type.FullName == fullType)
                              select type))
        {
            Debug.Log("Test");
            ilInstance = Activator.CreateInstance(type);
            if( ilInstance != null)
            {
                startMethod     = type.GetMethod("Start");
                destoryMethod   = type.GetMethod("OnDestroy");
                enableMethod    = type.GetMethod("OnEnable");
                disableMethod   = type.GetMethod("OnDisable");
                var awakeMethod = type.GetMethod("Awake");
                hasInitilized = true;
                // 把自己传过去
                // SerializeFields(); start 的时候调用 避免ILruntime 中的instance 为null
                if (awakeMethod != null)
                    awakeMethod.Invoke(ilInstance, new object[] { this });
            }
        }
    }

    public void SerializeFields()
    {
        if (ilInstance != null)
        {
            Type ilType = ilInstance.GetType();
            // 设置属性
            int fieldlen = fields.Length;
            for (int i = 0; i < fieldlen; i++)
            {
                FieldInfo info = ilType.GetField(fields[i].m);
                if( info != null)
                {
                    ConvertTo cv;
                    if (convertMap.TryGetValue(info.FieldType.Name, out cv))
                    {
                        info.SetValue(ilInstance, cv(ref fields[i]));
                    }
                    else
                    {
                        info.SetValue(ilInstance, ConvertObject(ref fields[i]));
                    }
                }
            }
        }
    }

    public System.Object GetILInstance()
    {
        return ilInstance;
    }

    void Awake()
    {
        ilInstance = null;
        Initialize();
    }

    // Use this for initialization
    void Start()
    {
        if (!hasInitilized) Initialize();
        SerializeFields();
        if (startMethod != null)
            startMethod.Invoke(ilInstance, null);
    }

    void OnDestroy()
    {
        if (destoryMethod != null)
            destoryMethod.Invoke(ilInstance, null);
    }

    void OnEnable()
    {
        if (enableMethod != null)
            enableMethod.Invoke(ilInstance, null);
    }

    void OnDisable()
    {
        if (disableMethod != null)
            disableMethod.Invoke(ilInstance, null);
    }


    static public System.Object ConvertObject(ref ILBehaviourBridage.RefField source)
    {
        if (source.vo is ILBehaviourBridage)
        {
            var ilinstance = (source.vo as ILBehaviourBridage).ilInstance;
            if (ilinstance != null) return ilinstance;
            return null;
        }
        return source.vo;
    }
#endif

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
}
