using System.Text;

namespace CSharpLibraries
{
    public class SandBox
    {
        public void run()
        {
            Console.WriteLine("hello");
            var str = "Ala ma kota. Sentence with a-dash";
            var reversed = Reverse(str);
            Console.WriteLine(reversed);

            var A = new[] { 0, 5, 5, 5, -22 };
            Console.WriteLine(Solve(A));

            var input1 = new List<DateTime>() {
                new DateTime(2016, 6,20),
                new DateTime(2016, 6,21),
                new DateTime(2016, 6,22),
                new DateTime(2016, 6,25),
                new DateTime(2016, 6,26),
            };
            var res = Dates(input1);
            foreach (var d in res)
            {
                Console.WriteLine(d.Item1);
                Console.WriteLine(d.Item2);
                Console.WriteLine("sep");
            }

            var input2 = new[] { 1, 1, 0, 1, 0, 0, 1, 1 };
            var res2 = Coins(input2);
            Console.WriteLine(res2);
        }

        public string Reverse(string str)
        {
            var result = new StringBuilder();
            var sentences = str.Split('.');
            foreach (var sentence in sentences)
            {
                var words = sentence.Split(' ');
                var reversedWords = new string[words.Length];
                for (var i = 0; i < words.Length; i++)
                {
                    var li = words.Length - i - 1;
                    reversedWords[li] = words[i];
                }
                var reversedSentence = string.Join(' ', reversedWords);
                result.Append(reversedSentence + ". ");
            }
            return result.ToString();
        }

        public int Solve(int[] array)
        {
            var unique = new HashSet<int>();
            foreach (var val in array)
            {
                unique.Add(val);
            }
            return unique.Count;
        }

        public List<Tuple<DateTime, DateTime>> Dates(List<DateTime> array)
        {
            array.Sort();
            List<Tuple<DateTime, DateTime>> result = new();
            for (var i = 0; i < array.Count; i += 2)
            {
                var secondIndex = i + 1;
                if (secondIndex >= array.Count)
                {
                    secondIndex = i;
                }
                result.Add(Tuple.Create(array[i], array[secondIndex]));
            }
            return result;
        }

        public int Coins(int[] array)
        {
            List<int> lookUp = new();

            int currentUp = -1;
            foreach (var val in array)
            {
                if (val != currentUp)
                {
                    lookUp.Add(1);
                }
                else if (val == currentUp)
                {
                    ++lookUp[lookUp.Count - 1];
                }
                currentUp = val;
            }
            if (lookUp.Count == 1)
            {
                return lookUp[0];
            }
            if (lookUp.Count == 2)
            {
                return lookUp[0] > lookUp[1] ? lookUp[0]++ : lookUp[1]++;
            }
            var max = 0;
            var maxAlone = 0;
            for (int i = 1; i < lookUp.Count - 1; i++)
            {
                var prev = lookUp[i - 1];
                var curr = lookUp[i];
                var next = lookUp[i + 1];
                if (curr == 1)
                {
                    var currentValue = prev + curr + next;
                    if (currentValue > max)
                    {
                        max = currentValue;
                    }
                }
                if (prev > maxAlone) maxAlone = prev;
                if (curr > maxAlone) maxAlone = curr;
                if (next > maxAlone) maxAlone = next;
            }
            if (max == 0)
            {
                return maxAlone++;
            }
            return max;
        }
    }
}