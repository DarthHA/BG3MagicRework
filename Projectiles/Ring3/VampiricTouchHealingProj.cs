using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Projectiles.Ring3
{
    public class VampiricTouchHealingProj : BaseMagicProj
    {
        public List<float> RelaRot = new();
        public List<float> RelaLength = new();
        public List<Vector2> RealPos = new();
        public List<Vector2> OldRealPos = new();

        public List<TimeLeftParticle> Particles = new();

        private float SavedDistance = 100;

        public int HealingAmount = 10;

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
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 0)   //展开
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    for (int i = 0; i < HealingAmount; i++)
                    {
                        RelaRot.Add(Main.rand.NextFloat() * MathHelper.TwoPi);
                        RelaLength.Add(Main.rand.Next(50, 200));
                        RealPos.Add(Vector2.Zero);
                        OldRealPos.Add(Vector2.Zero);
                    }
                }
                float t = (float)Math.Pow(Projectile.ai[1] / 40f, 0.5f);
                for (int i = 0; i < HealingAmount; i++)
                {
                    OldRealPos[i] = RealPos[i];
                    RealPos[i] = Projectile.Center + RelaRot[i].ToRotationVector2() * MathHelper.Lerp(0, RelaLength[i], t);
                }

                if (Projectile.ai[1] > 40)
                {
                    SavedDistance = owner.Distance(Projectile.Center);
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)   //追逐玩家
            {
                if (owner.Distance(Projectile.Center) < 15)
                {
                    Projectile.ai[0] = 2;
                    Projectile.ai[1] = 0;
                }
                else
                {
                    float speed = 10 + owner.velocity.Length();
                    Vector2 MoveVel = Vector2.Normalize(owner.Center - Projectile.Center) * speed;
                    Projectile.velocity = Projectile.velocity * 0.9f + MoveVel * 0.2f;
                    if (Projectile.velocity.Length() > speed) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * speed;
                }

                for (int i = 0; i < HealingAmount; i++)
                {
                    float t = owner.Distance(Projectile.Center) / SavedDistance;
                    if (t > 1) t = 1;
                    OldRealPos[i] = RealPos[i];
                    RealPos[i] = Projectile.Center + RelaRot[i].ToRotationVector2() * MathHelper.Lerp(0, RelaLength[i], t);
                }
            }
            else if (Projectile.ai[0] == 2)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 10)
                {
                    owner.Heal(HealingAmount * 10);
                    Projectile.Kill();
                }
            }

            for (int i = 0; i < RealPos.Count; i++)
            {
                Vector2 Pos0 = RealPos[i] + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 2;
                Particles.Add(new TimeLeftParticle(Pos0, 15));
                if (RealPos[i] != OldRealPos[i])
                {
                    Vector2 Unit = Vector2.Normalize(RealPos[i] - OldRealPos[i]);
                    for (float j = 0; j <= Projectile.velocity.Length(); j += 3)
                    {
                        Vector2 Pos = RealPos[i] - Unit * j + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 2;
                        Particles.Add(new TimeLeftParticle(Pos, 15));
                    }
                }
            }
            Particles.UpdateParticle();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            Particles.DrawParticle(TextureLibrary.CrispCircle, Color.Green);
            EasyDraw.AnotherDraw(DrawUtils.ReverseSubtract);
            Particles.DrawParticle(TextureLibrary.CrispCircle, Color.White, true, 0.045f * 0.8f);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
    }
}
