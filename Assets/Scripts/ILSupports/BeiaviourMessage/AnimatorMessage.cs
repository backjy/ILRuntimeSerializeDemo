using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeiaviourMessage
{
    [DisallowMultipleComponent]
    public class AnimatorMessage : _Message<AnimatorMessage>
    {
        public class AnimatorIntEvent : OnMessageEvent<int> { }
        public class AnimatorStringEvent : OnMessageEvent<string> { }
        public class AnimatorFloatEvent : OnMessageEvent<float> { }
        public class AnimatorObjectEvent : OnMessageEvent<UnityEngine.Object> { }

        public AnimatorIntEvent ikEvent = new AnimatorIntEvent();
        public AnimatorIntEvent intEvent = new AnimatorIntEvent();
        public AnimatorStringEvent strEvent = new AnimatorStringEvent();
        public AnimatorFloatEvent floatEvent = new AnimatorFloatEvent();
        public AnimatorObjectEvent objEvent = new AnimatorObjectEvent();

        private void OnAnimatorIK(int layerIndex)
        {
            ikEvent.Invoke(layerIndex);
        }
        
        private void OnAnimeEventString(string key)
        {
            strEvent.Invoke(key);
        }
        
        private void OnAnimeEventInt(int key)
        {
            intEvent.Invoke(key);
        }
        
        private void OnAnimeEventFloat(float key)
        {
            floatEvent.Invoke(key);
        }
        
        private void OnAnimeEventObject(UnityEngine.Object key)
        {
            objEvent.Invoke(key);
        }
    }
}