using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace Twilight.Projectiles
{
    public class JusticeDamage2 : ModProjectile
    {
        public override string Texture => "Twilight/Projectiles/Justice1";
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 10;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.hide = true;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.position = Main.screenPosition;
            Projectile.width = Main.screenWidth;
            Projectile.height = Main.screenHeight;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].statLife = Utils.Clamp(Main.player[Projectile.owner].statLife + 1, 0, Main.player[Projectile.owner].statLifeMax2);
            Main.player[Projectile.owner].HealEffect(1);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) <= 2000;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}