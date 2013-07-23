using System;

namespace DCISingleFile
{
	public class TransferFailedReason
	{
		public String Reason { get; protected set; }

		public TransferFailedReason(String reason)
		{
			Reason = reason;
		}
	}
}

