using System;
using System.Collections;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using UnityEngine;
using UnityEngine.UI;

public class Student
{
    public string name;
    public int age;
}

public class UITest : MonoBehaviour
{
    
    public void  Awake()
    {
        
      
        
    }

    private void OnClose()
    {
        
    }

    
   public void  Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



public class MyListItemViewsHolder : BaseItemViewsHolder
{
    /*
    public Text titleText;
    public Image backgroundImage;
    */


    // Retrieving the views from the item's root GameObject
    public override void CollectViews()
    {
        base.CollectViews();
        Debug.Log("CollectViews");
       
    }

    // Override this if you have children layout groups or a ContentSizeFitter on root that you'll use. 
    // They need to be marked for rebuild when this callback is fired
    /*
    public override void MarkForRebuild()
    {
        base.MarkForRebuild();

        LayoutRebuilder.MarkLayoutForRebuild(yourChildLayout1);
        LayoutRebuilder.MarkLayoutForRebuild(yourChildLayout2);
        YourSizeFitterOnRoot.enabled = true;
    }
    */

    // Override this if you've also overridden MarkForRebuild() and you have enabled size fitters there (like a ContentSizeFitter)
    /*
    public override void UnmarkForRebuild()
    {
        YourSizeFitterOnRoot.enabled = false;

        base.UnmarkForRebuild();
    }
    */
}
