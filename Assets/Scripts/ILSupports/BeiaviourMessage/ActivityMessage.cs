/*
 * Author:      熊哲
 * CreateTime:  9/26/2017 6:07:04 PM
 * Description:
 * 
*/
using UnityEngine;

namespace BeiaviourMessage
{
    [DisallowMultipleComponent]
    public class ActivityMessage : _Message<ActivityMessage>
    {
        public class ActivityEvent : OnMessageEvent { }

        public ActivityEvent awake = new ActivityEvent();
        public ActivityEvent start = new ActivityEvent();
        public ActivityEvent onEnable = new ActivityEvent();
        public ActivityEvent onDisable = new ActivityEvent();
        public ActivityEvent onDestroy = new ActivityEvent();

        void Awake()
        {
            awake.Invoke();
        }
        void Start()
        {
            start.Invoke();
        }
        void OnEnable()
        {
            onEnable.Invoke();
        }
        void OnDisable()
        {
            onDisable.Invoke();
        }
        void OnDestroy()
        {
            onDestroy.Invoke();
        }
    }
}