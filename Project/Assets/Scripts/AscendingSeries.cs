using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//递增数列等比显示类
public class AscendingSeries
{
    private List<int> nums;
    private int max;
    private int min;
    private int intervalcount;
    private List<IntervalInfo> intervalInfos = new List<IntervalInfo>();
    private float oneareaprogress = 0;
    
    public AscendingSeries(List<int> nums)
    {
        this.nums = nums;
        min = nums[0];
        max = this.nums[nums.Count - 1];
        intervalcount = this.nums.Count - 1;
        oneareaprogress = 1 / (float)intervalcount;
        
        for (int i = 1; i <= intervalcount; i++)
        {
            IntervalInfo intervalInfo = new IntervalInfo(this.nums[i-1],this.nums[i],i,oneareaprogress*(i-1),oneareaprogress*i);
            intervalInfos.Add(intervalInfo);
        }
    }

    private IntervalInfo GetIntervalByValue(int value)
    {
        IntervalInfo intervalInfo = null;
        for (int i = 0; i < intervalInfos.Count; i++)
        {
            if (value<=intervalInfos[i].Max && value>= intervalInfos[i].Min)
            {
                intervalInfo = intervalInfos[i];
                break;
            }
        }
        
        return intervalInfo;
    }

    public float GetProgressByValue(int value)
    {
        float progress = 0;
        IntervalInfo intervalInfo = GetIntervalByValue(value);
        if (intervalInfo==null)
        {
            return progress;
        }

        for (int i = 0; i < intervalInfo.Index-1; i++)
        {
            progress +=oneareaprogress;
        }
        progress += oneareaprogress* intervalInfo.GetProgressByValue(value);
        return progress;
    }
}

public class IntervalInfo
{
    private int min;
    private int max;
    private int index;
    private float minprogress;
    private float maxprogress;
    public IntervalInfo(int min,int max,int index,float minprogress,float maxprogress)
    {
        this.Min = min;
        this.Max = max;
        this.Index = index;
        this.Minprogress = minprogress;
        this.Maxprogress = this.maxprogress;
    }

    public int Min
    {
        get => min;
        set => min = value;
    }

    public int Max
    {
        get => max;
        set => max = value;
    }

    public int Index
    {
        get => index;
        set => index = value;
    }

    public float Minprogress
    {
        get => minprogress;
        set => minprogress = value;
    }

    public float Maxprogress
    {
        get => maxprogress;
        set => maxprogress = value;
    }

    public float GetProgressByValue(int value)
    {
        float progress = 0;
        if (value<=Min)
        {
            progress = 0;
        }
        else if(value>=Max)
        {
            progress = 1;
        }
        else
        {
            progress = (value - min) / (float)(Max - Min);
        }


        return progress;

    }
}
