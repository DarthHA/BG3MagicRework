using BG3MagicRework.Projectiles.VirtualEffects;
using BG3MagicRework.Static;
using BG3MagicRework.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BG3MagicRework.Projectiles.Channel
{
    public abstract class BaseRifieChannel : BaseChannel
    {

        //未处理的目标位置
        public float TargetRot = 0;

        public float Rotation = 0;
        public float RotationZ = 0;
        public float RotationX = 0;

        public Vector2 TipPos = Vector2.Zero;


        /// <summary>
        /// 4阶段弹幕发射时机
        /// </summary>
        public int P3ShootTimer = 10;

        /// <summary>
        /// 4阶段（后摇）持续时间
        /// </summary>
        public int P3TimeLeft = 30;

        /// <summary>
        /// 手持位置修正
        /// </summary>
        public Vector2 HoldOffset = Vector2.Zero;

        /// <summary>
        /// 枪火在Y角度的偏移
        /// </summary>
        public float GunShotOffset = 0;

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
            float WeaponLength = WeaponTex.Width;

            string mouseText = "";
            if (Phase <= 1)
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
                float rot = MathHelper.Lerp(MathHelper.Pi / 3, -MathHelper.Pi / 2, SmoothSwing(Timer / 20f));
                Vector2 unit = rot.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                Rotation = MathHelper.Lerp(MathHelper.Pi / 6, -MathHelper.Pi / 2, SmoothSwing(Timer / 20f));
                RotationZ = MathHelper.Lerp(MathHelper.Pi / 12, MathHelper.Pi / 2, Timer / 20f);
                Light = MathHelper.Lerp(0, 1, MathHelper.Clamp((Timer - 10) / 10f, 0, 1));

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
                Rotation = -MathHelper.Pi / 2f;
                RotationZ = MathHelper.Pi / 2f;
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
                        Phase = 4;
                        return;
                    }
                }

                bool canRelease = CanRelease(owner, ref mouseText);

                if (InstantAndFree)
                {
                    Timer = 0;
                    Phase = 2;
                }
                else if (owner.RightClick())       //你撤回了一条法术
                {
                    Timer = 0;
                    Phase = 4;
                }
                else if ((owner.LeftClick() || (currentRing == 0 && owner.LeftPress())) && canRelease)
                {
                    Timer = 0;
                    Phase = 2;
                }
                else
                {
                    Timer++;
                }
            }
            else if (Phase == 2) //持续5帧挥出去
            {
                TargetRot = (Main.MouseWorld - owner.Center).ToRotation();
                owner.direction = Math.Sign(Main.MouseWorld.X - owner.Center.X);

                float realTargetRot = owner.direction < 0 ? DrawUtils.ReverseX(TargetRot) : TargetRot;

                Rotation = MathHelper.Lerp(-MathHelper.Pi / 2, 0, Timer / 5f);
                RotationZ = MathHelper.Pi / 2f;
                RotationX = MathHelper.Lerp(0, realTargetRot, Timer / 5f);
                Light = 0.8f;

                float rot = Rotation;
                if (owner.direction < 0) rot = DrawUtils.ReverseX(rot);
                Vector2 unit = rot.ToRotationVector2();
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                if (Timer < 5)
                {
                    Timer++;
                }
                else
                {
                    Timer = 0;
                    Phase = 3;
                }
            }
            else if (Phase == 3)  //持续15帧后摇
            {
                TargetRot = (Main.MouseWorld - owner.Center).ToRotation();
                owner.direction = Math.Sign(Main.MouseWorld.X - owner.Center.X);

                Vector2 TargetVec = TargetRot.ToRotationVector2();
                float realTargetRot = TargetVec.ToRotation();
                float rot = realTargetRot;

                float realTargetRot2 = owner.direction < 0 ? DrawUtils.ReverseX(TargetRot) : TargetRot;
                Rotation = 0;
                RotationZ = MathHelper.Pi / 2f;
                RotationX = realTargetRot2;
                if (Timer < P3ShootTimer)
                {
                    Light = MathHelper.Lerp(0.8f, 1f, Timer / (float)P3ShootTimer);
                }
                else
                {
                    Light = MathHelper.Lerp(1f, 0, (Timer - P3ShootTimer) / (float)(P3TimeLeft - P3ShootTimer));
                }
                if (Timer >= P3ShootTimer)
                {
                    float time1 = (P3TimeLeft - P3ShootTimer) * 0.25f;
                    float time2 = (P3TimeLeft - P3ShootTimer) * 0.75f;
                    if (Timer - P3ShootTimer <= time1)
                    {
                        Rotation = MathHelper.Lerp(0, 1, (Timer - P3ShootTimer) / time1) * -0.6f;
                        rot += MathHelper.Lerp(1, 0, (Timer - P3ShootTimer - time1) / time2) * -0.6f * owner.direction;
                    }
                    else
                    {
                        Rotation = MathHelper.Lerp(1, 0, (Timer - P3ShootTimer - time1) / time2) * -0.6f;
                        rot += MathHelper.Lerp(1, 0, (Timer - P3ShootTimer - time1) / time2) * -0.6f * owner.direction;
                    }
                }

                Vector2 unit = rot.ToRotationVector2();
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                if (Timer == P3ShootTimer)  //第P3ShootTimer帧发射
                {
                    string tmp = "";
                    if (CanRelease(owner, ref tmp) && (InstantAndFree || owner.GetAvailableRings(currentRing).Contains(currentRing)))
                    {
                        Vector2 offSet11 = new Vector2(HoldOffset.X * owner.direction, HoldOffset.Y * owner.gravDir).RotatedBy(owner.direction < 0 ? DrawUtils.ReverseY(Rotation) : Rotation);
                        Vector2 offset12 = new Vector2(0, GunShotOffset).RotatedBy(owner.direction < 0 ? DrawUtils.ReverseY(Rotation) : Rotation);
                        TipPos = owner.Center + DrawUtils.GetTipPos(Rotation, RotationZ, RotationX, owner.direction < 0) * (WeaponLength + 8) + offSet11 + offset12;
                        ReleaseMagic(TipPos);
                    }

                }

                if (Timer < P3TimeLeft)
                {
                    Timer++;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            else if (Phase == 4) //10帧放手
            {
                float rot = MathHelper.Lerp(-MathHelper.Pi / 2, MathHelper.Pi / 3, SmoothThrow(Timer / 10f));
                Vector2 unit = rot.ToRotationVector2();
                unit.X = unit.X * owner.direction;
                owner.itemRotation = (float)Math.Atan2(unit.Y * owner.direction, unit.X * owner.direction);

                Rotation = MathHelper.Lerp(-MathHelper.Pi / 2f, MathHelper.Pi / 6, SmoothThrow(Timer / 10f));
                RotationZ = MathHelper.Lerp(MathHelper.Pi / 2, MathHelper.Pi / 12, Timer / 10f);
                Light = MathHelper.Lerp(0.8f, 0, MathHelper.Clamp(Timer / 10f, 0f, 1f));

                if (Timer < 10)
                {
                    Timer++;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Vector2 offSet21 = new Vector2(HoldOffset.X * owner.direction, HoldOffset.Y * owner.gravDir).RotatedBy(owner.direction < 0 ? DrawUtils.ReverseY(Rotation) : Rotation);
            Vector2 offset22 = new Vector2(0, GunShotOffset).RotatedBy(owner.direction < 0 ? DrawUtils.ReverseY(Rotation) : Rotation);
            TipPos = owner.Center + DrawUtils.GetTipPos(Rotation, RotationZ, RotationX, owner.direction < 0) * (WeaponLength + 8) + offSet21 + offset22;
            Lighting.AddLight(TipPos, LightColor.R / 255f, LightColor.G / 255f, LightColor.B / 255f);

            SetCursorInfo(mouseText);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            miscTimer += 0.01f;
            Player owner = Main.player[Projectile.owner];
            Texture2D WeaponTex = Terraria.GameContent.TextureAssets.Item[ItemType].Value;
            Texture2D LightTex = TextureLibrary.BloomFlare;

            Vector2 offSet = new Vector2(HoldOffset.X * owner.direction, HoldOffset.Y * owner.gravDir).RotatedBy(owner.direction < 0 ? DrawUtils.ReverseY(Rotation) : Rotation);
            DrawUtils.DrawSwing(WeaponTex, owner.Center + offSet - Main.screenPosition, lightColor, WeaponTex.Width, Rotation, RotationX, RotationZ, owner.direction < 0, 10);

            float modifiedLight = Light;
            Color modifiedColor = LightColor;
            float modifiedScale = Scale;
            if (Light > 0)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                bool hasShot = Phase == 3 && Timer >= P3ShootTimer;
                if (EverythingLibrary.spells[Spell].DrawLight(owner, currentRing, TipPos, ref modifiedLight, ref modifiedColor, ref modifiedScale, miscTimer, hasShot))
                {
                    Main.spriteBatch.Draw(LightTex, TipPos - Main.screenPosition, null, modifiedColor * modifiedLight, miscTimer, LightTex.Size() / 2f, 0.04f * modifiedScale, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(LightTex, TipPos - Main.screenPosition, null, Color.White * modifiedLight, miscTimer, LightTex.Size() / 2f, 0.02f * modifiedScale, SpriteEffects.None, 0);
                }
            }
            if (Phase <= 1)
            {
                if (ModContent.GetInstance<BG3Config>().ShowRangeIndicator)
                {
                    //float offset = WeaponTex.Width + 8 + HoldOffset.X;
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

    public class NormalRifieChannel : BaseRifieChannel
    {
        public static int Launch(Player owner, int itemType, string spell, int initialRing, Color lightColor, float lightScale, Vector2 holdOffset, int P3TimeLeft = 30, int P3ShootTimer = 15, float gunshotOffset = 0)
        {
            int protmp = Projectile.NewProjectile(owner.GetSource_FromThis("BG3Magic"), owner.Center, Vector2.Zero, ModContent.ProjectileType<NormalRifieChannel>(), 0, 0, owner.whoAmI);
            if (protmp >= 0 && protmp < 1000)
            {
                NormalRifieChannel modproj = Main.projectile[protmp].ModProjectile as NormalRifieChannel;
                modproj.ItemType = itemType;
                modproj.Spell = spell;
                modproj.currentRing = initialRing;
                modproj.LightColor = lightColor;
                modproj.Scale = lightScale;
                modproj.P3TimeLeft = P3TimeLeft;
                modproj.P3ShootTimer = P3ShootTimer;
                modproj.HoldOffset = holdOffset;
                modproj.GunShotOffset = gunshotOffset;
            }
            return protmp;
        }
    }
}
