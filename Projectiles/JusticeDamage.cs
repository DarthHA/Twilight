using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Twilight.Projectiles
{
    public class JusticeDamage : ModProjectile
    {
        public override string Texture => "Twilight/Projectiles/Justice1";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Justitia");
            DisplayName.AddTranslation(GameCulture.Chinese, "正义裁决");

        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 1;
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

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            float percent = Twilight.SinPaleDamage * target.GetGlobalNPC<ApoBirdGNPC>().Sins;
            if (target.realLife == -1)
            {
                int dmg = (int)(target.lifeMax * percent);

                if (dmg < target.life)
                {
                    target.life -= dmg;
                }
                else
                {
                    target.life = 0;
                    target.checkDead();
                }
                CombatText.NewText(target.Hitbox, Color.Cyan, dmg);
            }
            else
            {
                NPC owner = Main.npc[target.realLife];
                if (owner.active)
                {
                    int count = SegmentCounts(owner);
                    if (count == 0) count = 1;
                    int dmg = (int)(target.lifeMax * percent) / (int)Math.Pow(count, 0.666);
                    damage /= (int)Math.Pow(count, 0.666);
                    if (dmg < owner.life)
                    {
                        owner.life -= dmg;
                    }
                    else
                    {
                        owner.life = 0;
                        owner.checkDead();
                    }
                    CombatText.NewText(target.Hitbox, Color.Cyan, dmg);
                }
            }
            if (target.GetGlobalNPC<ApoBirdGNPC>().Sins > 0)
            {
                Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect3>(), 0, 0);
                target.GetGlobalNPC<ApoBirdGNPC>().Sins = 0;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].statLife = Utils.Clamp(Main.player[projectile.owner].statLife + 1, 0, Main.player[projectile.owner].statLifeMax2);
            Main.player[projectile.owner].HealEffect(1);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public int SegmentCounts(NPC npc)
        {
            int result = 0;
            foreach (NPC segs in Main.npc)
            {
                if (segs.realLife != -1 && segs.realLife == npc.whoAmI)
                {
                    result++;
                }
            }
            return result;
        }
    }
}