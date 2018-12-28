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
    // 对 HOT_Fixed 和正常模式采用不同的执行逻辑
    private bool hasInitilized = false;
#if USE_HOT_FIX
    // instance
    protected MonoBehaviourAdapter.Adaptor adaptor;

    public ILTypeInstance ILInstance
    {
        get { return adaptor!= null? adaptor.ILInstance: null; }
    }

    public void Initialize()
    {
        IType ilType;
        if (hasInitilized == true || string.IsNullOrEmpty(fullType)) return;
        if (ILAppDomain.Instance.LoadedTypes.TryGetValue(fullType, out ilType))
        {
            //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例; 手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
            var ilInstance = new ILTypeInstance(ilType as ILType, false);
            //接下来创建Adapter实例
            adaptor = gameObject.AddComponent<MonoBehaviourAdapter.Adaptor>();
            //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
            adaptor.ILInstance = ilInstance; adaptor.AppDomain = ILAppDomain.Instance;
            //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
            ilInstance.CLRInstance = adaptor;
            // 序列化属性
            SerializeFields();
            adaptor.Awake();
            hasInitilized = true;
            //交给ILRuntime的实例应该为ILInstance
        }
    }
    
    public void SerializeFields()
    {
        if(ILInstance != null)
        {
            ILType ilType = ILInstance.Type;
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
                        ILInstance[idx] = cv.Invoke(ref fields[i]);
                    }
                    else
                    {
                        ILInstance[idx] = ConvertObject(ref fields[i]);
                    }
                }
            }
        }
    }

    void Awake()
    {
        Initialize();
        SerializeFields();
    }

    // Use this for initialization
    void Start ()
    {
        if (!hasInitilized)
        {
            Initialize();
        }
    }

    static public System.Object ConvertObject(ref ILBehaviourBridage.RefField source)
    {
        if( source.vo is ILBehaviourBridage)
        {
            var ilinstance = (source.vo as ILBehaviourBridage).adaptor.ILInstance;
            return ilinstance!=null? ilinstance.CLRInstance: null;
        }
        return source.vo;
    }
#else

    // instance
    protected UnityEngine.Component ilInstance;
    protected MethodInfo startMethod;
    protected MethodInfo destoryMethod;
    protected MethodInfo enableMethod;
    protected MethodInfo disableMethod;
    
    public UnityEngine.Component ILInstance
    {
        get { return ilInstance; }
    }

    public void Initialize()
    {
        if (string.IsNullOrEmpty(fullType) || hasInitilized == true) return;
        foreach (var type in (from assmbly in AppDomain.CurrentDomain.GetAssemblies()
                              from type in assmbly.GetTypes()
                              where (type.FullName == fullType)
                              select type))
        {
            ilInstance = gameObject.AddComponent(type);
            SerializeFields();
            hasInitilized = true;
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
    
    void Awake()
    {
        Initialize();
    }

    // Use this for initialization
    void Start()
    {
        if (!hasInitilized) Initialize();
    }

    static public System.Object ConvertObject(ref ILBehaviourBridage.RefField source)
    {
        if (source.vo is ILBehaviourBridage)
        {
            var ilinstance = (source.vo as ILBehaviourBridage).ilInstance;
            return ilinstance;
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
