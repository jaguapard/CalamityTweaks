using Terraria;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System.Collections.Generic;
using CalamityTweaks.Helpers;

namespace CalamityTweaks.Enemies
{
    [AutoloadBossHead]
    public class SupremeCnidrionClone : SupremeCnidrion
    {
        protected PatternManager pm_phase2 = new();

        public static class CloneNumbers
        {
            public static class Charge
            {
                public static int Ticks = 80;
                public static float MinDist = 200f;
                public static float PlayerOffset = 250f;
                public static float MaxDist = 1000f;
                public static float Predictiveness = 0.0f;
            }

            public static class WaterBoltSequence
            {
                public static int ProjectileCount = 3;
                public static int TicksPerBolt = 10;
                public static int TicksDuration = 120;
            }

            public static class WaterBoltShotgun
            {
                public static int PreIdleTicks = 39;
                public static int PostIdleTicks = 80;
                public static int ProjectileCount = 5;
                public static float MaxSpreadDegrees = 60;
            }

            public static class WaterBoltWall
            {
                public static int PreIdleTicks = 79;
                public static int PostIdleTicks = 40;
                public static int ProjectileCount = 5;
                public static int MaxPixelSpread = 60;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Cnidrion Spawn");
        }
        public override void SetDefaults()
        {
            NPC.width = 365 / 3;
            NPC.height = 236 / 3;
            NPC.damage = 0;
            NPC.defense = 110;
            NPC.lifeMax = 1500000;
            NPC.knockBackResist = 0;
            NPC.value = Item.buyPrice(platinum: 1, gold: 50);
            NPC.aiStyle = -1;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            NPC.TargetClosestUpgraded(true);
            if (!NPC.HasValidTarget) this.NPC.velocity.Y += 1;
            this.targetPlayer = Main.player[NPC.target];

            ticksInCurrentPhase++;
            bool isFreeMoving = NPC.ai[1] > 0;

            if (pm_phase1.IsEmpty())
            {
                int attackType = (int)NPC.ai[0] % 3;
                Talk(attackType.ToString());
                if (attackType == 0)
                {
                    pm_phase1.AddAttack(CloneNumbers.WaterBoltSequence.TicksDuration, Attacks_WaterBoltSequence);
                }
                if (attackType == 1)
                {
                    pm_phase1.AddAttack(CloneNumbers.WaterBoltShotgun.PreIdleTicks, Attacks_DoNothing);
                    pm_phase1.AddAttack(1, Attacks_WaterBoltShotgun);
                    pm_phase1.AddAttack(CloneNumbers.WaterBoltShotgun.PostIdleTicks, Attacks_DoNothing);
                }
                if (attackType == 2)
                {
                    pm_phase1.AddAttack(CloneNumbers.WaterBoltWall.PreIdleTicks, Attacks_DoNothing);
                    pm_phase1.AddAttack(1, Attacks_WaterBoltWall);
                    pm_phase1.AddAttack(CloneNumbers.WaterBoltWall.PostIdleTicks, Attacks_DoNothing);
                }

                pm_phase2 = pm_phase1;
                pm_phase2.AddAttack(CloneNumbers.Charge.Ticks, Attacks_SpawnCharge);
            }

            if (isFreeMoving)
            {
                NPC.damage = Damage.CloneCharge;
                pm_phase2.Advance(1);
                pm_phase2.Attack();
            }
            else
            {
                NPC.damage = 0;
                pm_phase1.Advance(1);
                pm_phase1.Attack();
            }

            ticksSinceSpawn++;
        }

        public void Attacks_WaterBoltSequence(int currentAttackTick)
        {
            waterBoltSequence(CloneNumbers.WaterBoltSequence.ProjectileCount, CloneNumbers.WaterBoltSequence.TicksPerBolt, currentAttackTick);
        }

        public void Attacks_WaterBoltShotgun(int currentAttackTick)
        {
            waterBoltShotgun(CloneNumbers.WaterBoltShotgun.ProjectileCount, CloneNumbers.WaterBoltShotgun.MaxSpreadDegrees, currentAttackTick);
        }

        public void Attacks_WaterBoltWall(int currentAttackTick)
        {
            waterBoltWall(CloneNumbers.WaterBoltWall.ProjectileCount, CloneNumbers.WaterBoltWall.MaxPixelSpread, currentAttackTick);
        }

        public void Attacks_SpawnCharge(int currentAttackTick)
        {
            ChargeAttack(CloneNumbers.Charge.Ticks, CloneNumbers.Charge.MinDist, CloneNumbers.Charge.PlayerOffset, CloneNumbers.Charge.MaxDist, 0, currentAttackTick);
        }

        protected void waterBoltSequence(int projectileCount, int ticksPerBolt, int currentAttackTick)
        {
            if (currentAttackTick % ticksPerBolt != 0 || currentAttackTick > ticksPerBolt * projectileCount) return;
            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = NPC.GetSource_FromAI();
                Vector2 position = NPC.Center;
                Vector2 targetPosition = targetPlayer.Center;
                Vector2 direction = targetPosition - position;

                direction.Normalize();
                float speed = Numbers.SupremeWaterBoltSpray.ProjectileSpeed;
                int type = ProjectileID.PinkLaser; //TODO: change it to something watery
                int damage = Damage.SupremeWaterBolt;
                Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
            }
        }

        protected void waterBoltShotgun(int projectileCount, float maxSpreadDegrees, int currentAttackTick)
        {
            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                float maxSpreadRadians = maxSpreadDegrees * MathF.PI / 180;
                var source = NPC.GetSource_FromAI();
                Vector2 position = NPC.Center;
                Vector2 targetPosition = targetPlayer.Center;
                float targetAngle = position.AngleTo(targetPosition);

                for (int i = 0; i < projectileCount; ++i)
                {
                    float angleScale = (float)i / projectileCount - 0.5f;
                    float projectileAngle = targetAngle + maxSpreadRadians * angleScale;
                    Vector2 direction = new(MathF.Cos(projectileAngle), MathF.Sin(projectileAngle));
                    direction.Normalize();
                    float speed = Numbers.SupremeWaterBoltSpray.ProjectileSpeed;
                    int type = ProjectileID.PinkLaser; //TODO: change it to something watery
                    int damage = Damage.SupremeWaterBolt;
                    Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
                }
            }
        }

        protected void waterBoltWall(int projectileCount, int maxPixelSeparation, int currentAttackTick)
        {
            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = NPC.GetSource_FromAI();
                Vector2 position = NPC.Center;
                Vector2 targetPosition = targetPlayer.Center;
                Vector2 direction = targetPosition - position;
                Vector2 perpendicular = new(direction.Y, -direction.X);
                perpendicular.Normalize();

                for (int i = 0; i < projectileCount; ++i)
                {
                    Vector2 direction2 = direction + perpendicular * ((float)i / projectileCount - 0.5f) * maxPixelSeparation;
                    direction2.Normalize();
                    float speed = Numbers.SupremeWaterBoltSpray.ProjectileSpeed;
                    int type = ProjectileID.PinkLaser; //TODO: change it to something watery
                    int damage = Damage.SupremeWaterBolt;
                    Projectile.NewProjectile(source, position, direction2 * speed, type, damage, 0f, Main.myPlayer);
                }
            }
        }
    }
}