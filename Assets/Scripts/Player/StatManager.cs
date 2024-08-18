using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static string Health = "Health";
    public static string Food = "Food";

    [SerializeField] private List<Stat> stats;

    public Stat GetStat(string name)
    {
        foreach(Stat stat in stats)
        {
            if(stat.statName == name) return stat;
        }
        return null;
    }

    public void ResetStat(string name)
    {
        foreach (Stat stat in stats)
        {
            if (stat.statName == name) stat.Value = stat.MaxValue;
        }
    }

    public void ResetStats()
    {
        foreach(Stat stat in stats)
        {
            stat.Value = stat.InitialValue;
        }
    }
}
