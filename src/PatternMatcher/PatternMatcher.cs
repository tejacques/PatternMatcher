using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Functional.PatternMatching
{
    public class _
    {
        public static _ Instance = new _();
        private _() { }
    }

    public static class PatternMatcherExtensions
    {
        public static PatternMatcher<T> Match<T>(this T value)
        {
            return new PatternMatcher<T>(value);
        }

        public static PatternMatcher<TIn, TOut> Match<TIn, TOut>(
            this TIn value)
        {
            return new PatternMatcher<TIn, TOut>(value);
        }

        public static PatternMatcher<object, TOut> MatchWithResult<TOut>(
            this object value)
        {
            return new PatternMatcher<object, TOut>(value);
        }
    }

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
        private Dictionary<Type, Dictionary<T, Action>> _matchedValueTypes;
        private Action _matchedWildcard;

        public PatternMatcher()
        {
            this._hasValue = false;
            this._value = default(T);

            this._matchedTypes = null;
            this._matchedValueTypes = null;
            this._matchedWildcard = null;
        }

        public PatternMatcher(T value) : this()
        {
            this._hasValue = true;
            this._value = value;
        }

        #region Fluent API

        public PatternMatcher<T> With<TPattern>(Action<TPattern> action)
            where TPattern : T
        {
            Type type = typeof(TPattern);

            if (null == this._matchedTypes)
            {
                this._matchedTypes = new Dictionary<Type, Action<T>>();
            }

            if (this._matchedTypes.ContainsKey(type))
            {
                throw new MatchFailureException(
                    string.Format(
                        "There is already a pattern for type: {0}.",
                        type.Name));
            }

            this._matchedTypes.Add(
                type,
                (x) => action((TPattern)x));

            return this;
        }

        public PatternMatcher<T> With<TPattern>(Action action)
            where TPattern : _
        {
            return Else(action);
        }

        public PatternMatcher<T> With<TPattern>(TPattern value, Action action)
            where TPattern : T
        {
            Type type = typeof(TPattern);

            if (null == this._matchedValueTypes)
            {
                this._matchedValueTypes =
                    new Dictionary<Type, Dictionary<T, Action>>();
            }

            if (!this._matchedValueTypes.ContainsKey(type))
            {
                this._matchedValueTypes.Add(
                    type,
                    new Dictionary<T, Action>());
            }

            var matchedValues = this._matchedValueTypes[type];

            if (matchedValues.ContainsKey(value))
            {
                throw new MatchFailureException(
                    string.Format(
                        "There is already a pattern for type: {0} value: {1}.",
                        type.Name,
                        value.ToString()));
            }

            matchedValues.Add(value, action);

            return this;
        }

        public PatternMatcher<T> Else(Action action)
        {
            if (null != this._matchedWildcard)
            {
                throw new MatchFailureException(
                    "There is already a wildcard pattern.");
            }

            this._matchedWildcard = action;

            return this;
        }

        #endregion

        /// <summary>
        /// Runs the action whose pattern matches the value.
        /// </summary>
        public void Return()
        {
            Return(false);
        }

        /// <summary>
        /// Runs the action whose pattern matches the value.
        /// </summary>
        public void Return(bool allowNoMatch)
        {
            if (this._hasValue)
            {
                Return(this._value);
            }
            else if (null != this._matchedWildcard)
            {
                this._matchedWildcard();
            }
            else if (!allowNoMatch)
            {
                throw new MatchFailureException(
                    "The PatternMatcher has no value to match on.");
            }
        }

        /// <summary>
        /// Runs the action whose pattern matches the supplied option.
        /// </summary>
        /// <param name="value">The value to match on.</param>
        /// <param name="allowNoMatch">
        /// Whether or not to allow no match.
        /// </param>
        public void Return(T value)
        {
            Return(value, false);
        }

        /// <summary>
        /// Runs the action whose pattern matches the supplied option.
        /// </summary>
        /// <param name="value">The value to match on.</param>
        /// <param name="allowNoMatch">
        /// Whether or not to allow no match.
        /// </param>
        public void Return(T value, bool allowNoMatch)
        {
            Type type = value.GetType();

            Dictionary<T, Action> matchedValues;
            Action matchedAction;
            Action<T> matchedActionT;

            if (null != this._matchedValueTypes
                && this._matchedValueTypes
                    .TryGetValue(type, out matchedValues)
                && matchedValues.TryGetValue(value, out matchedAction))
            {
                matchedAction();
            }
            else if (null != this._matchedTypes
                && this._matchedTypes.TryGetValue(type, out matchedActionT))
            {
                matchedActionT(value);
            }
            else if (null != this._matchedWildcard)
            {
                this._matchedWildcard();
            }
            else if (!allowNoMatch)
            {
                throw new MatchFailureException(
                    "The PatternMatcher has no value to match on.");
            }
        }
    }

    public class PatternMatcher<TIn, TOut>
    {
        private TIn _value;
        private bool _hasValue;
        private Dictionary<Type, Func<TIn, TOut>> _matchedTypes;
        private Dictionary<Type, Dictionary<TIn, Func<TOut>>>
            _matchedValueTypes;
        private Func<TOut> _matchedWildcard;

        public PatternMatcher()
        {
            this._hasValue = false;
            this._value = default(TIn);

            this._matchedTypes = null;
            this._matchedValueTypes = null;
            this._matchedWildcard = null;
        }

        public PatternMatcher(TIn value) : this()
        {
            this._hasValue = true;
            this._value = value;
        }

        #region Fluent API

        public PatternMatcher<TIn, TOut> With<TPattern>(Func<TPattern, TOut> func)
            where TPattern : TIn
        {
            Type type = typeof(TPattern);

            if (null == this._matchedTypes)
            {
                this._matchedTypes = new Dictionary<Type, Func<TIn, TOut>>();
            }

            if (this._matchedTypes.ContainsKey(type))
            {
                throw new MatchFailureException(
                    string.Format(
                        "There is already a pattern for type: {0}.",
                        type.Name));
            }

            this._matchedTypes.Add(
                type,
                (x) => func((TPattern)x));

            return this;
        }

        public PatternMatcher<TIn, TOut> With<TPattern>(Func<TOut> func)
            where TPattern : _
        {
            return Else(func);
        }

        public PatternMatcher<TIn, TOut> With<TPattern>(
            TIn value,
            Func<TOut> func)
            where TPattern : TIn
        {
            Type type = typeof(TPattern);

            if (null == this._matchedValueTypes)
            {
                this._matchedValueTypes =
                    new Dictionary<Type, Dictionary<TIn, Func<TOut>>>();
            }

            if (!this._matchedValueTypes.ContainsKey(type))
            {
                this._matchedValueTypes.Add(
                    type,
                    new Dictionary<TIn, Func<TOut>>());
            }

            var matchedValues = this._matchedValueTypes[type];

            if (matchedValues.ContainsKey(value))
            {
                throw new MatchFailureException(
                    string.Format(
                        "There is already a pattern for type: {0} value: {1}.",
                        type.Name,
                        value.ToString()));
            }

            matchedValues.Add(value, func);

            return this;
        }

        public PatternMatcher<TIn, TOut> Else(Func<TOut> func)
        {
            if (null != this._matchedWildcard)
            {
                throw new MatchFailureException(
                    "There is already a wildcard pattern.");
            }

            this._matchedWildcard = func;

            return this;
        }

        #endregion

        /// <summary>
        /// Runs the func whose pattern matches the value.
        /// </summary>
        public TOut Return()
        {
            return Return(false);
        }

        /// <summary>
        /// Runs the func whose pattern matches the value.
        /// </summary>
        public TOut Return(bool allowNoMatch = false)
        {
            TOut result = default(TOut);

            if (this._hasValue)
            {
                result = Return(this._value);
            }
            else if (null != this._matchedWildcard)
            {
                result = this._matchedWildcard();
            }
            else if (!allowNoMatch)
            {
                throw new MatchFailureException(
                    "The PatternMatcher has no value to match on.");
            }

            return result;
        }

        /// <summary>
        /// Runs the func whose pattern matches the supplied option.
        /// </summary>
        /// <param name="value">The value to match on.</param>
        public TOut Return(TIn value)
        {
            return Return(value, false);
        }

        /// <summary>
        /// Runs the func whose pattern matches the supplied option.
        /// </summary>
        /// <param name="allowNoMatch">
        /// Whether or not to return a default value or throw an exception
        /// if there was nothing matched.
        /// </param>
        /// <param name="value">The value to match on.</param>
        public TOut Return(TIn value, bool allowNoMatch)
        {
            TOut result = default(TOut);
            Type type = value.GetType();

            Dictionary<TIn, Func<TOut>> matchedValues;
            Func<TOut> matchedFunc;
            Func<TIn, TOut> matchedFuncTIn;

            if (null != this._matchedValueTypes
                && this._matchedValueTypes.TryGetValue(type, out matchedValues)
                && matchedValues.TryGetValue(value, out matchedFunc))
            {
                result = matchedFunc();
            }
            else if (null != this._matchedTypes
                && this._matchedTypes.TryGetValue(type, out matchedFuncTIn))
            {
                result = matchedFuncTIn(value);
            }
            else if (null != this._matchedWildcard)
            {
                result = this._matchedWildcard();
            }
            else if (!allowNoMatch)
            {
                throw new MatchFailureException(string.Format(
                    "The input value: {0} was not matched.", value));
            }

            return result;
        }
    }

    public class MatchFailureException : Exception
    {
        public MatchFailureException(string message) : base(message) { }
    }
}
