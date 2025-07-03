using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring1;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring1
{
    public class WitchBoltSpell : BaseSpell
    {
        public override string Name => "WitchBolt";
        public override Color NameColor => Color.Blue;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override int InitialRing => 1;
        public override DiceDamage BaseDamage => new(12, 1, DamageElement.Lightning, CombatStat.Ring1Damage);
        public override DiceDamage RisingDamageAddition => new(12, 1, DamageElement.Lightning, CombatStat.Ring1Damage);
        public override DiceDamage ExtraDamage => new(12, 1, DamageElement.Lightning, CombatStat.CantripDamage);
        public override DiceDamage RisingDamageAdditionExtra => new(12, 1, DamageElement.Lightning, CombatStat.CantripDamage);
        public override int SpellRange => 36;
        public override bool Concentration => true;
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            Vector2 ShootVel = Vector2.Normalize(mousePosition - tipPosition) * 50;
            int protmp = player.NewMagicProj(tipPosition, ShootVel, ModContent.ProjectileType<WitchBoltProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as WitchBoltProj).RelaPos = tipPosition - player.Center;
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, true, false, true);
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).extraDiceDamage = player.GetDiceDamage(ExtraDamage, InitialRing, Ring, RisingDamageAdditionExtra);
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
            if (!HasShot)
            {
                for (int i = 0; i < 3; i++)
                {
                    ArcSegments segs = new();
                    Vector2 End = tipPos + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 40) * scale;
                    segs.GenerateSegs(tipPos, End, new Vector2(20, 20) * scale, 30f * scale);
                    segs.DrawSegs(Color.Blue * light * 1.1f);
                }
            }
            Texture2D LightTex = TextureLibrary.BloomFlare;
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.Blue * light, 0, LightTex.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, 0, LightTex.Size() / 2f, 0.03f * scale, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

    }
}
