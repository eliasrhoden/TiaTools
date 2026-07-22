using System.Collections.Generic;

using Siemens.Engineering;
using Siemens.Engineering.AddIn;
using Siemens.Engineering.AddIn.Menu;

namespace TiaTools
{
    public class TiaToolsAddInProvider : ProjectTreeAddInProvider
    {
        /// <summary>
        /// The instance of TIA Portal in which the Add-In works.
        /// <para>Enables Add-In to interact with TIA Portal.</para>
        /// </summary>
        TiaPortal m_TiaPortal;

        public TiaToolsAddInProvider(TiaPortal tiaPortal)
        {
            m_TiaPortal = tiaPortal;
        }

        protected override IEnumerable<ContextMenuAddIn> GetContextMenuAddIns()
        {
            yield return new TiaToolsContextMenuAddIn(m_TiaPortal);
        }
    }
}
