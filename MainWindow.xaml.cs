using System;
using System.Numerics;
using System.Windows;
using WpfApp_Wienner_Attack_BigInteger;
using WpfApp_Wienner_Attack_Bool;


namespace WpfApp_Wienner_Attack
	{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		private void button_WiennerAttack_Click(object sender, RoutedEventArgs e)
		{
			textBox_Process.Clear();
			String e_s = textBox_e_Input.Text;
			String n_s = textBox_n_Input.Text;
			if (string.IsNullOrEmpty(textBox_e_Input.Text) || (string.IsNullOrEmpty(textBox_n_Input.Text)))
			{
				MessageBox.Show("Введены не все параметры  для взлома шифрования RSA!", "Предупреждение", MessageBoxButton.OK);
				return;
			}
			BigInteger e_BigInt = BigInteger.Parse(e_s);
			BigInteger n = BigInteger.Parse(n_s);
			textBox_Process.Text += ($"Атака Винера для e = {e_s},n={n_s}...\n");
			//Атака  Винера
			var cf = BigIntegerFunctions.ContinuedFraction(e_BigInt, n);
			textBox_Process.Text += (" Цепная дробь CF(e/ N):\n");
			for (int i = 0; i < cf.Count; i++)
			{
				textBox_Process.Text += ((i == 0 ? "[" : ", ") + cf[i]);
			}
			textBox_Process.Text += ("]\n");
			var conv = BigIntegerFunctions.Convergents(cf);
			textBox_Process.Text += ("Сходимые (k, d):\n");
			foreach (var (k, d) in conv)
			{
				textBox_Process.Text += ($"k = {k}, d = {d}\n");
			}
			textBox_Process.Text += "\n";

			BigInteger dLimit = (BigInteger)(Math.Pow((double)n, 0.25) / 3.0);

			foreach (var (k, dCandidate) in conv)
			{
				if (k == 0 || dCandidate == 0) continue;
				if (dCandidate > dLimit) break;

				if ((e_BigInt * dCandidate - 1) % k != 0) continue;

				BigInteger phiCandidate = (e_BigInt * dCandidate - 1) / k;

				if (!BoolFunctions.TryRecoverPQFromPhi(n, phiCandidate, out BigInteger p, out BigInteger q))
					continue;

				// найдены корректные p,q => вычислим реальный d по phi
				BigInteger phiActual = (p - 1) * (q - 1);
				textBox_Process.Text += ("Атака Винера завершена  Успешно!\n");
				textBox_Process.Text += ("Найдены:\n");
				textBox_Process.Text += ($"p = {q}\n");
				textBox_Process.Text += ($"q = {p}\n");
				textBox_Process.Text += ($"φ = {phiActual}\n");
				textBox_Process.Text += ($"d = {dCandidate}\n");
				textBox_p_Output.Text = q.ToString();
				textBox_q_Output.Text = p.ToString();
				textBox_phi_Output.Text = phiActual.ToString();
				textBox_d_Output.Text = dCandidate.ToString();
				textBox_Process.Text += ($"Открытый ключ:[{e_BigInt},{n}]\n");
				textBox_Process.Text += ($"Закрытый ключ:[{dCandidate},{n}]");
				return;
			}
			textBox_Process.Text += ("Используем расширенный перебор...\n");
			int DMAX = 20000;
			for (BigInteger d = 1; d <= DMAX; d++)
			{
				BigInteger numerator = e_BigInt * d - 1;
				if (numerator <= 0) continue;

				// оптимизация: перебор делителей до sqrt(numerator) и рассмотрение пары (k, numerator/k)
				BigInteger limitK = (BigInteger)Math.Sqrt((double)numerator) + 1;
				for (BigInteger k = 1; k <= d; k++)
				{
					if (numerator % k != 0) continue;

					BigInteger phiCandidate = numerator / k;
					if (phiCandidate <= 0 || phiCandidate >= n) continue;

					if (!BoolFunctions.TryRecoverPQFromPhi(n, phiCandidate, out BigInteger p, out BigInteger q)) continue;

					BigInteger phiActual = (p - 1) * (q - 1);
	
					BigInteger p_r = q;
					BigInteger q_r = p;
					textBox_Process.Text += ("Расширенный перебор завершен  успешно!\n");
					textBox_Process.Text += ("Найдены:\n");
					textBox_Process.Text += ($"p = {q}\n");
					textBox_Process.Text += ($"q = {p}\n");
					textBox_Process.Text += ($"φ = {phiActual}\n");
					textBox_Process.Text += ($"d  = {d}\n");
					textBox_p_Output.Text = q.ToString();
					textBox_q_Output.Text = p.ToString();
					textBox_phi_Output.Text = phiActual.ToString();
					textBox_d_Output.Text = d.ToString();
					textBox_Process.Text += ($"Открытый ключ:[{e_BigInt},{n}]\n");
					textBox_Process.Text += ($"Закрытый ключ:[{d},{n}]");
					return;
				}
			}
			textBox_Process.Text += ($"Ключ не найден в пределах {DMAX}.");
			return;
		}
	}
}

