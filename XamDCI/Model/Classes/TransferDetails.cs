using System;

namespace DCISingleFile
{
	public class TransferDetails : TransferResult
	{
		public String Log { get; protected set; }

		public TransferDetails(String sourceAccountName, String sinkAccountName, Decimal amt)
		{
			Log = String.Format("{0}: Transferred ${1} from {2} to {3}", DateTime.UtcNow, amt, sourceAccountName, sinkAccountName);
		}

		public void Dispatch(TransferContext ctxt)
		{
			ctxt.AccomplishTransfer(this);
		}
	}
}

