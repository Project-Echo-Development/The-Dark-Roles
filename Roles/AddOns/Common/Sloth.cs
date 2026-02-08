using System.Collections.Generic;
using UnityEngine;
using TheDarkRoles.Attributes;
using TheDarkRoles.Roles.Core;
using static TheDarkRoles.Options;

namespace TheDarkRoles.Roles.AddOns.Common
{
    public static class Sloth
    {
        private static readonly int Id = 80400;
        private static Color RoleColor = Utils.GetRoleColor(CustomRoles.Sloth);
        public static string SubRoleMark = Utils.ColorString(RoleColor, "(Sloth)");
        private static List<byte> playerIdList = [];
        public static OptionItem OptionSlothSpeed;

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.Addons, CustomRoles.Sloth, RoleColor, new(1, 1, 1));
            AddOnsAssignData.Create(Id + 10, CustomRoles.Sloth, true, true, true);
            OptionSlothSpeed = FloatOptionItem.Create(Id + 20, "SlothSpeedMultiplier", new(0.1f, 0.9f, 0.1f), 0.7f, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Sloth])
                .SetValueFormat(OptionFormat.Multiplier);

        }

        [GameModuleInitializer]
        public static void Init() => playerIdList = [];
        public static void Add(byte playerId) => playerIdList.Add(playerId);
        public static bool IsEnable => playerIdList.Count > 0;
        public static bool IsThisRole(byte playerId) => playerIdList.Contains(playerId);
        public static void DoSpeed(PlayerControl player) => Main.AllPlayerSpeed[player.PlayerId] *= OptionSlothSpeed.GetFloat();

    }
}
