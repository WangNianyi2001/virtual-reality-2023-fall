using OpenCvSharp.Internal;
using System.IO;
using UnityEngine;

namespace Game {
	public class CascadeClassifier : OpenCvSharp.CascadeClassifier {
		public CascadeClassifier(TextAsset textAsset) : base() {
			string fileName = "cascade.xml";
			var file = File.Create(fileName);
			using(StreamWriter writer = new StreamWriter(file))
				writer.WriteLine(textAsset.text);

			ExceptionStatus status = NativeMethods.objdetect_CascadeClassifier_newFromFile(fileName, out ptr);
			NativeMethods.HandleException(status);

			file.Close();
			File.Delete(fileName);
		}
	}
}
