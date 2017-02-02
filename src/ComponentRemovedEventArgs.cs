using System;

namespace TS.ECS
{
	/// <summary>
	/// Component removed event arguments.
	/// </summary>
	public class ComponentRemovedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TS.ECS.ComponentRemovedEventArgs"/> class.
		/// </summary>
		/// <param name="entity">The entity from which the component was removed.</param>
		/// <param name="component">The removed component.</param>
		internal ComponentRemovedEventArgs(Entity entity, IComponent component)
		{
			Entity = entity;
			Component = component;
		}

		/// <summary>
		/// The entity from which the component was removed
		/// </summary>
		public Entity Entity { get; }

		/// <summary>
		/// The removed component
		/// </summary>
		public IComponent Component { get; }
	}
}
