using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IBuffEffect.cs
public interface IBuffEffect
{
    void Apply(BuffSO buffData, PlayerCore player);
    void Remove(BuffSO buffData, PlayerCore player);
}

