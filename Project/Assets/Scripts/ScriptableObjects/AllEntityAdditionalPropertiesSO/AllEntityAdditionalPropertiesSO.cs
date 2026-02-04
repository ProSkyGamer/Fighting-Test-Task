#region

using System.Collections.Generic;
using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "AllEntityAdditionalProperties", menuName = "Scriptable Objects/AllEntityAdditionalProperties")]
public class AllEntityAdditionalPropertiesSO : ScriptableObject
{
    public List<EntityBaseAdditionalProperty> allEntityAdditionalProperties;
}