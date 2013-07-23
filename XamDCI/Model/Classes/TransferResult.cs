using System;

namespace DCISingleFile
{
	public interface TransferResult
	{
		void Dispatch(TransferContext ctxt);
	}
}

