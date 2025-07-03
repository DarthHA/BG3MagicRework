using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Projectiles.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring3
{
    public class CallLightningRepeatSpell : BaseSpell
    {
        public override string Name => "CallLightningRepeat";
        public override Color NameColor => Color.Blue;
        public override SchoolOfMagic Shool => SchoolOfMagic.Conjuration;
        public override int InitialRing => 3;
        public override DiceDamage BaseDamage => new(10, 3, DamageElement.Lightning, CombatStat.Ring3Damage);
        public override DiceDamage RisingDamageAddition => new(10, 1, DamageElement.Lightning, CombatStat.Ring3Damage);
        public override int SpellRange => 36;
        public override int AOERadius => 8;
        public override bool Concentration => true;
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            if (player.GetConcentration<ConCallLightning>() != -1)
            {
                ConCallLightning con = player.GetModPlayer<DNDMagicPlayer>().ConcentrationSlot[player.GetConcentration<ConCallLightning>()] as ConCallLightning;
                con.UseTime--;

                int protmp = player.NewMagicProj(mousePosition, Vector2.Zero, ModContent.ProjectileType<CallLightningProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, false, false, true);
                    Vector2 TargetPosition;
                    if ((Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM)
                    {
                        TargetPosition = SomeUtils.GetNoBlockEndPos(player.Center, mousePosition, SpellRange * 16f * ((Main.projectile[protmp].ModProjectile as BaseMagicProj).DistantSpellMM ? 2 : 1));
                    }
                    else
                    {
                        TargetPosition = SomeUtils.GetTileBlockedEndPos(player.Center, mousePosition, SpellRange * 16f * ((Main.projectile[protmp].ModProjectile as BaseMagicProj).DistantSpellMM ? 2 : 1));
                    }
                    Main.projectile[protmp].Center = TargetPosition;
                }
            }
        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (!Collision.CanHit(owner.position, owner.width, owner.height, mousePosition, 1, 1) && !owner.CarefulSpellMM())
            {
                Warning += LangLibrary.CannotSee + "\n";
            }
            if (mousePosition.Distance(owner.Center) > owner.GetSpellRange(Name) * 16)
            {
                Warning += LangLibrary.OutOfRange + "\n";
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

    }
}
