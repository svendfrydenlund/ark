using ARK.Website.Common.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.EntityFramework.Main
{
    public partial class ArkDatabase
    {
        public override int SaveChanges()
        {
            DateTime now = DateTime.UtcNow;
            foreach (ObjectStateEntry entry in (this as IObjectContextAdapter).ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added))
            {
                if (!entry.IsRelationship)
                {
                    IJegHarIndsatTid jegHarIndsatTid = entry.Entity as IJegHarIndsatTid;
                    if (jegHarIndsatTid != null)
                    {
                        jegHarIndsatTid.IndsatTid = now;
                    }
                }
            }

            foreach (ObjectStateEntry entry in (this as IObjectContextAdapter).ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified))
            {
                if (!entry.IsRelationship)
                {
                    IJegHarOpdateretTid jegHarOpdateretTid = entry.Entity as IJegHarOpdateretTid;
                    if (jegHarOpdateretTid != null)
                    {
                        jegHarOpdateretTid.OpdateretTid = now;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}
