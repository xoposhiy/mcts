using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MctsLib
{
	public interface IMove<in TGame>
	{
		void ApplyTo(TGame game);
	}
}
