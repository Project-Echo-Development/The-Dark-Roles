using System.Collections.Generic;
using UnityEngine;
using TheDarkRoles.Attributes;
using TheDarkRoles.Roles.Core;
using static TheDarkRoles.Options;

namespace TheDarkRoles.Roles.AddOns.Common
{
    public static class Flash
    {
        private static readonly int Id = 80300;
        private static Color RoleColor = Utils.GetRoleColor(CustomRoles.Flash);
        public static string SubRoleMark = Utils.ColorString(RoleColor, "(Flash)");
        private static List<byte> playerIdList = [];
        public static OptionItem OptionFlashSpeed;

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.Addons, CustomRoles.Flash, RoleColor, new(1, 1, 1));
            AddOnsAssignData.Create(Id + 10, CustomRoles.Flash, true, true, true);
            OptionFlashSpeed = FloatOptionItem.Create(Id + 20, "FlashSpeedMultiplier", new(1.1f, 5.0f, 0.1f), 1.5f, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Flash])
                .SetValueFormat(OptionFormat.Multiplier);

        }

        [GameModuleInitializer]
        public static void Init() => playerIdList = [];
        public static void Add(byte playerId) => playerIdList.Add(playerId);
        public static bool IsEnable => playerIdList.Count > 0;
        public static bool IsThisRole(byte playerId) => playerIdList.Contains(playerId);
        public static void DoSpeed(PlayerControl player) => Main.AllPlayerSpeed[player.PlayerId] *= OptionFlashSpeed.GetFloat();

    }
}
