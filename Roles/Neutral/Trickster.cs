using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using TheDarkRoles.Roles.Core;
using TheDarkRoles.Roles.Core.Interfaces;

namespace TheDarkRoles.Roles.Neutral
{
    public sealed class Trickster : RoleBase, IKiller
    {
        public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Trickster),
            player => new Trickster(player),
            CustomRoles.Trickster,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Neutral,
            51700,
            SetupOptionItem,
            "tri",
            "#910f34",
            true,
            introSound: () => GetIntroSound(RoleTypes.Viper)
        );

        public static Dictionary<byte, byte> SwapPlayer = [];
        public static Dictionary<byte, bool> SwapSelected = [];
        public static OptionItem CanVent;

        static Trickster()
        {
            CustomRoleManager.OnMurderPlayerOthers.Add(OnAnyMurder);
        }

        public Trickster(PlayerControl player) : base(RoleInfo, player, () => HasTask.False) { }

        public static void SetupOptionItem()
        {
            CanVent = BooleanOptionItem.Create(RoleInfo, 12, "TricksterCanVent", true, false);
        }

        public void OnCheckMurderAsKiller(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            if (killer == null || target == null)
            {
                info.DoKill = false;
                return;
            }

            SwapPlayer ??= [];
            SwapSelected ??= [];

            if (SwapPlayer.ContainsKey(killer.PlayerId))
            {
                info.DoKill = false;
                return;
            }

            if (SwapPlayer.ContainsValue(target.PlayerId))
            {
                info.DoKill = false;
                return;
            }

            SwapPlayer[killer.PlayerId] = target.PlayerId;
            SwapSelected[target.PlayerId] = true;

            Utils.NotifyRoles();
            info.DoKill = false;
        }

        // Handling the swap kill when a Trickster is attempt murdered.
        // We once again need alot of safety checks due to black screens.
        public override bool OnCheckMurderAsTarget(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            if (!SwapPlayer.TryGetValue(target.PlayerId, out var mappedId)) return true;
            var killTarget = Utils.GetPlayerById(mappedId);
            var killTargetState = PlayerState.GetByPlayerId(killTarget.PlayerId);
            if (target == null || killer == null) return true;
            if (!target.Is(CustomRoles.Trickster)) return true;
            if (SwapSelected == null || SwapPlayer == null) return true;
            if (killTarget == null) return true;
            if (killTargetState == null) return true;
            if (killTargetState.IsDead) return true;

            killer.RpcMurderPlayerV2(killTarget);
            killTargetState.DeathReason = CustomDeathReason.TricksterSwap;
            killTargetState.SetDead();

            SwapPlayer.Remove(target.PlayerId);
            SwapSelected.Remove(mappedId);
            Utils.NotifyRoles();
            return false;
        }

        public override string GetMark(PlayerControl seer, PlayerControl seen, bool isForMeeting = false)
        {
            seen ??= seer;
            if (SwapSelected != null && SwapSelected.TryGetValue(seen.PlayerId, out bool marked) && marked)
                return Utils.ColorString(RoleInfo.RoleColor, "✓");
            return "";
        }

        public override void OnDestroy()
        {
            SwapPlayer?.Clear();
            SwapSelected?.Clear();
        }

        public bool IsSwapSelected(byte playerId, byte targetId)
        {
            if (SwapPlayer == null || SwapSelected == null) return false;
            if (!SwapPlayer.TryGetValue(playerId, out byte targetPlayerId)) return false;
            if (targetPlayerId != targetId) return false;
            return SwapSelected.TryGetValue(targetId, out bool selected) && selected;
        }

        /// <summary>
        /// We need to check if the marked target is dead...
        /// It would break the whole mod if it tries to kill a null player.
        /// Black screens noted in testing in this scenario.
        /// </summary>
        /// <param name="info"></param>
        private static void OnAnyMurder(MurderInfo info)
        {
            var target = info.AttemptTarget;
            if (target == null) return;

            var targetId = target.PlayerId;
            if (SwapSelected == null || SwapPlayer == null) return;
            if (!SwapSelected.ContainsKey(targetId)) return;

            var toRemove = SwapPlayer.Where(kv => kv.Value == targetId).Select(kv => kv.Key).ToArray();
            if (toRemove.Length == 0)
            {
                SwapSelected.Remove(targetId);
                Utils.NotifyRoles();
                return;
            }

            foreach (var killerId in toRemove)
                SwapPlayer.Remove(killerId);
            SwapSelected.Remove(targetId);

            Utils.NotifyRoles();
        }

        public bool CanUseKillButton() => true;
        public bool CanUseImpostorVentButton() => CanVent.GetBool();
        public float CalculateKillCooldown() => 30f;
        public bool CanUseSabotageButton() => false;
    }
}
