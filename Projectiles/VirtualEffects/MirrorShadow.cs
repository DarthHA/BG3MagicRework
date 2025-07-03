using BG3MagicRework.Buffs;
using BG3MagicRework.Static;
using BG3MagicRework.Static.Particles;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class MirrorShadow : BaseDrawOrbit
    {
        public int MaxCount = 0;
        public int CurrentCount = 0;
        public List<TmpParticle> tmpParticles = new();
        public override void SafeAI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;
            if (Projectile.ai[0] == 0)  //出现并维持
            {
                if (Projectile.ai[1] < 20) Projectile.ai[1]++;
                if (!owner.HasBuff(ModContent.BuffType<MirrorImageBuff>()) || CurrentCount <= 0)
                {
                    Projectile.ai[0] = 1;
                    return;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] < 0)
                {
                    Projectile.Kill();
                }
            }
            tmpParticles.UpdateParticle(0.92f, 0.92f);
        }

        public override void DrawFront(Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            for (int i = 0; i < CurrentCount; i++)
            {
                float r = (float)i / MaxCount * MathHelper.TwoPi - 0.001f;
                if (IsFront(r))
                {
                    float dist = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
                    Vector2 Pos = r.ToRotationVector2() * dist * 30;
                    Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, owner.position + Pos, owner.fullRotation, owner.fullRotationOrigin, 0.5f, 1f);
                }
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            tmpParticles.DrawParticle(TextureLibrary.Extra, Color.LightSkyBlue, true, new Vector2(1, 1));
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }

        public override void DrawBehind(Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            for (int i = 0; i < CurrentCount; i++)
            {
                float r = (float)i / MaxCount * MathHelper.TwoPi - 0.001f;
                if (!IsFront(r))
                {
                    float dist = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
                    Vector2 Pos = r.ToRotationVector2() * dist * 30;
                    Main.PlayerRenderer.DrawPlayer(Main.Camera, owner, owner.position + Pos, owner.fullRotation, owner.fullRotationOrigin, 0.5f, 1f);
                }
            }
        }

        public void DestroyAShadow()
        {
            Player owner = Main.player[Projectile.owner];
            if (CurrentCount > 0)
            {
                float r = (CurrentCount - 1) / (float)MaxCount * MathHelper.TwoPi;
                float dist = MathHelper.Lerp(0, 1, Projectile.ai[1] / 20f);
                Vector2 VectR = r.ToRotationVector2() * dist * 30;
                for (int i = 0; i < 40; i++)
                {
                    Vector2 ShootVel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(4, 8);
                    float scale = 0.3f + Main.rand.NextFloat() * 0.3f;
                    Vector2 Pos = owner.position + VectR + new Vector2(Main.rand.Next(owner.width), Main.rand.Next(owner.height));
                    tmpParticles.NewParticle(Pos, ShootVel, scale);
                }
            }
            CurrentCount = owner.GetModPlayer<DNDMagicPlayer>().MirrorImageCount;
        }
    }
}
