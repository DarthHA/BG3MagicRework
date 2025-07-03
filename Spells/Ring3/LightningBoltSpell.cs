using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring3
{
    public class LightningBoltSpell : BaseSpell
    {
        public override string Name => "LightningBolt";
        public override Color NameColor => Color.Blue;
        public override SchoolOfMagic Shool => SchoolOfMagic.Evocation;
        public override int InitialRing => 3;
        public override DiceDamage BaseDamage => new(6, 8, DamageElement.Lightning, CombatStat.Ring3Damage);

        public override DiceDamage RisingDamageAddition => new(6, 1, DamageElement.Lightning, CombatStat.Ring3Damage);
        public override int SpellRange => 0;
        public override int AOERadius => 42;
        public override bool Concentration => false;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            mousePosition = player.Center + Vector2.Normalize(mousePosition - player.Center) * AOERadius * 16;
            int protmp = player.NewMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<LightningBoltProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as LightningBoltProj).RelaPos = tipPosition - player.Center;
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, false, false, false, true);
                if ((Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM)
                {
                    Main.projectile[protmp].Center = SomeUtils.GetNoBlockEndPos(tipPosition, mousePosition, AOERadius * 16);
                }
                else
                {
                    Main.projectile[protmp].Center = SomeUtils.GetTileBlockedEndPos(tipPosition, mousePosition, AOERadius * 16);
                }
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
            float modifiedScale = HasShot ? 2 : 1;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.Blue * light, 0, LightTex.Size() / 2f, 0.06f * modifiedScale * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, 0, LightTex.Size() / 2f, 0.03f * modifiedScale * scale, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);

            return false;
        }
        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            float r = (Main.MouseWorld - player.Center).ToRotation();
            Vector2 UnitY = (r + MathHelper.Pi / 2f).ToRotationVector2();
            DrawUtils.DrawIndicatorLine(player.Center + UnitY * 40, player.Center + UnitY * 40 + r.ToRotationVector2() * player.GetAOERadius(Name) * 16);
            DrawUtils.DrawIndicatorLine(player.Center - UnitY * 40, player.Center - UnitY * 40 + r.ToRotationVector2() * player.GetAOERadius(Name) * 16);
            DrawUtils.DrawIndicatorLine(player.Center - UnitY * 40, player.Center + UnitY * 40);
            DrawUtils.DrawIndicatorLine(player.Center - UnitY * 40 + r.ToRotationVector2() * player.GetAOERadius(Name) * 16, player.Center + UnitY * 40 + r.ToRotationVector2() * player.GetAOERadius(Name) * 16);
            return false;
        }
    }
}
