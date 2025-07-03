using BG3MagicRework.Projectiles.VirtualEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class BlindedDNDBuff : ModBuff
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
                    Vector2 RandomPos = npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height));
                    float scale = Main.rand.NextFloat() * 0.5f + 0.5f;
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 r = (j * MathHelper.TwoPi / 4f).ToRotationVector2();
                        Dust dust15 = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.WhiteTorch, 0f, 0f, 120, default, 2f * scale);
                        dust15.position = RandomPos;
                        dust15.velocity = r * scale;
                        dust15.velocity += npc.velocity;
                        dust15.color = Main.DiscoColor;
                        dust15.noGravity = true;
                    }

                }
            }
        }
    }
}
