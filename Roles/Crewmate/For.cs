using System.Collections.Generic;
using AmongUs.GameOptions;
using TheDarkRoles.Roles.Core;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace TheDarkRoles.Roles.Crewmate
{
    public sealed class For : RoleBase
    {
        public static Dictionary<byte, int> Lives = [];
        public static Dictionary<byte, int> Votes = [];
        public static OptionItem ForLives;
        public static OptionItem ForVotes;

        public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(For),
            player => new For(player),
            CustomRoles.For,
            () => RoleTypes.Engineer,
            CustomRoleTypes.Crewmate,
            2600,
            SetupOptionItem,
            "for",
            "#8a4b13"
        );

        public For(PlayerControl player) : base(RoleInfo, player)
        {
            if (!Lives.ContainsKey(player.PlayerId)) Lives[player.PlayerId] = ForLives.GetInt();
            if (!Votes.ContainsKey(player.PlayerId)) Votes[player.PlayerId] = ForVotes.GetInt();
        }

        public static void SetupOptionItem()
        {
            ForLives = IntegerOptionItem.Create(RoleInfo, 10, "ForLives", new(0, 99, 1), 2, false);
            ForVotes = IntegerOptionItem.Create(RoleInfo, 11, "ForVotes", new(0, 99, 1), 2, false);
        }

        public override bool OnCheckMurderAsTarget(MurderInfo info)
        {
            return TryHandleCheckMurderAsTarget(info);
        }

        public override (byte? votedForId, int? numVotes, bool doVote) ModifyVote(byte voterId, byte sourceVotedForId, bool isIntentional)
        {
            var (votedForId, numVotes, doVote) = base.ModifyVote(voterId, sourceVotedForId, isIntentional);
            if (voterId == Player.PlayerId)
                numVotes = Votes[voterId];
            return (votedForId, numVotes, doVote);
        }

        public static void ApplySpeed(PlayerControl player) => Main.AllPlayerSpeed[player.PlayerId] *= 1.333f;

        public override string GetProgressText(bool comms = false) => Utils.ColorString(Utils.GetRoleColor(CustomRoles.For), $"♥ {Lives[Player.PlayerId]}");

        public static bool TryHandleCheckMurderAsTarget(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            if (target == null && killer == null) return true;
            if (!target.Is(CustomRoles.For) && PlayerState.GetByPlayerId(target.PlayerId)?.MainRole != CustomRoles.For) return true;
            if (!Lives.TryGetValue(target.PlayerId, out var remainingLives)) return true;

            if (remainingLives > 0)
            {
                Lives[target.PlayerId] = remainingLives - 1;
                try
                {
                    killer.SetKillCooldown();
                }
                catch
                {
                    killer.ResetKillCooldown();
                }
                Utils.NotifyRoles();
                return false;
            }
            return true;
        }
    }
}
