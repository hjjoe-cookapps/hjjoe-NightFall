using System.ComponentModel;
using UnityEngine;

namespace _Project.Scripts.Defines
{
    public enum Direction
    {
        Left,
        Right
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
        Dead,
        Default
    }

    public enum UnitState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

    public enum BuildingState
    {
        Wait,   // 낮에
        Idle,   // 밤에
        Crash
    }

    public enum AttackType
    {
        Melee,
        Ranged
    }

    public enum MonsterType
    {
        Melee,
        Ranged,
        Maneater,
        Sky
    }

    public enum BulidingType
    {
        Castle,
        Tower,
        Wall,
        Barracks,
        House,
        Farm,
        Field
    }

    public enum UnitType
    {

    }

    public enum Sound
    {
        BGM,
        Effect
    }


    public static class Defines
    {
        public static readonly LayerMask FriendLayer = LayerMask.GetMask("Player", "Building", "Unit");
        public static readonly LayerMask MonsterLayer = LayerMask.GetMask("Monster");

        public static readonly float HitRange = 2f;

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
