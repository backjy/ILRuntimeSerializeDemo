/*
 * Author:      熊哲
 * CreateTime:  9/26/2017 5:52:19 PM
 * Description:
 * 
*/
using UnityEngine;

namespace BeiaviourMessage
{
    [DisallowMultipleComponent]
    public class Trigger2DMessage : _Message<Trigger2DMessage>
    {
        public class TriggerEvent : OnMessageEvent<Collider2D> { }

        public TriggerEvent onTriggerEnter = new TriggerEvent();
        public TriggerEvent onTriggerStay = new TriggerEvent();
        public TriggerEvent onTriggerExit = new TriggerEvent();

        void OnTriggerEnter2D(Collider2D collider)
        {
            onTriggerEnter.Invoke(collider);
        }
        void OnTriggerStay2D(Collider2D collider)
        {
            onTriggerStay.Invoke(collider);
        }
        void OnTriggerExit2D(Collider2D collider)
        {
            onTriggerExit.Invoke(collider);
        }
    }
}