#region Copyright & License Information
/*
 * Copyright 2016-2019 The KKnD Developers (see AUTHORS)
 * This file is part of KKnD, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using OpenRA.Traits;

namespace OpenRA.Mods.Kknd.Traits.Veterancy
{
    [Desc("This actor has a different voice for each veterancy level.")]
    public class VeterancyVoicedInfo : ITraitInfo
    {
        [FieldLoader.Require]
        [Desc("Which voice sets to use.")]
        [VoiceSetReference]
        public readonly string[] VoiceSets = { };

        [Desc("Multiply volume with this factor.")]
        public readonly float Volume = 1f;

        public object Create(ActorInitializer init) { return new VeterancyVoiced(init.Self, this); }
    }

    public class VeterancyVoiced : IVoiced, INotifyCreated
    {
        private readonly VeterancyVoicedInfo info;
        private Veterancy veterancy;

        public VeterancyVoiced(Actor self, VeterancyVoicedInfo info)
        {
            this.info = info;
        }

        void INotifyCreated.Created(Actor self)
        {
            veterancy = self.TraitOrDefault<Veterancy>();
        }

        string IVoiced.VoiceSet { get { return info.VoiceSets[veterancy != null ? veterancy.Level : 0]; } }

        bool IVoiced.PlayVoice(Actor self, string phrase, string variant)
        {
            if (phrase == null)
                return false;

            var type = ((IVoiced)this).VoiceSet.ToLowerInvariant();
            var volume = info.Volume;
            return Game.Sound.PlayPredefined(SoundType.World, self.World.Map.Rules, null, self, type, phrase, variant, true, WPos.Zero, volume, true);
        }

        bool IVoiced.PlayVoiceLocal(Actor self, string phrase, string variant, float volume)
        {
            if (phrase == null)
                return false;

            var type = ((IVoiced)this).VoiceSet.ToLowerInvariant();
            return Game.Sound.PlayPredefined(SoundType.World, self.World.Map.Rules, null, self, type, phrase, variant, false, self.CenterPosition, volume, true);
        }

        bool IVoiced.HasVoice(Actor self, string voice)
        {
            var voices = self.World.Map.Rules.Voices[((IVoiced)this).VoiceSet.ToLowerInvariant()];
            return voices != null && voices.Voices.ContainsKey(voice);
        }
    }
}
