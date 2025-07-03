using BG3MagicRework.Projectiles.VirtualEffects;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class FearDNDBuff : ModBuff
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
            int count = (int)(npc.width * npc.height / 2000f) + 1;
            for (int i = 0; i < count; i++)
            {
                if (Main.rand.NextBool(30))
                {
                    Vector2 RandomPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height / 2));
                    float scale = Main.rand.NextFloat() * 0.5f + 0.5f;
                    Dust dust15 = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.PurpleCrystalShard, 0f, 0f, 120, default, 2f * scale);
                    dust15.position = RandomPos;
                    dust15.velocity += new Vector2(0, Math.Clamp(npc.height / 20f, 5, 999999));
                    dust15.noGravity = true;
                }
            }
        }
    }
}