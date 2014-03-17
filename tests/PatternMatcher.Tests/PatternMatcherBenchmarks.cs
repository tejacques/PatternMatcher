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
    public class PatternMatcherBenchmarks
    {
        int loops = 1000000;
        [Test]
        public void _Jit()
        {
            int tmp = loops;
            loops = 1;
            BenchmarkMatchWithResult();
            BenchmarkMatchWithResultCached();
            loops = tmp;
        }

        [Test]
        public void BenchmarkMatch()
        {
            for (int i = 0; i < loops; i++)
            {
                i.Match()
                    .With<int>(x => { })
                    .Return();
            }
        }

        [Test]
        public void BenchmarkMatchCached()
        {
            var pm = PatternMatcher.Match<int>()
                .With<int>(x => { });

            for (int i = 0; i < loops; i++)
            {
                pm.Return(i);
            }
        }

        [Test]
        public void BenchmarkMatchWithResult()
        {
            for (int i = 0; i < loops; i++)
            {
                var a = i.MatchWithResult<int>()
                    .With<int>(x => x)
                    .Return();
            }
        }

        [Test]
        public void BenchmarkMatchWithResultCached()
        {
            var pm = PatternMatcher.MatchWithResult<int>()
                    .With<int>(x => x);

            for (int i = 0; i < loops; i++)
            {
                var a = pm.Return(i);
            }
        }
    }
}
