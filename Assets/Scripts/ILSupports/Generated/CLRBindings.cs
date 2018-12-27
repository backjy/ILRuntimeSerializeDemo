using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            BeiaviourMessage__Message_1_UpdateMessage_Binding.Register(app);
            BeiaviourMessage_UpdateMessage_Binding.Register(app);
            BeiaviourMessage_OnMessageEvent_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            System_Object_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            System_String_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_WaitForSeconds_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
