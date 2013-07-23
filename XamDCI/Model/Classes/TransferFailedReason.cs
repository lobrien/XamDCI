using System;

namespace DCISingleFile
{
	public class TransferFailedReason : TransferResult
	{
		public String Reason { get; protected set; }

		public TransferFailedReason(String reason)
		{
			Reason = reason;
		}

		public void Dispatch(TransferContext ctxt)
		{
			ctxt.FailTransfer(this);
		}
	}
}

