using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Channel
{
    public abstract class BaseStaffChannel : BaseChannel              //戏法按照1.5秒算
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


        public List<Vector2> TrailPos = new();

        /// <summary>
        /// 3阶段弹幕发射时机
        /// </summary>
        public int P3ShootTimer = 15;

        /// <summary>
        /// 4阶段（后摇）持续时间
        /// </summary>
        public int P4TimeLeft = 15;


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
            if (Phase <= 3)
            {
                if (currentRing > 0)
                {
                    mouseText += string.Format(LangLibrary.XRingSpell, SomeUtils.RomanNumber(currentRing)) + "\n";
                }
                else
                {
                    mouseText += LangLibrary.Cantrips + "\n";
                }
            }

            if (Phase == 0) //20帧抬手
            {
                CanRelease(owner, ref mouseText);

                if (Timer == 1)
                {
                    DrawBehindRing.SummonThis(Projectile.whoAmI);
                    DrawFrontRing.SummonThis(Projectile.whoAmI);
                }
                float rot = MathHelper.Lerp(MathHelper.Pi / 3, -MathHelper.Pi / 4 * 3, SmoothSwing(Timer / 20f));
                Vector2 unit = rot.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                Rotation = MathHelper.Lerp(MathHelper.Pi / 6, -MathHelper.Pi / 4 * 3, SmoothSwing(Timer / 20f));
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
            else if (Phase == 1)  //持续到单击待机，期间可以改环
            {
                Rotation = -MathHelper.Pi / 4 * 3 + (float)Math.Sin(Timer / 60f * MathHelper.TwoPi) * MathHelper.Pi / 12f;
                RotationZ = MathHelper.Pi / 2f + ((float)Math.Cos(Timer / 60f * MathHelper.TwoPi) - 1) * MathHelper.Pi / 12f;
                Light = 0.85f + (float)Math.Cos(Timer / 60f * MathHelper.TwoPi) * 0.15f;

                Vector2 unit = Rotation.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                //改环
                if (!InstantAndFree)
                {
                    ScrollChangeRing(owner);
                    if (!CheckAnyRingAvailable(owner))
                    {
                        Timer = 0;
                        Phase = 5;
                        OldRotation = Rotation;
                        OldRotationZ = RotationZ;
                        OldLight = Light;
                        return;
                    }
                }

                bool canRelease = CanRelease(owner, ref mouseText);

                if (InstantAndFree)
                {
                    Timer = 0;
                    Phase = 2;
                    TargetRot = (Main.MouseWorld - owner.Center).ToRotation();
                    OldRotation = Rotation;
                    OldRotationZ = RotationZ;
                    OldLight = Light;
                    owner.direction = Math.Sign(Main.MouseWorld.X - owner.Center.X);
                }
                else if (owner.RightClick())       //你撤回了一条法术
                {
                    Timer = 0;
                    Phase = 5;
                    OldRotation = Rotation;
                    OldRotationZ = RotationZ;
                    OldLight = Light;
                }
                else if ((owner.LeftClick() || (currentRing == 0 && owner.LeftPress())) && canRelease)
                {
                    Timer = 0;
                    Phase = 2;
                    TargetRot = (Main.MouseWorld - owner.Center).ToRotation();
                    OldRotation = Rotation;
                    OldRotationZ = RotationZ;
                    OldLight = Light;
                    owner.direction = Math.Sign(Main.MouseWorld.X - owner.Center.X);
                }
                else
                {
                    Timer++;
                }
            }
            else if (Phase == 2)       //持续10帧复位
            {
                CanRelease(owner, ref mouseText);

                Rotation = MathHelper.Lerp(OldRotation, -MathHelper.Pi / 5 * 4, Timer / 10f);
                RotationZ = MathHelper.Lerp(OldRotationZ, MathHelper.Pi / 6f, Timer / 10f);
                Light = MathHelper.Lerp(OldLight, 0.8f, Timer / 10f);

                Vector2 unit = Rotation.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);
                if (Timer < 10)
                {
                    Timer++;
                }
                else
                {
                    Timer = 0;
                    Phase = 3;
                }
            }
            else if (Phase == 3) //持续15帧挥出去
            {
                CanRelease(owner, ref mouseText);

                float realTargetRot = owner.direction < 0 ? DrawUtils.ReverseX(TargetRot) : TargetRot;

                Rotation = MathHelper.Lerp(-MathHelper.Pi / 5 * 4, 0, SmoothThrow(Timer / 15f));
                RotationZ = MathHelper.Lerp(MathHelper.Pi / 4, MathHelper.Pi / 6, SmoothThrow(Timer / 15f));
                RotationX = MathHelper.Lerp(0, realTargetRot, SmoothThrow(Timer / 15f));
                Light = MathHelper.Lerp(0.8f, 1f, Timer / 15f);

                float rot = Rotation;
                if (owner.direction < 0) rot = DrawUtils.ReverseX(rot);
                Vector2 unit = rot.ToRotationVector2();
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                if (Timer == P3ShootTimer)  //第P3ShootTimer帧发射
                {
                    string tmp = "";
                    if (CanRelease(owner, ref tmp) && (InstantAndFree || owner.GetAvailableRings(currentRing).Contains(currentRing)))
                    {
                        TipPos = owner.Center + DrawUtils.GetTipPos(Rotation, RotationZ, RotationX, owner.direction < 0) * (WeaponLength + (IsStaff ? 8 : 16));
                        ReleaseMagic(TipPos);
                    }

                }
                if (Timer < 15)
                {
                    Timer++;
                }
                else
                {
                    Timer = 0;
                    Phase = 4;
                }
            }
            else if (Phase == 4)  //持续15帧后摇
            {
                Vector2 TargetVec = TargetRot.ToRotationVector2();
                float realTargetRot = TargetVec.ToRotation();
                float rot = realTargetRot;
                Vector2 unit = rot.ToRotationVector2();

                float realTargetRot2 = owner.direction < 0 ? DrawUtils.ReverseX(TargetRot) : TargetRot;
                Rotation = 0;
                RotationZ = MathHelper.Pi / 6f;
                RotationX = realTargetRot2;
                if (P4TimeLeft >= 15)
                {
                    Light = MathHelper.Lerp(1f, 0, MathHelper.Clamp((Timer - (P4TimeLeft - 15)) / 15f, 0f, 1f));
                }
                else
                {
                    Light = MathHelper.Lerp(1f, 0, Timer / (float)P4TimeLeft);
                }
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);
                if (Timer < P4TimeLeft)
                {
                    Timer++;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            else if (Phase == 5)       //持续10帧撤回复位
            {
                Rotation = MathHelper.Lerp(OldRotation, -MathHelper.Pi / 4 * 3, Timer / 5f);
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
                    Phase = 6;
                }
            }
            else if (Phase == 6) //10帧放手
            {
                float rot = MathHelper.Lerp(-MathHelper.Pi / 4 * 3, MathHelper.Pi / 3, SmoothThrow(Timer / 10f));
                Vector2 unit = rot.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                Rotation = MathHelper.Lerp(-MathHelper.Pi / 4 * 3, MathHelper.Pi / 6, SmoothThrow(Timer / 10f));
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

            if (Phase == 3 || Phase == 4)
            {
                TrailPos.Add(TipPos - owner.Center);
                if (TrailPos.Count > 5) TrailPos.RemoveAt(0);
            }

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
                bool hasShot = (Phase == 3 && Timer >= P3ShootTimer) || Phase == 4;
                if (EverythingLibrary.spells[Spell].DrawLight(owner, currentRing, TipPos, ref modifiedLight, ref modifiedColor, ref modifiedScale, miscTimer, hasShot))
                {
                    if (TrailPos.Count > 0)
                    {
                        List<CustomVertexInfo> bars = new();
                        List<Vector2> modifiedTrail = DrawUtils.SmoothTrajectory(TrailPos, 30, 0.5f);
                        for (int i = 0; i < modifiedTrail.Count; i++)
                        {
                            float width = MathHelper.Lerp(4, 0, 1 - (float)i / modifiedTrail.Count);
                            bars.Add(new CustomVertexInfo(owner.Center + modifiedTrail[i] - Vector2.Normalize(modifiedTrail[i]) * width - Main.screenPosition, modifiedColor * modifiedLight, new Vector3(1f - (float)i / modifiedTrail.Count, 0f, 1)));
                            bars.Add(new CustomVertexInfo(owner.Center + modifiedTrail[i] + Vector2.Normalize(modifiedTrail[i]) * width - Main.screenPosition, modifiedColor * modifiedLight, new Vector3(1f - (float)i / modifiedTrail.Count, 1f, 1)));
                        }
                        bars.Add(new CustomVertexInfo(TipPos - Vector2.Normalize(TipPos) * 4 - Main.screenPosition, modifiedColor * modifiedLight, new Vector3(0f, 0f, 1)));
                        bars.Add(new CustomVertexInfo(TipPos + Vector2.Normalize(TipPos) * 4 - Main.screenPosition, modifiedColor * modifiedLight, new Vector3(0f, 1f, 1)));
                        DrawUtils.DrawTrail(TrailTex, bars, modifiedColor * modifiedLight, BlendState.Additive);

                        EasyDraw.AnotherDraw(BlendState.Additive);
                    }
                    Main.spriteBatch.Draw(LightTex, TipPos - Main.screenPosition, null, modifiedColor * modifiedLight, miscTimer, LightTex.Size() / 2f, 0.04f * modifiedScale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, TipPos - Main.screenPosition, null, Color.White * modifiedLight, miscTimer, LightTex.Size() / 2f, 0.02f * modifiedScale, SpriteEffects.None, 0);
                }

            }
            if (Phase <= 1)
            {
                if (ModContent.GetInstance<BG3Config>().ShowRangeIndicator)
                {
                    //float offset = DrawUtils.GetTipPos(0, MathHelper.Pi / 6, 0).Length() * (WeaponLength + (IsStaff ? 8 : 16));
                    DrawSpellRangeInfo(owner);
                }
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
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
    }


    public class NormalStaffChannel : BaseStaffChannel
    {
        public static int Launch(Player owner, int itemType, string spell, int initialRing, Color lightColor, float lightScale, int P4TimeLeft = 15)
        {
            int protmp = Projectile.NewProjectile(owner.GetSource_FromThis("BG3Magic"), owner.Center, Vector2.Zero, ModContent.ProjectileType<NormalStaffChannel>(), 0, 0, owner.whoAmI);
            if (protmp >= 0 && protmp < 1000)
            {
                NormalStaffChannel modproj = Main.projectile[protmp].ModProjectile as NormalStaffChannel;
                modproj.ItemType = itemType;
                modproj.Spell = spell;
                modproj.currentRing = initialRing;
                modproj.LightColor = lightColor;
                modproj.Scale = lightScale;
                modproj.P4TimeLeft = P4TimeLeft;
            }
            return protmp;
        }
    }
}
