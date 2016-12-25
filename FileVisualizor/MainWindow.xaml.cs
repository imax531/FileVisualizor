using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileVisualizor {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		byte[] currentFile;
		int lim, page, maxpage;
		const int size = 10;
		const int dx = 20;
		Dictionary<int, Rectangle> rectangles;
		Dictionary<byte, SolidColorBrush> byteToColor;

		public MainWindow() {
			InitializeComponent();
			AllowDrop = true;
			ResizeMode = ResizeMode.NoResize;
			FillByteToColor();
			lim = (int)((this.Width - dx) / size) * 50;
			initRectangles();
			cobGen.ItemsSource = FileGenerator.list;
		}

		public void initRectangles() {
			rectangles = new Dictionary<int, Rectangle>(lim);
			for (int i = 0; i < lim; i++) {
				Rectangle r = new Rectangle();
				r.Width = size;
				r.Height = size;
				Canvas.SetLeft(r, (i * size) % (this.Width - dx));
				Canvas.SetTop(r, ((i * size) / ((this.Width - dx)) * size));
				r.Fill = new SolidColorBrush(new Color());
				rectangles.Add(i, r);
				canvas.Children.Add(r);
			}
		}

		const int maxVal = 255 / 15;
		private void FillByteToColor() {
			byteToColor = new Dictionary<byte, SolidColorBrush>(256);
			for (int b = 0; b < 256; b++) {
				Color c = new Color();
				c.A = 255;
				c.R = (byte)(((b >> 0) & 15) * maxVal);
				c.G = (byte)(((b >> 4) & 15) * maxVal);
				c.B = 64;
				c.B += (byte)(((b >> 3) & 7) * maxVal);

				byteToColor.Add((byte)b, new SolidColorBrush(c));
			}
		}

		protected override void OnDrop(DragEventArgs e) {
			base.OnDrop(e);

			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				OpenFile(files[0]);
			}

		}

		private void OpenFile(String file) {
			try {
				currentFile = System.IO.File.ReadAllBytes(file);
				maxpage = (int)Math.Ceiling((double)currentFile.Length / lim);
				movePage(0);
			} catch (Exception e) { }
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);

			if (e.Key == Key.Left) {
				movePage(--page);
			} else if (e.Key == Key.Right) {
				movePage(++page);
			}
		}

		private void updatePageLabel() {
			lblPage.Content = "Page " + (page + 1) + "/" + maxpage;
		}

		private void movePage(int p) {
			if (!IsValidPage(p)) return;

			page = p;
			updatePageLabel();
			int offset = lim * page;

			int i;
			for (i = 0; i < rectangles.Count && i + offset < currentFile.Length; i++) {
				rectangles[i].Fill = byteToColor[currentFile[i + offset]];
			}
			for (; i < rectangles.Count; i++) {
				rectangles[i].Fill = null;
			}
		}

		private bool IsValidPage(int p) {
			if (currentFile == null)
				return false;

			if (p < 0) {
				page = 0;
				return false;
			} else if (p >= maxpage) {
				page = maxpage - 1;
				return false;
			}

			return true;
		}

		private void btnGen_Click(object sender, RoutedEventArgs e) {
			FileGenerator.GenerateFile(cobGen.SelectedIndex);
		}

		private void btnPrev_Click(object sender, RoutedEventArgs e) {
			movePage(--page);
		}

		private void btnNext_Click(object sender, RoutedEventArgs e) {
			movePage(++page);
		}
	}
}
