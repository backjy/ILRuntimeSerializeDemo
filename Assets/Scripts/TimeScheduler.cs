using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeSchedulerType
{
    CoroutineUpdate = 0,
    Update = 1,
    //Frame = 3,
}

public class TimeScheduleNode
{
    // 缩放
    public float Scale = 1.0f;
    // 是否暂停
    public bool Paused = false;
    // 是否使用真实流失时间
    public bool RealTime = false;
    // 是否结束
    public bool Finished = false;
    // excute Times
    public uint ExcutedTimes
    {
        get { return timesExecuted; }
    }

    // 是否在当前update 中添加的Node 如果是就下一次执行
    public bool bUpdateLock = false;

    // 重复次数 0 代表无限循环
    public uint Repeat = 0;
    // 执行间隔
    private float interval = 0;
    public float Interval {
        set
        {
            interval = Mathf.Max(0, value);
        }
        get
        {
            return interval;
        }
    }
    // 是否延迟调用
    private bool useDelay = false;
    private float delay = 0;
    public float Delay
    {
        set
        {
            delay = Mathf.Max( 0, value);
            useDelay = true;
        }
        get
        {
            return delay;
        }
    }
    // 设置关联的生命周期
    public bool bLifeTimeRef = false;
    public WeakReference _refObject;
    public System.Object RefObject
    {
        set
        {
            _refObject = new WeakReference(value);
            bLifeTimeRef = true;
        }
        get
        {
            return _refObject.IsAlive? _refObject.Target: null;
        }
    }
    // 回调函数
    System.Action< TimeScheduleNode, float> callback;
    // 计时器优先级
    private int iPriorty = 0;
    // 计时器执行 事件类型 在初始化时候决定
    private TimeSchedulerType schType;
    // 初始化
    public TimeScheduleNode(System.Action<TimeScheduleNode, float> cb, TimeSchedulerType _type, int priorty)
    {
        this.callback = cb;
        this.schType = _type;
        this.iPriorty = priorty;
        this.bLifeTimeRef = false;
    }

    public void ResetCallback(System.Action<TimeScheduleNode, float> cb)
    {
        this.callback = cb;
    }

    public int GetPriorty()
    {
        return iPriorty;
    }

    public TimeSchedulerType GetSchType()
    {
        return schType;
    }
    
    private float elapsed = 0;
    private uint timesExecuted = 0;
    public void Update( float delta)
    {
        elapsed += delta * Scale;
        if (useDelay)
        {
            if (  elapsed < delay)
            {
                return;
            }
            useDelay = false;
            elapsed -= delay;
        }
        if ( elapsed >= interval)
        {
            // 如果间隔是0 那么减去 本身值 保持elapsed 不会递增上去
            elapsed -= (interval < 0.00001) ? elapsed : interval;
            // 触发事件
            Trigger( delta);
        }
    }

    // 激活当前事件
    public void Trigger(float delta)
    {
        timesExecuted += 1;
        if (Repeat != 0)
        {
            if (timesExecuted >= Repeat)
            {
                // 自动结束 在update 遍历完成的时候处理
                Finished = true;
            }
        }
        if (callback != null)
        {
            try
            {
                callback(this, delta);
            }
            catch ( System.Exception exp)
            {
                Debug.LogError(exp);
            }
        }
    }
};

public class TimeSchedulePriorty
{
    public Hashtable priortys = new Hashtable();
    ArrayList priortyKeys = new ArrayList();
    public int entityCount = 0;

    private bool bUpdateLock = false;

    public void AddScheduler(TimeScheduleNode _node, int priorty)
    {
        LinkedList<TimeScheduleNode> priortyList = null;
        if ( !priortys.ContainsKey(priorty))
        {
            priortyList = new LinkedList<TimeScheduleNode>();
            priortys[priorty] = priortyList;
            if ( priortyKeys.Contains( priorty) == false)
            {
                priortyKeys.Add(priorty);
                priortyKeys.Sort();
            }
        }
        else
        {
            priortyList = (LinkedList<TimeScheduleNode>)priortys[priorty]; //change from original
        }
        _node.bUpdateLock = bUpdateLock;
        priortyList.AddLast(_node);
        entityCount++;
    }

    public void RemoveSchedule(TimeScheduleNode _node)
    {
        if (_node != null)
        {
            _node.Finished = true;
            _node.ResetCallback(null);
        }
    }

