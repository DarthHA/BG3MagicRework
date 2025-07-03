using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class DisadvantageTerrainBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        //困难地形不该有buff消除提示的
        public override void Update(NPC npc, ref int buffIndex)
        {
            /*
            if (npc.buffTime[buffIndex] == 0)
            {
                AdvancedCombatText.NewText(npc.getRect(), Color.White, Lang.GetBuffName(Type), true);
            }
            */
        }
    }
}
