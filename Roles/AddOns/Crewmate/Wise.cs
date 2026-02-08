using System.Collections.Generic;
using UnityEngine;
using TheDarkRoles.Attributes;
using TheDarkRoles.Roles.Core;
using static TheDarkRoles.Options;
using TheDarkRoles.Roles.AddOns.Common;
using System.Linq;

namespace TheDarkRoles.Roles.AddOns.Crewmate
{
    public static class Wise
    {
        private static readonly int Id = 80500;
        private static Color RoleColor = Utils.GetRoleColor(CustomRoles.Wise);
        public static string SubRoleMark = Utils.ColorString(RoleColor, "(Wise)");
        private static List<byte> playerIdList = [];

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.Addons, CustomRoles.Wise, RoleColor, new(1, 1, 1));
            AddOnsAssignData.Create(Id + 10, CustomRoles.Wise, true, true, true);
        }

        public static void OnFirstMeeting(PlayerControl pc)
        {
            var random = IRandom.Instance;
            var alivePlayers = Main.AllAlivePlayerControls as IList<PlayerControl> ?? [.. Main.AllAlivePlayerControls];

            if (alivePlayers.Count <= 1) return;
            PlayerControl target;
            do
                target = alivePlayers[random.Next(alivePlayers.Count)];
            while (target == null || target == pc);
            Utils.SendMessage($"You get a feeling theres a {target.GetCustomRole()} in the lobby.", pc.PlayerId);
        }

        [GameModuleInitializer]
        public static void Init() => playerIdList = [];
        public static void Add(byte playerId) => playerIdList.Add(playerId);
        public static bool IsEnable => playerIdList.Count > 0;
        public static bool IsThisRole(byte playerId) => playerIdList.Contains(playerId);
    }
}
