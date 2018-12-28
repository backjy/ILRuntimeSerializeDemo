using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Runtime.Enviorment;

[CustomEditor(typeof(MonoBehaviourAdapter.Adaptor), true)]
public class MonoBehaviourAdapterEditor : UnityEditor.UI.GraphicEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        MonoBehaviourAdapter.Adaptor clr = target as MonoBehaviourAdapter.Adaptor;
        var instance = clr.ILInstance;
        if (instance != null)
        {
            EditorGUILayout.LabelField("Script", clr.ILInstance.Type.FullName);
            foreach (var i in instance.Type.FieldMapping)
            {
                //这里是取的所有字段，没有处理不是public的
                var name = i.Key;
                var type = instance.Type.FieldTypes[i.Value];
                
                var cType = type.TypeForCLR;
                if (cType.IsPrimitive)//如果是基础类型
                {
                    if (cType == typeof(float))
                    {
                        instance[i.Value] = EditorGUILayout.FloatField(name, (float)instance[i.Value]);
                    }
                    else if( cType == typeof(int))
                    {
                        instance[i.Value] = EditorGUILayout.IntField(name, (int)instance[i.Value]);
                    }
                    else if (cType == typeof(long))
                    {
                        instance[i.Value] = EditorGUILayout.LongField(name, (long)instance[i.Value]);
                    }
                    else if (cType == typeof(double))
                    {
                        instance[i.Value] = EditorGUILayout.DoubleField(name, (double)instance[i.Value]);
                    }
                }
                else
                {
                    object obj = instance[i.Value];
                    if (typeof(UnityEngine.Object).IsAssignableFrom(cType))
                    {
                        //处理Unity类型
                        var res = EditorGUILayout.ObjectField(name, obj as UnityEngine.Object, cType, true);
                        instance[i.Value] = res;
                    }
                    else if (cType == typeof(string))
                    {
                        instance[i.Value] = EditorGUILayout.TextField(name, (string)instance[i.Value]);
                    }
                    else if (cType == typeof(Color))
                    {
                        instance[i.Value] = EditorGUILayout.ColorField(name, (Color)instance[i.Value]);
                    }
                    else if (cType == typeof(Color32))
                    {
                        instance[i.Value] = (Color32)EditorGUILayout.ColorField(name, (Color32)instance[i.Value]);
                    }
                    else if (cType == typeof(Vector2))
                    {
                        instance[i.Value] = EditorGUILayout.Vector2Field(name, (Vector2)instance[i.Value]);
                    }
                    else if (cType == typeof(Vector3))
                    {
                        instance[i.Value] = EditorGUILayout.Vector3Field(name, (Vector3)instance[i.Value]);
                    }
                    else if (cType == typeof(Vector4))
                    {
                        instance[i.Value] = EditorGUILayout.Vector4Field(name, (Vector4)instance[i.Value]);
                    }
                    else if (cType == typeof(Rect))
                    {
                        instance[i.Value] = EditorGUILayout.RectField(name, (Rect)instance[i.Value]);
                    }
                    else
                    {
                        //其他类型现在没法处理
                        if (obj != null)
                            EditorGUILayout.LabelField(name, obj.ToString());
                        else
                            EditorGUILayout.LabelField(name, "(null)");
                    }
                }
            }
        }
    }
}
