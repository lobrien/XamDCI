using System;

namespace DCISingleFile
{
	public class TransferContext
	{
		public TransferSource Source { get; protected set; }

		public TransferSink Sink { get; protected set; }

		public Decimal Amount{ get; protected set; }

		public TransferContext(TransferSource source, TransferSink sink, Decimal amount)
		{
			Source = source;
			Sink = sink;
			Amount = amount;
		}

		public TransferContext Run()
		{
			var result = Source.TransferTo(Sink, Amount);
			result.Dispatch(this);
			return this;
		}

		public void AccomplishTransfer(TransferDetails details)
		{
			TransferAccomplished(this, new TArgs<TransferDetails>(details));
		}

		public void FailTransfer( TransferFailedReason reason)
		{
			TransferFailed(this, new TArgs<TransferFailedReason>(reason));
		}

		public event EventHandler<TArgs<TransferDetails>> TransferAccomplished = delegate {};
		public event EventHandler<TArgs<TransferFailedReason>> TransferFailed = delegate{};
	}
}

