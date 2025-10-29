using System.Collections.Generic;

public class BarracksBehaviour : BuildingBehaviour
{
   private HashSet<UnitBehaviour> _unitBehaviours = new();

    public override void StartWave()
    {
        base.StartWave();
        foreach (var unit in _unitBehaviours)
        {
            unit.StartWave();
        }
    }

    public override void EndWave()
    {
        base.EndWave();
        foreach (var unit in _unitBehaviours)
        {
            unit.EndWave();
        }
    }

    protected virtual void Active()
    {

    }
}
