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
    public class Joint2DMessage : _Message<Joint2DMessage>
    {
        public class JointEvent : OnMessageEvent<float> { }

        public JointEvent joint = new JointEvent();

        void OnJoint2DBreak( float msg)
        {
            joint.Invoke(msg);
        }
    }
}