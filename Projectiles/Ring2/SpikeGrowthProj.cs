using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class SpikeGrowthProj : BaseMagicProj
    {
        public int numVines = 20;
        internal static float deltaR = MathHelper.Pi / 32f;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 90;
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
                if (Projectile.Distance(owner.Center) > GetSpellRange<SpikeGrowthSpell>() * 16f * 6f)
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
                    Projectile.ai[1]++;
                }
                float length = MathHelper.Lerp(0, numVines * 30 + 15, MathHelper.Clamp(Projectile.ai[1] / 40f, 0, 1));
                int t = (int)(length / 30f) + 1;
                float Radius = GetRadius(t);
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.Distance(Projectile.Center) <= Radius &&
                        (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, player.TopLeft, player.width, player.height)))
                    {
                        player.AddBuff(ModContent.BuffType<DisadvantageTerrainBuff>(), 2);
                    }
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy(null, true) && npc.Hitbox.Distance(Projectile.Center) < Radius &&
                     (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, npc.TopLeft, npc.width, npc.height)))
                    {
                        npc.DeepAddCCBuff(ModContent.BuffType<DisadvantageTerrainBuff>(), 2);
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
            Texture2D middle = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.VilethornBase].Value;
            Texture2D tip = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.VilethornTip].Value;
            float length = MathHelper.Lerp(0, numVines + 0.5f, MathHelper.Clamp(Projectile.ai[1] / 40f, 0, 1));
            int t = (int)length;
            float alpha = length - t;
            for (int i = 0; i < numVines * 2; i++)
            {
                Color ColorWithLight;
                float baseRot = MathHelper.TwoPi / (numVines * 2) * i;
                Vector2 CurrentPos = baseRot.ToRotationVector2() * 25;
                for (int j = 0; j < t; j++)
                {
                    Vector2 DrawPos0 = Projectile.Center + CurrentPos;
                    ColorWithLight = Lighting.GetColor((int)(DrawPos0.X / 16f), (int)(DrawPos0.Y / 16f), Color.White);
                    Main.spriteBatch.Draw(middle, DrawPos0 - Main.screenPosition, null, ColorWithLight, baseRot + deltaR * j + MathHelper.Pi / 2f, middle.Size() / 2f, 1f, SpriteEffects.None, 0);
                    CurrentPos += (baseRot + deltaR * j).ToRotationVector2() * 30;
                }

                Vector2 DrawPos = Projectile.Center + CurrentPos;
                ColorWithLight = Lighting.GetColor((int)(DrawPos.X / 16f), (int)(DrawPos.Y / 16f), Color.White);
                if (alpha <= 0.5f)
                {
                    Main.spriteBatch.Draw(tip, DrawPos - Main.screenPosition, null, ColorWithLight * alpha * 2, baseRot + deltaR * t + MathHelper.Pi / 2f + MathHelper.Pi / 64f, middle.Size() / 2f, 1f, SpriteEffects.None, 0);
                }
                else
                {
                    Main.spriteBatch.Draw(middle, DrawPos - Main.screenPosition, null, ColorWithLight * (alpha - 0.5f) * 2f, baseRot + deltaR * t + MathHelper.Pi / 2f, middle.Size() / 2f, 1f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tip, DrawPos - Main.screenPosition, null, ColorWithLight * (1 - (alpha - 0.5f) * 2f), baseRot + deltaR * t + MathHelper.Pi / 2f, middle.Size() / 2f, 1f, SpriteEffects.None, 0);
                }
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