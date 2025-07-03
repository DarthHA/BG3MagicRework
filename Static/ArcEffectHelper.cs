using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
namespace BG3MagicRework.Static
{
    public static class ArcEffectHelper
    {
        public static void GenerateSegs(this ArcSegments Segs, Vector2 Begin, Vector2 End, Vector2 Step, float width)
        {
            Segs.SegPos.Clear();
            float Len = End.Distance(Begin);
            if (Len >= Step.X)
            {
                Vector2 UnitX = Vector2.Normalize(End - Begin);
                Vector2 UnitY = (UnitX.ToRotation() + MathHelper.Pi / 2f).ToRotationVector2();
                float X = 0;
                do
                {
                    X += Main.rand.NextFloat() * Step.X + 1;
                    float Y = (Main.rand.NextFloat() * 2 - 1) * Step.Y / 2f;
                    Segs.SegPos.Add(new Vector2(X, Y));
                } while (X <= Len);
                Segs.SegPos.Add(Begin + new Vector2(Len, 0));
                Segs.Begin = Begin;
                Segs.End = End;
                Segs.Steps = Step;
                Segs.Width = width;
            }
        }

        public static void DrawSegs(this ArcSegments Segs, Color color, int intensity = 5)
        {
            if (Segs.SegPos.Count > 2)
            {
                Texture2D texExtra = TextureLibrary.BloomLine;
                void Draw(Color color, float scale)
                {
                    List<CustomVertexInfo> bars = new();
                    float width = scale / 2f;
                    Vector2 UnitX = Vector2.Normalize(Segs.End - Segs.Begin);
                    Vector2 UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
                    bars.Add(new CustomVertexInfo(Segs.Begin + UnitY * width - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)));
                    bars.Add(new CustomVertexInfo(Segs.Begin - UnitY * width - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)));
                    for (int i = 1; i < Segs.SegPos.Count - 1; i++)
                    {
                        Vector2 UnitY2 = (Segs.SegPos[i - 1].X * UnitX + Segs.SegPos[i - 1].Y * UnitY + (Segs.SegPos[i + 1].X * UnitX + Segs.SegPos[i + 1].Y * UnitY) - 2 * (Segs.SegPos[i].X * UnitX + Segs.SegPos[i].Y * UnitY)).ToRotation().ToRotationVector2();
                        bars.Add(new CustomVertexInfo(Segs.Begin + (Segs.SegPos[i].X * UnitX + Segs.SegPos[i].Y * UnitY) + UnitY2 * width - Main.screenPosition, Color.White, new Vector3((float)i / Segs.SegPos.Count, 0f, 1)));
                        bars.Add(new CustomVertexInfo(Segs.Begin + (Segs.SegPos[i].X * UnitX + Segs.SegPos[i].Y * UnitY) - UnitY2 * width - Main.screenPosition, Color.White, new Vector3((float)i / Segs.SegPos.Count, 1f, 1)));
                    }
                    bars.Add(new CustomVertexInfo(Segs.Begin + (Segs.SegPos[Segs.SegPos.Count - 1].X * UnitX + Segs.SegPos[Segs.SegPos.Count - 1].Y * UnitY) + UnitY * width - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)));
                    bars.Add(new CustomVertexInfo(Segs.Begin + (Segs.SegPos[Segs.SegPos.Count - 1].X * UnitX + Segs.SegPos[Segs.SegPos.Count - 1].Y * UnitY) - UnitY * width - Main.screenPosition, Color.White, new Vector3(1, 1f, 1)));

                    DrawUtils.DrawTrail(texExtra, bars, color, BlendState.Additive);
                }

                //Draw(color * 0.75f, Segs.Size.Y);


                for (float i = 0; i < 1; i += 1f / intensity)
                {
                    Draw(color * (1 - i) * 0.8f, Segs.Width * (0.1f + i) * 3);
                }
                Draw(Color.White * (color.A / 255f), Segs.Width * 0.5f);
            }
        }


    }

    public class ArcSegments
    {
        public List<Vector2> SegPos = new();
        public Vector2 Begin = Vector2.Zero;
        public Vector2 End = Vector2.Zero;
        public float Width = 1;
        public Vector2 Steps = new(1, 1);
    }
}
