using System;
using System.Runtime.Serialization;
using Vintagestory.API.Server;
using System.Collections.Generic;
using System.Drawing;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Server;

class RestrictedRole : IComparable<IPlayerRole>, IPlayerRole {

    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int PrivilegeLevel { get; set; }

    public PlayerSpawnPos DefaultSpawn { get; set; }

    public PlayerSpawnPos ForcedSpawn { get; set; }

    public List<string> Privileges { get; set; }

    public HashSet<string> RuntimePrivileges { get; set; }

    public EnumGameMode DefaultGameMode { get; set; }

    public Color Color { get; set; }

    public int LandClaimAllowance { get; set; }

    public Vec3i LandClaimMinSize { get; set; } = new Vec3i(5, 5, 5);


    public int LandClaimMaxAreas { get; set; } = 3;


    public bool AutoGrant { get; set; }

    public RestrictedRole()
    {
        PrivilegeLevel = 0;
        Privileges = new List<string>();
        RuntimePrivileges = new HashSet<string>();
        Color = Color.White;
    }

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        if (Privileges == null)
        {
            Privileges = new List<string>();
        }
        if (RuntimePrivileges == null)
        {
            RuntimePrivileges = new HashSet<string>();
        }
    }

    public void GrantPrivilege(params string[] privileges)
    {
        foreach (string item in privileges)
        {
            if (!Privileges.Contains(item))
            {
                Privileges.Add(item);
            }
        }
    }

    public void RevokePrivilege(string privilege)
    {
        Privileges.Remove(privilege);
    }

    public int CompareTo(IPlayerRole other)
    {
        return PrivilegeLevel.CompareTo(other.PrivilegeLevel);
    }

    public bool IsSuperior(IPlayerRole clientGroup)
    {
        if (clientGroup == null)
        {
            return true;
        }
        return PrivilegeLevel > clientGroup.PrivilegeLevel;
    }

    public bool EqualLevel(IPlayerRole clientGroup)
    {
        return PrivilegeLevel == clientGroup.PrivilegeLevel;
    }

    public override string ToString()
    {
        return $"{Code}:{Name}:{PrivilegeLevel}:{ServerConfig.PrivilegesString(Privileges)}:{Color.ToString()}";
    }

}