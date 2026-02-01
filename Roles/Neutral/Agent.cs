using System.Collections.Generic;
using AmongUs.GameOptions;
using TheDarkRoles.Roles.Core;
using TheDarkRoles.Roles.Core.Interfaces;

namespace TheDarkRoles.Roles.Neutral
{
    public sealed class Agent : RoleBase, IKiller
    {
        public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Agent),
            player => new Agent(player),
            CustomRoles.Agent,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Neutral,
            51600,
            SetupOptionItem,
            "ag",
            "#ff6633",
            true,
            countType: CountTypes.Agent,
            introSound: () => GetIntroSound(RoleTypes.Crewmate)
        );

        public Agent(PlayerControl player) : base(RoleInfo, player, () => HasTask.False)
        {
            Marked = new(GameData.Instance.PlayerCount);
        }

        public static void SetupOptionItem()
        {
            MarkCooldown = FloatOptionItem.Create(RoleInfo, 11, "AgentMarkCooldown", new(5f, 100f, 1f), 10f, false).SetValueFormat(OptionFormat.Seconds);
            CanVent = BooleanOptionItem.Create(RoleInfo, 12, "AgentCanVent", true, false);
        }

        public static Dictionary<byte, bool> Marked;
        public static OptionItem MarkCooldown;
        public static OptionItem CanVent;

        public override void Add()
        {
            foreach (var player in Main.AllPlayerControls)
                Marked.Add(player.PlayerId, false);
        }

        public void OnCheckMurderAsKiller(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            if (!IsMarked(target.PlayerId) && target is not null)
                Marked[target.PlayerId] = true;
            info.DoKill = false;
        }

        public override void OnStartMeeting()
        {
            if (GameStates.IsInGame)
            {
                foreach (var player in Main.AllPlayerControls)
                {
                    if (Marked[player.PlayerId] && player.PlayerId != Player.PlayerId)
                    {
                        player.SetRealKiller(Player);
                        player.RpcMurderPlayer(player);
                        var state = PlayerState.GetByPlayerId(player.PlayerId);
                        state.DeathReason = CustomDeathReason.Hit;
                        state.SetDead();
                        Marked.Clear();
                    }
                }
            }
        }

        public override string GetMark(PlayerControl seer, PlayerControl seen, bool isForMeeting = false)
        {
            seen ??= seer;
            if (IsMarked(seen.PlayerId))
                return Utils.ColorString(RoleInfo.RoleColor, "⦿");
            return "";
        }

        public bool IsMarked(byte targetId) => Marked.TryGetValue(targetId, out bool marked) && marked;

        public bool CanUseKillButton() => true;
        public bool CanUseImpostorVentButton() => CanVent.GetBool();
        public float CalculateKillCooldown() => MarkCooldown.GetFloat();
        public bool CanUseSabotageButton() => false;
    }
}
