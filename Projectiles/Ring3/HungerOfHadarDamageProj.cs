using BG3MagicRework.BaseType;
using BG3MagicRework.Buffs.Enemy;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class HungerOfHadarDamageProj : BaseMagicProj
    {
        public List<SmokeParticle> smokeParticles = new();
        public Vector2 RelaPos = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.netImportant = true;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 0)   //出现和维持
            {
                if (owner.IsDead())
                {
                    Projectile.ai[0] = 1;
                    return;
                }
                //断专注就会消失
                if (owner.GetConcentration(ConUUID) == -1)
                {
                    Projectile.ai[0] = 1;
                    return;
                }
                if (Projectile.Distance(owner.Center) > GetSpellRange<HungerOfHadarSpell>() * 16f * 6f)
                {
                    Projectile.ai[0] = 1;
                    return;
                }

                if (Projectile.ai[1] < 20)
                {
                    Projectile.ai[1]++;
                }
                Projectile.localAI[0]++;
                if (Main.rand.NextBool(2) || Main.rand.NextBool(2))
                {
                    Vector2 Pos = Projectile.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(70, GetAOERadius<HungerOfHadarSpell>() * 16 - 30);
                    float scale = 0.5f + Main.rand.NextFloat() * 0.5f;
                    Color smokeColor = Main.rand.NextBool() ? Color.White : Color.IndianRed;
                    smokeParticles.Add(new SmokeParticle(Pos, Vector2.Zero, scale, smokeColor * 0.5f));
                }

                if (Projectile.ai[1] >= 20)
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        if (player.Distance(Projectile.Center) <= GetAOERadius<HungerOfHadarSpell>() * 16 &&
                            (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, player.TopLeft, player.width, player.height)))
                        {
                            player.AddBuff(ModContent.BuffType<DisadvantageTerrainBuff>(), 2);
                            player.AddBuff(ModContent.BuffType<BlindedDNDBuff_Player>(), 2);
                        }
                    }
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.CanBeChasedBy(null, true) && npc.Hitbox.Distance(Projectile.Center) < GetAOERadius<HungerOfHadarSpell>() * 16 &&
                         (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, npc.TopLeft, npc.width, npc.height)))
                        {
                            npc.DeepAddCCBuff(ModContent.BuffType<DisadvantageTerrainBuff>(), 2);
                            npc.DeepAddCCBuff(ModContent.BuffType<BlindedDNDBuff>(), 2);
                        }
                    }
                }
            }
            else if (Projectile.ai[0] == 1)//消失
            {
                if (Projectile.ai[1] > 0)
                {
                    Projectile.ai[1]--;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            smokeParticles.UpdateParticle(0.93f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float light = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
            DrawRing(Projectile.Center, GetAOERadius<HungerOfHadarSpell>() * 16, 100, Color.White * light);
            //画个底面圆挡一下得了
            DrawRing2(Projectile.Center, GetAOERadius<HungerOfHadarSpell>() * 16, Color.White * light);

            EasyDraw.AnotherDraw(DrawUtils.ReverseSubtract);
            smokeParticles.DrawParticle();
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public override bool? SafeCanHitNPC(NPC target)
        {
            if (Projectile.ai[1] < 20) return false;
            return null;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) < 16 * GetAOERadius<HungerOfHadarSpell>() &&
                (CarefulSpellMM || Collision.CanHit(Projectile.Center, 1, 1, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height));
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void DrawRing(Vector2 Center, float radius, float width, Color color)
        {
            Texture2D tex = TextureLibrary.Perlin;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i;
                Vector2 Pos1 = Center + rot.ToRotationVector2() * radius + new Vector2(0, width / 2f);
                Vector2 Pos2 = Center + rot.ToRotationVector2() * radius - new Vector2(0, width / 2f);
                bars.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(i / 60f, 0f, 1f)));
                bars.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(i / 60f, 1f, 1f)));
            }
            DrawUtils.DrawRoSVert(tex, bars, color, new Vector2(0, 0.5f), new Vector2(0.1f, 1f), new Vector2(0, -Projectile.localAI[0] / 500), DrawUtils.ReverseSubtract);
        }

        public void DrawRing2(Vector2 Center, float radius, Color color)
        {
            Texture2D tex = TextureLibrary.Perlin;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float XPos = (i - 120) / 120f * radius;
                float YPos = (float)Math.Sqrt(radius * radius - XPos * XPos);
                bars.Add(new CustomVertexInfo(Center + new Vector2(XPos, YPos) - Main.screenPosition, Color.White, new Vector3(i / 240f, 0, 1f)));
                bars.Add(new CustomVertexInfo(Center + new Vector2(XPos, -YPos) - Main.screenPosition, Color.White, new Vector3(i / 240f, 1, 1f)));
            }
            DrawUtils.DrawLoopTrail(tex, bars, color, 1f, -Projectile.localAI[0] / 500f, DrawUtils.ReverseSubtract);
        }

    }
}
