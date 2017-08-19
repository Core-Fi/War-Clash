using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdate
{
    void Update(float deltaTime);
}
public interface IFixedUpdate
{
    void FixedUpdate(long deltaTime);
}
