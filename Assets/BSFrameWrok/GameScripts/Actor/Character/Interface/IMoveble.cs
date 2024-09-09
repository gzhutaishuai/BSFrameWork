using UnityEngine;
using UnityEngine.AI;

public interface IMoveable
{
    NavMeshAgent agent { get; set; }

    void Move(Vector3 targetPos);

    WaitUntil WaitForMesh();
}
