using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring3
{
    public class SleetStormSpell : BaseSpell
    {
        public override string Name => "SleetStorm";
        public override Color NameColor => Color.Cyan;
        public override SchoolOfMagic Shool => SchoolOfMagic.Conjuration;
        public override int InitialRing => 3;
        public override int SpellRange => 36;
        public override int AOERadius => 18;
        public override bool Concentration => false;
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int protmp = player.New1DmgMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<SleetStormProj>(), Ring);
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
            List<CustomVertexInfo> bars1 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-30,-15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-30,15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,-15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,15).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            List<CustomVertexInfo> bars2 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-15,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-15,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(15,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(15,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.White * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.White * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, color * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
            return false;
        }

        public override void DrawBehind(Player owner, int ring, float light, Color color, float scale, float miscTimer)
        {
            DrawRing(owner.Center, 50, 20, Color.SkyBlue * light, miscTimer * owner.direction);
        }


        public void DrawRing(Vector2 Center, float radius, float width, Color color, float miscTimer)
        {
            Texture2D tex = TextureLibrary.Ribbon;
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i < 60; i++)
            {
                float rot = MathHelper.TwoPi / 60f * i;
                Vector2 Pos1 = Center + rot.ToRotationVector2() * (radius + width / 2f);
                Vector2 Pos2 = Center + rot.ToRotationVector2() * (radius - width / 2f);
                bars.Add(new CustomVertexInfo(Pos1 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 0, 1f)));
                bars.Add(new CustomVertexInfo(Pos2 - Main.screenPosition, Color.White, new Vector3(1 / 60f * i, 1, 1f)));
            }
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius + width / 2f, 0) - Main.screenPosition, Color.White, new Vector3(1f, 0, 1f)));
            bars.Add(new CustomVertexInfo(Center + new Vector2(radius - width / 2f, 0) - Main.screenPosition, Color.White, new Vector3(1f, 1, 1f)));
            DrawUtils.DrawLoopTrail(tex, bars, color, 0.33f, miscTimer, BlendState.Additive);
        }

        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            int width = player.GetAOERadius(Name) * 16 + (Ring - 3) * 16 * 3;
            DrawUtils.DrawIndicatorLine(new Vector2(Main.MouseWorld.X - width, Main.screenPosition.Y), new Vector2(Main.MouseWorld.X - width, Main.screenPosition.Y + Main.screenHeight));
            DrawUtils.DrawIndicatorLine(new Vector2(Main.MouseWorld.X + width, Main.screenPosition.Y), new Vector2(Main.MouseWorld.X + width, Main.screenPosition.Y + Main.screenHeight));
            return false;
        }
    }
}
