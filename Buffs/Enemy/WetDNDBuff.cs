using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class WetDNDBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (SomeUtils.WaterCollision(npc) || SomeUtils.LavaCollision(npc)) return;           //水中不触发特效
            Vector2 vector5 = npc.position;
            vector5.X -= 2f;
            vector5.Y -= 2f;
            if (Main.rand.NextBool(2))
            {
                Dust dust8 = Dust.NewDustDirect(vector5, npc.width + 4, npc.height + 2, DustID.Wet, 0f, 0f, 50, default, 0.8f);
                if (Main.rand.NextBool(2))
                {
                    dust8.alpha += 25;
                }
                if (Main.rand.NextBool(2))
                {
                    dust8.alpha += 25;
                }
                dust8.noLight = true;
                dust8.velocity *= 0.2f;
                Dust dust27 = dust8;
                dust27.velocity.Y = dust27.velocity.Y + 0.2f;
                dust8.velocity += npc.velocity;
            }
            else
            {
                Dust dust9 = Dust.NewDustDirect(vector5, npc.width + 8, npc.height + 8, DustID.Wet, 0f, 0f, 50, default, 1.1f);
                if (Main.rand.NextBool(2))
                {
                    dust9.alpha += 25;
                }
                if (Main.rand.NextBool(2))
                {
                    dust9.alpha += 25;
                }
                dust9.noLight = true;
                dust9.noGravity = true;
                dust9.velocity *= 0.2f;
                Dust dust28 = dust9;
                dust28.velocity.Y = dust28.velocity.Y + 1f;
                dust9.velocity += npc.velocity;
            }
        }
    }
}