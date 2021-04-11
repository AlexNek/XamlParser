using System.Runtime.Serialization;

namespace Parser
{
    /// <summary>
    /// Class Tools.
    /// </summary>
    internal static class Tools
    {
        /// <summary>
        /// The object identifier generator
        /// </summary>
        private static readonly ObjectIDGenerator _objectIdGenerator = new ObjectIDGenerator();

        /// <summary>
        /// Gets the object identifier.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Int64.</returns>
        public static long GetObjId(object obj)
        {
            return _objectIdGenerator.GetId(obj, out bool _);
        }
    }
}
