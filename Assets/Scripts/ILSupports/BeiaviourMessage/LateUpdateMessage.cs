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
    public class LateUpdateMessage : _Message<LateUpdateMessage>
    {
        public class UpdateEvent : OnMessageEvent { }
        
        public UpdateEvent lateUpdate = new UpdateEvent();
        
        void LateUpdate()
        {
            lateUpdate.Invoke();
        }
    }
}