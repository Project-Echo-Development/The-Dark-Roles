using System.Collections.Generic;
using AmongUs.GameOptions;

using TheDarkRoles.Roles.Core;
using TheDarkRoles.Roles.Core.Interfaces;

namespace TheDarkRoles.Roles.Crewmate;
public sealed class Magician : RoleBase
{
    public static Dictionary<byte, bool> HasVented = new();
    private static OptionItem OptionVentCooldown;
    public static int vent;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Magician),
            player => new Magician(player),
            CustomRoles.Magician,
            () => RoleTypes.Engineer,
            CustomRoleTypes.Crewmate,
            21100,
            SetupOptionItem,
            "mag",
            "#E500F8"
        );
    public Magician(PlayerControl player) : base(RoleInfo, player) { }
  
    private static void SetupOptionItem()
    {
        OptionVentCooldown = FloatOptionItem.Create(RoleInfo, 10, "MagicianVentCooldown", new(0f, 90f, 2.5f), 10f, false)
            .SetValueFormat(OptionFormat.Seconds);
    }
    public override void ApplyGameOptions(IGameOptions opt)
    {
        AURoleOptions.EngineerInVentMaxTime = 1;
        AURoleOptions.EngineerCooldown = OptionVentCooldown.GetFloat();
    }

    public static void OnExitVent(PlayerControl pc) => pc.RpcRandomVentTeleport();
}