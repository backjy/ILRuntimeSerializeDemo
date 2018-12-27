using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTest : ILComponent {
    
    public RectTransform parent;
    // Use this for initialization
    [SerializeField]
    public TestSpace2.NS1 ns1;
    void Start () {
        Debug.Log("Load Test");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
