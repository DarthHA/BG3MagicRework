

using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring3
{
    public class GlyphOfWardingSpell : BaseSpell
    {
        public override string Name => "GlyphOfWarding";
        public override Color NameColor => Color.LightGoldenrodYellow;
        public override SchoolOfMagic Shool => SchoolOfMagic.Abjuration;
        public override int InitialRing => 3;
        public override DiceDamage BaseDamage => new(8, 5, DamageElement.Unknown, CombatStat.Ring3Damage);
        public override DiceDamage RisingDamageAddition => new(8, 1, DamageElement.Unknown, CombatStat.Ring3Damage);
        public override int SpellRange => 36;
        public override int AOERadius => 16;
        public override bool Concentration => false;
        public override int TimeSpan => -1;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<GlyphOfWardingProj>() && proj.owner == player.whoAmI)
                {
                    (proj.ModProjectile as GlyphOfWardingProj).Destroy();
                }
            }
            int protmp = player.NewMagicProj(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<GlyphOfWardingProj>(), player.GetDiceDamage(BaseDamage, InitialRing, Ring, RisingDamageAddition), Ring);
            if (protmp >= 0 && protmp < 1000)
            {
                (Main.projectile[protmp].ModProjectile as BaseMagicProj).ActivateMetaMagic(player, true, true, false, false, true);
                (Main.projectile[protmp].ModProjectile as GlyphOfWardingProj).damageElement = GetDamageType(player.GetModPlayer<DNDMagicPlayer>().GoWType);
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
            int gowType = owner.GetModPlayer<DNDMagicPlayer>().GoWType;
            DamageElement eleType = GetDamageType(gowType);
            color = eleType == DamageElement.None ? Color.White : SomeUtils.GetColor(eleType);

            Texture2D texRibbon = TextureLibrary.Ribbon;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            if (eleType == DamageElement.None)
            {
                for (int i = 0; i < 4; i++)
                {
                    List<CustomVertexInfo> bars = new()
                    {
                    new CustomVertexInfo(tipPos + new Vector2(-30,-3).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 2) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(-30,3).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 2) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(0,-7).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 2) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(0,7).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 2) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                    };
                    DrawUtils.DrawRoSLaser(texRibbon, bars, Color.White * light, 0.4f, 1f, miscTimer * 2 + 0.4f * i, BlendState.Additive);
                }
                List<CustomVertexInfo> bars2 = new()
                {
                    new CustomVertexInfo(tipPos + new Vector2(-30,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(-30,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(30,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                    new CustomVertexInfo(tipPos + new Vector2(30,30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
                DrawUtils.DrawRoSLaser(texRibbon, bars2, Color.White * 0.8f * light, 0.4f, 1f, miscTimer, BlendState.Additive);
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, color * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.02f * scale, SpriteEffects.None, 0);
            }
            else if (eleType == DamageElement.Lightning)
            {
                for (int i = 0; i < 3; i++)
                {
                    ArcSegments segs = new();
                    Vector2 End = tipPos + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 40) * scale;
                    segs.GenerateSegs(tipPos, End, new Vector2(20, 20) * scale, 30f * scale);
                    segs.DrawSegs(Color.Blue * light * 1.1f);
                }
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.Blue * light, 0, LightTex.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, 0, LightTex.Size() / 2f, 0.03f * scale, SpriteEffects.None, 0);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            else if (eleType == DamageElement.Thunder)
            {
                List<CustomVertexInfo> bars2 = new();
                for (int i = 0; i <= 240; i++)
                {
                    float r = i * MathHelper.TwoPi / 240f + miscTimer;
                    Vector2 Pos1 = r.ToRotationVector2() * 1;
                    Vector2 Pos2 = r.ToRotationVector2() * 40;
                    bars2.Add(new CustomVertexInfo(tipPos + Pos1 - Main.screenPosition, Color.White, new Vector3(0, i / 240f, 1f)));
                    bars2.Add(new CustomVertexInfo(tipPos + Pos2 - Main.screenPosition, Color.White, new Vector3(1, i / 240f, 1f)));
                }
                DrawUtils.DrawRoSLaser(TextureLibrary.Perlin, bars2, Color.White * 0.8f * light, 0.4f, 2f, -miscTimer * 4, BlendState.Additive);
                for (int i = 0; i < 4; i++)
                {
                    List<CustomVertexInfo> bars = new()
                    {
                        new CustomVertexInfo(tipPos + new Vector2(-30,-3).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 8) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                        new CustomVertexInfo(tipPos + new Vector2(-30,3).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 8) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                        new CustomVertexInfo(tipPos + new Vector2(0,-7).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 8) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                        new CustomVertexInfo(tipPos + new Vector2(0,7).RotatedBy(MathHelper.Pi / 2f * i + miscTimer * 8) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                    };
                    DrawUtils.DrawRoSLaser(texRibbon, bars, Color.White * light, 0.4f, 1f, miscTimer * 2 + 0.4f * i, BlendState.Additive);
                }
            }
            else if (eleType == DamageElement.Cold)
            {
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
            }
            else
            {
                List<CustomVertexInfo> bars3 = new()
                {
                new CustomVertexInfo(tipPos + new Vector2(-40,-20).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(-40,20).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(0, 1f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(40,-30).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 0f, 1)),
                new CustomVertexInfo(tipPos + new Vector2(40,20).RotatedBy(MathHelper.Pi / 2f) - Main.screenPosition, Color.White, new Vector3(1, 1f, 1))
                };
                DrawUtils.DrawRoSLaser(texRibbon, bars3, color * light, 0.4f, 1f, miscTimer, BlendState.Additive);
                EasyDraw.AnotherDraw(BlendState.Additive);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, color * light, miscTimer, LightTex.Size() / 2f, 0.06f * scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(LightTex, tipPos - Main.screenPosition, null, Color.White * light, miscTimer, LightTex.Size() / 2f, 0.04f * scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public static DamageElement GetDamageType(int index)
        {
            switch (index)
            {
                case 0:
                    return DamageElement.Acid;
                case 1:
                    return DamageElement.Cold;
                case 2:
                    return DamageElement.Fire;
                case 3:
                    return DamageElement.Lightning;
                case 4:
                    return DamageElement.Thunder;
                default:
                    return DamageElement.None;
            }
        }
    }
}