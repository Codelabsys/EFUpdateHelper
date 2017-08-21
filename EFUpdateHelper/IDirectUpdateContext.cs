using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Assets.EFUpdateHelper
{
    public interface IDirectUpdateContext
    {
        UpdateMode? CurrentSaveOperationMode { get; set; }
    }

}
