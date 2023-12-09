

using Terraria;
using Terraria.ModLoader;

namespace Twilight.Items
{

    public class TwilightDamage : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Melee || damageClass == Ranged || damageClass == Magic)
                return StatInheritanceData.Full;
            if (damageClass == Generic)
            {
                return new StatInheritanceData(
                    damageInheritance: 3f,
                    critChanceInheritance: 1f,
                    attackSpeedInheritance: 0f,
                    armorPenInheritance: 3f,
                    knockbackInheritance: 3f
                    );
            }
            return new StatInheritanceData(
                damageInheritance: 0f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0f,
                knockbackInheritance: 0f
            );

        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == Melee || damageClass == Magic || damageClass == Ranged || damageClass == Generic)
                return true;
            return false;
        }

        public override void SetDefaultStats(Player player)
        {
            player.GetCritChance<TwilightDamage>() += 10;
        }

        public override bool UseStandardCritCalcs => true;

        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            // This method lets you prevent certain common statistical tooltip lines from appearing on items associated with this DamageClass.
            // The four line names you can use are "Damage", "CritChance", "Speed", and "Knockback". All four cases default to true, and thus will be shown. For example...
            if (lineName == "Speed")
                return false;
            if (lineName == "Knockback")
                return false;
            return true;
        }

    }
}