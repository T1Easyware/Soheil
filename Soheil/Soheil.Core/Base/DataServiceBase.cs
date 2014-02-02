using System;
using Soheil.Dal;
namespace Soheil.Core.Base
{
    public abstract class DataServiceBase : IDisposable
    {
		public SoheilEdmContext context { get; protected set; }

		public virtual void Dispose()
		{
			if (context != null)
				context.Dispose();
		}

		protected string getCloneName(string originalName)
		{
			const string COPY = " - Copy";
			return originalName + COPY;
			//if (originalName.Contains(COPY))
			//{
			//	if (originalName.EndsWith(COPY))
			//		return originalName + "1";
			//	else
			//	{
			//		int num;
			//		int idx = originalName.IndexOf(COPY) + COPY.Length;
			//		string numStr = originalName.Substring(idx);
			//		if (int.TryParse(numStr, out num))
			//		{
			//			return originalName + (num + 1).ToString();
			//		}
			//		else return originalName + "1";
			//	}
			//}
			//else return originalName + COPY;
		}
	}
}