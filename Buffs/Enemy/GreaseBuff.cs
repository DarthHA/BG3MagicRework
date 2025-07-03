using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Buffs.Enemy
{
    public class GreaseBuff : ModBuff
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
            if (SomeUtils.WaterCollision(npc)) return;           //水中不触发特效
            if (npc.HasBuff(ModContent.BuffType<BurningDNDBuff>())) return;
            if (!Main.rand.NextBool(3))
            {
                int num = 175;
                Color newColor = new(0, 0, 0, 250);
                Vector2 vector = npc.position;
                vector.X -= 2f;
                vector.Y -= 2f;
                if (Main.rand.NextBool(2))
                {
                    Dust dust7 = Dust.NewDustDirect(vector, npc.width + 4, npc.height + 2, DustID.TintableDust, 0f, 0f, num, newColor, 1.4f);
                    if (Main.rand.NextBool(2))
                    {
                        dust7.alpha += 25;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        dust7.alpha += 25;
                    }
                    dust7.noLight = true;
                    dust7.velocity *= 0.2f;
                    Dust dust23 = dust7;
                    dust23.velocity.Y = dust23.velocity.Y + 0.2f;
                    dust7.velocity += npc.velocity;
                }
            }
        }
    }
}