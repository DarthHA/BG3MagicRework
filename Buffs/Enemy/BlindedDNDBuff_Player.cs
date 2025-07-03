using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class BlindedDNDBuff_Player : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}
