/*
 * Author:      熊哲
 * CreateTime:  9/26/2017 5:19:35 PM
 * Description:
 * ----- 以下说明为基于网络资料的个人理解，不保证可靠性 -----
 * MonoBehaviour的内部Message是一种反射调用的消息机制，但是并非所有的MonoBehaviour都需要响应所有消息；
 * Unity会判断哪些MonoBehaviour需要响应对应的消息（判断是否存在对应的事件方法）来避免无意义的调用；
 * 官方也建议，在MonoBehaviour中去除不用的事件方法来避免额外的性能消耗；
 * 因此，这里对MonoBehaviour传入Lua的消息进行了分类封装；
 * 另外，分类之后能很清楚地看到某个物体需要在lua上响应哪些事件，也能很方便地移除不需要继续响应的事件；
*/
using UnityEngine;

namespace BeiaviourMessage
{
    public abstract class _Message<T> : MonoBehaviour
        where T : _Message<T>
    {
        /* 用法示例：
         * local message = CS.EZFramework.XLuaExtension.UpdateMessage.Require(gameOject)    -- 获取
         * message.update:AddAction(func)   -- 添加
         * message.fixedUpdate:RemoveAction(func)   -- 移除
         * message.lateUpdate:Clear()   -- 清空
         * message:Dismiss()    -- 销毁，或者CS.EZFramework.XLuaExtension.UpdateMessage.Dismiss(gameOject)
         */
        public static T Require(GameObject obj)
        {
            T transmitter = obj.GetComponent<T>();
            if (transmitter == null)
            {
                transmitter = obj.AddComponent<T>();
            }
            return transmitter;
        }
        public static void Dismiss(GameObject obj)
        {
            if (obj == null) return;
            T message = obj.GetComponent<T>();
            if (message == null) return;
            message.Dismiss();
        }
        public void Dismiss()
        {
            Destroy(this);
        }
    }
    
    public abstract class OnMessageEvent
    {
        private event System.Action m_Event;

        public void AddAction(System.Action action)
        {
            m_Event += action;
        }
        public void RemoveAction(System.Action action)
        {
            m_Event -= action;
        }
        public void Clear()
        {
            m_Event = null;
        }
        public void Invoke()
        {
            if (m_Event != null) m_Event.Invoke();
        }
    }
    
    public abstract class OnMessageEvent<T>
    {
        private event System.Action<T> m_Event;

        public void AddAction(System.Action<T> action)
        {
            m_Event += action;
        }
        public void RemoveAction(System.Action<T> action)
        {
            m_Event -= action;
        }
        public void Clear()
        {
            m_Event = null;
        }
        public void Invoke(T arg)
        {
            if (m_Event != null) m_Event.Invoke(arg);
        }
    }
}