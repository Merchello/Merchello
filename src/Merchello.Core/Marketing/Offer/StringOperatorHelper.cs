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


    }
}