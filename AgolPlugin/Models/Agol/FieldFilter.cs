using AgolPlugin.Models.Common;
using System;

namespace AgolPlugin.Models.Agol
{
    public class FieldFilter : ModelBase
    {
        private string _fieldName;
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; OnPropertyChanged(); }
        }

        private string _fieldType;
        public string FieldType
        {
            get { return _fieldType; }
            set { _fieldType = value; OnPropertyChanged(); }
        }

        private FilterOperator _operator;
        public FilterOperator Operator
        {
            get { return _operator; }
            set { _operator = value; OnPropertyChanged(); }
        }

        private object _value1;
        public object Value1
        {
            get { return _value1; }
            set { _value1 = value; OnPropertyChanged(); }
        }

        private object _value2;
        public object Value2
        {
            get { return _value2; }
            set { _value2 = value; OnPropertyChanged(); }
        }

        public override string ToString()
        {
            if (Operator.IsRange)
                return $"{FieldName} {Operator} {GetValue1()} AND {GetValue2()}";
            else
                return $"{FieldName} {Operator} {GetValue1()}";
        }

        private string GetValue1()
        {
            switch (FieldType)
            {
                case "string":
                    return Operator == FilterOperators.Like ? $"'%{Value1}%'" : $"'{Value1}'";
                case "int":
                case "dec":
                    return Value1.ToString();
                case "date":
                    return $"TIMESTAMP '{((Value1 as DateTime?) ?? DateTime.MinValue).ToUniversalTime():yyyy-MM-dd HH:mm:ss}'";
                case "guid":
                    return $"'{Value1}'";
                default:
                    return string.Empty;
            }
        }

        private string GetValue2()
        {
            switch (FieldType)
            {
                case "string":
                case "int":
                case "dec":
                    return Value1.ToString();
                case "date":
                    return $"TIMESTAMP '{((Value2 as DateTime?) ?? DateTime.MaxValue).ToUniversalTime():yyyy-MM-dd HH:mm:ss}'";
                case "guid":
                    return $"'{Value1}'";
                default:
                    return string.Empty;
            }
        }

        public bool GetIsValid()
        {
            return !string.IsNullOrEmpty(FieldName)
                && FieldType != null
                && Operator.IsRange ? (Value1 != null && Value2 != null) : Value1 != null;
        }
    }
}