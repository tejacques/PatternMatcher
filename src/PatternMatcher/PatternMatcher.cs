using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functional.PatternMatching
{
    public static class PatternMatcher
    {
        public static PatternMatcher<object> Match()
        {
            return new PatternMatcher<object>();
        }

        public static PatternMatcher<object, TOut> MatchWithResult<TOut>()
        {
            return new PatternMatcher<object, TOut>();
        }

        public static PatternMatcher<T> Match<T>()
        {
            return new PatternMatcher<T>();
        }

        public static PatternMatcher<TIn, TOut> Match<TIn, TOut>()
        {
            return new PatternMatcher<TIn, TOut>();
        }

    }
    public class PatternMatcher<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;
        private Dictionary<Type, Action<T>> _matchedTypes;
        private Dictionary<T, Action> _matchedValues;

        public PatternMatcher()
        {
            this._hasValue = false;
            this._value = default(T);

            this._matchedTypes = new Dictionary<Type, Action<T>>();
            this._matchedValues = new Dictionary<T, Action>();
        }

        public PatternMatcher(T value) : this()
        {
            this._hasValue = true;
            this._value = value;
        }

        #region Fluent API

        public PatternMatcher<T> With<TPattern>(Action<T> action)
        {
            if (this._matchedTypes.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "There is already a pattern for type: {0}.",
                        typeof(T).Name));

            }

            this._matchedTypes.Add(typeof(T), action);

            return this;
        }

        public PatternMatcher<T> With<TPattern>(T value, Action action)
        {
            if (this._matchedValues.ContainsKey(value))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "There is already a pattern for type: {0} value: {1}.",
                        typeof(T).Name,
                        value.ToString()));
            }

            this._matchedValues.Add(value, action);

            return this;
        }

        #endregion

        /// <summary>
        /// Runs the action whose pattern matches the value.
        /// </summary>
        public void Return()
        {
            if (this._hasValue)
            {
                Return(this._value);
            }
        }

        /// <summary>
        /// Runs the action whose pattern matches the supplied option.
        /// </summary>
        /// <param name="value">The value to match on.</param>
        public void Return(T value)
        {
            if (this._matchedValues.ContainsKey(value))
            {
                this._matchedValues[value]();
            }
            else if (this._matchedTypes.ContainsKey(typeof(T)))
            {
                this._matchedTypes[typeof(T)](value);
            }
        }
    }

    public class PatternMatcher<TIn, TOut>
    {
        private TIn _value;
        private bool _hasValue;
        private Dictionary<Type, Func<TIn, TOut>> _matchedTypes;
        private Dictionary<TIn, Func<TOut>> _matchedValues;

        public PatternMatcher()
        {
            this._hasValue = false;
            this._value = default(TIn);

            this._matchedTypes = new Dictionary<Type, Func<TIn, TOut>>();
            this._matchedValues = new Dictionary<TIn, Func<TOut>>();
        }

        public PatternMatcher(TIn value) : this()
        {
            this._hasValue = true;
            this._value = value;
        }

        #region Fluent API

        public PatternMatcher<TIn, TOut> With<TPattern>(Func<TIn, TOut> func)
        {
            if (this._matchedTypes.ContainsKey(typeof(TIn)))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "There is already a pattern for type: {0}.",
                        typeof(TIn).Name));

            }

            this._matchedTypes.Add(typeof(TIn), func);

            return this;
        }

        public PatternMatcher<TIn, TOut> With<TPattern>(
            TIn value,
            Func<TOut> func)
        {
            if (this._matchedValues.ContainsKey(value))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "There is already a pattern for type: {0} value: {1}.",
                        typeof(TIn).Name,
                        value.ToString()));
            }

            this._matchedValues.Add(value, func);

            return this;
        }

        #endregion

        /// <summary>
        /// Runs the func whose pattern matches the value.
        /// </summary>
        public TOut Return()
        {
            TOut result = default(TOut);

            if (this._hasValue)
            {
                result = Return(this._value);
            }

            return result;
        }

        /// <summary>
        /// Runs the func whose pattern matches the supplied option.
        /// </summary>
        /// <param name="value">The value to match on.</param>
        public TOut Return(TIn value)
        {
            TOut result = default(TOut);

            if (this._matchedValues.ContainsKey(value))
            {
                result = this._matchedValues[value]();
            }
            else if (this._matchedTypes.ContainsKey(typeof(TIn)))
            {
                this._matchedTypes[typeof(TIn)](value);
            }

            return result;
        }
    }

    public class MatchFailureException : Exception
    {
        public MatchFailureException(string message) : base(message) { }
    }
}
