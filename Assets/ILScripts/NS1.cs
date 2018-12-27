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
        static int counter = 0;
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
        
        // Use this for initialization
        public void Start()
        {
            Debug.Log(nsScript);
            Debug.Log(nsScript == this);
            UpdateMessage.Require(this.gameObject).update.AddAction(this.Update);
            

            var obj = new GameObject(_bridage.ToString());
            obj.transform.parent = transform;
            text = obj.GetComponent<Text>();
            Debug.Log("text:", text);
            if( text == null) text = obj.AddComponent<Text>();
            testValue = 0;
            StartCoroutine(cortunction());
            Debug.Log("counter:" + (++counter));
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

