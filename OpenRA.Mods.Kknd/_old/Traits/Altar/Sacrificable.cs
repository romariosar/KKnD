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

using System.Collections.Generic;
using System.Drawing;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Kknd.Orders;
using OpenRA.Traits;

namespace OpenRA.Mods.Kknd.Traits.Altar
{
	[Desc("Actor can be sacrificed.")]
	class SacrificableInfo : ITraitInfo
	{
		[Desc("Cursor used for order.")]
		public readonly string Cursor = "enter";

		[Desc("Voice used when ordering to enter enemy building.")]
		[VoiceReference] public readonly string Voice = "Sacrifice";

		public object Create(ActorInitializer init) { return new Sacrificable(init, this); }
	}

	class Sacrificable : IIssueOrder, IResolveOrder, IOrderVoice
	{
		private readonly SacrificableInfo info;

		public Sacrificable(ActorInitializer init, SacrificableInfo info)
		{
			this.info = info;
		}

		public IEnumerable<IOrderTargeter> Orders
		{
			get { yield return new SacrificeOrderTargeter(info.Cursor); }
		}

		public Order IssueOrder(Actor self, IOrderTargeter order, Target target, bool queued)
		{
			return order.OrderID == SacrificeOrderTargeter.Id ? new Order(order.OrderID, self, target, queued) : null;
		}

		public string VoicePhraseForOrder(Actor self, Order order)
		{
			if (order.OrderString != SacrificeOrderTargeter.Id)
				return null;

			return info.Voice;
		}

		public void ResolveOrder(Actor self, Order order)
		{
			if (order.OrderString != SacrificeOrderTargeter.Id)
				return;

			if (order.Target.Type != TargetType.Actor || order.Target.Actor == null)
				return;

			self.CancelActivity();
			self.SetTargetLine(order.Target, Color.Yellow);
			//self.QueueActivity(new Sacrifice(self, order.Target));
		}
	}
}
