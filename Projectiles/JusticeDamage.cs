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
            // DisplayName.SetDefault("Justitia");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "正义裁决");

        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 1;
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float percent = TwilightData.SinPaleDamage * target.GetGlobalNPC<ApoBirdGNPC>().Sins;
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
                Main.player[Projectile.owner].addDPS(dmg);
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
                    modifiers.FinalDamage /= (int)Math.Pow(count, 0.666);
                    if (dmg < owner.life)
                    {
                        owner.life -= dmg;
                    }
                    else
                    {
                        owner.life = 0;
                        owner.checkDead();
                    }
                    Main.player[Projectile.owner].addDPS(dmg);
                    CombatText.NewText(target.Hitbox, Color.Cyan, dmg);
                }
            }
            if (target.GetGlobalNPC<ApoBirdGNPC>().Sins > 0)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect3>(), 0, 0);
                target.GetGlobalNPC<ApoBirdGNPC>().Sins = 0;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].statLife = Utils.Clamp(Main.player[Projectile.owner].statLife + 1, 0, Main.player[Projectile.owner].statLifeMax2);
            Main.player[Projectile.owner].HealEffect(1);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        internal int SegmentCounts(NPC npc)
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