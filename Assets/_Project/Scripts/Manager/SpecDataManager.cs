using JetBrains.Annotations;

public partial class SpecDataManager : Singleton<SpecDataManager>
{
    protected override void Init()
    {
        LoadFromResource();
    }

    [CanBeNull]
    public BuildingStatus GetBuildingStatus(BuildingBehaviour instance, int level)
    {
        int id = 0;
        switch (instance)
        {
            case CastleBehaviour castle:
                id = (int) BuildingType.Castle * 100 + level;
                break;
            case FarmBehaviour farm:
                id = (int) BuildingType.House * 100 + level;
                break;
            case BarracksBehaviour barracks:
                id = (int) BuildingType.Barracks * 100 + level;
                break;
            case TowerBehaviour tower:
                id = (int) BuildingType.Tower * 100 + level;
                break;
            default:
                id = (int) BuildingType.Wall * 100 + level;
                break;
        }

        return BuildingStatus[id];
    }

    public MonsterStatus GetMonsterStatus(MonsterType type)
    {
        return MonsterStatus[(int) type];
    }

    public UnitStatus GetUnitStatus(UnitType type)
    {
        return UnitStatus[(int) type];
    }

    public UnitStatus GetUnitStatus(UnitBehaviour instance)
    {
        return UnitStatus[(int) instance.UnitType];
    }

    public TowerStatus GetTowerStatus(TowerBehaviour instance)
    {
        return TowerStatus[instance.Level];
    }

    public TowerStatus GetTowerStatus(int level)
    {
        return TowerStatus[level];
    }
}
