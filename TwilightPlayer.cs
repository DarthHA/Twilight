using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Buffs;
using Twilight.Items;
using Twilight.Projectiles;
using Twilight.Sky;
using Twilight.UI;

namespace Twilight
{
    public class TwilightPlayer : ModPlayer
    {
        public int ApoBirdCD = 0;
        public int CurrentEgg = 0;
        public int EggTimer = 0;
        public int SpecialAttackCD = 0;
        public bool RightClick = false;
        public int PunishmentCD = 0;
        public override void UpdateBiomeVisuals()
        {
            player.ManageSpecialBiomeVisuals("Twilight:ApoBirdSky", true, default);
        }
        public override void PreUpdateBuffs()
        {
            if (Twilight.FindBody() != -1 && ApoBirdSky.CurrentState == 2)
            {
                player.AddBuff(ModContent.BuffType<ApocalypseBuff>(), 2);
                
                switch (CurrentEgg)
                {
                    case 0:
                        player.AddBuff(ModContent.BuffType<BigEyeBuff>(), 1200 - EggTimer);
                        break;
                    case 1:
                        player.AddBuff(ModContent.BuffType<LongArmBuff>(), 1200 - EggTimer);
                        break;
                    case 2:
                        player.AddBuff(ModContent.BuffType<SmallPeakBuff>(), 1200 - EggTimer);
                        break;
                    default:
                        break;
                }
                if (SpecialAttackCD > 0)
                {
                    player.AddBuff(ModContent.BuffType<SpecialAttackCDBuff>(), SpecialAttackCD);
                }
            }

        }

        public override void PostUpdateMiscEffects()
        {
            if (Twilight.FindBody() != -1 && ApoBirdSky.CurrentState == 2)
            {
                for (int i = 0; i < player.buffImmune.Length; i++)
                {
                    if (Main.debuff[i] && !Main.buffNoTimeDisplay[i] &&
                        i != BuffID.PotionSickness &&
                        i != BuffID.ManaSickness &&
                        i != ModContent.BuffType<ApocalypseBuff>() &&
                        i != ModContent.BuffType<BigEyeBuff>() &&
                        i != ModContent.BuffType<SmallPeakBuff>() &&
                        i != ModContent.BuffType<LongArmBuff>() &&
                        i != ModContent.BuffType<SpecialAttackCDBuff>())
                    {
                        player.buffImmune[i] = true;
                    }
                }
                player.maxMinions -= 999;
                player.maxTurrets -= 999;

            }
        }
        public override void ResetEffects()
        {
            if (Twilight.FindBody() != -1 && ApoBirdSky.CurrentState == 2)
            {
                EggTimer++;
                if (EggTimer > 1200)
                {
                    EggTimer = 0;
                    CurrentEgg = (CurrentEgg + 1) % 3;
                    player.GetModPlayer<TwilightUIPlayer>().Initialise(CurrentEgg);
                }
                if (Main.npc[Twilight.FindBody()].ai[1] == 0)
                {
                    if (SpecialAttackCD > 0)
                    {
                        SpecialAttackCD--;
                    }
                }
                if (!player.HasBuff(ModContent.BuffType<PunishmentBuff>()))
                {
                    if (PunishmentCD > 0)
                    {
                        PunishmentCD--;
                    }
                }
            }
            if (ApoBirdCD > 0)
            {
                ApoBirdCD--;
                player.AddBuff(ModContent.BuffType<TwilightCDBuff>(), ApoBirdCD);
            }
            RightClick = false;
        }

