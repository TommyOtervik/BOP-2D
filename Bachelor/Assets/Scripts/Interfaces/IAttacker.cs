using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker<T>
{
    void Attack(T damage);
}
