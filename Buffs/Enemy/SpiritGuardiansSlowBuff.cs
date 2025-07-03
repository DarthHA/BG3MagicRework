using BG3MagicRework.Projectiles.VirtualEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class SpiritGuardiansSlowBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.buffTime[buffIndex] == 0)
            {
                AdvancedCombatText.NewText(npc.getRect(), Color.White, Lang.GetBuffName(Type), true);
            }
        }

    }
}
