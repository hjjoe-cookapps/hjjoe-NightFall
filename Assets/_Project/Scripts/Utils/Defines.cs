using UnityEngine;

namespace _Project.Scripts.Defines
{
    public enum AttackType
    {
        Melee,
        Ranged
    }


    public enum PlayerState
    {
        Idle,
        Attack,
        Skill
    }

    public enum MonsterState
    {
        Walk,
        Chase,
        Attack,
        Dead
    }

    public static class Defines
    {
        public static readonly LayerMask FriendLayer = LayerMask.GetMask("Player", "Building", "Unit");
        public static readonly LayerMask MonsterLayer = LayerMask.GetMask("Monster");

        public static class Player
        {
            public static readonly Quaternion RightRotation =  Quaternion.Euler(0f, 0f, 0f);
            public static readonly Quaternion LeftRotation =  Quaternion.Euler(0f, 180f, 0f);
        }

        public static class Monster
        {
            public static readonly Quaternion RightRotation =  Quaternion.Euler(0f, 180f, 0f);
            public static readonly Quaternion LeftRotation =  Quaternion.Euler(0f, 0f, 0f);
        }

    }

}
