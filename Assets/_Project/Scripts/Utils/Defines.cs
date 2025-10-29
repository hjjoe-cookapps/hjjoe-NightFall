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
        Move,
        Attack,
        Skill,
        Dead
    }

    public enum MonsterState
    {
        Move,
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
        Return,
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
        Monster_1_1_1_Mouse,
        Monster_1_1_2_Pot,
        Monster_1_1_3_Bunny,
        Monster_1_1_Boss_Treeman
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
    }

}
