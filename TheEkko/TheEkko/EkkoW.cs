﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheBrand.ComboSystem;

namespace TheEkko
{
    class EkkoW : Skill
    {
        public EkkoW(Spell spell)
            : base(spell)
        {
            spell.SetSkillshot(0.4f + 3, 350, 0f, false, SkillshotType.SkillshotCircle);
        }

        public override void Cast(Obj_AI_Hero target, bool force = false, HitChance minChance = HitChance.Low)
        {
            if (HasBeenSafeCast() || target == null || target.GetWaypoints().Last().Distance(target) > 50) return;
            var prediction = Spell.GetPrediction(target, true);
            if (prediction.Hitchance < minChance) return;
            SafeCast(() => Spell.Cast(prediction.CastPosition));

        }

        public override int GetPriority()
        {
            return 3;
        }
    }
}
