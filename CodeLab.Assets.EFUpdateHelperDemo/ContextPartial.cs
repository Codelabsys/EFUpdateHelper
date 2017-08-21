using CodeLab.Assets.EFUpdateHelper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLab.Assets.EFUpdateHelperDemo
{

    public partial class BookLibraryEntities : DbContext, IDirectUpdateContext
    {
        public UpdateMode? CurrentSaveOperationMode { get; set; } = null;

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            //Let entity do validations
            var result = base.ValidateEntity(entityEntry, items);

            //then lets remove errors related to unchanged properties
            return this.RemoveEFFalseAlarms(result, entityEntry);
        }
    }
}
