using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class BeiaviourMessage_UpdateMessage_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(BeiaviourMessage.UpdateMessage);

            field = type.GetField("update", flag);
            app.RegisterCLRFieldGetter(field, get_update_0);
            app.RegisterCLRFieldSetter(field, set_update_0);


        }



        static object get_update_0(ref object o)
        {
            return ((BeiaviourMessage.UpdateMessage)o).update;
        }
        static void set_update_0(ref object o, object v)
        {
            ((BeiaviourMessage.UpdateMessage)o).update = (BeiaviourMessage.UpdateMessage.UpdateEvent)v;
        }


    }
}
