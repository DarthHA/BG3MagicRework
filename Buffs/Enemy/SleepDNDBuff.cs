using BG3MagicRework.Dusts;
using BG3MagicRework.Projectiles.VirtualEffects;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class SleepDNDBuff : ModBuff
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
            float baseScale = (float)Math.Sqrt(npc.width * npc.height) / 100f;
            if (baseScale < 1) baseScale = 1;
            if (Main.rand.NextBool(40))
            {
                float scale = (Main.rand.NextFloat() * 0.25f + 0.75f) * baseScale;
                Dust dust15 = Dust.NewDustDirect(npc.position, npc.width, npc.height / 3, ModContent.DustType<ZZZDust>(), 0f, 0f, 120, default, 2f * scale);
                dust15.velocity = new Vector2(Main.rand.Next(-3, 3), -Main.rand.Next(5, 10));
                dust15.velocity += npc.velocity;
                dust15.noGravity = true;
            }
        }

    }
}