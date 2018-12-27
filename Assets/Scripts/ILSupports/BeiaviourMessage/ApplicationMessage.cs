/*
 * Author:      熊哲
 * CreateTime:  9/26/2017 5:58:43 PM
 * Description:
 * 
*/
using UnityEngine;

namespace BeiaviourMessage
{
    [DisallowMultipleComponent]
    public class ApplicationMessage : _Message<ApplicationMessage>
    {
        public class ApplicationEvent : OnMessageEvent<bool> { }
        public class ApplicationQuitEvent : OnMessageEvent { }

        public ApplicationEvent onApplicationFocus = new ApplicationEvent();
        public ApplicationEvent onApplicationPause = new ApplicationEvent();
        public ApplicationQuitEvent onApplicationQuit = new ApplicationQuitEvent();

        void OnApplicationFocus(bool focusStatus)
        {
            onApplicationFocus.Invoke(focusStatus);
        }
        void OnApplicationPause(bool pauseStatus)
        {
            onApplicationPause.Invoke(pauseStatus);
        }
        void OnApplicationQuit()
        {
            onApplicationQuit.Invoke();
        }
    }
}