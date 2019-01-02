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
    public class NS1 : MonoBehaviour
    {
        static int counter = 0;
        public int testValue;
        public int testValue1;
        public string testValue2;
        public Color tColor;
        [UnityEngine.SerializeField]
        public Color32 tColor2;

        public Vector2 v2;

        public Vector3 v3;

        public Vector4 v4;
        public Rect rect;
        
        //public GameObject gameobj;
        [UnityEngine.SerializeField]
        public NS1 nsScript;

        [SerializeField]
        public GameObject prefab;
        Text text;
        
        // Use this for initialization
        public void Start()
        {
            Debug.Log(nsScript);
            Debug.Log(nsScript == this);
            UpdateMessage.Require(this.gameObject).update.AddAction(this.Update);
            Debug.Log("Start....");
            Debug.Log(this.prefab);

            if (counter == 0)
            {
                this.tColor2 = Color.green; v2.x = v2.y = 1; v3.x = v3.y = v3.z = 2;
                Instantiate(prefab, transform.parent);
            }
            else if (counter == 1)
                Instantiate(this, transform.parent);
            else if (counter == 2)
            {
                Instantiate(this.gameObject, transform.parent);
            }
            if( true)
            {
                var obj = new GameObject(ToString());
                obj.transform.parent = transform;
                text = obj.GetComponent<Text>();
                Debug.Log("text:" + text);
                if (text == null) text = obj.AddComponent<Text>();
            }
            testValue = 0;
            StartCoroutine(cortunction());
            Debug.Log("counter:" + (++counter));
        }

        // Update is called once per frame
        void Update()
        {
            if( text!= null)
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

