using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class MachRing : ModProjectile
    {
        public override string Texture => "BG3MagicRework/Images/PlaceHolder";
        public Color RingColor = Color.White;
        public float Radius = 200;
        public float RingWidth = 20;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
        }


        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 20) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            float radius = MathHelper.Lerp(1, Radius, MathHelper.Clamp(Projectile.ai[1] / 5f, 0, 1));
            float light = MathHelper.Lerp(1, 0, (Projectile.ai[1] - 10f) / 10f);
            DrawRing(Projectile.Center, Projectile.rotation + MathHelper.Pi / 2, radius, RingWidth, RingColor * light);
            DrawRing(Projectile.Center, Projectile.rotation + MathHelper.Pi / 2, radius, RingWidth * 0.75f, Color.White * light);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void DrawRing(Vector2 Center, float rotation, float radius, float width, Color color)
        {
            Texture2D tex = TextureLibrary.Ribbon;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i < 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i;
                Vector2 Pos1 = rot.ToRotationVector2() * (radius + width / 2f);
                Pos1.Y *= 0.3f;
                Pos1 = Pos1.RotatedBy(rotation);
                Vector2 Pos2 = rot.ToRotationVector2() * (radius - width / 2f);
                Pos2.Y *= 0.3f;
                Pos2 = Pos2.RotatedBy(rotation);
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1, 1f)));
            }
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius + width / 2f, 0).RotatedBy(rotation) - Main.screenPosition, Color.White, new Vector3(1f, 0, 1f)));
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius - width / 2f, 0).RotatedBy(rotation) - Main.screenPosition, Color.White, new Vector3(1f, 1, 1f)));
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, Projectile.ai[0] / 300f, BlendState.Additive);
        }

        public static int Summon(Vector2 Position, float rotation, float radius, float width, Color color)
        {
            int protmp = Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis("BG3Magic"), Position, Vector2.Zero, ModContent.ProjectileType<MachRing>(), 0, 0, Main.myPlayer);
            if (protmp >= 0 && protmp < 1000)
            {
                Main.projectile[protmp].rotation = rotation;
                MachRing modproj = Main.projectile[protmp].ModProjectile as MachRing;
                modproj.Radius = radius;
                modproj.RingWidth = width;
                modproj.RingColor = color;
            }
            return protmp;
        }
    }
}
