using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Twilight.Buffs
{
    public class SinBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

    }
}