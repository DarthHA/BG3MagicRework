using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring3;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring3
{
    public class VampiricTouchSpell : BaseSpell
    {
        public override string Name => "VampiricTouch";
        public override Color NameColor => Color.DarkGreen;
        public override SchoolOfMagic Shool => SchoolOfMagic.Necromancy;
        public override int InitialRing => 3;
        public override DiceDamage BaseDamage => new(6, 3, DamageElement.Necrotic, CombatStat.Ring3Damage);
        public override DiceDamage RisingDamageAddition => new(6, 1, DamageElement.Necrotic, CombatStat.Ring3Damage);
        public override int SpellRange => 24;
        public override bool Concentration => false;         //先改成false，按照规则上的专注施法过于超模
        public override int TimeSpan => 60;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            (bool careful, bool distant, _, _, bool twinned) = player.ActivateMetaMagic(true, true, false, false, true);
            int target = SomeUtils.FindEnemyByOwner(Main.MouseWorld, player.Center, SpellRange * (distant ? 2 : 1) * 16, careful);
            if (target != -1)
            {
                int protmp = player.NewMagicProj(Main.npc[target].Center, Vector2.Zero, ModContent.ProjectileType<VampiricTouchProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), 0, Ring);
                if (protmp >= 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as VampiricTouchProj).TargetNPC = target;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).CarefulSpellMM = careful;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).DistantSpellMM = distant;
                    (Main.projectile[protmp].ModProjectile as BaseMagicProj).TwinnedSpellMM = twinned;
                }
            }

        }


        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = true;
            if (SomeUtils.FindEnemyBySelf(owner.Center, owner.GetSpellRange(Name) * 16, owner.CarefulSpellMM()) == -1)
            {
                success = false;
                Warning += LangLibrary.NoTarget + "\n";
            }
            return success;
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;

            for (int i = 0; i < 3; i++)
            {
                List<CustomVertexInfo> bars = new();
                for (int k = 0; k < 10; k++)
                {
                    float x = MathHelper.Lerp(-30, 0, k / 9f);
                    float deltaY = 5 * (float)Math.Sin(miscTimer * 24 + k / 10f * MathHelper.TwoPi);
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, -5 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + miscTimer * 6) - Main.screenPosition, Color.White, new Vector3(k / 9f, 0f, 1)));
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, 5 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + miscTimer * 6) - Main.screenPosition, Color.White, new Vector3(k / 9f, 1f, 1)));
                }
                DrawUtils.DrawRoSLaser(texRibbon, bars, color * light * 1.3f, 0.4f, 1f, miscTimer + 0.4f * i, BlendState.Additive);
            }
            for (int i = 0; i < 3; i++)
            {
                List<CustomVertexInfo> bars = new();
                for (int k = 0; k < 10; k++)
                {
                    float x = MathHelper.Lerp(-25, 0, k / 9f);
                    float deltaY = 5 * (float)Math.Sin(miscTimer * 24 + k / 10f * MathHelper.TwoPi);
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, -3 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + miscTimer * 6) - Main.screenPosition, Color.White, new Vector3(k / 9f, 0f, 1)));
                    bars.Add(new CustomVertexInfo(tipPos + new Vector2(x, 3 + deltaY).RotatedBy(MathHelper.TwoPi / 3f * i + miscTimer * 6) - Main.screenPosition, Color.White, new Vector3(k / 9f, 1f, 1)));
                }
                DrawUtils.DrawRoSLaser(texRibbon, bars, Color.White * light * 1.3f, 0.4f, 1f, miscTimer + 0.4f * i, DrawUtils.ReverseSubtract);
            }
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, color * light * 1.3f, miscTimer, LightTex.Size() / 2f, 0.12f * scale, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(DrawUtils.ReverseSubtract);
            Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light * 1.6f, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            return false;
        }

    }
}
