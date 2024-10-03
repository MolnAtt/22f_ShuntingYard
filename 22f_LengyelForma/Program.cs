using System;
using System.Collections.Generic;
using System.Linq;

namespace _22f_LengyelForma
{
	internal class Program
	{

		static int Kiszámol_lengyel_egyszámjegyes(string lengyel)
		{
			Stack<int> stack = new Stack<int>();

            foreach (char c in lengyel)
            {
                switch (c)
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						stack.Push(int.Parse(c.ToString()));
						break;
					case '+':
						int a = stack.Pop();
						int b = stack.Pop();
						stack.Push(a + b);
						break;
					case '*':
						stack.Push(stack.Pop() * stack.Pop());
						break;
					case '-':
						stack.Push(stack.Pop() - stack.Pop());
						break;
					case '/':
						stack.Push(stack.Pop() / stack.Pop());
						break;
					case '%':
						stack.Push(stack.Pop() % stack.Pop());
						break;
				}
			}
			return stack.Peek();
        }


		static int Kiszámol(Stack<string> input)
		{
			Stack<int> számolóverem = new Stack<int>();

			foreach (string s in input.Reverse())
			{
				if (Szám(s))
					számolóverem.Push(int.Parse(s));
				else
					switch (s)
					{
						case "+":
							számolóverem.Push(számolóverem.Pop() + számolóverem.Pop());
							break;
						case "*":
							számolóverem.Push(számolóverem.Pop() * számolóverem.Pop());
							break;
						case "-":
							számolóverem.Push(számolóverem.Pop() - számolóverem.Pop());
							break;
						case "/":
							számolóverem.Push(számolóverem.Pop() / számolóverem.Pop());
							break;
						case "%":
							számolóverem.Push(számolóverem.Pop() % számolóverem.Pop());
							break;
						case "^":
							számolóverem.Push((int)Math.Pow(számolóverem.Pop(), számolóverem.Pop()));
							break;
						case "f":
							int a = számolóverem.Pop();
							int b = számolóverem.Pop();
							int c = számolóverem.Pop();
							if (c < a) (a, c) = (c, a);
							Console.WriteLine($"{a} {b} {c}");
							számolóverem.Push(Convert.ToInt32(a <= b && b <= c));
							break;
					}
				// ronda kiírás:
				Console.Write("[ ");
				foreach (int item in számolóverem)
				{
					Console.Write(item);
					Console.Write(" ");
				}
                Console.WriteLine(" ]");
            }
			return számolóverem.Peek();
		}
		static Dictionary<string, int> prioritás = new Dictionary<string, int>
		{
			 {"+", 1 },
			 {"-", 1 },
			 {"*", 2 },
			 {"/", 2 },
			 {"^", 3 },
			 {"f", 4 } // háromargumentumú függvény, f(a b c) = `b` a számegyenesen `a` és `c` között helyezkedik-e el
		};
		static HashSet<string> prefix_függvények = new HashSet<string>
		{
			"f"
		};
		static Stack<string> TolatóUdvar(string vonat)
		{
			Stack<string> output = new Stack<string>();
			Stack<char> operator_stack = new Stack<char>();

			vonat = Ha_nincs_körülötte_zárójel_akkor_rakunk_köré_egyet(vonat);

			int i = 0;
			string adat = "";
			while (i < vonat.Length)
			{
				(adat, i) = Beolvas(vonat, i);

				if (Szám(adat))
					output.Push(adat);
				else if (adat == "(")
					operator_stack.Push(adat[0]);
				else if (Művelet(adat))
				{
					Alacsonyabb_precedenciájúig_vagy_nyitójelig_átpakol(operator_stack, output, adat);
					operator_stack.Push(adat[0]);
				}
				else if (adat == ")")
				{
					Nyitózárójelig_átpakol(operator_stack, output);
					operator_stack.Pop(); // kidobjuk a nyitó zárójelet
					if (operator_stack.Count > 0 && prefix_függvények.Contains(operator_stack.Peek() + ""))
					{
						output.Push(operator_stack.Pop() + "");
					}
				}
				Diagnosztika(output, operator_stack);
			}
			return output;
		}
        private static void Nyitózárójelig_átpakol(Stack<char> operator_stack, Stack<string> output)
		{
			while (operator_stack.Peek() != '(')
				output.Push(operator_stack.Pop().ToString());
		}
		private static void Alacsonyabb_precedenciájúig_vagy_nyitójelig_átpakol(Stack<char> operator_stack, Stack<string> output, string adat)
		{
			while (operator_stack.Count > 0 && operator_stack.Peek() != '(' && prioritás[adat] <= prioritás[operator_stack.Peek().ToString()] )
				output.Push(operator_stack.Pop().ToString());
		}
		public static void Diagnosztika(Stack<string> output, Stack<char> operator_stack)
		{
			Console.Write("output = [ ");
			foreach (string adat in output.Reverse())
			{
				Console.Write(adat + " ");
			}
			Console.WriteLine("]");

			Console.Write("operator_stack = [ ");
			foreach (char op in operator_stack.Reverse())
			{
				Console.Write(op + " ");
			}
			Console.WriteLine("]");
            Console.WriteLine("--------------------------------------------");
        }
		private static bool Művelet(string adat) => prioritás.ContainsKey(adat);
		private static bool Szám(string adat) => int.TryParse(adat, out int result);
		private static (string, int) Beolvas(string vonat, int honnan)
		{
			if (!Számjel(vonat[honnan]))
				return (vonat[honnan].ToString(), honnan + 1);
			int i = honnan;
			while (Számjel(vonat[i]))
				i++;
			return (vonat.Substring(honnan, i - honnan), i);
		}
		private static bool Számjel(char v) => "0123456789".Contains(v);




		/// <summary>
		/// Megszámolja a zárójelmélységet: "(" a jel +1-et jelent, a ")" -1-et jelent, és ahogy adja ezeket össze, úgy 0-át kap, de úgy, hogy az összeadogatás során egyszer sem jött ki -1. Ha 0-át kapott, az azt jelenti, hogy jó benne a zárójelezés. Ha emellett közbülső helyen volt 0, 
		/// </summary>
		/// <param name="vonat"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		private static string Ha_nincs_körülötte_zárójel_akkor_rakunk_köré_egyet(string vonat)
		{
			int melyseg = 0;
			int db_0 = 0;
            foreach (char c in vonat)
            {
                switch (c) 
				{
					case '(':
						melyseg++;
						break;
					case ')':
						melyseg--;
						break;
				}
				if (melyseg < 0)
					throw new Exception("Hibás zárójelezés! (túl sok csukó zárójel)");
				if (melyseg==0)
					db_0++;
            }
			if (melyseg > 0)
				throw new Exception("Hibás zárójelezés! (túl sok nyitó zárójel)");
			return 1 == db_0 ? vonat : "(" + vonat + ")";
        }

		static void Main(string[] args)
		{

			//string lengyel = Console.ReadLine();
			//Console.WriteLine(Kiszámol_lengyel_egyszámjegyes(lengyel));

			string input = "3 + 4 * 2 + ( 1 - 5 ) ^ 2 * 2 + f ( 3 4 5 ) * 4";

			//Console.WriteLine(Ha_nincs_körülötte_zárójel_akkor_rakunk_köré_egyet(input));

			Stack<string> verem = TolatóUdvar(input); // ())(()
			
            Console.WriteLine(verem.Count);
            Console.WriteLine(Kiszámol(verem));

			//Console.WriteLine(Beolvas("(2234*3342-3213+443)", 10));

			Console.ReadKey();
        }
	}
}
