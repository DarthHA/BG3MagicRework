using BG3MagicRework.Projectiles.Ring2;
using BG3MagicRework.Spells.Ring2;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class RotateDaggers : BaseDrawOrbit
    {
        public class DaggerUnit(float rot, float r, float vel, float height, float scale)
        {
            public float Rotation = rot;
            public float Radius = r;
            public float Velocity = vel;
            public float Height = height;
            public float Scale = scale;
            public float Light = 0;
        }

        public override bool BehindNPCs => true;

        public int TargetNPC = -1;
        public bool CarefulSpellMM = false;
        public List<DaggerUnit> Daggers = new();

        public override void SafeSetdefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.ignoreWater = false;
        }

        public override void SafeAI()
        {
            Projectile ProjOwner = Main.projectile[(int)Projectile.ai[0]];
            if (!ProjOwner.active || ProjOwner.type != ModContent.ProjectileType<CloudOfDaggersProj>())
            {
                Projectile.Kill();
                return;
            }

            if (Daggers.Count == 0)
            {
                int direction = Main.rand.Next(2) * 2 - 1;
                for (int i = 0; i < 80; i++)
                {
                    float rot = Main.rand.NextFloat() * MathHelper.TwoPi;
                    float r = (Projectile.width - 200) / 2f + (Main.rand.NextFloat() * 0.6f + 0.8f) * EverythingLibrary.GetSpell<CloudOfDaggersSpell>().AOERadius * 16;
                    float vel = MathHelper.TwoPi / 120f * (Main.rand.NextFloat() + 1) * direction;
                    float height = (Main.rand.NextFloat() * 2 - 1) * 1.4f * EverythingLibrary.GetSpell<CloudOfDaggersSpell>().AOERadius * 16;
                    float scale = 1f;
                    Daggers.Add(new(rot, r, vel, height, scale));
                }
            }

            foreach (DaggerUnit unit in Daggers)
            {
                unit.Rotation += unit.Velocity;
                unit.Scale = MathHelper.Lerp(0, 1, ProjOwner.ai[1] / 30f);
                if (ProjOwner.ai[0] == 0)
                {
                    if (ProjOwner.ai[1] < 10)
                    {
                        unit.Light = MathHelper.Lerp(0, 1, ProjOwner.ai[1] / 10f);
                    }
                    else
                    {
                        unit.Light = MathHelper.Lerp(1, 0, MathHelper.Clamp((ProjOwner.ai[1] - 10) / 10f, 0, 1));
                    }
                }
            }
        }

        public override void DrawFront(Color lightColor)
        {
            Texture2D tex = TextureLibrary.DaggerVert;
            Texture2D tex2 = TextureLibrary.BloomFlare;
            foreach (DaggerUnit unit in Daggers)
            {
                if (IsFront(unit.Rotation))
                {
                    Vector2 unitY = (unit.Rotation - MathHelper.Pi / 2f).ToRotationVector2();
                    Vector2 d1 = unit.Rotation.ToRotationVector2() * unit.Radius + new Vector2(0, unit.Height) + unitY * tex.Width * unit.Scale / 2f * Math.Sign(unit.Velocity);
                    Vector2 d2 = unit.Rotation.ToRotationVector2() * unit.Radius + new Vector2(0, unit.Height) - unitY * tex.Width * unit.Scale / 2f * Math.Sign(unit.Velocity);
                    d1.Y *= 0.4f;
                    d2.Y *= 0.4f;
                    Color color = Lighting.GetColor((int)((Projectile.Center + d1).X / 16f), (int)((Projectile.Center + d1).Y / 16f), Color.White);
                    List<CustomVertexInfo> vertexInfos = new()
                    {
                        new CustomVertexInfo(Projectile.Center + d1 - new Vector2(0, tex.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(0, 0, 1)),
                        new CustomVertexInfo(Projectile.Center + d1 + new Vector2(0, tex.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(0, 1, 1)),
                        new CustomVertexInfo(Projectile.Center + d2 - new Vector2(0, tex.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(1, 0, 1)),
                        new CustomVertexInfo(Projectile.Center + d2 + new Vector2(0, tex.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(1, 1, 1))
                    };
                    DrawUtils.DrawTrail(tex, vertexInfos, color, BlendState.AlphaBlend);
                }
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            foreach (DaggerUnit unit in Daggers)
            {
                if (IsFront(unit.Rotation))
                {
                    if (unit.Light > 0)
                    {
                        Vector2 DrawPos = unit.Rotation.ToRotationVector2() * unit.Radius + new Vector2(0, unit.Height);
                        DrawPos.Y *= 0.4f;
                        DrawPos = Projectile.Center + DrawPos - Main.screenPosition;
                        Main.spriteBatch.Draw(tex2, DrawPos, null, Color.LightCyan, 0, tex2.Size() / 2f, 0.05f * unit.Light, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(tex2, DrawPos, null, Color.White, 0, tex2.Size() / 2f, 0.03f * unit.Light, SpriteEffects.None, 0);
                    }
                }
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }

        public override void DrawBehind(Color lightColor)
        {
            Texture2D tex = TextureLibrary.DaggerVert;
            Texture2D tex2 = TextureLibrary.BloomFlare;
            foreach (DaggerUnit unit in Daggers)
            {
                if (!IsFront(unit.Rotation))
                {
                    Vector2 unitY = (unit.Rotation - MathHelper.Pi / 2f).ToRotationVector2();
                    Vector2 d1 = unit.Rotation.ToRotationVector2() * unit.Radius + new Vector2(0, unit.Height) + unitY * tex.Width * unit.Scale / 2f * Math.Sign(unit.Velocity);
                    Vector2 d2 = unit.Rotation.ToRotationVector2() * unit.Radius + new Vector2(0, unit.Height) - unitY * tex.Width * unit.Scale / 2f * Math.Sign(unit.Velocity);
                    d1.Y *= 0.4f;
                    d2.Y *= 0.4f;
                    Color color = Lighting.GetColor((int)((Projectile.Center + d1).X / 16f), (int)((Projectile.Center + d1).Y / 16f), Color.White);
                    List<CustomVertexInfo> vertexInfos = new()
                    {
                        new CustomVertexInfo(Projectile.Center + d1 - new Vector2(0, tex.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(0, 0, 1)),
                        new CustomVertexInfo(Projectile.Center + d1 + new Vector2(0, tex.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(0, 1, 1)),
                        new CustomVertexInfo(Projectile.Center + d2 - new Vector2(0, tex.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(1, 0, 1)),
                        new CustomVertexInfo(Projectile.Center + d2 + new Vector2(0, tex.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(1, 1, 1))
                    };
                    DrawUtils.DrawTrail(tex, vertexInfos, color, BlendState.AlphaBlend);
                }
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            foreach (DaggerUnit unit in Daggers)
            {
                if (!IsFront(unit.Rotation))
                {
                    if (unit.Light > 0)
                    {
                        Vector2 DrawPos = unit.Rotation.ToRotationVector2() * unit.Radius + new Vector2(0, unit.Height);
                        DrawPos.Y *= 0.4f;
                        DrawPos = Projectile.Center + DrawPos - Main.screenPosition;
                        Main.spriteBatch.Draw(tex2, DrawPos, null, Color.LightCyan, 0, tex2.Size() / 2f, 0.05f * unit.Light, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(tex2, DrawPos, null, Color.White, 0, tex2.Size() / 2f, 0.03f * unit.Light, SpriteEffects.None, 0);
                    }
                }
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }
    }
}