        public static float GetDamageBonus(Player player)
        {
            float dmg = 1;
            dmg *= (player.meleeDamage + player.rangedDamage + player.magicDamage + player.minionDamage - 3);
            if (player.statLife <= player.statLifeMax2 / 2)
            {
                dmg += 1;
            }
            return dmg;
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (player.HasBuff(ModContent.BuffType<BigEyeBuff>()))
            {
                damage = (int)(damage * 0.75f);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (player.HasBuff(ModContent.BuffType<BigEyeBuff>()))
            {
                damage = (int)(damage * 0.75f);
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (player.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
            {
                if (PunishmentCD == 0)
                {
                    player.AddBuff(ModContent.BuffType<PunishmentBuff>(), 300);
                }
            }

        }
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (player.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
            {
                if (PunishmentCD == 0)
                {
                    player.AddBuff(ModContent.BuffType<PunishmentBuff>(), 300);
                }
            }
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (FakeModPlayer.AnyLanterns2() != -1)
            {
                float k = Utils.Clamp(Twilight.SalvationCritRange - target.Distance(Main.player[EyeLantern.FakePlayer].Center), 0, Twilight.SalvationCritRange) / Twilight.SalvationCritRange;
                k *= 2;
                if (!crit)
                {
                    if (Main.rand.NextFloat() <= k)
                    {
                        crit = true;
                    }
                }
            }

            if (player.HasBuff(ModContent.BuffType<PunishmentBuff>()))
            {
                damage = (int)(damage * Twilight.PunishmentDamage);
                player.ClearBuff(ModContent.BuffType<PunishmentBuff>());
                PunishmentCD = 60;
            }
            if (player.HasBuff(ModContent.BuffType<LongArmBuff>()))
            {
                if (Main.rand.NextFloat() <= Twilight.SinChance)
                {
                    target.buffImmune[ModContent.BuffType<SinBuff>()] = false;
                    target.AddBuff(ModContent.BuffType<SinBuff>(), 99999);
                }
            }
            if (player.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
            {
                damage += (int)(damage * 0.06f * target.GetGlobalNPC<ApoBirdGNPC>().Sins);
            }
            if (player.HasBuff(ModContent.BuffType<LongArmBuff>()))
            {
                float percent = (float)target.life / target.lifeMax;
                if (percent == MaxLifePercent())
                {
                    if (!AnyJudgementEffect())
                    {
                        int protmp = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect2>(), 0, 0, player.whoAmI);
                        Main.projectile[protmp].scale = 0.5f;
                    }
                    int dmg = (int)(damage * Twilight.TiltedScaleDamage * (1f + target.GetGlobalNPC<ApoBirdGNPC>().Sins / 5f));
                    Projectile.NewProjectile(Main.screenPosition, Vector2.Zero, ModContent.ProjectileType<JusticeDamage2>(), dmg, 0, player.whoAmI);
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!proj.minion && FakeModPlayer.AnyLanterns2() != -1)
            {
                float k = Utils.Clamp(Twilight.SalvationCritRange - target.Distance(Main.player[EyeLantern.FakePlayer].Center), 0, Twilight.SalvationCritRange) / Twilight.SalvationCritRange;
                k *= 2;
                if (!crit)
                {
                    if (Main.rand.NextFloat() <= k)
                    {
                        crit = true;
                    }
                }
            }
            if (player.HasBuff(ModContent.BuffType<PunishmentBuff>()))
            {
                damage = (int)(damage * Twilight.PunishmentDamage);
                player.ClearBuff(ModContent.BuffType<PunishmentBuff>());
                PunishmentCD = 60;
            }
            if (player.HasBuff(ModContent.BuffType<LongArmBuff>()) &&
                 proj.type != ModContent.ProjectileType<JusticeDamage>() &&
                 proj.type != ModContent.ProjectileType<JusticeDamage2>())
            {
                if (Main.rand.NextFloat() <= Twilight.SinChance)
                {
                    target.buffImmune[ModContent.BuffType<SinBuff>()] = false;
                    target.AddBuff(ModContent.BuffType<SinBuff>(), 99999);
                }
            }
            if (player.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
            {
                damage += (int)(damage * 0.06f * target.GetGlobalNPC<ApoBirdGNPC>().Sins);
            }
            if (player.HasBuff(ModContent.BuffType<LongArmBuff>()))
            {
                if (proj.type != ModContent.ProjectileType<JusticeDamage2>() &&
                    proj.type != ModContent.ProjectileType<JusticeDamage>())
                {
                    float percent = (float)target.life / target.lifeMax;
                    if (percent == MaxLifePercent())
                    {
                        if (!AnyJudgementEffect())
                        {
                            int protmp = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect2>(), 0, 0, player.whoAmI);
                            Main.projectile[protmp].scale = 0.5f;
                        }
                        int dmg = (int)(damage * Twilight.TiltedScaleDamage * (1f + target.GetGlobalNPC<ApoBirdGNPC>().Sins / 5f));
                        Projectile.NewProjectile(Main.screenPosition, Vector2.Zero, ModContent.ProjectileType<JusticeDamage2>(), dmg, 0, player.whoAmI);
                    }
                }
            }
        }

        private bool AnyJudgementEffect()
        {
            foreach(Projectile proj in Main.projectile)
            {
                if(proj.active && proj.type == ModContent.ProjectileType<JustitiaEffect2>())
                {
                    return true;
                }
            }
            return false;
        }

        public float MaxLifePercent()
        {
            float result = -1;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.CanBeChasedBy())
                {
                    float percent = (float)npc.life / npc.lifeMax;
                    if (percent > result)
                    {
                        result = percent;
                    }
                }
            }
            return result;
        }
        /*
        public override void OnEnterWorld(Player player)
        {
            Texture2D tex = mod.GetTexture("Projectiles/Fuck");
            Color[] data = new Color[tex.Width * tex.Height];
            tex.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                byte gray = data[i].R;
                data[i].R = 0;
                data[i].G = 0;
                data[i].B = 0;
                data[i].A = (byte)(255 - gray);
            }
            tex.SetData(data);
            Stream stream = File.Create("C:/fuck.png");
            tex.SaveAsPng(stream, tex.Width, tex.Height);

        }
        */

        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.type == ModContent.ItemType<TwilightItem>())
            {
                return false;
            }
            return true;
        }
    }
}