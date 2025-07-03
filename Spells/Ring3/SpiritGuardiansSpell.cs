using BG3MagicRework.BaseType;
using BG3MagicRework.Concentrations;
using BG3MagicRework.Projectiles.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring3
{
    public class SpiritGuardiansSpell : BaseSpell
    {
        public override string Name => "SpiritGuardians";
        public override Color NameColor => Color.Gold;
        public override SchoolOfMagic Shool => SchoolOfMagic.Conjuration;
        public override int InitialRing => 3;
        public override DiceDamage BaseDamage => new(8, 3, DamageElement.Radiant, CombatStat.Ring3Damage / 10f);
        public override DiceDamage RisingDamageAddition => new(8, 1, DamageElement.Radiant, CombatStat.Ring3Damage / 10f);
        public override int SpellRange => 0;
        public override int AOERadius => 15;
        public override bool Concentration => true;
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            int protmp = player.NewMagicProj(player.Center, Vector2.Zero, ModContent.ProjectileType<SpiritGuardiansProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as SpiritGuardiansProj).RelaPos = tipPosition - player.Center;
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, false, true, false, false);

                BaseConcentration con = player.GenerateConcentration<ConSpiritGuardians>(Ring, TimeSpan * 60 * ((Main.projectile[protmp].ModProjectile as BaseMagicProj).ExtendedSpellMM ? 2 : 1), true);
                if (con != null)
                {
                    con.projIndex = protmp;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).ConUUID = con.UUID;
                }

            }
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            scale *= 3f;
            return true;
        }

        public override void DrawBehind(Player owner, int ring, float light, Color color, float scale, float miscTimer)
        {
            EasyDraw.AnotherDraw(BlendState.Additive);
            Texture2D tex = TextureLibrary.BloomFlare;
            Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, color * light, miscTimer, tex.Size() / 2f, 0.3f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * light, miscTimer, tex.Size() / 2f, 0.25f * scale, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }

        public override bool ModifyDrawRangeInfo(Player player, int Ring)
        {
            int radius = player.GetAOERadius(Name) * 16 + (Ring - 3) * 50;
            DrawUtils.DrawIndicatorRing(player.Center, radius);
            return false;
        }
    }
}