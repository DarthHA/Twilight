using Terraria.ModLoader;

namespace Twilight
{
    public class TwilightGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool CanKill = true;
    }
}