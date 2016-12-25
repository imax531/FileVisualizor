using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileVisualizor {
	class FileGenerator {

		public static List<string> list { get; }

		static FileGenerator() {
			list = new List<string>();
			list.Add("Simple pattern");
		}

		public static void GenerateFile(int index) {
			switch (index) {
				case 0:
					GenerateSimplePattern();
					break;
			}
		}

		static void GenerateSimplePattern() {
			byte[] file = new byte[1024 * 3];
			for (int i = 0; i < file.Length; i++) {
				if (i % 64 < 8)
					file[i] = 200;
				else
					file[i] = 100;
			}

			string path = "SimplePattern";
			System.IO.File.WriteAllBytes(path, file);
		}

	}
}
