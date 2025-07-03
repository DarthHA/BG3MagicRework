using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Projectiles.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring2
{
    public class SilenceSpell : BaseSpell
    {
        public override string Name => "Silence";
        public override Color NameColor => Color.LightPink;
        public override SchoolOfMagic Shool => SchoolOfMagic.Illusion;
        public override int InitialRing => 2;
        public override int SpellRange => 36;
        public override int AOERadius => 18;
        public override bool Concentration => true;
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int protmp = player.NewMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<SilenceRing>(), Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, true, false, false);
                Vector2 TargetPosition;
                if ((Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM)       //穿墙修正
                {
                    TargetPosition = SomeUtils.GetNoBlockEndPos(player.Center, mousePosition, (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetSpellRange(Name) * 16);
                }
                else
                {
                    TargetPosition = SomeUtils.GetTileBlockedEndPos(player.Center, mousePosition, (Main.projectile[protmp].ModProjectile as BaseMagicProj).GetSpellRange(Name) * 16);
                }
                Main.projectile[protmp].Center = TargetPosition;

                BaseConcentration con = player.GenerateConcentration<ConSilence>(Ring, TimeSpan * ((Main.projectile[protmp].ModProjectile as BaseMagicProj).ExtendedSpellMM ? 2 : 1) * 60, true);
                if (con != null)
                {
                    con.projIndex = protmp;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = con.UUID;
                }
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
                new CustomVertexInfo(tipPos + new Vector2(-40,-40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-40,40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(40,-40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(40,40).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.White * light, 0.4f, 1f, miscTimer, DrawUtils.ReverseSubtract);
            DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.Red * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.Red * light, miscTimer, LightTex.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.01f * scale, SpriteEffects.None, 0);
            return false;
        }


        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            float radius = (player.GetAOERadius(Name) + 6 * (Ring - 2)) * 16;
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
