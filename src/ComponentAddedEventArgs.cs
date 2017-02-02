using System;

namespace TS.ECS
{
	/// <summary>
	/// Component added event arguments.
	/// </summary>
	public class ComponentAddedEventArgs: EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TS.ECS.ComponentAddedEventArgs"/> class.
		/// </summary>
		/// <param name="entity">The entity to which the component was added.</param>
		/// <param name="component">The new component.</param>
		internal ComponentAddedEventArgs(Entity entity, IComponent component)
		{
			Entity = entity;
			Component = component;
		}

		/// <summary>
		/// The entity to which the component was added
		/// </summary>
		public Entity Entity { get; }

		/// <summary>
		/// The new component
		/// </summary>
		public IComponent Component { get; }
	}
}
