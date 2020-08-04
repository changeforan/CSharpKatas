using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpKatas
{

    static class StrExt
    {
        private static Random random => new Random(147);

        public static string ToSentenceCase(this string s)
        {
            System.Threading.Thread.Sleep(random.Next(5) * 1000);
            return $"{s.ToUpper()[0]}{s.ToLower().Substring(1)}";
        }
    }

    public class ListFormater
    {
        int counter;

        // public List<string> Format(List<string> list)
        //     => list
        //        .Select(StrExt.ToSentenceCase)
        //        .Select(s => $"{++counter}. {s}")
        //        .ToList();

        public List<string> Format(List<string> list)
        {
            var taskList = list.Select(FormatStringAsync).ToList();
            return Task
                .WhenAll(taskList)
                .Result
                .ToList();
        }

        private async Task<string> FormatStringAsync(string s)
        {
            var ret = await Task.Run(() => {
                var t = s.ToSentenceCase();
                return $"{++counter}. {t}";
            });
            return ret;
        }
    }

    [TestFixture]
    public class K11Test
    {
        private readonly List<string> shoppingList = new List<string> { "coffee beans", "BANANAS", "Dates" };

        private readonly List<string> expeted = new List<string> { "1. Coffee beans", "2. Bananas", "3. Dates" };

        [Test]
        public void FormatList()
        {
            var actual = new ListFormater().Format(shoppingList);
            Assert.AreEqual(this.expeted, actual);
        }

    }
}