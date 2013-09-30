using Functional.PatternMatching;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatcherTests
{
    [TestFixture]
    public class PatternMatcherTests
    {
        [Test]
        public void TestMatch()
        {
            Dictionary<string, bool> matched = new Dictionary<string, bool>();

            Action resetMatched = () =>
            {
                matched.Clear();
            };

            Action<string> checkMatched = (m) =>
            {
                Assert.IsTrue(matched.ContainsKey(m));
                Assert.AreEqual(1, matched.Count);
            };

            var pm = PatternMatcher.Match<object>();

            pm = pm.With<int>(0, () => matched.Add("intZero", true))
                   .With<int>(x => matched.Add("int", true))
                   .With<string>(s => matched.Add("string", true))
                   .With<double>(1.0, () => {})
                   .Else(() => matched.Add("wildcard", true));

            pm.Return();
            checkMatched("wildcard");
            resetMatched();

            pm.Return(0.0);
            checkMatched("wildcard");
            resetMatched();

            pm = (new object()).Match();

            pm = pm.With<int>(0, () => matched.Add("intZero", true))
                   .With<int>(x => matched.Add("int", true))
                   .With<string>(s => matched.Add("string", true))
                   .Else(() => matched.Add("wildcard", true));

            pm.Return();
            checkMatched("wildcard");
            resetMatched();

            Action<object> matcher =
                PatternMatcher.Match()
                    .With<int>(0, () => matched.Add("intZero", true))
                    .With<int>(x => matched.Add("int", true))
                    .With<string>(s => matched.Add("string", true))
                    .Else(() => matched.Add("wildcard", true))
                    .Return;

            matcher(0);
            checkMatched("intZero");
            resetMatched();

            matcher(10);
            checkMatched("int");
            resetMatched();

            matcher("string");
            checkMatched("string");
            resetMatched();

            matcher(new { Unmatched = true });
            checkMatched("wildcard");
            resetMatched();
        }

        [Test]
        public void TestMatchWithResult()
        {
            var pm = PatternMatcher.Match<object, string>()
                .With<int>(0, () => "Zero")
                .With<int>(x => x.ToString())
                .With<string>(s => s)
                .With<double>(1.0, () => "One Point Zero")
                .Else(() => "Wildcard");

            Assert.AreEqual("Wildcard", pm.Return());
            Assert.AreEqual("Wildcard", pm.Return(0.0));

            pm = (new object()).Match<object, string>();
            pm = (new object()).MatchWithResult<string>();

            pm = pm.With<int>(0, () => "Zero")
                   .With<int>(x => x.ToString())
                   .With<string>(s => s)
                   .With<double>(1.0, () => "One Point Zero")
                   .Else(() => "Wildcard");

            Assert.AreEqual("Wildcard", pm.Return());
            Assert.AreEqual("Wildcard", pm.Return(0.0));

            Func<object, string> matcher =
                PatternMatcher.MatchWithResult<string>()
                    .With<int>(0, () => "Zero")
                    .With<int>(x => x.ToString())
                    .With<string>(s => s)
                    .Else(() => "Wildcard")
                    .Return;

            Assert.AreEqual("Zero", matcher(0));
            Assert.AreEqual("string", matcher("string"));
            Assert.AreEqual("10", matcher(10));
            Assert.AreEqual("Wildcard", matcher(new { Unmatched = true }));
        }

        [Test]
        public void TestExceptions()
        {
            var pm = PatternMatcher.Match()
                .With<int>(0, () => { })
                .With<int>(x => { })
                .With<string>(s => { })
                .Else(() => { });

            Assert.Throws(typeof(MatchFailureException),
                () => pm.With<int>(0, () => { }));

            Assert.Throws(typeof(MatchFailureException),
                () => pm.With<int>(i => { }));

            Assert.Throws(typeof(MatchFailureException),
                () => pm.Else(() => { }));

            pm = PatternMatcher.Match();

            Assert.Throws(typeof(MatchFailureException),
                () => pm.Return());

            Assert.Throws(typeof(MatchFailureException),
                () => pm.Return("any value"));

            var pm2 = PatternMatcher.MatchWithResult<string>()
                .With<int>(0, () => "Zero")
                .With<int>(x => x.ToString())
                .With<string>(s => s)
                .Else(() => "Wildcard");

            Assert.Throws(typeof(MatchFailureException),
                () => pm2.With<int>(0, () => "Zero"));

            Assert.Throws(typeof(MatchFailureException),
                () => pm2.With<int>(i => i.ToString()));

            Assert.Throws(typeof(MatchFailureException),
                () => pm2.Else(() => "Wildcard"));

            pm2 = PatternMatcher.MatchWithResult<string>();

            Assert.Throws(typeof(MatchFailureException),
                () => pm2.Return());

            Assert.Throws(typeof(MatchFailureException),
                () => pm2.Return("any value"));
        }

        [Test]
        public void TestStaticVars()
        {
            var wildcard = _.Instance;
        }
    }
}
