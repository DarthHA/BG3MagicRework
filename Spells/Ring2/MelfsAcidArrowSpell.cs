using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring2
{
    public class MelfsAcidArrowSpell : BaseSpell
    {
        public override string Name => "MelfsAcidArrow";
        public override Color NameColor => Color.GreenYellow;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override DiceDamage BaseDamage => new(4, 4, DamageElement.Acid, CombatStat.Ring2Damage);
        public override DiceDamage RisingDamageAddition => new(4, 2, DamageElement.Acid, CombatStat.Ring2Damage);
        public override DiceDamage ExtraDamage => new(4, 2, DamageElement.Acid, CombatStat.Ring2Damage);
        public override DiceDamage RisingDamageAdditionExtra => new(4, 1, DamageElement.Acid, CombatStat.Ring2Damage);
        public override int InitialRing => 2;
        public override int SpellRange => 36;
        public override int AOERadius => 6;
        public override int TimeSpan => 6;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 Vel = Vector2.Normalize(mousePosition - tipPosition) * 20f;
            int protmp = player.NewMagicProj(tipPosition, Vel, ModContent.ProjectileType<MelfsAcidArrowProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).extraDiceDamage = player.GetDiceDamage(ExtraDamage, InitialRing, Ring, RisingDamageAdditionExtra);
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, false, false, true);
            }
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (mousePosition.Distance(owner.Center) > owner.GetSpellRange(Name) * 16)
            {
                Warning += LangLibrary.OutOfRange + "\n";
            }
            if (!owner.CarefulSpellMM() && !Collision.CanHit(owner.position, owner.width, owner.height, mousePosition, 1, 1))
            {
                Warning += LangLibrary.CannotSee + "\n";
            }
            return success;
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            if (!HasShot)
            {
                Draw710(tipPos, 30, -miscTimer, Color.GreenYellow * light * 0.9f, miscTimer);
                EasyDraw.AnotherDraw(BlendState.Additive);
                Texture2D tex = TextureLibrary.BloomFlare;
                Main.spriteBatch.Draw(tex, tipPos - Main.screenPosition, null, Color.White, miscTimer, tex.Size() / 2f, 0.03f, SpriteEffects.None, 0);
            }
            return false;
        }

        public void Draw710(Vector2 Center, float radius, float progress, Color color, float rot = 0)
        {
            List<CustomVertexInfo> bars = new();
            for (int i = 0; i <= 240; i++)
            {
                float r = i * MathHelper.TwoPi / 240f + rot;
                Vector2 Pos1 = r.ToRotationVector2() * 1;
                Vector2 Pos2 = r.ToRotationVector2() * radius;
                bars.Add(new CustomVertexInfo(Center + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                bars.Add(new CustomVertexInfo(Center + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
            }
            DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars, color, 0.5f, 2f, progress, BlendState.Additive);
        }
    }
}
