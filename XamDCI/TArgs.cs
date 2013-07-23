using System;

namespace DCISingleFile
{
	public class TArgs<T> : EventArgs
	{
		public T Value { get; protected set; }

		public TArgs(T value)
		{
			Value = value;
		}
	}

}

