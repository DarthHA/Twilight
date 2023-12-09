using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities;
using Twilight.Projectiles;
using Twilight.Sky;
using Twilight.UI;

namespace Twilight.Items
{
    public class TwilightItem : ModItem
    {
        public static int AttackCycle = 0;
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 999;
        }
        public override void SetDefaults()
        {
            Item.damage = 250;
            Item.crit = 10;
            Item.DamageType = ModContent.GetInstance<TwilightDamage>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.knockBack = 0;
            Item.value = 0;
            Item.rare = ItemRarityID.Expert;
            Item.shoot = ModContent.ProjectileType<TwilightSpecial>();
            Item.autoReuse = true;
        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (tt != null)
            {
                tt.OverrideColor = new Color(255 - Main.DiscoR, 255 - Main.DiscoG, 255 - Main.DiscoB);
            }

            if (!ItemSlot.ShiftInUse)
            {
                string description = Language.GetTextValue("Mods.Twilight.ItemTooltipExtra1");
                tooltips.Add(new TooltipLine(Mod, "tooltip", description));
            }
            else
            {
                string description = Language.GetTextValue("Mods.Twilight.ItemTooltipExtra2");
                tooltips.Add(new TooltipLine(Mod, "tooltip", description));
            }
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            float[] crits = { player.GetCritChance(DamageClass.Melee), player.GetCritChance(DamageClass.Ranged), player.GetCritChance(DamageClass.Magic), player.GetCritChance(DamageClass.Generic) };
            crit = crits.Max() + 10f;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {

        }
        public override bool CanUseItem(Player player)
        {
            if (ApoBirdSky.CurrentState == ApoBirdSky.State.NoSky)
            {
                if (player.altFunctionUse == 2 && player.GetModPlayer<TwilightPlayer>().ApoBirdCD > 0)
                {
                    return false;
                }
                return true;
            }
            if (ApoBirdSky.CurrentState == ApoBirdSky.State.ApoSky && BirdUtils.FindBody() != -1)
            {
                if (player.GetModPlayer<TwilightPlayer>().AbilityCD == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (ApoBirdSky.CurrentState == ApoBirdSky.State.NoSky)
                {
                    SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/BossBirdBirth") { Volume = 0.5f }, player.Center);
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TwilightSummon>(), 0, 0, player.whoAmI);
                    player.GetModPlayer<TwilightUIPlayer>().Initialise(3);
                    player.GetModPlayer<TwilightPlayer>().CurrentEgg = 0;
                }
                else
                {
                    Vector2 ShootVel = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile.NewProjectile(source, player.Center, ShootVel, ModContent.ProjectileType<TwilightSpecial>(), 0, 0, player.whoAmI);
                }
            }
            else
            {
                if (damage < 250) damage = 250;
                if (player.statLife <= player.statLifeMax2 / 2)
                {
                    damage += 250;
                }
                if (ApoBirdSky.CurrentState == ApoBirdSky.State.NoSky)
                {
                    Vector2 ShootVel = Vector2.Normalize(Main.MouseWorld - player.Center);
                    if (AttackCycle == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/TwilightAtk1") { Volume = 0.5f }, player.Center);
                        Projectile.NewProjectile(source, player.Center, ShootVel, ModContent.ProjectileType<TwilightStrike>(), (int)(TwilightData.StrikeDamage * damage), knockback, player.whoAmI);
                    }
                    else if (AttackCycle == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/TwilightAtk2") { Volume = 0.5f }, player.Center);
                        Projectile.NewProjectile(source, player.Center, ShootVel, ModContent.ProjectileType<TwilightSlash>(), (int)(TwilightData.SlashDamage * damage), knockback, player.whoAmI);
                    }
                    else if (AttackCycle == 2)
                    {
                        Projectile.NewProjectile(source, player.Center, ShootVel, ModContent.ProjectileType<TwilightSwing>(), (int)(TwilightData.SwingDamage * damage), knockback, player.whoAmI);
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TwilightJustitia>(), 0, 0, player.whoAmI);
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/EyeBulletStart") { Volume = 0.5f }, player.Center);
                        Projectile.NewProjectile(source, player.Center, ShootVel, ModContent.ProjectileType<TwilightShoot>(), (int)(TwilightData.ShootDamage * damage), knockback, player.whoAmI);
                    }
                    AttackCycle = (AttackCycle + 1) % 4;
                }
                else
                {
                    Vector2 ShootVel = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile.NewProjectile(source, player.Center, ShootVel, ModContent.ProjectileType<TwilightSpecial>(), 0, 0, player.whoAmI);
                }

            }

            return false;
        }

        public override bool AllowPrefix(int pre)
        {
            return false;
        }
        public override bool? PrefixChance(int pre, UnifiedRandom rand)
        {
            return false;
        }
    }

}