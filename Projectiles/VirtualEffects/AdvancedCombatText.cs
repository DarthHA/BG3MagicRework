using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.VirtualEffects
{
    public class AdvancedCombatText : ModProjectile
    {
        public string Text = "";
        public Color Color = Color.White;

        public int alphaDir = 1;
        public int lifeTime = 60;
        public bool DeleteLine = false;

        public override string Texture => "BG3MagicRework/Images/PlaceHolder";
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 0f;
            Projectile.timeLeft = 9999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Opacity = 1;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override void AI()
        {
            Projectile.Opacity += alphaDir * 0.05f;
            if (Projectile.Opacity <= 0.6)
                alphaDir = 1;

            if (Projectile.Opacity >= 1f)
            {
                Projectile.Opacity = 1f;
                alphaDir = -1;
            }

            Projectile.velocity.X *= 0.93f;
            Projectile.velocity.Y *= 0.92f;

            lifeTime--;
            if (lifeTime <= 0)
            {
                Projectile.scale -= 0.1f;
                if (Projectile.scale < 0.1f)
                {
                    Projectile.Kill();
                    return;
                }

            }
            else
            {
                if (Projectile.scale < 1)
                    Projectile.scale += 0.1f;

                if (Projectile.scale > 1)
                    Projectile.scale = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 origin = FontAssets.CombatText[0].Value.MeasureString(Text) / 2f;
            float scale = Projectile.scale / 1f;
            float cR = Color.R;
            float cG = Color.G;
            float cB = Color.B;
            float cA = Color.A;
            cR *= scale * Projectile.Opacity * 0.3f;
            cB *= scale * Projectile.Opacity * 0.3f;
            cG *= scale * Projectile.Opacity * 0.3f;
            cA *= scale * Projectile.Opacity;
            Color color = new((int)cR, (int)cG, (int)cB, (int)cA);
            for (int l = 0; l < 5; l++)
            {
                float offsetX = 0f;
                float offsetY = 0f;
                switch (l)
                {
                    case 0:
                        offsetX -= 1f;
                        break;
                    case 1:
                        offsetX += 1f;
                        break;
                    case 2:
                        offsetY -= 1f;
                        break;
                    case 3:
                        offsetY += 1f;
                        break;
                    default:
                        cR = Color.R * scale * Projectile.Opacity;
                        cB = Color.B * scale * Projectile.Opacity;
                        cG = Color.G * scale * Projectile.Opacity;
                        cA = Color.A * scale * Projectile.Opacity;
                        color = new((int)cR, (int)cG, (int)cB, (int)cA);
                        break;
                }
                if (Main.player[Main.myPlayer].gravDir == -1f)
                {
                    float num17 = Projectile.position.Y - Main.screenPosition.Y;
                    num17 = Main.screenHeight - num17;
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.CombatText[0].Value, Text, Projectile.position.X - Main.screenPosition.X + offsetX + origin.X, num17 + offsetY + origin.Y, color, Color.Black, origin, Projectile.scale);

                    if (DeleteLine)
                    {
                        Vector2 Left = new Vector2(Projectile.position.X + offsetX + origin.X, num17 + offsetY + origin.Y + Main.screenPosition.Y) - new Vector2(origin.X * Projectile.scale, 0);
                        float length = origin.X * 2 * MathHelper.Lerp(0, 1, 1 - lifeTime / 60f) * Projectile.scale;
                        Vector2 OffSet = new Vector2(0, -6) * Projectile.scale;
                        if (OffSet.X != length)
                        {
                            Utils.DrawLine(Main.spriteBatch, Left + OffSet, Left + new Vector2(length, 0) + OffSet, color, color, 2 * Projectile.scale);
                        }
                    }

                }
                else
                {
                    DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.CombatText[0].Value, Text, new Vector2(Projectile.position.X - Main.screenPosition.X + offsetX + origin.X, Projectile.position.Y - Main.screenPosition.Y + offsetY + origin.Y), color, Projectile.rotation, origin, Projectile.scale, 0, 0f);

                    if (DeleteLine)
                    {
                        Vector2 Left = new Vector2(Projectile.position.X + offsetX + origin.X, Projectile.position.Y + offsetY + origin.Y) - new Vector2(origin.X * Projectile.scale, 0);
                        float length = origin.X * 2 * MathHelper.Lerp(0, 1, 1 - lifeTime / 60f) * Projectile.scale;
                        Vector2 OffSet = new Vector2(0, -6) * Projectile.scale;
                        if (OffSet.X != length)
                        {
                            Utils.DrawLine(Main.spriteBatch, Left + OffSet, Left + new Vector2(length, 0) + OffSet, color, color, 2 * Projectile.scale);
                        }
                    }

                }
            }

            return false;
        }

        public static int NewText(Rectangle location, Color color, string text, bool delete = false)
        {
            int protmp = Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), location.TopLeft(), Vector2.Zero, ModContent.ProjectileType<AdvancedCombatText>(), 0, 0, Main.myPlayer);
            if (protmp >= 0 && protmp < 1000)
            {
                Vector2 vector = FontAssets.CombatText[0].Value.MeasureString(text);
                Main.projectile[protmp].Opacity = 1f;
                (Main.projectile[protmp].ModProjectile as AdvancedCombatText).alphaDir = -1;
                Main.projectile[protmp].position.X = location.X + location.Width * 0.5f - vector.X * 0.5f;
                Main.projectile[protmp].position.Y = location.Y + location.Height * 0.25f - vector.Y * 0.5f;
                Main.projectile[protmp].position.X += Main.rand.Next(-(int)(location.Width * 0.5f), (int)(location.Width * 0.5f) + 1);
                Main.projectile[protmp].position.Y += Main.rand.Next(-(int)(location.Height * 0.5f), (int)(location.Height * 0.5f) + 1);
                (Main.projectile[protmp].ModProjectile as AdvancedCombatText).Color = color;
                (Main.projectile[protmp].ModProjectile as AdvancedCombatText).Text = text;
                Main.projectile[protmp].velocity.Y = -7f;
                if (Main.player[Main.myPlayer].gravDir == -1f)
                {
                    Main.projectile[protmp].velocity.Y *= -1f;
                    Main.projectile[protmp].position.Y = location.Y + location.Height * 0.75f + vector.Y * 0.5f;
                }
                (Main.projectile[protmp].ModProjectile as AdvancedCombatText).DeleteLine = delete;
                return protmp;
            }
            return -1;
        }
    }
}
