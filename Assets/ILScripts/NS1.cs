using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BeiaviourMessage;
using UnityEngine.UI;

namespace TestSpace
{
    [ILScriptAttribute]
    public class NSTest
    {

    }
}

namespace TestSpace2
{
    [ILScriptAttribute]
    public class NS1 : ILComponent
    {

        public int testValue;
        public int testValue1;
        public string testValue2;
        public Color tColor;
        public Color32 tColor2;

        public Vector2 v2;

        public Vector3 v3;

        public Vector4 v4;
        public Rect rect;
        
        //public GameObject gameobj;
        [UnityEngine.SerializeField]
        public NS1 nsScript;

        Text text;

        public override void Awake(ILBehaviourBridage bridage)
        {
            base.Awake(bridage);
            UpdateMessage.Require(this.gameObject).update.AddAction(this.Update);
            Debug.Log(bridage);
            text = this.gameObject.AddComponent<Text>();
            var obj = new GameObject(bridage.ToString());
            obj.transform.parent = transform;
            text = obj.AddComponent<Text>();
            testValue = 0;
            //text.text = bridage.ToString();
        }

        // Use this for initialization
        void Start()
        {
            _bridage.StartCoroutine(cortunction());
        }

        // Update is called once per frame
        void Update()
        {
            text.text = "update" + (testValue ++);
        }

        IEnumerator cortunction()
        {
            Debug.Log("开始协程,t=" + Time.time);
            yield return new WaitForSeconds(3);
            Debug.Log("结束协程,t=" + Time.time);
        }
    }

}

