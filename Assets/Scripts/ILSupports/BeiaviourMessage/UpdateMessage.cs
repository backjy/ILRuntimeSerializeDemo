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
    public class UpdateMessage : _Message<UpdateMessage>
    {
        public class UpdateEvent : OnMessageEvent { }

        public UpdateEvent update = new UpdateEvent();

        void Update()
        {
            update.Invoke();
        }
    }
}