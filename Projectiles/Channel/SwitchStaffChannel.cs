using BG3MagicRework.BaseType;
using BG3MagicRework.Spells.Ring3;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Channel
{
    public abstract class BaseSwitchStaffChannel : BaseChannel
    {
        //未处理的目标位置
        public float TargetRot = 0;

        public float Rotation = 0;
        public float RotationZ = 0;
        public float RotationX = 0;

        public Vector2 TipPos = Vector2.Zero;
        //用于复位
        public float OldRotation = 0;
        public float OldRotationZ = 0;
        public float OldLight = 0;

        /// <summary>
        /// 贴图是否为斜着的法杖
        /// </summary>
        public bool IsStaff = true;


        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.netImportant = true;
        }

        public virtual void SafeSetDefaults()
        {

        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.IsDead() || owner.CCed)
            {
                Projectile.Kill();
                return;
            }
            owner.itemAnimation = 2;
            owner.itemTime = 2;
            owner.heldProj = Projectile.whoAmI;
            Projectile.Center = owner.Center;

            Texture2D WeaponTex = Terraria.GameContent.TextureAssets.Item[ItemType].Value;
            float WeaponLength = IsStaff ? WeaponTex.Size().Length() : WeaponTex.Width;

            string mouseText = "";

            ModifyMouseText(owner, ref mouseText);

            if (Phase == 0) //20帧抬手
            {
                float rot = MathHelper.Lerp(MathHelper.Pi / 3, -MathHelper.Pi / 3, SmoothSwing(Timer / 20f));
                Vector2 unit = rot.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                Rotation = MathHelper.Lerp(MathHelper.Pi / 6, -MathHelper.Pi / 3, SmoothSwing(Timer / 20f));
                RotationZ = MathHelper.Lerp(MathHelper.Pi / 12, MathHelper.Pi / 2, Timer / 20f);
                Light = MathHelper.Lerp(0, 1, SmoothSwing(Timer / 20f));

                if (Timer < 20)
                {
                    Timer++;
                }
                else
                {
                    Timer = 0;
                    Phase = 1;
                }
            }
            else if (Phase == 1)  //持续到单击待机
            {
                Rotation = -MathHelper.Pi / 3 + (float)Math.Sin(Timer / 60f * MathHelper.TwoPi) * MathHelper.Pi / 12f;
                RotationZ = MathHelper.Pi / 2f + ((float)Math.Cos(Timer / 60f * MathHelper.TwoPi) - 1) * MathHelper.Pi / 12f;
                Light = 0.85f + (float)Math.Cos(Timer / 60f * MathHelper.TwoPi) * 0.15f;

                Vector2 unit = Rotation.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                //改环

                if (PlayerInput.ScrollWheelDeltaForUI != 0)
                {
                    int factor = PlayerInput.ScrollWheelDeltaForUI / 120;
                    OnScroll(owner, factor);
                }
                if (owner.LeftClick() || owner.RightClick())  //左键和右键都会退出，毕竟这只是调节用的
                {
                    Timer = 0;
                    Phase = 3;
                    OldRotation = Rotation;
                    OldRotationZ = RotationZ;
                    OldLight = Light;
                }
                else
                {
                    Timer++;
                }
            }
            else if (Phase == 3)       //持续10帧撤回复位
            {
                Rotation = MathHelper.Lerp(OldRotation, -MathHelper.Pi / 3, Timer / 5f);
                RotationZ = MathHelper.Lerp(OldRotationZ, MathHelper.Pi / 2f, Timer / 5f);
                Light = MathHelper.Lerp(OldLight, 0.8f, Timer / 5f);

                Vector2 unit = Rotation.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);
                if (Timer < 5)
                {
                    Timer++;
                }
                else
                {
                    Timer = 0;
                    Phase = 4;
                }
            }
            else if (Phase == 4) //10帧放手
            {
                float rot = MathHelper.Lerp(-MathHelper.Pi / 3, MathHelper.Pi / 3, SmoothThrow(Timer / 10f));
                Vector2 unit = rot.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                Rotation = MathHelper.Lerp(-MathHelper.Pi / 3, MathHelper.Pi / 6, SmoothThrow(Timer / 10f));
                RotationZ = MathHelper.Lerp(MathHelper.Pi / 2, MathHelper.Pi / 12, Timer / 10f);
                Light = MathHelper.Lerp(0.8f, 0, Timer / 10f);

                if (Timer < 10)
                {
                    Timer++;
                }
                else
                {
                    Projectile.Kill();
                }
            }

            TipPos = owner.Center + DrawUtils.GetTipPos(Rotation, RotationZ, RotationX, owner.direction < 0) * (WeaponLength + (IsStaff ? 8 : 16));
            Lighting.AddLight(TipPos, LightColor.R / 255f, LightColor.G / 255f, LightColor.B / 255f);

            SetCursorInfo(mouseText);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            miscTimer += 0.01f;
            Player owner = Main.player[Projectile.owner];
            Texture2D WeaponTex = Terraria.GameContent.TextureAssets.Item[ItemType].Value;
            Texture2D LightTex = TextureLibrary.BloomFlare;
            Texture2D TrailTex = TextureLibrary.SmallTrail;
            float WeaponLength = IsStaff ? WeaponTex.Size().Length() : WeaponTex.Width;

            if (ItemType == ItemID.CrystalSerpent)
            {
                DrawUtils.DrawSwingStaff(WeaponTex, owner.Center - Main.screenPosition, lightColor, WeaponLength, DrawUtils.ReverseX(Rotation), DrawUtils.ReverseY(RotationX), RotationZ, owner.direction >= 0, 10 * (float)Math.Sin(RotationZ));
            }
            else if (ItemType == ItemID.WeatherPain)
            {
                DrawUtils.DrawSwingVertical(WeaponTex, owner.Center - Main.screenPosition, lightColor, WeaponTex.Height, Rotation, RotationX, RotationZ, owner.direction < 0, 10 * (float)Math.Sin(RotationZ));
            }
            else if (IsStaff)
            {
                DrawUtils.DrawSwingStaff(WeaponTex, owner.Center - Main.screenPosition, lightColor, WeaponLength, Rotation, RotationX, RotationZ, owner.direction < 0, 10 * (float)Math.Sin(RotationZ));
            }
            else
            {
                DrawUtils.DrawSwing(WeaponTex, owner.Center - Main.screenPosition, lightColor, WeaponLength, Rotation, RotationX, RotationZ, owner.direction < 0, 10 * (float)Math.Sin(RotationZ));
            }
            float modifiedLight = Light;
            Color modifiedColor = LightColor;
            float modifiedScale = Scale;
            if (Light > 0)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                if (DrawLight(owner, TipPos, ref modifiedLight, ref modifiedColor, ref modifiedScale, miscTimer))
                {
                    Main.spriteBatch.Draw(LightTex, TipPos - Main.screenPosition, null, modifiedColor * modifiedLight, miscTimer, LightTex.Size() / 2f, 0.04f * modifiedScale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, TipPos - Main.screenPosition, null, Color.White * modifiedLight, miscTimer, LightTex.Size() / 2f, 0.02f * modifiedScale, SpriteEffects.None, 0);
                }
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            }
            return false;
        }

        private float SmoothSwing(float k)
        {
            return (float)Math.Pow(k, 0.3f);
        }

        private float SmoothThrow(float k)
        {
            return (float)Math.Pow(k, 4f);
        }

        public virtual bool DrawLight(Player owner, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ScrollWheelDelta">负数为向下滚，正数为向上滚</param>
        public virtual void OnScroll(Player owner, float ScrollWheelDelta)
        {

        }

        public virtual void ModifyMouseText(Player owner, ref string text)
        {

        }
    }


    public class GoWSwitchChannel : BaseSwitchStaffChannel
    {
        public static int Launch(Player owner, int itemType, string spell, Color lightColor, float lightScale)
        {
            int protmp = Projectile.NewProjectile(owner.GetSource_FromThis("BG3Magic"), owner.Center, Vector2.Zero, ModContent.ProjectileType<GoWSwitchChannel>(), 0, 0, owner.whoAmI);
            if (protmp >= 0 && protmp < 1000)
            {
                GoWSwitchChannel modproj = Main.projectile[protmp].ModProjectile as GoWSwitchChannel;
                modproj.ItemType = itemType;
                modproj.Spell = spell;
                modproj.LightColor = lightColor;
                modproj.Scale = lightScale;
            }
            return protmp;
        }

        public override void ModifyMouseText(Player owner, ref string text)
        {
            string skillText = EverythingLibrary.spells[Spell].GetName();
            SomeUtils.AddColorString(ref skillText, EverythingLibrary.spells[Spell].NameColor);
            DamageElement damageElement = GlyphOfWardingSpell.GetDamageType(owner.GetModPlayer<DNDMagicPlayer>().GoWType);
            string elementText = damageElement.GetLocalize();
            SomeUtils.AddColorString(ref elementText, damageElement.GetColor());
            text += skillText + ": " + elementText;
        }

        public override bool DrawLight(Player owner, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer)
        {
            int gowType = owner.GetModPlayer<DNDMagicPlayer>().GoWType;
            DamageElement eleType = GlyphOfWardingSpell.GetDamageType(gowType);
            color = LightColor = SomeUtils.GetColor(eleType);

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

        public override void OnScroll(Player owner, float ScrollWheelDelta)
        {
            DNDMagicPlayer modplayer = owner.GetModPlayer<DNDMagicPlayer>();
            modplayer.GoWType += Math.Sign(ScrollWheelDelta);
            if (modplayer.GoWType < 0) modplayer.GoWType = 4;
            if (modplayer.GoWType > 4) modplayer.GoWType = 0;
        }
    }
}
