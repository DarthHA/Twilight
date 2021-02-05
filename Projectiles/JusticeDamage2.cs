using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace Twilight.Projectiles
{
    public class JusticeDamage2 : ModProjectile
    {
        public override string Texture => "Twilight/Projectiles/Justice1";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tilted Scale");
            DisplayName.AddTranslation(GameCulture.Chinese, "倾斜的天平");
        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.hide = true;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            projectile.position = Main.screenPosition;
            projectile.width = Main.screenWidth;
            projectile.height = Main.screenHeight;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].statLife = Utils.Clamp(Main.player[projectile.owner].statLife + 1, 0, Main.player[projectile.owner].statLifeMax2);
            Main.player[projectile.owner].HealEffect(1);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(projectile.Center) <= 2000;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}