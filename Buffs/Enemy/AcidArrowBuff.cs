using BG3MagicRework.Projectiles.VirtualEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class AcidArrowBuff : ModBuff
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
                    Dust dust15 = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.CursedTorch, 0f, 0f, 120, default, 0.2f);
                    dust15.noGravity = true;
                    dust15.fadeIn = 1.9f;
                    dust15.noLight = true;
                    dust15.velocity += npc.velocity;
                }
            }
        }
    }
}
