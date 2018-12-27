using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTest : MonoBehaviour {
    
    public RectTransform parent;
    public GameObject prefab;
    // Use this for initialization
    void Start () {
        if ( parent && prefab)
        {
            GameObject.Instantiate(prefab, parent);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
