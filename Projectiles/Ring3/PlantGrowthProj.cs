using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class PlantGrowthProj : BaseMagicProj
    {
        public int numVines = 20;
        internal static float deltaR = MathHelper.Pi / 32f;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 0)  //生长和维持阶段
            {
                if (owner.IsDead())
                {
                    Projectile.ai[0] = 1;
                    return;
                }
                if (Projectile.Distance(owner.Center) > GetSpellRange<PlantGrowthSpell>() * 16f * 6f)
                {
                    Projectile.ai[0] = 1;
                    return;
                }
                //断专注了
                if (owner.GetConcentration(ConUUID) == -1)
                {
                    Projectile.ai[0] = 1;
                    return;
                }

                if (Projectile.ai[1] < 40)
                {
                    Projectile.ai[1] += 4f;
                }
                float length = MathHelper.Lerp(0, numVines * 30 + 15, MathHelper.Clamp(Projectile.ai[1] / 40f, 0, 1));
                int t = (int)(length / 30f) + 1;
                float Radius = GetRadius(t);
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.Distance(Projectile.Center) <= Radius &&
                        (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, player.TopLeft, player.width, player.height)))
                    {
                        player.AddBuff(ModContent.BuffType<DisadvantageTerrainBuff2>(), 2);
                    }
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy(null, true) && npc.Hitbox.Distance(Projectile.Center) < Radius &&
                     (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, npc.TopLeft, npc.width, npc.height)))
                    {
                        npc.DeepAddCCBuff(ModContent.BuffType<DisadvantageTerrainBuff2>(), 2);
                    }
                }
            }
            else if (Projectile.ai[0] == 1)  //消失，原因可能包含时间过长或者提前解除专注
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] <= 0) Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float length = MathHelper.Lerp(0, numVines + 0.5f, MathHelper.Clamp(Projectile.ai[1] / 40f, 0, 1));
            int t = (int)length;
            lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), Color.White);
            for (int i = 0; i < numVines * 2; i++)
            {
                float frame = i % 6;
                float baseRot = MathHelper.TwoPi / (numVines * 2) * i;
                Vector2 UnitX = baseRot.ToRotationVector2();
                Vector2 UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                Vector2 CurrentPos = Vector2.Zero;
                List<CustomVertexInfo> bars = new();
                for (int j = 0; j <= t + 1; j++)
                {
                    UnitX = (baseRot + deltaR * j).ToRotationVector2();
                    UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                    bars.Add(new CustomVertexInfo(Projectile.Center + CurrentPos - UnitY * 16f - Main.screenPosition, Color.White, new Vector3(j / (float)(t + 1), 1 / 6f * frame, 0)));
                    bars.Add(new CustomVertexInfo(Projectile.Center + CurrentPos + UnitY * 16f - Main.screenPosition, Color.White, new Vector3(j / (float)(t + 1), 1 / 6f * (frame + 1), 0)));
                    CurrentPos += (baseRot + deltaR * j).ToRotationVector2() * 30;
                }
                DrawUtils.DrawTrail(TextureLibrary.BloodRoot, bars, lightColor, BlendState.AlphaBlend);
            }
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = MathHelper.Lerp(0, numVines * 30 + 15, MathHelper.Clamp(Projectile.ai[1] / 40f, 0, 1));
            int t = (int)(length / 30f) + 1;
            return targetHitbox.Distance(Projectile.Center) < GetRadius(t) &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }

        public static float GetRadius(int num)
        {
            int baseWidth = 30;
            float x = 0, y = 0;
            for (int i = 0; i < num; i++)
            {
                x += baseWidth * (float)Math.Cos(deltaR * i);
                y += baseWidth * (float)Math.Sin(deltaR * i);
            }
            return new Vector2(x, y).Length();
        }

    }
}
