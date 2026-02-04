#region

using System.Collections.Generic;
using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "AllBoostersSO", menuName = "Scriptable Objects/AllBoostersSO")]
public class AllBoostersSO : ScriptableObject
{
    public List<BaseBoosterSingle> allBoostersSingle;
}