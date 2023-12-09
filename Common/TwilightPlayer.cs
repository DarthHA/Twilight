using Microsoft.Xna.Framework;
using rail;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Buffs;
using Twilight.Items;
using Twilight.Projectiles;
using Twilight.Sky;

namespace Twilight
{
    public class TwilightPlayer : ModPlayer
    {
        public int ApoBirdCD = 0;
        public int CurrentEgg = 0;
        public int AbilityCD = 0;
        public int PunishmentCD = 0;

        public int NextNormalAttack = 0;

        public override void PreUpdateBuffs()
        {
            if (BirdUtils.FindBody() != -1 && ApoBirdSky.CurrentState == ApoBirdSky.State.ApoSky)
            {
                Player.AddBuff(ModContent.BuffType<ApocalypseBuff>(), 2);

                switch (CurrentEgg)
                {
                    case 0:
                        Player.AddBuff(ModContent.BuffType<BigEyeBuff>(), 2);
                        break;
                    case 1:
                        Player.AddBuff(ModContent.BuffType<LongArmBuff>(), 2);
                        break;
                    case 2:
                        Player.AddBuff(ModContent.BuffType<SmallPeakBuff>(), 2);
                        break;
                    default:
                        break;
                }
                if (AbilityCD > 0)
                {
                    Player.AddBuff(ModContent.BuffType<SpecialAttackCDBuff>(), AbilityCD);
                }
            }

        }

        public override void PostUpdateMiscEffects()
        {
            if (BirdUtils.FindBody() != -1 && ApoBirdSky.CurrentState == ApoBirdSky.State.ApoSky)
            {
                for (int i = 0; i < Player.buffImmune.Length; i++)
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
                        Player.buffImmune[i] = true;
                    }
                }
                Player.maxMinions -= 999;
                Player.maxTurrets -= 999;
                Player.gravControl = false;
                Player.gravControl2 = false;
            }
            SkyManager.Instance.Activate("Twilight:ApoBirdSky");
        }
        public override void ResetEffects()
        {
            if (BirdUtils.FindBody() != -1 && ApoBirdSky.CurrentState == ApoBirdSky.State.ApoSky)
            {
                if (Main.npc[BirdUtils.FindBody()].ai[1] == 0)
                {
                    if (AbilityCD > 0)
                    {
                        AbilityCD--;
                    }
                }
                if (!Player.HasBuff(ModContent.BuffType<PunishmentBuff>()))
                {
                    if (PunishmentCD > 0)
                    {
                        PunishmentCD--;
                    }
                }
            }
            else
            {
                AbilityCD = 0;
                PunishmentCD = 0;
            }
            if (ApoBirdCD > 0)
            {
                ApoBirdCD--;
                Player.AddBuff(ModContent.BuffType<TwilightCDBuff>(), ApoBirdCD);
            }
        }
        public override void UpdateDead()
        {
            ApoBirdCD = 0;
            if (ApoBirdSky.CurrentState == ApoBirdSky.State.NoSky && BirdUtils.FindBody() == -1)
            {
                AbilityCD = 0;
                CurrentEgg = 0;
                PunishmentCD = 0;
            }
        }

        public static float GetDamageBonus(Player Player)
        {
            float dmg = 1;
            if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<TwilightItem>())
            {
                dmg = Player.GetWeaponDamage(Player.HeldItem) / 250f;
                if (dmg < 1) dmg = 1;
                if (Player.statLife <= Player.statLifeMax2 / 2)
                {
                    dmg += 1;
                }
            }
            return dmg;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Player.HasBuff(ModContent.BuffType<BigEyeBuff>()))
            {
                modifiers.FinalDamage *= 0.75f;
            }
        }


        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (Player.HasBuff(ModContent.BuffType<BigEyeBuff>()))
            {
                modifiers.FinalDamage *= 0.75f;
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Player.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
            {
                if (PunishmentCD == 0)
                {
                    Player.AddBuff(ModContent.BuffType<PunishmentBuff>(), 600);
                }
            }

        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Player.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
            {
                if (PunishmentCD == 0)
                {
                    Player.AddBuff(ModContent.BuffType<PunishmentBuff>(), 600);
                }
            }
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyHitEither(target, ref modifiers);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            ModifyHitEither(target, ref modifiers, proj.type);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEither(target, damageDone);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEither(target, damageDone);
        }

        internal void ModifyHitEither(NPC target, ref NPC.HitModifiers modifiers, int projType = -1)
        {
            
            if (modifiers.DamageType == ModContent.GetInstance<TwilightDamage>())
            {
                int critChance = 10;
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<TwilightItem>())
                {
                    critChance = Player.GetWeaponCrit(Player.HeldItem);
                }
                if (Main.rand.Next(100) < critChance)
                {
                    modifiers.SetCrit();
                }
            }
            


            if (Player.HasBuff(ModContent.BuffType<PunishmentBuff>()))
            {
                modifiers.FinalDamage *= TwilightData.PunishmentDamage;
                Player.ClearBuff(ModContent.BuffType<PunishmentBuff>());
                PunishmentCD = 180;
            }
            if (Player.HasBuff(ModContent.BuffType<LongArmBuff>()) &&
                 projType != ModContent.ProjectileType<JusticeDamage>() &&
                 projType != ModContent.ProjectileType<JusticeDamage2>())
            {
                if (Main.rand.NextFloat() <= TwilightData.SinChance)
                {
                    SomeUtils.ApplySin(target);
                }
            }
            if (Player.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
            {
                modifiers.FinalDamage *= 1 + 0.06f * target.GetGlobalNPC<ApoBirdGNPC>().Sins;
            }

            int lantern2 = EyeLantern2.FindLantern2();
            if (lantern2 != -1)
            {
                float dist = target.Distance(Main.projectile[lantern2].Center);
                if (dist < TwilightData.SalvationCritRange)
                {
                    float chance = 1 - dist / TwilightData.SalvationCritRange;
                    if (Main.rand.NextFloat() < chance)
                    {
                        modifiers.CritDamage *= 2;
                    }
                }
            }
        }


        internal void OnHitEither(NPC target, int damageDone)
        {
            if (Player.HasBuff(ModContent.BuffType<LongArmBuff>()))
            {
                float percent = (float)target.life / target.lifeMax;
                if (percent == MaxLifePercent())
                {
                    if (!AnyJudgementEffect())
                    {
                        int protmp = Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect2>(), 0, 0, Player.whoAmI);
                        Main.projectile[protmp].scale = 0.5f;
                    }
                    int dmg = (int)(damageDone * TwilightData.TiltedScaleDamage * (1f + target.GetGlobalNPC<ApoBirdGNPC>().Sins / 5f));
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), Main.screenPosition, Vector2.Zero, ModContent.ProjectileType<JusticeDamage2>(), dmg, 0, Player.whoAmI);
                }
            }
        }


        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (ApoBirdGNPC.FuckingInvincible || ApoBirdSky.CurrentState == ApoBirdSky.State.Ending || ApoBirdSky.CurrentState == ApoBirdSky.State.Revealing)
            {
                return true;
            }
            return false;
        }

        internal bool AnyJudgementEffect()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<JustitiaEffect2>())
                {
                    return true;
                }
            }
            return false;
        }

        internal float MaxLifePercent()
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
        public override void OnEnterWorld(Player Player)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Twilight/Projectiles/Fuck");
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