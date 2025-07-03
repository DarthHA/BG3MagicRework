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
    public class GreaseSpell : BaseSpell
    {
        public override string Name => "Grease";
        public override Color NameColor => Color.Yellow;
        public override SchoolOfMagic Shool => SchoolOfMagic.Conjuration;
        public override int InitialRing => 1;
        public override int SpellRange => 36;
        public override int AOERadius => 12;
        public override bool Concentration => false;
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int protmp = player.New1DmgMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<GreaseProj>(), Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, true, false, false);
                Vector2 TargetPosition;
                if ((Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM)
                {
                    TargetPosition = SomeUtils.GetNoBlockEndPos(player.Center, mousePosition, (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetSpellRange(Name) * 16);
                }
                else
                {
                    TargetPosition = SomeUtils.GetTileBlockedEndPos(player.Center, mousePosition, (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetSpellRange(Name) * 16);
                }
                Main.projectile[protmp].Center = TargetPosition;
            }
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (mousePosition.Distance(owner.Center) > owner.GetSpellRange(Name) * 16)
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
                    float x = MathHelper.Lerp(-20, 0, k / 9f);
                    float deltaY = 4 * (float)Math.Sin(miscTimer * 8 + k / 10f * MathHelper.TwoPi);
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, -3 + deltaY).RotatedBy(MathHelper.TwoPi / 4f * i + miscTimer * 4) - Main.screenPosition, Color.White, new Vector3(k / 9f, 0f, 1)));
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, 3 + deltaY).RotatedBy(MathHelper.TwoPi / 4f * i + miscTimer * 4) - Main.screenPosition, Color.White, new Vector3(k / 9f, 1f, 1)));
                }
                DrawUtils.DrawRoSLaser(texRibbon, bars, Color.Orange * light * 1.3f, 0.4f, 1f, miscTimer * 4 + 0.4f * i, BlendState.Additive);
            }

            for (int i = 0; i < 4; i++)
            {
                List<CustomVertexInfo> bars = new();
                for (int k = 0; k < 10; k++)
                {
                    float x = MathHelper.Lerp(-15, 0, k / 9f);
                    float deltaY = 4 * (float)Math.Sin(miscTimer * 8 + k / 10f * MathHelper.TwoPi);
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, -2 + deltaY).RotatedBy(MathHelper.TwoPi / 4f * i + miscTimer * 4) - Main.screenPosition, Color.White, new Vector3(k / 9f, 0f, 1)));
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, 2 + deltaY).RotatedBy(MathHelper.TwoPi / 4f * i + miscTimer * 4) - Main.screenPosition, Color.White, new Vector3(k / 9f, 1f, 1)));
                }
                DrawUtils.DrawRoSLaser(texRibbon, bars, Color.White * light * 1.3f, 0.4f, 1f, miscTimer * 4 + 0.4f * i, BlendState.Additive);
            }

            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.Orange * light * 1.3f, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.01f * scale, SpriteEffects.None, 0);
            return false;
        }


        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            float radius = (player.GetAOERadius(Name) + 4 * (Ring - 1)) * 16;
            Vector2 mouseWorld = Main.MouseWorld;
            DrawUtils.DrawIndicatorRing(player.Center, player.GetSpellRange(Name) * 16);
            if (mouseWorld.Distance(player.Center) > player.GetSpellRange(Name) * 16)
            {
                mouseWorld = player.Center + Vector2.Normalize(mouseWorld - player.Center) * player.GetSpellRange(Name) * 16;
            }
            DrawUtils.DrawIndicatorRing(mouseWorld, radius);
            return false;
        }
    }
}
