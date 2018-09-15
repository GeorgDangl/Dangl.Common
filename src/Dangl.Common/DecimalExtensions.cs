using System;

namespace Dangl
{
    /// <summary>
    /// Extensions for <see cref="decimal"/>
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// This will keep the sign (positive or negative) and return the
        /// original value if it is less than or equal the maxAbsoluteValue
        /// or the maxAbsoluteValue.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValueAbsolute">Must be bigger than zero.</param>
        /// <returns></returns>
        public static decimal WithMaxAbsoluteValue(this decimal value, decimal maxValueAbsolute)
        {
            if (maxValueAbsolute <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValueAbsolute), "The maximum absolute value must be given as a positive decimal");
            }

            if (Math.Abs(value) <= maxValueAbsolute)
            {
                return value;
            }
            else if (value < 0m)
            {
                return -maxValueAbsolute;
            }

            return maxValueAbsolute;
        }
    }
}