    public void UpdateWithTime( float delta, float realDelta)
    {
        bUpdateLock = true;
        if (priortyKeys != null && priortyKeys.Count > 0)
        {
            // 只遍历update 之前发生添加的key, 触发中新加的等到下轮循环触发
            ArrayList akeys = new ArrayList(priortyKeys);
            int count = akeys.Count;
            for (int idx = 0; idx < count; idx++)
            {
                int priorty = (int)akeys[idx];
                LinkedList<TimeScheduleNode> priortyList = (LinkedList<TimeScheduleNode>)priortys[priorty];
                LinkedListNode<TimeScheduleNode> curNode = priortyList.First;
                while (curNode != null)
                {
                    var next = curNode.Next;
                    // 准备下一个遍历节点
                    TimeScheduleNode node = curNode.Value;
                    if (node.bUpdateLock == false)
                    {
                        // 如果和unity 的Object 关联生命周期的话 当Object 为nil是 删除该遍历节点
                        // 或者node 已经finished
                        if ((node.bLifeTimeRef == true && node.RefObject == null) ||
                            node.Finished == true)
                        {
                            priortyList.Remove(curNode);
                            entityCount--;
                        }
                        else
                        {
                            if (!node.Paused)
                            {
                                float fixDelta = (node.RealTime == true) ? (realDelta) : (delta);
                                node.Update(fixDelta);
                                if (node.Finished == true)
                                {
                                    priortyList.Remove(curNode);
                                    entityCount--;
                                }
                            }
                        }
                    }
                    node.bUpdateLock = false;
                    curNode = next;
                }
            }
        }
        bUpdateLock = false;
    }
};

public class TimeScheduler : MonoBehaviour {

    static TimeScheduler defaultScheduler;

    static public TimeScheduler Instance()
    {
        if( !defaultScheduler)
        {
            GameObject schdulerNode = new GameObject("Default Time Schduler");
            // Add the NotificationCenter component, and set it as the defaultCenter
            defaultScheduler = schdulerNode.AddComponent<TimeScheduler>();
            DontDestroyOnLoad(schdulerNode);
        }
        return defaultScheduler;
    }

    private void Start()
    {
        StartCoroutine(onCoroutineUpdate());
    }

    private TimeSchedulePriorty coroutinePriortys = new TimeSchedulePriorty();
    private TimeSchedulePriorty updatePriortys = new TimeSchedulePriorty();
    //private TSchedulerPriorty framePriortys = new TSchedulerPriorty();

    public TimeScheduleNode Schedule( System.Action<TimeScheduleNode, float> _cb, float interval = 0, TimeSchedulerType _type = TimeSchedulerType.CoroutineUpdate, int priority = 0)
    {
        if (_cb != null)
        {
            TimeScheduleNode timer = new TimeScheduleNode(_cb, _type, priority);
            timer.Interval = interval;
            if (_type == TimeSchedulerType.CoroutineUpdate)
            {
                coroutinePriortys.AddScheduler(timer, priority);
            }
            else if (_type == TimeSchedulerType.Update)
            {
                updatePriortys.AddScheduler(timer, priority);
            }
            return timer;
        }
        return null;
    }

    public TimeScheduleNode ScheduleOnce( System.Action<TimeScheduleNode, float> _cb, float interval = 0, TimeSchedulerType _type = TimeSchedulerType.CoroutineUpdate, int priority = 0)
    {
        TimeScheduleNode timer = Schedule( _cb, interval, _type, priority);
        timer.Repeat = 1;
        timer.Interval = interval;
        return timer;
    }

    public void RemoveSchedule( TimeScheduleNode _node)
    {
        if ( _node != null)
        {
            _node.Finished = true;
            _node.ResetCallback(null);
        }
    }

    private void Update()
    {
        updatePriortys.UpdateWithTime(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private IEnumerator onCoroutineUpdate()
    {
        while( true)
        {
            yield return new WaitForEndOfFrame();
            coroutinePriortys.UpdateWithTime(Time.deltaTime, Time.unscaledDeltaTime);
            if( coroutinePriortys.entityCount <= 0)
            {
                coroutinePriortys.entityCount = 0;
                yield return new WaitWhile(() => coroutinePriortys.entityCount > 0);
            }
        }
    }
}
