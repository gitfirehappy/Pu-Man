using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 可被对象池管理的对象接口
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 对象被对象池回收时调用
    /// </summary>
    void OnRelease();

    /// <summary>
    /// 对象从对象池获取时调用
    /// </summary>
    void OnGet();

    /// <summary>
    /// 设置管理此对象的对象池
    /// </summary>
    void SetPool(IObjectPool<GameObject> pool);
} 