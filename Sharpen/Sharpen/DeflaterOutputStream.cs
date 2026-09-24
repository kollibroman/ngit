namespace Sharpen
{
	using ICSharpCode.SharpZipLib.Zip.Compression;

	internal class DeflaterOutputStream : OutputStream
	{
		public DeflaterOutputStream (OutputStream s)
		{
			Wrapped = new DeflaterOutputStream (s.GetWrappedStream ());
		}

		public DeflaterOutputStream (OutputStream s, Deflater d)
		{
			Wrapped = new DeflaterOutputStream (s.GetWrappedStream (), d);
		}
		
		public DeflaterOutputStream (OutputStream s, Deflater d, int bufferSize)
		{
			Wrapped = new DeflaterOutputStream (s.GetWrappedStream (), d, bufferSize);
		}

		public void Finish ()
		{
			((DeflaterOutputStream)Wrapped).Finish ();
		}
	}
}
