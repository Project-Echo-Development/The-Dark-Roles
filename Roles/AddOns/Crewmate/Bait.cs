using System.Collections.Generic;
using TheDarkRoles.Attributes;
using TheDarkRoles.Roles.AddOns.Common;
using TheDarkRoles.Roles.Core;
using UnityEngine;
using static TheDarkRoles.Options;

namespace TheDarkRoles.Roles.AddOns.Crewmate
{
    public static class Bait
    {
        private static readonly int Id = 80600;
        private static Color RoleColor = Utils.GetRoleColor(CustomRoles.Bait);
        public static string SubRoleMark = Utils.ColorString(RoleColor, "(Bait)");
        private static List<byte> playerIdList = [];

        public static void SetupCustomOption()
        {
            SetupRoleOptions(Id, TabGroup.Addons, CustomRoles.Bait, RoleColor, new(1, 1, 1));
            AddOnsAssignData.Create(Id + 10, CustomRoles.Bait, true, false, false);
        }

        [GameModuleInitializer]
        public static void Init() => playerIdList = [];
        public static void Add(byte playerId) => playerIdList.Add(playerId);
        public static bool IsEnable => playerIdList.Count > 0;
        public static bool IsThisRole(byte playerId) => playerIdList.Contains(playerId);

        public static void OnBaitDeath(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            if (target.Is(CustomRoles.Bait) && !info.IsSuicide)
                _ = new LateTask(() => killer.CmdReportDeadBody(target.Data), 0.15f, "Bait Self Report");
            Logger.Info($"Bait OnBaitDeath: killer={killer?.PlayerId}, target={target.PlayerId}, IsSuicide={info.IsSuicide}", "Bait Report");
        }
    }
}