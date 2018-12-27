/*
 * Author:      熊哲
 * CreateTime:  9/26/2017 5:56:03 PM
 * Description:
 * 
*/
using UnityEngine;

namespace BeiaviourMessage
{
    [DisallowMultipleComponent]
    public class RenderMessage : _Message<RenderMessage>
    {
        public class RenderEvent : OnMessageEvent { }

        public RenderEvent visible = new RenderEvent();
        public RenderEvent invisible = new RenderEvent();

        private void OnBecameVisible()
        {
            visible.Invoke();
        }

        private void OnBecameInvisible()
        {
            invisible.Invoke();
        }
    }
}