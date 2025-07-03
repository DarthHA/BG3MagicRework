using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class BurningDNDBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.Next(4) < 3)
            {
                Dust dust4 = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.Torch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                dust4.noGravity = true;
                dust4.velocity *= 1.8f;
                dust4.velocity.Y = dust4.velocity.Y - 0.5f;
                if (Main.rand.NextBool(4))
                {
                    dust4.noGravity = false;
                    dust4.scale *= 0.5f;
                }
                dust4.velocity += npc.velocity;
            }
            Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
        }
    }
}