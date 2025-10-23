using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PoolMonoBehaviour : MonoBehaviour
{
    public int PoolCreateCount = 10;

    public virtual void PoolRelease()
    {
    }
}
