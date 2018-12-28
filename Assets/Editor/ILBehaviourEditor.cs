using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEditorInternal;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
[Serializable]
[CustomEditor(typeof(ILBehaviourBridage), true)]
public class ILBehaviourEditor : Editor
{
    static Type TryGetType( string className)
    {
        foreach (var type in (from assmbly in AppDomain.CurrentDomain.GetAssemblies()
                              from type in assmbly.GetTypes()
                              where (type.FullName == className)
                              select type))
        {
            return type;
        }
        return null;
    }
    
    static string pattern = @".*?\snamespace\s[\S\s]*?(?={)";
    static public Type GetFullType( TextAsset asset)
    {
        if (asset == null) return null;
        string fpath = AssetDatabase.GetAssetPath(asset);
        if (!fpath.EndsWith(".cs")) return null;
        string className = Path.GetFileNameWithoutExtension(fpath);
        Type curtype = TryGetType(className);
        if( curtype == null)
        {
            var matches = Regex.Matches(asset.text, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                var np = match.Value.Replace("namespace", "").Trim();
                curtype = TryGetType(string.Format("{0}.{1}", np, className));
                if ( curtype != null) { break; }
            }
        }
        return curtype;
    }
    
    private int getMemberIndex(string member, SerializedProperty propertys)
    {
        // 返回该数据原始索引
        for (int idx = 0; idx < propertys.arraySize; idx++)
        {
            SerializedProperty sub = propertys.GetArrayElementAtIndex(idx);
            var memberName = sub != null? sub.FindPropertyRelative("m"): null;
            if ( memberName != null && memberName.stringValue == member)
            {
                return idx;
            }
        }
        return -1;
    }
   
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty scriptAsset = serializedObject.FindProperty("scriptFile");
        EditorGUILayout.PropertyField(scriptAsset);
        SerializedProperty fullType = serializedObject.FindProperty("fullType");
        var curtype = GetFullType(scriptAsset.objectReferenceValue as TextAsset);
        EditorGUILayout.LabelField("Script Full Name:", fullType.stringValue);
        fullType.stringValue = curtype != null ? curtype.FullName : "";
        if( curtype != null)
        {
            // 修改size
            SerializedProperty pro = serializedObject.FindProperty("fields");
            FieldInfo[] fields = curtype.GetFields();
            int arraySize = 0;
            foreach (FieldInfo field in fields) {
                // 避免自己结算自己member 参数
                var attr = field.GetCustomAttributes(typeof(SerializeField), false).FirstOrDefault();
                if (field.IsPublic == false || attr == null) { continue; }
                if (curtype.FullName == typeof(ILBehaviourBridage).FullName)
                {
                    if (field.Name == "fields" || field.Name == "fullType" || field.Name == "scriptFile") { continue; }
                }
                arraySize = arraySize + 1;
            }
            if (pro != null)
            {
                SerializedProperty sizepro = pro.FindPropertyRelative("Array.size");
                //EditorGUILayout.PropertyField(sizepro);
                sizepro.intValue = arraySize;
                pro.isExpanded = true;
                if (arraySize == 0) return;
                var index = 0;
                foreach (FieldInfo field in fields)
                {
                    var attr = field.GetCustomAttributes(typeof(SerializeField), false).FirstOrDefault();
                    if (field.IsPublic == false || attr == null) { continue; }
                    // 避免自己结算自己member 参数
                    if (curtype.FullName == typeof(ILBehaviourBridage).FullName)
                    {
                        if (field.Name == "fields" || field.Name == "fullType" || field.Name == "scriptFile") { continue; }
                    }
                    var oidx = getMemberIndex(field.Name, pro);
                    if (oidx != index)
                    {
                        pro.MoveArrayElement(oidx, index);
                    }
                    SerializedProperty sub = pro.GetArrayElementAtIndex(index);
                    if (sub == null)
                    {
                        pro.InsertArrayElementAtIndex(index);
                        sub = pro.GetArrayElementAtIndex(index);
                    }
                    var tp = sub.FindPropertyRelative("t");
                    var mp = sub.FindPropertyRelative("m");
                    mp.stringValue = field.Name;
                    var vs = sub.FindPropertyRelative("vs");
                    var vo = sub.FindPropertyRelative("vo");
                    tp.stringValue = field.FieldType.Name;
                    switch (field.FieldType.Name)
                    {
                        case "Int32":
                            {
                                int number;
                                Int32.TryParse(vs.stringValue, out number);
                                vs.stringValue = EditorGUILayout.IntField(field.Name, number).ToString();
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "Int64":
                            {
                                long number;
                                Int64.TryParse(vs.stringValue, out number);
                                vs.stringValue = EditorGUILayout.LongField(field.Name, number).ToString();
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "Boolean":
                            {
                                bool number;
                                Boolean.TryParse(vs.stringValue, out number);
                                vs.stringValue = EditorGUILayout.Toggle(field.Name, number).ToString();
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "Single":
                            {
                                float number;
                                float.TryParse(vs.stringValue, out number);
                                vs.stringValue = EditorGUILayout.FloatField(field.Name, number).ToString();
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "Double":
                            {
                                double number;
                                double.TryParse(vs.stringValue, out number);
                                vs.stringValue = EditorGUILayout.DoubleField(field.Name, number).ToString();
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "String":
                            {
                                vs.stringValue = EditorGUILayout.TextField(field.Name, vs.stringValue);
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "Color":
                        case "Color32":
                            {
                                Color color;
                                ColorUtility.TryParseHtmlString(vs.stringValue, out color);
                                color = EditorGUILayout.ColorField(field.Name, color);
                                vs.stringValue = "#" + ColorUtility.ToHtmlStringRGBA(color);
                                vo.objectReferenceValue = null;
                                tp.stringValue = "String";
                                break;
                            }
                        case "Vector2":
                            {
                                Vector2 v2;
                                ExtensionUtility.StringToVector2(vs.stringValue, out v2);
                                v2 = EditorGUILayout.Vector2Field(field.Name, v2);
                                vs.stringValue = ExtensionUtility.Vector2ToString(v2);
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "Vector3":
                            {
                                Vector3 v3;
                                ExtensionUtility.StringToVector3(vs.stringValue, out v3);
                                v3 = EditorGUILayout.Vector3Field(field.Name, v3);
                                vs.stringValue = ExtensionUtility.Vector3ToString(v3);
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "Vector4":
                            {
                                Vector4 v4;
                                ExtensionUtility.StringToVector4(vs.stringValue, out v4);
                                v4 = EditorGUILayout.Vector4Field(field.Name, v4);
                                vs.stringValue = ExtensionUtility.Vector4ToString(v4);
                                vo.objectReferenceValue = null;
                                break;
                            }
                        case "Rect":
                            {
                                Rect v4;
                                ExtensionUtility.StringToRect(vs.stringValue, out v4);
                                v4 = EditorGUILayout.RectField(field.Name, v4);
                                vs.stringValue = ExtensionUtility.RectToString(v4);
                                vo.objectReferenceValue = null;
                                break;
                            }
                        default:
                            {
                                // 这里如何区分是脚本类型
                                vs.stringValue = "";
                                var subattr = field.FieldType.GetCustomAttributes(typeof(ILScriptAttribute), false).FirstOrDefault();
                                if (subattr != null)
                                {
                                    //var test = field.FieldType.GetCustomAttributes(typeof(SerializableAttribute), false).FirstOrDefault();
                                    string fname = string.Format("{0}({1})", field.Name, field.FieldType.Name);
                                    var obj = EditorGUILayout.ObjectField(fname, vo.objectReferenceValue, typeof(UnityEngine.Object), true);
                                    //EditorGUILayout.ObjectField(vo, typeof(ILBehaviourBridage), new GUIContent(fname));
                                    if (obj is GameObject)
                                    {
                                        var components = (obj as GameObject).GetComponents<ILBehaviourBridage>();
                                        foreach( var com in components)
                                        {
#if USE_HOT_FIX
                                            if( com.ILInstance.Type.FullName == field.FieldType.Name)
                                            {
                                                vo.objectReferenceValue = com; break;
                                            }
#else
                                            if (com.ILInstance.GetType().FullName == field.FieldType.Name)
                                            {
                                                vo.objectReferenceValue = com; break;
                                            }
#endif
                                        }
                                    }
                                    else if (obj is ILBehaviourBridage)
                                    {
                                        if(field.FieldType.FullName != (vo.objectReferenceValue as ILBehaviourBridage).fullType)
                                        {
                                            vo.objectReferenceValue = null;
                                        }
                                        else
                                        {
                                            vo.objectReferenceValue = obj;
                                        }
                                    } 
                                    else
                                    {
                                        vo.objectReferenceValue = null;
                                    }
                                }
                                else
                                {
                                    //var test = field.FieldType.GetCustomAttributes(typeof(SerializableAttribute), false).FirstOrDefault();
                                    EditorGUILayout.ObjectField(vo, field.FieldType, new GUIContent(field.Name));
                                }
                                tp.stringValue = field.FieldType.FullName;
                                break;
                            }
                    }
                    index++;
                }
            }
            else
            {
                Debug.Log("null property:" + arraySize);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif