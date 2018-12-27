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
    public class FixedUpdateMessage : _Message<FixedUpdateMessage>
    {
        public class UpdateEvent : OnMessageEvent { }
        public UpdateEvent fixedUpdate = new UpdateEvent();
        
        void FixedUpdate()
        {
            fixedUpdate.Invoke();
        }
    }
}