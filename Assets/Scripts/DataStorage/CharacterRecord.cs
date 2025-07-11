using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterRecord
{
    public PlayerType playerType; // 使用枚举作为唯一标识
    public int highestWave;
}
