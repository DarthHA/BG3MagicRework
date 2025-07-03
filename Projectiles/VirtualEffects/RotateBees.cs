using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class RotateBees : BaseDrawOrbit
    {
        public class BeeUnit(float rot, float r, float vel, float height, float scale, bool isBig, int frame)
        {
            public float Rotation = rot;
            public float Radius = r;
            public float Velocity = vel;
            public float Height = height;
            public float Scale = scale;
            public bool IsBig = isBig;
            public int Frame = frame;
            public float Factor = 0;
        }

        public override bool BehindNPCs => true;

        public int TargetNPC = -1;
        public bool CarefulSpellMM = false;
        public List<BeeUnit> Bees = new();

        public override void SafeSetdefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.ignoreWater = false;
        }

        public override void SafeAI()
        {
            Projectile.ai[1]++;

            if (TargetNPC != -1 && (Main.npc[TargetNPC].CanBeChasedBy() || Main.npc[TargetNPC].immortal))
            {
                NPC target = Main.npc[TargetNPC];
                Projectile.width = target.width + 200;
                Projectile.height = target.height + 100;
                Projectile.Center = target.Center;
            }

            if (Projectile.ai[1] == 1)
            {
                int direction = Main.rand.Next(2) * 2 - 1;
                int count = (int)MathHelper.Clamp((Projectile.width - 200) * (Projectile.height - 100) / 100f, 20, 9999);
                for (int i = 0; i < count; i++)
                {
                    float rot = Main.rand.NextFloat() * MathHelper.TwoPi;
                    float r = (Projectile.width - 200) / 2f + Main.rand.Next(20, 100);
                    float vel = MathHelper.TwoPi / 120f * (Main.rand.NextFloat() + 1) * direction;
                    float height = (Main.rand.NextFloat() * 2 - 1) * Projectile.height;
                    float scale = 0.75f + 0.25f * Main.rand.NextFloat();
                    bool isbig = Main.rand.NextBool(4);
                    int frame = Main.rand.Next(3);
                    Bees.Add(new(rot, r, vel, height, scale, isbig, frame));
                }
            }
            Projectile.localAI[0]++;//5帧一帧
            foreach (BeeUnit unit in Bees)
            {
                unit.Rotation += unit.Velocity;
                if (Projectile.localAI[0] % 5 == 0)
                {
                    unit.Frame = (unit.Frame + 1) % 4;
                }
                if (Projectile.ai[1] < 30)
                {
                    unit.Factor = MathHelper.Lerp(0, 1, Projectile.ai[1] / 30f);
                }
                else if (Projectile.ai[1] < 150)
                {
                    unit.Factor = 1;
                }
                else
                {
                    unit.Factor += 20f / unit.Radius;
                }
            }
            if (Projectile.ai[1] < 150)
            {
                if (Projectile.wet && !CarefulSpellMM)
                {
                    Projectile.ai[1] = 150;
                }
            }
            if (Projectile.ai[1] > 180) Projectile.Kill();
        }

        public override void DrawFront(Color lightColor)
        {
            foreach (BeeUnit unit in Bees)
            {
                if (IsFront(unit.Rotation))
                {
                    Texture2D tex = unit.IsBig ? TextureLibrary.Bee : TextureLibrary.SmallBee;
                    Rectangle frame = new(0, tex.Height / 4 * unit.Frame, tex.Width, tex.Height / 4);
                    Vector2 unitY = (unit.Rotation - MathHelper.Pi / 2f).ToRotationVector2();
                    Vector2 d1 = unit.Rotation.ToRotationVector2() * unit.Radius * unit.Factor + new Vector2(0, unit.Height) * unit.Factor + unitY * frame.Width * unit.Scale / 2f * Math.Sign(unit.Velocity);
                    Vector2 d2 = unit.Rotation.ToRotationVector2() * unit.Radius * unit.Factor + new Vector2(0, unit.Height) * unit.Factor - unitY * frame.Width * unit.Scale / 2f * Math.Sign(unit.Velocity);
                    d1.Y *= 0.4f;
                    d2.Y *= 0.4f;
                    Color color = Lighting.GetColor((int)((Projectile.Center + d1).X / 16f), (int)((Projectile.Center + d1).Y / 16f), Color.White);
                    List<CustomVertexInfo> vertexInfos = new()
                    {
                        new CustomVertexInfo(Projectile.Center + d1 - new Vector2(0, frame.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(0, unit.Frame / 4f, 1)),
                        new CustomVertexInfo(Projectile.Center + d1 + new Vector2(0, frame.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(0, (unit.Frame + 1) / 4f, 1)),
                        new CustomVertexInfo(Projectile.Center + d2 - new Vector2(0, frame.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(1, unit.Frame / 4f, 1)),
                        new CustomVertexInfo(Projectile.Center + d2 + new Vector2(0, frame.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(1, (unit.Frame + 1) / 4f, 1))
                    };
                    DrawUtils.DrawTrail(tex, vertexInfos, color, BlendState.AlphaBlend);
                }
            }


        }

        public override void DrawBehind(Color lightColor)
        {
            foreach (BeeUnit unit in Bees)
            {
                if (!IsFront(unit.Rotation))
                {
                    Texture2D tex = unit.IsBig ? TextureLibrary.Bee : TextureLibrary.SmallBee;
                    Rectangle frame = new(0, tex.Height / 4 * unit.Frame, tex.Width, tex.Height / 4);
                    Vector2 unitY = (unit.Rotation - MathHelper.Pi / 2f).ToRotationVector2();
                    Vector2 d1 = unit.Rotation.ToRotationVector2() * unit.Radius * unit.Factor + new Vector2(0, unit.Height) * unit.Factor + unitY * frame.Width * unit.Scale / 2f * Math.Sign(unit.Velocity);
                    Vector2 d2 = unit.Rotation.ToRotationVector2() * unit.Radius * unit.Factor + new Vector2(0, unit.Height) * unit.Factor - unitY * frame.Width * unit.Scale / 2f * Math.Sign(unit.Velocity);
                    d1.Y *= 0.4f;
                    d2.Y *= 0.4f;
                    Color color = Lighting.GetColor((int)((Projectile.Center + d1).X / 16f), (int)((Projectile.Center + d1).Y / 16f), Color.White);
                    List<CustomVertexInfo> vertexInfos = new()
                    {
                        new CustomVertexInfo(Projectile.Center + d1 - new Vector2(0, frame.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(0, unit.Frame / 4f, 1)),
                        new CustomVertexInfo(Projectile.Center + d1 + new Vector2(0, frame.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(0, (unit.Frame + 1) / 4f, 1)),
                        new CustomVertexInfo(Projectile.Center + d2 - new Vector2(0, frame.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(1, unit.Frame / 4f, 1)),
                        new CustomVertexInfo(Projectile.Center + d2 + new Vector2(0, frame.Height / 2f * unit.Scale) - Main.screenPosition, Color.White, new Vector3(1, (unit.Frame + 1) / 4f, 1))
                    };
                    DrawUtils.DrawTrail(tex, vertexInfos, color, BlendState.AlphaBlend);
                }
            }



        }


    }
}
