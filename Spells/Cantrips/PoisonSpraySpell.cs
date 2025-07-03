using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Cantrips;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Cantrips
{
    public class PoisonSpraySpell : BaseSpell
    {
        public override string Name => "PoisonSpray";
        public override Color NameColor => Color.GreenYellow;
        public override SchoolOfMagic Shool => SchoolOfMagic.Conjuration;
        public override DiceDamage BaseDamage => new(12, 1, DamageElement.Poison, CombatStat.CantripDamage);
        public override DiceDamage RisingDamageAddition => new(12, 1, DamageElement.Poison, CombatStat.CantripDamage);
        public override int InitialRing => 0;
        public override int AOERadius => 18;
        public override bool Concentration => false;
        public override int TimeSpan => 0;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 Vel = Vector2.Normalize(mousePosition - tipPosition);
            int protmp = player.NewMagicProj(tipPosition, Vel, ModContent.ProjectileType<PoisonSprayProj>(), player.GetDiceDamage(BaseDamage, InitialRing, InitialRing, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, false, false, false, true);
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
            List<CustomVertexInfo> bars3 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-30,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-30,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars3, Color.White * light, 0.4f, 1f, miscTimer, DrawUtils.ReverseSubtract);
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, color * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.03f * scale, SpriteEffects.None, 0);
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
