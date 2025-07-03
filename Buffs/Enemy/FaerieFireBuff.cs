using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class FaerieFireBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.buffTime[buffIndex] == 0)
            {
                AdvancedCombatText.NewText(npc.getRect(), Color.White, Lang.GetBuffName(Type), true);
            }
            SomeUtils.AddLightCircle(npc.Center, Math.Max(npc.width, npc.height), Color.Purple);

        }
    }
}