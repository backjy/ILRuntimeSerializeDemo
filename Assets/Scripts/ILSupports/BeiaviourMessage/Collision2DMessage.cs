/*
 * Author:      熊哲
 * CreateTime:  9/26/2017 5:25:44 PM
 * Description:
 * 
*/
using UnityEngine;

namespace BeiaviourMessage
{
    [DisallowMultipleComponent]
    public class Collision2DMessage : _Message<Collision2DMessage>
    {
        public class CollisionEvent : OnMessageEvent<Collision2D> { }

        public CollisionEvent onCollisionEnter = new CollisionEvent();
        public CollisionEvent onCollisionStay = new CollisionEvent();
        public CollisionEvent onCollisionExit = new CollisionEvent();

        void OnCollisionEnter2D(Collision2D collision)
        {
            onCollisionEnter.Invoke(collision);
        }
        void OnCollisionStay2D(Collision2D collision)
        {
            onCollisionStay.Invoke(collision);
        }
        void OnCollisionExit2D(Collision2D collision)
        {
            onCollisionExit.Invoke(collision);
        }
    }
}