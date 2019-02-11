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

using OpenRA.Mods.Common.Traits;

namespace OpenRA.Mods.Kknd.Traits.Production
{
	// TODO merge to FactoryPrerequisite, make Prerequisite standard -> Make mobile base require repairbay.
	[Desc("Adds support for required tech level to Buildable.")]
	public class AdvancedBuildableInfo : BuildableInfo
	{
		[Desc("The factory level required to build this actor.")]
		public readonly int Level = 0;
	}

	public class AdvancedBuildable : Buildable { }
}
