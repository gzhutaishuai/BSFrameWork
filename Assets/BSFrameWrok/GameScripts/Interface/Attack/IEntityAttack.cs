using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackDamage
{
    public int comonDamage;
    public int buildingDamage;
}

public interface IEntityAttack 
{
    AttackDamage attackDamage { get; }
}
