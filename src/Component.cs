namespace TS.ECS
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// 
        /// </summary>
        public Entity Parent { get; internal set; }
    }
}
