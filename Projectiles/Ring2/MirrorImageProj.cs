using BG3MagicRework.BaseType;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Projectiles.Ring2
{
    public class MirrorImageProj : BaseMagicProj
    {
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
            overPlayers.Add(index);
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead())
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 40) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float light = 1f;
            if (Projectile.ai[1] > 20) light = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 20) / 20f);
            float height1 = MathHelper.Lerp(130, 0, Projectile.ai[1] / 40f);
            float height2 = MathHelper.Lerp(0, 100, Projectile.ai[1] / 40f);
            DrawRing(Projectile.Center + new Vector2(0, 30), 50, height1, Color.White * light);

            float rot = Projectile.ai[1] / 40 * MathHelper.TwoPi;
            DrawSpiral(Projectile.Center + new Vector2(0, 20), 50, 20, height2 * 2f + 10, Color.LightBlue * light, -rot);
            DrawSpiral(Projectile.Center + new Vector2(0, 20), 30, 20, height2 * 4f + 30, Color.LightCoral * light, rot + 0.8f);
            DrawSpiral(Projectile.Center + new Vector2(0, 20), 80, 20, height2, Color.LightYellow * light, rot + 0.4f);

            DrawSpiral(Projectile.Center + new Vector2(0, 10), 40, 20, 0, Color.LightBlue * light, rot + 1.2f);

            DrawSpiral(Projectile.Center + new Vector2(0, 10), height2 * 1.5f, 20, 0, Color.LightBlue * light, rot + 1.6f);
            return false;
        }

        public void DrawSpiral(Vector2 Center, float radius, float width, float height, Color color, float rotation = 0, float FullCircle = 1)
        {
            Texture2D tex = TextureLibrary.Ribbon;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i < 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i * FullCircle + rotation;
                Vector2 R = rot.ToRotationVector2();
                R.Y *= 0.5f;
                Vector2 Pos1 = Center + R * radius + new Vector2(0, -1) * width / 2f + new Vector2(0, -1) * height * i / 60f;
                Vector2 Pos2 = Center + R * radius + new Vector2(0, 1) * width / 2f + new Vector2(0, -1) * height * i / 60f; ;
                bars.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0f, 1f)));
                bars.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1f, 1f)));
            }
            bars.Add(new CustomVertexInfo(Center + rotation.ToRotationVector2() * radius + new Vector2(0, -1) * width / 2f + new Vector2(0, -1) * height - Main.screenPosition, Color.White, new Vector3(1f, 0f, 1f)));
            bars.Add(new CustomVertexInfo(Center + rotation.ToRotationVector2() * radius + new Vector2(0, 1) * width / 2f + new Vector2(0, -1) * height - Main.screenPosition, Color.White, new Vector3(1f, 1f, 1f)));
            DrawUtils.DrawRoSLaser(tex, bars, color, 0.2f, 1f, 0f, BlendState.Additive);
        }

        public void DrawRing(Vector2 Center, float radius, float width, Color color)
        {
            Texture2D tex = TextureLibrary.LightFieldVert;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i < 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i;
                Vector2 R = rot.ToRotationVector2();
                R.Y *= 0.5f;
                Vector2 Pos1 = Center + R * radius + new Vector2(0, -1) * width;
                Vector2 Pos2 = Center + R * radius;
                bars.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0f, 1f)));
                bars.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1f, 1f)));
            }
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius, -1 * width) - Main.screenPosition, Color.White, new Vector3(1f, 0f, 1f)));
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius, 0) - Main.screenPosition, Color.White, new Vector3(1f, 1f, 1f)));
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, 0f, BlendState.Additive);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
