namespace AgolPlugin.Models.Agol
{
    public struct FilterOperator
    {
        public FilterOperator(string description, string operatorText, bool isRange)
        {
            Description = description;
            OperatorText = operatorText;
            IsRange = isRange;
        }

        public string Description { get; private set; }
        public string OperatorText { get; private set; }
        public bool IsRange { get; private set; }

        public override string ToString()
        {
            return OperatorText;
        }

        public static bool operator ==(FilterOperator op1, FilterOperator op2)
        {
            return op1.OperatorText == op2.OperatorText;
        }
        public static bool operator !=(FilterOperator op1, FilterOperator op2)
        {
            return op1.OperatorText != op2.OperatorText;
        }
    }

    public static class FilterOperators
    {
        private static FilterOperator? _lessThanEqualTo;
        public static FilterOperator LessThanEqualTo
        {
            get
            {
                if (_lessThanEqualTo == null)
                    _lessThanEqualTo = new FilterOperator("<=", "<=", false);
                return (FilterOperator)_lessThanEqualTo;
            }
        }

        private static FilterOperator? _greaterThanEqualTo;
        public static FilterOperator GreaterThanEqualTo
        {
            get
            {
                if (_greaterThanEqualTo == null)
                    _greaterThanEqualTo = new FilterOperator(">=", ">=", false);
                return (FilterOperator)_greaterThanEqualTo;
            }
        }

        private static FilterOperator? _lessThan;
        public static FilterOperator LessThan
        {
            get
            {
                if (_lessThan == null)
                    _lessThan = new FilterOperator("<", "<", false);
                return (FilterOperator)_lessThan;
            }
        }

        private static FilterOperator? _greaterThan;
        public static FilterOperator GreaterThan
        {
            get
            {
                if (_greaterThan == null)
                    _greaterThan = new FilterOperator(">", ">", false);
                return (FilterOperator)_greaterThan;
            }
        }

        private static FilterOperator? _equalTo;
        public static FilterOperator EqualTo
        {
            get
            {
                if (_equalTo == null)
                    _equalTo = new FilterOperator("=", "=", false);
                return (FilterOperator)_equalTo;
            }
        }

        private static FilterOperator? _notEqualTo;
        public static FilterOperator NotEqualTo
        {
            get
            {
                if (_notEqualTo == null)
                    _notEqualTo = new FilterOperator("!=", "<>", false);
                return (FilterOperator)_notEqualTo;
            }
        }

        private static FilterOperator? _between;
        public static FilterOperator Between
        {
            get
            {
                if (_between == null)
                    _between = new FilterOperator("Between", "between", true);
                return (FilterOperator)_between;
            }
        }

        private static FilterOperator? _like;
        public static FilterOperator Like
        {
            get
            {
                if (_like == null)
                    _like = new FilterOperator("Like", "like", false);
                return (FilterOperator)_like;
            }
        }
    }
}