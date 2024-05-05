/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "/Docs", as it contains everything you need to know in order to get started, including FAQ
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.DataHelpers;

// You should modify the namespace to your own or - if you're sure there won't ever be conflicts - remove it altogether


	// There are 2 important callbacks you need to implement, apart from Start(): CreateViewsHolder() and UpdateViewsHolder()
	// See explanations below
	public class CommonList : OSA<BaseParamsWithPrefab, BaseItemViewsHolder> 
	{
		// This is called once, the first time this adapter is used
		public delegate void OnItemInitHandler(int index, BaseItemViewsHolder vh) ;
		public delegate void OnItemUpdateHandler(BaseItemViewsHolder vh);
		public delegate void OnItemRecycleHandler(BaseItemViewsHolder vh, int index);

		public event OnItemInitHandler ItemInitEvent;
		public event OnItemUpdateHandler ItemUpdateEvent;
		public event OnItemRecycleHandler ItemRecycleEvent;
		
		
		// This is called once, the first time this adapter is used

		protected override void Awake()
		{
			base.Awake();
			if (!IsInitialized) Init();
		}

		protected override void Start()
		{
			

			// Calling this initializes internal data and prepares the adapter to handle item count changes
			base.Start();

			// Retrieve the models from your data source and set the items count
			/*
			RetrieveDataAndUpdate(500);
			*/
		}

		public override void ResetItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			base.ResetItems(itemsCount, contentPanelEndEdgeStationary, keepVelocity);
		}

		protected override BaseItemViewsHolder  CreateViewsHolder(int itemIndex) 
		{
			var instance = new BaseItemViewsHolder();

			// Using this shortcut spares you from:
			// - instantiating the prefab yourself
			// - enabling the instance game object
			// - setting its index 
			// - calling its CollectViews()
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
			if (ItemInitEvent != null)
			{
				ItemInitEvent(itemIndex, instance);
				instance.componentBase.Visible = true;
			}
			return instance;
		}

		// This is called anytime a previously invisible item become visible, or after it's created, 
		// or when anything that requires a refresh happens
		// Here you bind the data from the model to the item's views
		// *For the method's full description check the base implementation
		

		protected override void UpdateViewsHolder(BaseItemViewsHolder newOrRecycled)
		{

			//newOrRecycled 转成 T类型
		
			
				if (ItemUpdateEvent!=null)
				{
					bool isvis = newOrRecycled.componentBase.Visible;
					Debug.Log("isvis 是" + isvis.ToString());

                    if (newOrRecycled.componentBase.Visible == false)
					{
						newOrRecycled.componentBase.OnShow();

                    }
					else
					{
						newOrRecycled.componentBase.UpdateView();
					}
					ItemUpdateEvent(newOrRecycled);
				}
			
			
			
			
		}


		
		protected override void OnBeforeRecycleOrDisableViewsHolder(BaseItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
			
				if (ItemRecycleEvent!=null)
				{
					inRecycleBinOrVisible.componentBase.OnHide();
                    ItemRecycleEvent(inRecycleBinOrVisible, newItemIndex);
				}
			
			
		}
		

	
		


		


	
}
