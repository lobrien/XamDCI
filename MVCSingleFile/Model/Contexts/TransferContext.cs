using System;

namespace DCISingleFile
{
	class TransferContext
	{
		private TransferSource src;

		public TransferSource Source
		{
			get
			{
				return src;
			}
			set
			{
				if(src != null)
				{
					src.TransferAccomplished -= this.OnSourceTransferAccomplished;
					src.TransferFailed -= this.OnSourceTransferFailed;
				}
				src = value;
				value.TransferAccomplished += this.OnSourceTransferAccomplished;
				value.TransferFailed += this.OnSourceTransferFailed;
			}
		}

		public TransferSink Sink { get; protected set; }

		public Decimal Amount{ get; protected set; }

		public TransferContext(TransferSource source, TransferSink sink, Decimal amount)
		{
			Source = source;
			Sink = sink;
			Amount = amount;
		}

		private void AssertNotNull(Object o)
		{
			if(o == null)
			{
				throw new NullReferenceException();
			}
		}

		private void ValidatePreconditions()
		{
			AssertNotNull(Source);
			AssertNotNull(Sink);
			AssertNotNull(Amount);
		}

		public TransferContext Run()
		{
			ValidatePreconditions();
			Source.TransferTo(Sink, Amount);
			return this;
		}

		void OnSourceTransferAccomplished(Object src, TArgs<TransferDetails> details)
		{
			TransferAccomplished(this, details);
		}

		void OnSourceTransferFailed(Object src, TArgs<TransferFailedReason> reason)
		{
			TransferFailed(this, reason);
		}

		public event EventHandler<TArgs<TransferDetails>> TransferAccomplished = delegate {};
		public event EventHandler<TArgs<TransferFailedReason>> TransferFailed = delegate{};
	}
}

