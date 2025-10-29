using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Defines
{
    [Serializable]
    public struct StageStatus
    {
        public List<WaveStatus> Waves;
    }

    [Serializable]
    public struct WaveStatus
    {
        public List<TroopStatus> Troops;
    }

    [Serializable]
    public struct TroopStatus
    {
        public List<GeneratorStatus> Generators;
    }

    [Serializable]
    public struct GeneratorStatus
    {
        public MonsterType Type;
        public int GenerateCount;
        public Vector3 Position;
    }
}
