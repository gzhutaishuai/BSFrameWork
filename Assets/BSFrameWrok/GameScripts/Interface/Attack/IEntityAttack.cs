using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackDamage
{
    public int comonDamage;
    public int buildingDamage;
    public float attackSpeed;
}

[System.Serializable]
public struct Defense
{
    public int baseDefense;
}
public interface IEntityAttack 
{
    AttackDamage attackDamage { get; }
}
