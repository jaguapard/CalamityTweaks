using Terraria;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityTweaks.Enemies
{
    using Helpers;
    using System.Collections.Generic;
    using System;
    using Terraria.ID;
    using Terraria.ModLoader;

    [AutoloadBossHead]
    public class SupremeCnidrion : ModNPC
    {
        protected PatternManager pm_phase1 = new();
        protected int currBossPhase = -1;
        protected int prevBossPhase = -2;
        protected int ticksInCurrentPhase = 0;
        protected int ticksSinceSpawn = 0;
        protected List<int> spawns = new();

        protected Vector2 deathHailTargetPos;
        protected Vector2 currentChargeVelocity;
        protected Player targetPlayer;

        public static class Damage //Damage values are designed for Master Death mode originally (first number) and are scaled appropriately (second number, the multiplier) 
        {
            public static int NonPredictiveCharge = (int)(1250 * 0.4);
            public static int PredictiveCharge = (int)(950 * 0.4);
            public static int CloneCharge = (int)(850 * 0.4);
            public static int SupremeWaterBolt = (int)(860 * 0.2);
            public static int SupremeWaterBoltAscending = (int)(690 * 0.2);
            public static int WaterTide = (int)(1250 * 0.2);
            public static int SteamBreath = (int)(1250 * 0.2);
            public static int WaterDeathHail = (int)(840 * 0.2);
            public static int PredictiveWaterArrow = (int)(910 * 0.2);
        }

        public static class Numbers
        {
            public static float BossPhase2LifePct = 60;
            public static float BossPhase3LifePct = 30;
            public static int BossPhase4_InvisibilityDispelTicks = 300;

            public static class NonPredictiveCharge
            {
                public static int Ticks = 80;
                public static float MinDist = 600f;
                public static float PlayerOffset = 400f;
                public static float MaxDist = 1800f;
                public static float Predictiveness = 0.0f;
            }

            public static class SupremeWaterBoltSpray
            {
                public static int ProjectileCount = 50;
                public static float MaxSpreadRadians = 20 * MathF.PI / 180;
                public static int TicksPerBolt = 4;
                public static float ProjectileSpeed = 14f;
                public static int TicksTotal = ProjectileCount * TicksPerBolt; //don't touch it
            }

            public static class PredictiveCharge
            {
                public static int Ticks = 90;
                public static float MinDist = 400f;
                public static float PlayerOffset = 300f;
                public static float MaxDist = 1800f;
                public static float Predictiveness = 1.5f;
            }

            public static class WaterDeathHail
            {
                public static float SafeSpaceSize = 0.1f;
                public static float MaxFirstRoll = 0.7f;
                public static int ProjectilePairs = 40;
                public static int TicksPerPair = 3;
                public static float ProjectileSpeed = 4.5f;
                public static int PreIdleTicks = 40;
                public static int PostIdleTicks = 120;
                public static int TicksTotal = TicksPerPair * ProjectilePairs;
            }
            
            public static class WaterArrow
            {
                public static int Volleys = 5;
                public static int TicksPerVolley = 90;
                public static int ArrowPairsPerVolley = 2;
                public static float ArrowMaxSeparationRadians = 90 * MathF.PI / 180;
                public static float Predictiveness = 1f;
                public static float Speed = 10f;
                public static int PostIdleTicks = 40;

                public static Vector2 ArrowSpawnOffset = new(0, -300);
                public static int TicksDuration = Volleys * TicksPerVolley;                
            }
            public static class Clones
            {
                public static int Phase2Count = 3;
                public static int Phase3Count = 3;
                public static float OrbitRadius = 300f;
                public static int OrbitIntervalTicks = 400;
                public static float OrbitSeparationRadians = (360f / Phase2Count) / 180f * MathF.PI;
            }

            public static class Talk
            {
                public static byte R = 150;
                public static byte G = 200;
                public static byte B = 150;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Cnidrion");
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (currBossPhase == 3) return false;
            else return true;
        }

        public override void SetDefaults()
        {
            NPC.width = 365;
            NPC.height = 236;
            NPC.damage = Damage.NonPredictiveCharge;
            NPC.defense = 150;
            NPC.lifeMax = 5500000;
            NPC.knockBackResist = 0;
            NPC.value = Item.buyPrice(platinum: 30);
            NPC.aiStyle = -1;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;

            pm_phase1
                .AddAttack(Numbers.NonPredictiveCharge.Ticks, Attacks_NonPredictiveCharge)
                .AddAttack(Numbers.NonPredictiveCharge.Ticks, Attacks_NonPredictiveCharge)
                .AddAttack(Numbers.NonPredictiveCharge.Ticks, Attacks_NonPredictiveCharge)
                .AddAttack(Numbers.NonPredictiveCharge.Ticks, Attacks_NonPredictiveCharge)
                .AddAttack(Numbers.SupremeWaterBoltSpray.TicksTotal, Attacks_WaterBolt)
                .AddAttack(Numbers.PredictiveCharge.Ticks, Attacks_PredictiveCharge)
                .AddAttack(Numbers.PredictiveCharge.Ticks, Attacks_PredictiveCharge)
                .AddAttack(Numbers.WaterDeathHail.PreIdleTicks, Attacks_DoNothing)
                .AddAttack(Numbers.WaterDeathHail.TicksTotal, Attacks_WaterDeathHail)
                .AddAttack(Numbers.WaterDeathHail.PostIdleTicks, Attacks_DoNothing) //prevent cheap hits after deathhail
                .AddAttack(Numbers.WaterArrow.TicksDuration, Attacks_WaterArrow)
                .AddAttack(Numbers.WaterArrow.PostIdleTicks, Attacks_DoNothing);
        }

        public override void AI()
        {
            NPC.TargetClosestUpgraded(true);
            if (!NPC.HasValidTarget) NPC.velocity.Y += 1;
            this.targetPlayer = Main.player[NPC.target];
            ticksInCurrentPhase++;

            float lifePct = NPC.GetLifePercent() * 100;
            if (lifePct > Numbers.BossPhase2LifePct) SetTargetPhase(1);
            else if (lifePct > Numbers.BossPhase3LifePct) SetTargetPhase(2);
            else
            {
                if (currBossPhase == 3)
                {
                    if (!IsAnySpawnAlive()) SetTargetPhase(4);
                }
                else if (currBossPhase < 3) SetTargetPhase(3);
            }

            if (NPC.HasValidTarget && currBossPhase < 3)
            {
                pm_phase1.Advance(1);
                pm_phase1.Attack();
            }
            if (currBossPhase == 2) HandleSpawnsOrbiting();
            if (currBossPhase == 3 && NPC.HasValidTarget) NPC.position = targetPlayer.Center; //HACK: follow player to prevent despawning
            if (currBossPhase == 4)
            {
                if (ticksInCurrentPhase < Numbers.BossPhase4_InvisibilityDispelTicks)
                {
                    float visibilityProgress = (float)ticksInCurrentPhase / Numbers.BossPhase4_InvisibilityDispelTicks;
                    NPC.alpha = (int)(255f * (1 - visibilityProgress));
                }
                else if (ticksInCurrentPhase == Numbers.BossPhase4_InvisibilityDispelTicks)
                {
                    NPC.alpha = 0;
                    NPC.damage = Damage.NonPredictiveCharge;
                    NPC.immortal = false;
                }
                else
                {
                    pm_phase1.Advance(1);
                    pm_phase1.Attack();
                }
            }

            ticksSinceSpawn++;
        }

        private void HandleSpawnsOrbiting()
        {
            int orbitTick = ticksSinceSpawn % Numbers.Clones.OrbitIntervalTicks;
            for (int i = 0; i < spawns.Count; ++i)
            {
                if (Main.npc[spawns[i]].netID != ModContent.NPCType<SupremeCnidrionClone>()) continue;

                float orbitProgress = (float)orbitTick / Numbers.Clones.OrbitIntervalTicks;
                float currentAngle = 2 * MathF.PI * orbitProgress + i * Numbers.Clones.OrbitSeparationRadians;
                Main.npc[spawns[i]].position = this.NPC.position + new Vector2(Numbers.Clones.OrbitRadius * MathF.Sin(currentAngle), Numbers.Clones.OrbitRadius * MathF.Cos(currentAngle));
            }
        }

        public void Attacks_NonPredictiveCharge(int currentAttackTick)
        {
            ChargeAttack(Numbers.NonPredictiveCharge.Ticks, Numbers.NonPredictiveCharge.MinDist, Numbers.NonPredictiveCharge.PlayerOffset, Numbers.NonPredictiveCharge.MaxDist, Numbers.NonPredictiveCharge.Predictiveness, currentAttackTick);
        }

        public void Attacks_WaterBolt(int currentAttackTick)
        {
            WaterBoltAttack(Numbers.SupremeWaterBoltSpray.ProjectileCount, Numbers.SupremeWaterBoltSpray.MaxSpreadRadians, Numbers.SupremeWaterBoltSpray.TicksPerBolt, currentAttackTick);
        }

        public void Attacks_PredictiveCharge(int currentAttackTick)
        {
            ChargeAttack(Numbers.PredictiveCharge.Ticks, Numbers.PredictiveCharge.MinDist, Numbers.PredictiveCharge.PlayerOffset, Numbers.PredictiveCharge.MaxDist, Numbers.PredictiveCharge.Predictiveness, currentAttackTick);
        }

        public void Attacks_WaterDeathHail(int currentAttackTick)
        {
            WaterDeathHailAttack(Numbers.WaterDeathHail.SafeSpaceSize, Numbers.WaterDeathHail.MaxFirstRoll, Numbers.WaterDeathHail.TicksPerPair, Numbers.WaterDeathHail.ProjectilePairs, currentAttackTick);
        }

        public void Attacks_WaterArrow(int currentAttackTick)
        {
            WaterArrowAttack(currentAttackTick);
        }

        public void Attacks_DoNothing(int currentAttackTick) //intentionally blank, used in pattern manager to delay attacks
        {
        }

        public bool IsAnySpawnAlive()
        {
            foreach (var s in spawns)
            {
                if (Main.npc[s].netID == ModContent.NPCType<SupremeCnidrionClone>() && Main.npc[s].life > 0) return true;
            }
            return false;
        }

        protected void SetTargetPhase(int phase)
        {
            prevBossPhase = currBossPhase;
            currBossPhase = phase;

            if (prevBossPhase != currBossPhase) //on phase transition
            {
                ticksInCurrentPhase = 0;

                if (phase == 1) Talk("Long have I waited for this. I've heard rumors about you. Nobody took me seriously, they kept laughing at me, saying I'm a weakling mini-boss! Now, I will prove everyone wrong by defeating you!");

                if (phase == 2)
                {
                    Talk("Your performance is surprising. Given how much I worked on myself, you are a powerful opponent. I can respect that, but I have a few tricks too.");
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < Numbers.Clones.Phase2Count; ++i)
                        {
                            spawns.Add(NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + i * 100, (int)NPC.position.Y, ModContent.NPCType<SupremeCnidrionClone>(), 1, ai0: i, ai1: 0));
                        }
                    }
                }

                if (phase == 3)
                {
                    Talk("Alright, I'll leave fighting to the little ones for now");
                    NPC.immortal = true;
                    NPC.alpha = 255; //set to transparent
                    NPC.damage = 0;
                    for (int i = 0; i < Numbers.Clones.Phase3Count; ++i)
                    {
                        spawns.Add(NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + i * 200, (int)NPC.position.Y, ModContent.NPCType<SupremeCnidrionClone>(), 1, ai0: 3 + i, ai1: 1));
                    }

                    foreach (var s in spawns)
                    {
                        if (Main.npc[s].netID == ModContent.NPCType<SupremeCnidrionClone>()) Main.npc[s].ai[1] = 1f;
                    }
                }

                if (phase == 4)
                {
                    Talk("Time to get serious!");
                }
            }
        }
        protected void ChargeAttack(int targetTickDuration, float minDist, float playerOffset, float maxDist, float predictiveness, int currentAttackTick)
        {
            int idleTicks = targetTickDuration / 3;

            if (currentAttackTick == idleTicks)
            {
                int chargeTickDuration = targetTickDuration / 3;
                Vector2 targetPos = targetPlayer.Center + targetPlayer.velocity * (chargeTickDuration * 0.5f) * predictiveness;
                Vector2 dir = targetPos - NPC.Center;
                Vector2 unitDir = dir.SafeNormalize(Vector2.Zero);

                float dirLen = dir.Length();
                Vector2 chargeTargetPoint = NPC.Center + unitDir * MathF.Max(MathF.Min(dirLen + playerOffset, maxDist), minDist);

                this.currentChargeVelocity = (chargeTargetPoint - NPC.Center) / chargeTickDuration;
            }

            if (currentAttackTick < idleTicks || currentAttackTick > targetTickDuration - idleTicks) NPC.velocity = Vector2.Zero;
            else
            {
                NPC.velocity = this.currentChargeVelocity;
            }
        }

        protected void WaterBoltAttack(int boltCount, float maxSpreadRadians, int ticksPerBolt, int currentAttackTick) //TODO: fix this attack's weirdness
        {
            NPC.velocity = Vector2.Zero;

            int fullAttackDuration = ticksPerBolt * (boltCount - 1);
            if (currentAttackTick > fullAttackDuration) return;

            if (currentAttackTick % ticksPerBolt == 0)
            {
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var source = NPC.GetSource_FromAI();
                    Vector2 position = NPC.Center;
                    Vector2 targetPosition = targetPlayer.Center;
                    Vector2 direction = targetPosition - position;

                    float radius = direction.Length();
                    float targetAngle = position.AngleTo(targetPosition);
                    float currentSpreadPhase = (float)(currentAttackTick) / fullAttackDuration; //from 0 to 1
                    float spreadPhaseAdjusted = currentSpreadPhase - 0.5f; //from -0.5 to 0.5
                    float alterationModifier = ((currentAttackTick / ticksPerBolt) % 2) * 2 - 1; //-1 and 1. Makes bolts converge onto the player
                    float projectileAngle = targetAngle + maxSpreadRadians * spreadPhaseAdjusted * alterationModifier;

                    direction = new Vector2(radius * MathF.Cos(projectileAngle), radius * MathF.Sin(projectileAngle));
                    direction.Normalize();
                    int type = ProjectileID.PinkLaser; //TODO: change it to something watery
                    int damage = Damage.SupremeWaterBolt;
                    Projectile.NewProjectile(source, position, direction * Numbers.SupremeWaterBoltSpray.ProjectileSpeed, type, damage, 0f, Main.myPlayer);
                }
            }
        }

        protected void WaterDeathHailAttack(float safeSpaceSize, float maxFirstRoll, int ticksPerBolt, int boltPairs, int currentAttackTick)
        {
            int fullAttackDuration = boltPairs * ticksPerBolt;
            NPC.velocity = Vector2.Zero;

            int attackCycleTick = currentAttackTick;
            if (attackCycleTick == 1)
            {
                Random r = new();
                this.NPC.ai[0] = (float)r.NextDouble() * maxFirstRoll;
            }
            if (attackCycleTick % ticksPerBolt != 0) return;

            int currentProjectileNumber = attackCycleTick / ticksPerBolt;
            float leftProjectileSpacing = 1920 * NPC.ai[0] / boltPairs;
            float rightProjectileSpacing = 1920 * (1 - NPC.ai[0] - safeSpaceSize) / boltPairs;

            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = NPC.GetSource_FromAI();
                Vector2 position = targetPlayer.Center - new Vector2(960f, 540f);
                Vector2 direction = new(0f, 1f);

                direction.Normalize();
                float speed = Numbers.WaterDeathHail.ProjectileSpeed;
                int type = ProjectileID.PinkLaser; //TODO: change it to something watery
                int damage = Damage.WaterDeathHail;
                Vector2 adjDir = direction * speed;

                Vector2 leftOffset = new(leftProjectileSpacing * currentProjectileNumber, 0f);
                Vector2 rightOffset = new(1920f - rightProjectileSpacing * currentProjectileNumber, 0f);
                Projectile.NewProjectile(source, position + leftOffset, adjDir, type, damage, 0f, Main.myPlayer);
                Projectile.NewProjectile(source, position + rightOffset, adjDir, type, damage, 0f, Main.myPlayer);
            }
        }

        public void WaterArrowAttack(int currentAttackTick)
        {
            if (currentAttackTick % Numbers.WaterArrow.TicksPerVolley != 0) return;
            if (!(NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)) return;

            float speed = Numbers.WaterArrow.Speed;
            var source = NPC.GetSource_FromAI();

            Vector2 polarOffset = Helpers.Funcs.toPolar(Numbers.WaterArrow.ArrowSpawnOffset);
            float pathLength = polarOffset.X;
            float ticksToReach = pathLength / speed;
            Vector2 predictionOffsetPerPair = (targetPlayer.velocity * ticksToReach * Numbers.WaterArrow.Predictiveness) / Numbers.WaterArrow.ArrowPairsPerVolley;
            Vector2 arrowSpawnPos = targetPlayer.Center + Numbers.WaterArrow.ArrowSpawnOffset;

            int type = ProjectileID.PinkLaser; //TODO: change it to something watery
            int damage = Damage.PredictiveWaterArrow;

            for (int i = 0; i < Numbers.WaterArrow.ArrowPairsPerVolley; ++i)
            {
                Vector2 currOffset = predictionOffsetPerPair * i;
                currOffset.Normalize();
                Projectile.NewProjectile(source, arrowSpawnPos, currOffset*speed, type, damage, 0f, Main.myPlayer);
            }
        }

        protected void Talk(string message) //TODO: add localization stuff?
        {
            if (Main.netMode != NetmodeID.Server)
            {
                //string text = Language.GetTextValue("Mods.ExampleMod.NPCTalk", Lang.GetNPCNameValue(npc.type), message);
                string text = message;
                Main.NewText(text, Numbers.Talk.R, Numbers.Talk.G, Numbers.Talk.B);
            }
            else
            {
                //Terraria.Localization.NetworkText text = Terraria.Localization.NetworkText.FromKey("Mods.ExampleMod.NPCTalk", Lang.GetNPCNameValue(npc.type), message);
                Terraria.Localization.NetworkText text = Terraria.Localization.NetworkText.FromLiteral(message);
                Terraria.Chat.ChatHelper.BroadcastChatMessage(text, new Color(Numbers.Talk.R, Numbers.Talk.G, Numbers.Talk.B));
            }
        }
    }
}