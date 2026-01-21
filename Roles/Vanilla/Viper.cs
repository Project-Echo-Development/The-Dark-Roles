using AmongUs.GameOptions;
using TheDarkRoles.Roles.Core;
using TheDarkRoles.Roles.Core.Interfaces;

namespace TownOfHost.Roles.Vanilla;

public sealed class Viper : RoleBase, IImpostor
{
    public Viper(PlayerControl player) : base(RoleInfo, player) { }
    public static readonly SimpleRoleInfo RoleInfo = SimpleRoleInfo.CreateForVanilla(typeof(Viper), player => new Viper(player), RoleTypes.Viper);
}