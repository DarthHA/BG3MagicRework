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
    public class MelfsMinuteMeteorsReleaseSpell : BaseSpell
    {
        public override string Name => "MelfsMinuteMeteorsRelease";
        public override Color NameColor => Color.Orange;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override int InitialRing => 3;
        public override DiceDamage BaseDamage => new(6, 2, DamageElement.Fire, CombatStat.Ring3Damage);
        public override int SpellRange => 36;
        public override int AOERadius => 6;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int protmp = player.GetProj(ModContent.ProjectileType<MelfsMinuteMeteorsController>());
            if (protmp != -1)
            {
                (Main.projectile[protmp].ModProjectile as MelfsMinuteMeteorsController).ReleaseCount += 2;
                (Main.projectile[protmp].ModProjectile as MelfsMinuteMeteorsController).ActivateMetaMagic(player, true, true, false, false, true);
            }
        }
        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            List<CustomVertexInfo> bars1 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-30,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-30,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(30,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
            DrawUtils.DrawRoSLaser(texRibbon, bars1, Color.Orange * light, 0.4f, 1f, miscTimer, BlendState.Additive);
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.Red * light, miscTimer, LightTex.Size() / 2f, 0.03f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.025f * scale, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(DrawUtils.ReverseSubtract);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light * 1.3f, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
            return false;
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
    }
}
