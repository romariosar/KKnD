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

namespace OpenRA.Mods.Kknd.Traits.AttackNotifications
{
    [Desc("Specifies the notification for the AdvancedAttackNotifier.")]
    public class AttackNotificationInfo : ITraitInfo
    {
        [Desc("The audio notification type to play.")]
        [FieldLoader.RequireAttribute] public string[] Notifications = { };

        public bool RadarPings = true;

        public object Create(ActorInitializer init) { return new AttackNotification(); }
    }

    public class AttackNotification { }
}
