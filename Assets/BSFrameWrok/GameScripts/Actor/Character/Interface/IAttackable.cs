using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    UnitAttack unitAttack { get; set; }

    void Attack();
}
