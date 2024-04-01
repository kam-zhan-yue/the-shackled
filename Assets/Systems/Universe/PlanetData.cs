using System;
using Sirenix.OdinInspector;

[Serializable]
public class PlanetData
{
    [TableColumnWidth(10)]
    public string id;
    
    [TableColumnWidth(80)]
    public Planet prefab;
    
    [TableColumnWidth(10)]
    public bool moons;
}