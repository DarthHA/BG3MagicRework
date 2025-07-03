using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace BG3MagicRework.Static
{
    public static class DrawUtils
    {
        public static BlendState ReverseSubtract
        {
            get
            {
                BlendState blendState = new()
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    AlphaSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.One,
                    AlphaDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.ReverseSubtract
                };
                return blendState;
            }
        }

        /// <summary>
        /// 横着的武器挥舞绘制
        /// </summary>
        /// <param name="Tex"></param>
        /// <param name="SwingCenter"></param>
        /// <param name="WeaponLength"></param>
        /// 武器挥舞角度
        /// <param name="RotationY"></param>
        /// 武器挥舞方向
        /// <param name="RotationX"></param>
        /// 武器挥舞斜度
        /// <param name="RotationZ"></param>
        /// 是否翻转绘制
        /// <param name="Reverse"></param>
        /// 武器距离转轴中心位置
        /// <param name="HandOffset"></param>
        public static void DrawSwing(Texture2D Tex, Vector2 SwingCenter, Color color, float WeaponLength, float RotationY, float RotationX = 0, float RotationZ = MathHelper.Pi / 2, bool Reverse = false, float HandOffset = 0)
        {
            //SwingCenter += Main.screenPosition;
            float WeaponHeight = WeaponLength / Tex.Width * Tex.Height;
            Vector2 UnitX = RotationY.ToRotationVector2() * WeaponLength;
            Vector2 UnitY = (RotationY + MathHelper.Pi / 2f).ToRotationVector2() * WeaponHeight;
            Vector2 NewUnitX = RotationY.ToRotationVector2() * (WeaponLength + HandOffset);
            Vector2 Pos1 = NewUnitX - UnitX - UnitY / 2f;
            Vector2 Pos2 = NewUnitX - UnitX + UnitY / 2f;
            Vector2 Pos3 = NewUnitX - UnitY / 2f;
            Vector2 Pos4 = NewUnitX + UnitY / 2f;
            float k = (float)Math.Sin(RotationZ);
            Pos1.Y *= k;
            Pos2.Y *= k;
            Pos3.Y *= k;
            Pos4.Y *= k;
            Pos1 = Pos1.RotatedBy(RotationX);
            Pos2 = Pos2.RotatedBy(RotationX);
            Pos3 = Pos3.RotatedBy(RotationX);
            Pos4 = Pos4.RotatedBy(RotationX);

            if (Reverse)
            {
                Pos1.X = -Pos1.X;
                Pos2.X = -Pos2.X;
                Pos3.X = -Pos3.X;
                Pos4.X = -Pos4.X;
            }

            List<CustomVertexInfo> vertexInfos = new()
            {
                new CustomVertexInfo(SwingCenter + Pos1, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(SwingCenter + Pos2, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(SwingCenter + Pos3, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(SwingCenter + Pos4, Color.White, new Vector3(1, 1f, 1))
            };
            DrawTrail(Tex, vertexInfos, color, BlendState.AlphaBlend);
        }

        /// <summary>
        /// 法杖类武器挥舞绘制
        /// </summary>
        /// <param name="Tex"></param>
        /// <param name="SwingCenter"></param>
        /// <param name="WeaponLength"></param>
        /// 武器挥舞角度
        /// <param name="RotationY"></param>
        /// 武器挥舞方向
        /// <param name="RotationX"></param>
        /// 武器挥舞斜度
        /// <param name="RotationZ"></param>
        /// 是否翻转绘制
        /// <param name="Reverse"></param>
        /// 武器距离转轴中心位置
        /// <param name="HandOffset"></param>
        public static void DrawSwingStaff(Texture2D Tex, Vector2 SwingCenter, Color color, float WeaponLength, float RotationY, float RotationX = 0, float RotationZ = MathHelper.Pi / 2, bool Reverse = false, float HandOffset = 0)
        {
            //SwingCenter += Main.screenPosition;
            float ImageLength = Tex.Size().Length();
            float TexWidth = Tex.Width / ImageLength * WeaponLength;
            float TexHeight = Tex.Height / ImageLength * WeaponLength;
            float NewTexWidth = Tex.Width / ImageLength * (WeaponLength + HandOffset);
            float NewTexHeight = Tex.Height / ImageLength * (WeaponLength + HandOffset);
            Vector2 Pos1 = new Vector2(NewTexWidth - TexWidth, NewTexHeight - TexHeight).RotatedBy(RotationY - MathHelper.Pi / 4f);
            Vector2 Pos2 = new Vector2(NewTexWidth - TexWidth, TexHeight).RotatedBy(RotationY - MathHelper.Pi / 4f);
            Vector2 Pos3 = new Vector2(NewTexWidth, NewTexHeight - TexHeight).RotatedBy(RotationY - MathHelper.Pi / 4f);
            Vector2 Pos4 = new Vector2(NewTexWidth, NewTexHeight).RotatedBy(RotationY - MathHelper.Pi / 4f);
            float k = (float)Math.Sin(RotationZ);
            Pos1.Y *= k;
            Pos2.Y *= k;
            Pos3.Y *= k;
            Pos4.Y *= k;
            Pos1 = Pos1.RotatedBy(RotationX);
            Pos2 = Pos2.RotatedBy(RotationX);
            Pos3 = Pos3.RotatedBy(RotationX);
            Pos4 = Pos4.RotatedBy(RotationX);

            if (Reverse)
            {
                Pos1.X = -Pos1.X;
                Pos2.X = -Pos2.X;
                Pos3.X = -Pos3.X;
                Pos4.X = -Pos4.X;
            }

            List<CustomVertexInfo> vertexInfos = new()
            {
                new CustomVertexInfo(SwingCenter + Pos1, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(SwingCenter + Pos2, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(SwingCenter + Pos3, Color.White, new Vector3(1, 1f, 1)),
                new CustomVertexInfo(SwingCenter + Pos4, Color.White, new Vector3(1, 0f, 1))
            };
            DrawTrail(Tex, vertexInfos, color, BlendState.AlphaBlend);
        }

        /// <summary>
        /// 竖着的武器挥舞绘制
        /// </summary>
        /// <param name="Tex"></param>
        /// <param name="SwingCenter"></param>
        /// <param name="WeaponLength"></param>
        /// 武器挥舞角度
        /// <param name="RotationY"></param>
        /// 武器挥舞方向
        /// <param name="RotationX"></param>
        /// 武器挥舞斜度
        /// <param name="RotationZ"></param>
        /// 是否翻转绘制
        /// <param name="Reverse"></param>
        /// 武器距离转轴中心位置
        /// <param name="HandOffset"></param>
        public static void DrawSwingVertical(Texture2D Tex, Vector2 SwingCenter, Color color, float WeaponLength, float RotationY, float RotationX = 0, float RotationZ = MathHelper.Pi / 2, bool Reverse = false, float HandOffset = 0)
        {
            //SwingCenter += Main.screenPosition;
            float WeaponWidth = WeaponLength / Tex.Height * Tex.Width;
            Vector2 UnitX = RotationY.ToRotationVector2() * WeaponLength;
            Vector2 UnitY = (RotationY + MathHelper.Pi / 2f).ToRotationVector2() * WeaponWidth;
            Vector2 NewUnitX = RotationY.ToRotationVector2() * (WeaponLength + HandOffset);
            Vector2 Pos1 = NewUnitX - UnitX - UnitY / 2f;
            Vector2 Pos2 = NewUnitX - UnitX + UnitY / 2f;
            Vector2 Pos3 = NewUnitX - UnitY / 2f;
            Vector2 Pos4 = NewUnitX + UnitY / 2f;
            float k = (float)Math.Sin(RotationZ);
            Pos1.Y *= k;
            Pos2.Y *= k;
            Pos3.Y *= k;
            Pos4.Y *= k;
            Pos1 = Pos1.RotatedBy(RotationX);
            Pos2 = Pos2.RotatedBy(RotationX);
            Pos3 = Pos3.RotatedBy(RotationX);
            Pos4 = Pos4.RotatedBy(RotationX);

            if (Reverse)
            {
                Pos1.X = -Pos1.X;
                Pos2.X = -Pos2.X;
                Pos3.X = -Pos3.X;
                Pos4.X = -Pos4.X;
            }

            List<CustomVertexInfo> vertexInfos = new()
            {
                new CustomVertexInfo(SwingCenter + Pos1, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(SwingCenter + Pos2, Color.White, new Vector3(1, 1f, 1)),
                new CustomVertexInfo(SwingCenter + Pos3, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(SwingCenter + Pos4, Color.White, new Vector3(1, 0f, 1))
            };
            DrawTrail(Tex, vertexInfos, color, BlendState.AlphaBlend);
        }

        public static void DrawBooks(Texture2D Tex, Vector2 DrawPos, int Direction, float RotationZ, float Scale, Color color)
        {
            Vector2 CalcDistanceAndProjection(Vector2 A, Vector2 L1, Vector2 L2)
            {
                float rot = (L2 - L1).ToRotation();
                Vector2 RelaA = A - L1;
                Vector2 result = RelaA.RotatedBy(-rot);
                return result;
            }
            Vector2 Rotate(Vector2 vec, float rotZ, float k = 0.5f)
            {
                Vector2 vec0 = new(vec.X, 0);
                vec0 = vec0.RotatedBy(rotZ);
                vec0.Y *= k;
                return vec0 + new Vector2(0, vec.Y);
            }
            Vector2 ReverseToPoint(Vector2 _A, Vector2 L1, Vector2 L2)
            {
                float rot = (L2 - L1).ToRotation();
                Vector2 result = L1 + _A.RotatedBy(rot);
                return result;
            }
            float Width = Tex.Height;
            float Height = Tex.Width;
            List<CustomVertexInfo> bars0 = new();
            Vector2 Pos1 = new Vector2(-Width / 2 * Direction, -Height / 2) * Scale;
            Vector2 Pos2 = new Vector2(Width / 2 * Direction, -Height / 2) * Scale;
            Vector2 Pos3 = new Vector2(-Width / 2 * Direction, Height / 2) * Scale;
            Vector2 Pos4 = new Vector2(Width / 2 * Direction, Height / 2) * Scale;

            Vector2 L1 = new Vector2(Width / 3 * Direction, Height / 3) * Scale;
            Vector2 L2 = new Vector2(Width / 2 * Direction, Height / 2) * Scale;

            Vector2 _Pos1 = Rotate(CalcDistanceAndProjection(Pos1, L1, L2), RotationZ);
            Vector2 _Pos2 = Rotate(CalcDistanceAndProjection(Pos2, L1, L2), RotationZ);
            Vector2 _Pos3 = Rotate(CalcDistanceAndProjection(Pos3, L1, L2), RotationZ);
            Vector2 _Pos4 = Rotate(CalcDistanceAndProjection(Pos4, L1, L2), RotationZ);

            Pos1 = ReverseToPoint(_Pos1, L1, L2);
            Pos2 = ReverseToPoint(_Pos2, L1, L2);
            Pos3 = ReverseToPoint(_Pos3, L1, L2);
            Pos4 = ReverseToPoint(_Pos4, L1, L2);

            bars0.Add(new(DrawPos + Pos1 - Main.screenPosition, Color.White, new Vector3(1, 1, 1)));
            bars0.Add(new(DrawPos + Pos2 - Main.screenPosition, Color.White, new Vector3(1, 0, 1)));
            bars0.Add(new(DrawPos + Pos3 - Main.screenPosition, Color.White, new Vector3(0, 1, 1)));
            bars0.Add(new(DrawPos + Pos4 - Main.screenPosition, Color.White, new Vector3(0, 0, 1)));
            DrawUtils.DrawTrail(Tex, bars0, color, BlendState.AlphaBlend);
        }


        public static void DrawTrail(Texture2D tex, List<CustomVertexInfo> bars, Color color, BlendState blendState)
        {
            List<CustomVertexInfo> triangleList = new();
            if (bars.Count > 2)
            {
                for (int k = 0; k < bars.Count - 2; k += 2)
                {
                    triangleList.Add(bars[k]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 3]);
                }

                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, blendState, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = tex;

                EffectLibrary.NormalColorEffect.Parameters["color"].SetValue(color.ToVector4());
                EffectLibrary.NormalColorEffect.CurrentTechnique.Passes[0].Apply();
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public static void DrawGradientTrail(Texture2D tex, List<CustomVertexInfo> bars, Color color, BlendState blendState)
        {
            List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                for (int k = 0; k < bars.Count - 2; k += 2)
                {
                    triangleList.Add(bars[k]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 3]);
                }

                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, blendState, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

                EffectLibrary.GradientColorEffect.Parameters["color"].SetValue(color.ToVector4());
                EffectLibrary.GradientColorEffect.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = tex;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public static void DrawLoopTrail(Texture2D tex, List<CustomVertexInfo> bars, Color color, float length, float progress, BlendState blendState)
        {
            List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                for (int k = 0; k < bars.Count - 2; k += 2)
                {
                    triangleList.Add(bars[k]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 3]);
                }

                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, blendState, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

                EffectLibrary.SimpleLoopEffect.Parameters["length"].SetValue(length);
                EffectLibrary.SimpleLoopEffect.Parameters["progress"].SetValue(progress);
                EffectLibrary.SimpleLoopEffect.Parameters["color"].SetValue(color.ToVector4());
                EffectLibrary.SimpleLoopEffect.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = tex;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public static void DrawRoSLaser(Texture2D tex, List<CustomVertexInfo> bars, Color color, float offset, float length, float progress, BlendState blendState)
        {
            List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                for (int k = 0; k < bars.Count - 2; k += 2)
                {
                    triangleList.Add(bars[k]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 3]);
                }

                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, blendState, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

                EffectLibrary.RoSLoopEffect.Parameters["length"].SetValue(length);
                EffectLibrary.RoSLoopEffect.Parameters["progress"].SetValue(progress);
                EffectLibrary.RoSLoopEffect.Parameters["offset"].SetValue(offset);
                EffectLibrary.RoSLoopEffect.Parameters["color"].SetValue(color.ToVector4());
                EffectLibrary.RoSLoopEffect.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = tex;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public static void DrawRoSVert(Texture2D tex, List<CustomVertexInfo> bars, Color color, Vector2 offset, Vector2 length, Vector2 progress, BlendState blendState)
        {
            List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
            if (bars.Count > 2)
            {
                for (int k = 0; k < bars.Count - 2; k += 2)
                {
                    triangleList.Add(bars[k]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 3]);
                }

                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, blendState, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

                EffectLibrary.RoSLoopVertEffect.Parameters["length"].SetValue(length);
                EffectLibrary.RoSLoopVertEffect.Parameters["progress"].SetValue(progress);
                EffectLibrary.RoSLoopVertEffect.Parameters["offset"].SetValue(offset);
                EffectLibrary.RoSLoopVertEffect.Parameters["color"].SetValue(color.ToVector4());
                EffectLibrary.RoSLoopVertEffect.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = tex;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public static void DrawASphere(Texture2D tex, Vector2 Pos, Color color1, Color color2, float radius, float rotX, float progressZ, BlendState blendState)
        {
            List<CustomVertexInfo> triangleList = new()
            {
                new CustomVertexInfo(Pos - new Vector2(radius, radius).RotatedBy(rotX), Color.White, new Vector3(-1, 1, 0)),
                new CustomVertexInfo(Pos - new Vector2(radius, -radius).RotatedBy(rotX), Color.White, new Vector3(-1, -1, 0)),
                new CustomVertexInfo(Pos - new Vector2(-radius, -radius).RotatedBy(rotX), Color.White, new Vector3(1, -1, 0)),
                new CustomVertexInfo(Pos- new Vector2(radius, radius).RotatedBy(rotX), Color.White, new Vector3(-1, 1, 0)),
                new CustomVertexInfo(Pos - new Vector2(-radius, -radius).RotatedBy(rotX), Color.White, new Vector3(1, -1, 0)),
                new CustomVertexInfo(Pos - new Vector2(-radius, radius).RotatedBy(rotX), Color.White, new Vector3(1, 1, 0)),
            };

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));

            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, blendState, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            EffectLibrary.RollingSphere.Parameters["progress"].SetValue(progressZ);
            EffectLibrary.RollingSphere.Parameters["circleCenter"].SetValue(new Vector3(0, 0, -2));
            EffectLibrary.RollingSphere.Parameters["radiusOfCircle"].SetValue(1f);
            EffectLibrary.RollingSphere.Parameters["color1"].SetValue(color1.ToVector4());
            EffectLibrary.RollingSphere.Parameters["color2"].SetValue(color2.ToVector4());
            EffectLibrary.RollingSphere.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            gd.Textures[0] = tex;
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
        }

        public static void DrawCrack(Texture2D texChest, Texture2D texCrack, Rectangle rectChest, Rectangle rectCrack, Vector2 Position, Color colorCrack)
        {
            EasyDraw.AnotherDraw(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            gd.Textures[1] = texChest;
            gd.Textures[2] = texCrack;
            EffectLibrary.CrackEffect.Parameters["xy1"].SetValue(new Vector2((float)rectChest.X / texChest.Width, (float)rectChest.Y / texChest.Height));
            EffectLibrary.CrackEffect.Parameters["size1"].SetValue(new Vector2((float)rectChest.Width / texChest.Width, (float)rectChest.Height / texChest.Height));
            EffectLibrary.CrackEffect.Parameters["xy2"].SetValue(new Vector2((float)rectCrack.X / texCrack.Width, (float)rectCrack.Y / texCrack.Height));
            EffectLibrary.CrackEffect.Parameters["size2"].SetValue(new Vector2((float)rectCrack.Width / texCrack.Width, (float)rectCrack.Height / texCrack.Height));
            EffectLibrary.CrackEffect.Parameters["colorCrack"].SetValue(colorCrack.ToVector4());
            EffectLibrary.CrackEffect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("BG3MagicRework/PlaceHolder").Value, Position - Main.screenPosition, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            EasyDraw.AnotherDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public static void DrawLoopMask(Texture2D tex1, Texture2D tex2, Vector2 DrawPos, Color color, float rotation, float scale, BlendState blendState, float length, float progress)
        {
            EasyDraw.AnotherDraw(SpriteSortMode.Immediate, blendState);
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            gd.Textures[1] = tex2;
            EffectLibrary.LoopMaskEffect.Parameters["length"].SetValue(length);
            EffectLibrary.LoopMaskEffect.Parameters["progress"].SetValue(progress);
            EffectLibrary.LoopMaskEffect.Parameters["color"].SetValue(color.ToVector4());
            EffectLibrary.LoopMaskEffect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(tex1, DrawPos - Main.screenPosition, null, Color.White, rotation, tex1.Size() / 2f, scale, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public static void DrawWhite(Texture2D tex, Vector2 Position, Rectangle? rectangle, Color color, float rot, Vector2 origin, float scale, SpriteEffects spriteEffects, BlendState blendState)
        {
            EasyDraw.AnotherDraw(SpriteSortMode.Immediate, blendState);
            EffectLibrary.WhiteEffect.Parameters["newColor"].SetValue(color.ToVector4());
            EffectLibrary.WhiteEffect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(tex, Position - Main.screenPosition, rectangle, Color.White, rot, origin, scale, spriteEffects, 0);
            EasyDraw.AnotherDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public static void DrawGradientCircle(Texture2D tex, Vector2 DrawPos, Color color, float rotation, float scale, BlendState blendState, float radius, float offset)
        {
            EasyDraw.AnotherDraw(SpriteSortMode.Immediate, blendState);
            EffectLibrary.GradientCircleEffect.Parameters["color"].SetValue(color.ToVector4());
            EffectLibrary.GradientCircleEffect.Parameters["offset"].SetValue(offset);
            EffectLibrary.GradientCircleEffect.Parameters["radius"].SetValue(radius);
            EffectLibrary.GradientCircleEffect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(tex, DrawPos - Main.screenPosition, null, Color.White, rotation, tex.Size() / 2f, scale, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public static void DrawGradientCircle(Texture2D tex, Vector2 DrawPos, Color color, float rotation, Vector2 scale, BlendState blendState, float radius, float offset)
        {
            EasyDraw.AnotherDraw(SpriteSortMode.Immediate, blendState);
            EffectLibrary.GradientCircleEffect.Parameters["color"].SetValue(color.ToVector4());
            EffectLibrary.GradientCircleEffect.Parameters["offset"].SetValue(offset);
            EffectLibrary.GradientCircleEffect.Parameters["radius"].SetValue(radius);
            EffectLibrary.GradientCircleEffect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(tex, DrawPos - Main.screenPosition, null, Color.White, rotation, tex.Size() / 2f, scale, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        public static Vector2 GetTipPos(float RotY, float RotZ, float RotX, bool Reverse = false)
        {
            Vector2 result = RotY.ToRotationVector2();
            result.Y *= (float)Math.Sin(RotZ);
            result = result.RotatedBy(RotX);
            if (Reverse) result.X = -result.X;
            return result;
        }

        public static float ReverseX(float k)
        {
            Vector2 vec = k.ToRotationVector2();
            vec.X *= -1;
            return vec.ToRotation();
        }

        public static float ReverseY(float k)
        {
            Vector2 vec = k.ToRotationVector2();
            vec.Y *= -1;
            return vec.ToRotation();
        }

        public static List<Vector2> SmoothTrajectory(List<Vector2> originalPoints, int segmentsPerSegment = 5, float tension = 0.5f)
        {
            if (originalPoints.Count < 2)
                return new(originalPoints);

            // 创建带边界虚拟点的新列表（处理首尾边界条件）
            List<Vector2> extendedPoints = new();
            extendedPoints.Add(originalPoints[0]); // 首部虚拟点
            extendedPoints.AddRange(originalPoints);
            extendedPoints.Add(originalPoints[originalPoints.Count - 1]); // 尾部虚拟点

            List<Vector2> smoothed = new List<Vector2>();

            // 保留第一个原始点
            smoothed.Add(originalPoints[0]);

            int n = originalPoints.Count;

            for (int i = 0; i < n - 1; i++)
            {
                // 获取四个控制点（从扩展列表中取）
                Vector2 p0 = extendedPoints[i];
                Vector2 p1 = extendedPoints[i + 1];
                Vector2 p2 = extendedPoints[i + 2];
                Vector2 p3 = extendedPoints[i + 3];

                // 在当前段生成中间点
                for (int s = 1; s <= segmentsPerSegment; s++)
                {
                    float t = s / (float)segmentsPerSegment;
                    Vector2 point = CalculateCatmullRomPoint(t, p0, p1, p2, p3, tension);
                    smoothed.Add(point);
                }
            }

            return smoothed;
        }

        private static Vector2 CalculateCatmullRomPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float tension)
        {
            // Catmull-Rom样条计算公式
            Vector2 a = 2f * p1;
            Vector2 b = (p2 - p0) * tension;
            Vector2 c = (2f * p0 - 5f * p1 + 4f * p2 - p3) * tension;
            Vector2 d = (-p0 + 3f * p1 - 3f * p2 + p3) * tension;

            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (a + (b * t) + (c * t2) + (d * t3));
        }

        public static void DrawIndicatorRing(Vector2 Center, float radius)
        {
            List<CustomVertexInfo> bars = new();
            int count = (int)(MathHelper.TwoPi * radius / 10f);
            for (int i = 0; i <= count; i++)
            {
                float r = i / (float)count * MathHelper.TwoPi;
                bars.Add(new CustomVertexInfo(Center - Main.screenPosition + r.ToRotationVector2() * (radius - 2), Color.White, new Vector3(i / (float)count, 0, 1)));
                bars.Add(new CustomVertexInfo(Center - Main.screenPosition + r.ToRotationVector2() * (radius + 2), Color.White, new Vector3(i / (float)count, 1, 1)));
            }
            DrawUtils.DrawTrail(TextureLibrary.LargeTrail, bars, Color.White, BlendState.Additive);
        }

        public static void DrawIndicatorLine(Vector2 Begin, Vector2 End)
        {
            if (Begin == End) return;
            List<CustomVertexInfo> bars = new();
            Vector2 UnitX = Vector2.Normalize(End - Begin);
            Vector2 UnitY = UnitX.RotatedBy(MathHelper.Pi / 2f);
            bars.Add(new CustomVertexInfo(Begin - UnitY * 2 - Main.screenPosition, Color.White, new Vector3(0, 0, 1)));
            bars.Add(new CustomVertexInfo(Begin + UnitY * 2 - Main.screenPosition, Color.White, new Vector3(0, 1, 1)));
            bars.Add(new CustomVertexInfo(End - UnitY * 2 - Main.screenPosition, Color.White, new Vector3(1, 0, 1)));
            bars.Add(new CustomVertexInfo(End + UnitY * 2 - Main.screenPosition, Color.White, new Vector3(1, 1, 1)));
            DrawUtils.DrawTrail(TextureLibrary.LargeTrail, bars, Color.White, BlendState.Additive);
        }

        public static void DrawIndicatorFan(Vector2 Center, float radius, float Begin, float End)
        {
            if (End < Begin)
            {
                Utils.Swap<float>(ref Begin, ref End);
            }
            while (End - Begin >= MathHelper.TwoPi)
            {
                End += MathHelper.TwoPi;
            }
            if (End == Begin) return;
            int count = (int)(radius * (End - Begin) / 10f);
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= count; i++)
            {
                float r = Begin + i / (float)count * (End - Begin);
                bars.Add(new CustomVertexInfo(Center - Main.screenPosition + r.ToRotationVector2() * (radius - 2), Color.White, new Vector3(i / (float)count, 0, 1)));
                bars.Add(new CustomVertexInfo(Center - Main.screenPosition + r.ToRotationVector2() * (radius + 2), Color.White, new Vector3(i / (float)count, 1, 1)));
            }
            DrawUtils.DrawTrail(TextureLibrary.LargeTrail, bars, Color.White, BlendState.Additive);
        }
    }
}
