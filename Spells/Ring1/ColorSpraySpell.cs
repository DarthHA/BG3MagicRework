using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring1
{
    public class ColorSpraySpell : BaseSpell
    {
        public override string Name => "ColorSpray";
        public override Color NameColor => Color.LightPink;
        public override SchoolOfMagic Shool => SchoolOfMagic.Illusion;
        public override int InitialRing => 1;
        public override int AOERadius => 24;
        public override int DifficultyClass => 15;
        public override bool Concentration => false;
        public override int TimeSpan => 6;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 Vel = Vector2.Normalize(mousePosition - tipPosition);
            int protmp = player.New1DmgMagicProj(tipPosition, Vel, ModContent.ProjectileType<ColorSprayProj>(), Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, false, true, true, false);
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).DifficultyClass = player.GetDifficultyClass(Name) + (Ring - InitialRing);
            }
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (mousePosition.Distance(owner.Center) > owner.GetAOERadius(Name) * 16)
            {
                Warning += LangLibrary.OutOfRange + "\n";
            }
            if (!Collision.CanHit(owner.position, owner.width, owner.height, mousePosition, 1, 1) && !owner.CarefulSpellMM())
            {
                Warning += LangLibrary.CannotSee + "\n";
            }
            return success;
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            for (int i = 0; i < 4; i++)
            {
                List<CustomVertexInfo> bars = new();
                for (int k = 0; k < 10; k++)
                {
                    float x = MathHelper.Lerp(-30, 0, k / 9f);
                    float deltaY = 5 * (float)Math.Sin(miscTimer * 3 + k / 10f * MathHelper.TwoPi);
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, -5 + deltaY).RotatedBy(MathHelper.TwoPi / 4f * i + miscTimer) - Main.screenPosition, Color.White, new Vector3(k / 9f, 0f, 1)));
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, 5 + deltaY).RotatedBy(MathHelper.TwoPi / 4f * i + miscTimer) - Main.screenPosition, Color.White, new Vector3(k / 9f, 1f, 1)));
                }
                DrawUtils.DrawRoSLaser(texRibbon, bars, Main.DiscoColor * light * 1.3f, 0.4f, 1f, miscTimer + 0.4f * i, BlendState.Additive);
            }

            for (int i = 0; i < 4; i++)
            {
                List<CustomVertexInfo> bars = new();
                for (int k = 0; k < 10; k++)
                {
                    float x = MathHelper.Lerp(-25, 0, k / 9f);
                    float deltaY = 5 * (float)Math.Sin(miscTimer * 3 + k / 10f * MathHelper.TwoPi);
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, -3 + deltaY).RotatedBy(MathHelper.TwoPi / 4f * i + miscTimer) - Main.screenPosition, Color.White, new Vector3(k / 9f, 0f, 1)));
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, 3 + deltaY).RotatedBy(MathHelper.TwoPi / 4f * i + miscTimer) - Main.screenPosition, Color.White, new Vector3(k / 9f, 1f, 1)));
                }
                DrawUtils.DrawRoSLaser(texRibbon, bars, Color.White * light * 1.3f, 0.4f, 1f, miscTimer + 0.4f * i, BlendState.Additive);
            }

            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Main.DiscoColor * light * 1.3f, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            float r = (Main.MouseWorld - player.Center).ToRotation();
            DrawUtils.DrawIndicatorFan(player.Center, player.GetAOERadius(Name) * 16, r - MathHelper.Pi / 3, r + MathHelper.Pi / 3);
            DrawUtils.DrawIndicatorLine(player.Center, player.Center + (r - MathHelper.Pi / 3f).ToRotationVector2() * player.GetAOERadius(Name) * 16);
            DrawUtils.DrawIndicatorLine(player.Center, player.Center + (r + MathHelper.Pi / 3f).ToRotationVector2() * player.GetAOERadius(Name) * 16);
            return false;
        }
    }
}
