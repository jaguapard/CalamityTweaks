using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CalamityTweaks.Helpers
{
    public delegate void PatternAttackDelegate(int currentAttackTick);
    public class PatternAttack
    {
        public PatternAttack(int durationTicks, PatternAttackDelegate del, int startTick)
        {
            this.durationTicks = durationTicks;
            this.attack = del;
            this.startTick = startTick;
        }

        public int durationTicks;
        public int startTick;
        public PatternAttackDelegate attack;
        public int EndTick 
        { 
            get { return startTick + durationTicks; } 
        }
    }

    public class PatternManager
    {
        private List<PatternAttack> attacks = new();
        private int currentAttackTick = 0;
        private int currentPatternTick = 0;
        private int patternDurationTicks = 0;
        private PatternAttack currentAttack = null;
        private int currentEndTick = 0;
        private int currentAttackIndex = 0;

        public PatternManager AddAttack(int durationTicks, PatternAttackDelegate del)
        {
            attacks.Add(new(durationTicks, del, currentEndTick));
            currentEndTick += durationTicks;
            patternDurationTicks += durationTicks;
            return this;
        }

        public bool IsEmpty()
        {
            return attacks.Count == 0;
        }

        public void Advance(int ticks = 1)
        {
            currentPatternTick += ticks;
            currentAttackTick += ticks;
            int maxSteps = attacks.Count;
            int i = 0;
            int oldAttackIndex = currentAttackIndex;

            while (!Funcs.InRange(currentPatternTick, attacks[currentAttackIndex].startTick, attacks[currentAttackIndex].EndTick) && i < maxSteps)
            {
                currentAttackIndex++;
                currentAttackIndex %= attacks.Count;
            }

            if (i < maxSteps) //if matching attack is found
            {
                currentAttack = attacks[currentAttackIndex];
            }
            else currentAttack = null;

            if (oldAttackIndex != currentAttackIndex) //attack is switched
            {
                currentAttackTick = currentPatternTick - currentAttack.startTick;
            }
            currentPatternTick %= patternDurationTicks;
        }

        /*public void Reset()
        {

        }*/

        public void Attack()
        {
            if (this.currentAttack != null) currentAttack.attack(currentAttackTick);
        }

        public void RecalculateDuration()
        {
            int totalDuration = 0;
            foreach (var attack in attacks)
            {
                totalDuration += attack.durationTicks;
            }
            patternDurationTicks = totalDuration;
        }
    }
}
