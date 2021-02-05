using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Projectiles;
using Twilight.Sky;

namespace Twilight.Items
{
    public class TwilightItem : ModItem
    {
        public static int AttackCycle = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight");
            DisplayName.AddTranslation(GameCulture.Chinese, "薄暝");
            Tooltip.SetDefault(
                "\"Just like how [c/FFA500:the ever-watching eyes],\n" +
                "[c/87CEEB: the scale that could measure any and all sin],\n" +
                "[c/FF0000: and the beak that could swallow everything] protected the peace of the Black Forest...\n" +
                "The wielder of this armament may also bring peace as they did\"\n\n" +
                "[c/FF0000:ALEPH] E.G.O. Weapon\n" +
                "Affected by melee, ranged and magic damage bonus\n" +
                "The base damage is doubled when your health is below 50%\n" +
                "Left click to use four powerful attacks in turn\n" +
                "Right click to synchronize with the Apocalypse Bird\n" +
                "In Apocalypse Bird mode, you will alternately switch states between the three birds\n" +
                "Every state will bring unique buffs\n" +
                "Hold the left button to launch a normal attack, press the right button to launch a special attack.\n" +
                "Switch weapons to desynchronize\n" +
                "Note that you cannot synchronize again within 30 seconds after desynchronizing.");
            Tooltip.AddTranslation(GameCulture.Chinese,
                "“[c/FFA500:大鸟那永不闭合的眼睛]、\n" +
                "[c/87CEEB:高鸟那能衡量一切罪恶的天平]、\n" +
                "[c/FF0000:小鸟那能吞噬一切的巨口]，\n" +
                "这三者守护着黑森林的和平。\n" +
                "而那些能够同时驾驭这三者的人也能带来和平。”\n" +
                "[c/FF0000:ALEPH]级E.G.O.武器\n" +
                "同时受到近战，远程和魔法攻击的增益\n" +
                "生命值低于50%时基础伤害翻倍\n" +
                "左键轮流使用四种强力的攻击\n" +
                "右键使自己和终末鸟同化\n" +
                "终末鸟模式中会在三鸟状态中轮流切换\n" +
                "每种模式都会带来独特的增益\n" +
                "按住左键发动普通攻击，按下右键发动特殊攻击。\n" +
                "切换武器以解除同化\n" +
                "注意，解除同化后你在30秒内不能再次进入同化状态。");
        }

        public override void SetDefaults()
        {
            item.damage = 250;
            item.melee = true;
            item.ranged = true;
            item.magic = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 0;
            item.value = 0;
            item.rare = ItemRarityID.Expert;
            item.shoot = ModContent.ProjectileType<MeleeHitEffect>();
            //item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.channel = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string damagetype = Language.ActiveCulture == GameCulture.Chinese ? " 混合伤害" : " mixed ";
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
            if (tt != null)
            {
                // We want to grab the last word of the tooltip, which is the translated word for 'damage' (depending on what language the player is using)
                // So we split the string by whitespace, and grab the last word from the returned arrays to get the damage word, and the first to get the damage shown in the tooltip
                string[] splitText = tt.text.Split(' ');
                string damageValue = splitText.First();
                string damageWord = splitText.Last();
                // Change the tooltip text
                if (Language.ActiveCulture == GameCulture.Chinese)
                {
                    tt.text = damageValue + damagetype;
                }
                else
                {
                    tt.text = damageValue + damagetype + damageWord;
                }
                tt.overrideColor = new Color(255 - Main.DiscoR, 255 - Main.DiscoG, 255 - Main.DiscoB);
                
            }
        }


        public override bool CanUseItem(Player player)
        {
            if (ApoBirdSky.CurrentState == 2)
            {
                item.channel = true;
            }
            else
            {
                item.channel = false;
            }
            if (ApoBirdSky.CurrentState == 0)
            {
                if (player.altFunctionUse == 2 && player.GetModPlayer<TwilightPlayer>().ApoBirdCD > 0)
                {
                    return false;
                }
                return true;
            }
            if (ApoBirdSky.CurrentState == 2 && Twilight.FindBody() != -1)
            {
                if (player.altFunctionUse == 2)
                {
                    if (player.GetModPlayer<TwilightPlayer>().SpecialAttackCD == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                if (ApoBirdSky.CurrentState == 0)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/BossBirdBirth"), player.Center);
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TwilightSummon>(), 0, 0, player.whoAmI);
                }
                else
                {
                    player.GetModPlayer<TwilightPlayer>().RightClick = true;
                }
            }
            else
            {

                if (ApoBirdSky.CurrentState == 0)
                {
                    Vector2 ShootVel = Vector2.Normalize(Main.MouseWorld - player.Center);
                    if (AttackCycle == 0)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/TwilightAtk1"), player.Center);
                        Projectile.NewProjectile(player.Center, ShootVel, ModContent.ProjectileType<TwilightStrike>(), (int)(Twilight.StrikeDamage * damage), knockBack, player.whoAmI);
                    }
                    else if (AttackCycle == 1)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/TwilightAtk2"), player.Center);
                        Projectile.NewProjectile(player.Center, ShootVel, ModContent.ProjectileType<TwilightSlash>(), (int)(Twilight.SlashDamage * damage), knockBack, player.whoAmI);
                    }
                    else if (AttackCycle == 2)
                    {
                        Projectile.NewProjectile(player.Center, ShootVel, ModContent.ProjectileType<TwilightSwing>(), (int)(Twilight.SwingDamage * damage), knockBack, player.whoAmI);
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TwilightJustitia>(), 0, 0, player.whoAmI);
                    }
                    else
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/EyeBulletStart"), player.Center);
                        Projectile.NewProjectile(player.Center, ShootVel, ModContent.ProjectileType<TwilightShoot>(), (int)(Twilight.ShootDamage * damage), knockBack, player.whoAmI);
                    }
                    AttackCycle = (AttackCycle + 1) % 4;
                }
            }

            return false;
        }

        public override bool AllowPrefix(int pre)
        {
            return false;
        }
        
    }

}