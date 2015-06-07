namespace Merchello.Core.Marketing.Offer
{
    /// <summary>
    /// Helper class.
    /// </summary>
    internal class StringOperatorHelper
    {
        /// <summary>
        /// The text for operator string.
        /// </summary>
        /// <param name="op">
        /// The op.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string TextForOperatorString(string op)
        {
            string operatorText;

            switch (op)
            {
                case "<":
                    operatorText = "less than";
                    break;
                case "lt":
                    operatorText = "less than";
                    break;
                case "<=":
                    operatorText = "less than or equal to";
                    break;
                case "lte":
                    operatorText = "less than or equal to";
                    break;
                case "=":
                    operatorText = "equals";
                    break;
                case "eq":
                    operatorText = "equals";
                    break;
                case ">":
                    operatorText = "greater than";
                    break;
                case "gt":
                    operatorText = "greater than";
                    break;
                case ">=":
                    operatorText = "greater than or equal to";
                    break;
                case "gte":
                    operatorText = "greater than or equal to";
                    break;
                default:
                    operatorText = string.Empty;
                    break;
            }

            return operatorText;
        }

        /// <summary>
        /// Constructs a conditional expression based on the string operator
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="op">
        /// The op.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Evaluate(decimal left, decimal right, string op)
        {
            bool result;

            switch (op)
            {
                case "<":
                    result = left < right;
                    break;
                case "lt":
                    result = left < right;
                    break;
                case "<=":
                    result = left <= right;
                    break;
                case "lte":
                    result = left <= right;
                    break;
                case "=":
                    result = left == right;
                    break;
                case "eq":
                    result = left == right;
                    break;
                case ">":
                    result = left > right;
                    break;
                case "gt":
                    result = left > right;
                    break;
                case ">=":
                    result = left >= right;
                    break;
                case "gte":
                    result = left >= right;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }
    }
}