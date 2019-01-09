using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListTest : MonoBehaviour
{
    public UIListView view;
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        view = view == null ? GetComponent<UIListView>() : view;
        view.initializeCell = this.InitializeCell;
        view.Refresh(15);
    }

    RectTransform InitializeCell( RectTransform cell, int index)
    {
        if( cell == null)
        {
            cell = Instantiate(prefab).transform as RectTransform;
        }
        var txt = cell.GetComponent<Text>();
        txt.text = "txt:" + index;
        cell.SetWidth(50);
        //cell.SetHeight(Random.Range(30, 50));
        return cell;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
