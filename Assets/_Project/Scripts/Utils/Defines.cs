using System.ComponentModel;
using UnityEngine;

namespace _Project.Scripts.Defines
{
    public static class Defines
    {
        public static readonly LayerMask FriendLayer = LayerMask.GetMask("Player", "Building", "Unit");
        public static readonly LayerMask MonsterLayer = LayerMask.GetMask("Monster");

        public static readonly float HitRange = 0.5f;
    }

}
