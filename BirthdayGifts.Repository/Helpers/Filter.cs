using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayGifts.Repository.Helpers
{
    public class Filter
    {
        private readonly string _field;
        private readonly object _value;
        private readonly string _operator;

        public Filter(string field, object value, string op = "=")
        {
            _field = field;
            _value = value;
            _operator = op;
        }

        private Filter() { }

        public string ToSql()
        {
            return $"[{_field}] {_operator} @{_field}";
        }

        public string ParameterName => $"@{_field}";
        public object Value => _value;

        public static Filter Empty => new Filter();
        public Dictionary<string, object> Conditions { get; set; } = new Dictionary<string, object>();

        public Filter AddCondition(string field, object value)
        {
            Conditions[field] = value;
            return this;
        }
    }
}
