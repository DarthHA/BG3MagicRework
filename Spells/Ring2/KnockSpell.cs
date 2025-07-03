using BG3MagicRework.BaseType;
using BG3MagicRework.Projectiles.Ring2;
using BG3MagicRework.Static;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BG3MagicRework.Spells.Ring2
{
    public class KnockSpell : BaseSpell
    {
        public override string Name => "Knock";
        public override Color NameColor => Color.LightBlue;
        public override int InitialRing => 2;
        public override SchoolOfMagic Shool => SchoolOfMagic.Transmutation;
        public override int SpellRange => 12;
        public override bool Concentration => false;
        public override int TimeSpan => 0;

        public override void Shoot(Player player, ModProjectile modproj, Vector2 tipPosition, Vector2 mousePosition, int Ring)
        {
            (bool careful, bool distant, bool _, bool _, bool _) = player.ActivateMetaMagic(true, true, false, false, false);
            int range = SpellRange * (distant ? 2 : 1) * 16;
            int npcIndex = -1;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == NPCID.BoundTownSlimeOld)
                {
                    if (npc.Distance(player.Center) < range && (careful || Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1)))
                    {
                        npcIndex = npc.whoAmI;
                    }
                }
            }
            if (npcIndex != -1)            //优先开史莱姆箱
            {
                int protmp = player.NewMagicProj(Main.npc[npcIndex].Center, Vector2.Zero, ModContent.ProjectileType<KnockProj>(), Ring);
                if (protmp > 0 && protmp < 1000)
                {
                    (Main.projectile[protmp].ModProjectile as KnockProj).TargetIndex = npcIndex;
                    (Main.projectile[protmp].ModProjectile as KnockProj).IsNPC = true;
                }
            }
            else
            {
                int targetChest = -1;
                float minDistance = 9999;
                for (int i = 0; i < Main.chest.Length; i++)
                {
                    if (Main.chest[i] != null)
                    {
                        Tile ChestTopLeft = Main.tile[Main.chest[i].x, Main.chest[i].y];
                        if (CanUnlockBiomeChest(ChestTopLeft, Ring))
                        {
                            Vector2 WorldPos = new Vector2(Main.chest[i].x + 1, Main.chest[i].y + 1) * 16;
                            if (player.Distance(WorldPos) <= range && (careful || Collision.CanHit(WorldPos, 1, 1, player.Center, 1, 1)))
                            {
                                if (Chest.IsLocked(Main.chest[i].x, Main.chest[i].y))
                                {
                                    if (targetChest == -1 || Main.MouseWorld.Distance(WorldPos) < minDistance)
                                    {
                                        targetChest = i;
                                        minDistance = Main.MouseWorld.Distance(WorldPos);
                                    }
                                }
                            }
                        }
                    }
                }

                if (targetChest != -1)
                {
                    int protmp = player.NewMagicProj(new Vector2(Main.chest[targetChest].x, Main.chest[targetChest].y) * 16, Vector2.Zero, ModContent.ProjectileType<KnockProj>(), Ring);
                    if (protmp > 0 && protmp < 1000)
                    {
                        (Main.projectile[protmp].ModProjectile as KnockProj).TargetIndex = targetChest;
                        (Main.projectile[protmp].ModProjectile as KnockProj).IsNPC = false;
                    }
                }
            }

        }

        public override bool CanRelease(Player owner, ModProjectile modproj, Vector2 mousePosition, int Ring, ref string Warning)
        {
            bool success = false;
            int range = owner.GetSpellRange(Name) * 16;
            int npcIndex = -1;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == NPCID.BoundTownSlimeOld)
                {
                    if (npc.Distance(owner.Center) < range && (owner.CarefulSpellMM() || Collision.CanHit(npc.Center, 1, 1, owner.Center, 1, 1)))
                    {
                        npcIndex = npc.whoAmI;
                    }
                }
            }
            if (npcIndex != -1)            //优先开史莱姆箱
            {
                success = true;
            }
            else
            {
                for (int i = 0; i < Main.chest.Length; i++)
                {
                    if (Main.chest[i] != null)
                    {
                        Tile ChestTopLeft = Main.tile[Main.chest[i].x, Main.chest[i].y];
                        if (CanUnlockBiomeChest(ChestTopLeft, Ring))
                        {
                            Vector2 WorldPos = new Vector2(Main.chest[i].x + 1, Main.chest[i].y + 1) * 16;
                            if (owner.Distance(WorldPos) <= range && (owner.CarefulSpellMM() || Collision.CanHit(WorldPos, 1, 1, owner.Center, 1, 1)))
                            {
                                if (Chest.IsLocked(Main.chest[i].x, Main.chest[i].y))
                                {
                                    success = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (!success)
            {
                Warning += LangLibrary.NoTarget + "\n";
            }

            return success;
        }

        public override bool DrawLight(Player owner, int ring, Vector2 tipPos, ref float light, ref Color color, ref float scale, float miscTimer, bool HasShot)
        {
            return true;
        }

        private bool CanUnlockBiomeChest(Tile tile, int ring)
        {
            bool biomechest = tile.TileType == 21 && tile.TileFrameX >= 828 && tile.TileFrameX <= 990 && tile.TileFrameY >= 0 && tile.TileFrameY <= 18;
            biomechest = biomechest || (tile.TileType == 467 && tile.TileFrameX >= 468 && tile.TileFrameX <= 486 && tile.TileFrameY >= 0 && tile.TileFrameY <= 18);
            //沙漠箱子
            return ring >= 4 || !biomechest;
        }
    }
}
