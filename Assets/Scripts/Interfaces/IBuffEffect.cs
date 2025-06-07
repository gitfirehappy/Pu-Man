using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffEffect
{
    void Apply(PlayerCore player);
    void Remove(PlayerCore player); // 可选，用于移除Buff
}

