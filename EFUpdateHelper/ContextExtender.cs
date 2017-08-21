using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

namespace CodeLab.Assets.EFUpdateHelper
{
    public static class ContextExtender
    {
        public static int SaveChanges(this DbContext context, UpdateMode updatemode)
        {


            IDirectUpdateContext directContext = context as IDirectUpdateContext;

            if (directContext != null)
            {
                directContext.CurrentSaveOperationMode = updatemode;

            }

            int result = context.SaveChanges();
            return result;

        }

        public static void PrepareEntityForUpdate<TEntity>(this DbContext context, TEntity entity, DbSet<TEntity> dbEntitySet)
           where TEntity : class, new()
        {
            if (!context.GetLoadedEntityIfAny(ref entity))
            {
                //now safe to attach since it is either not loaded , or detached
                dbEntitySet.Attach(entity);
               
            }
        }

        private static bool GetLoadedEntityIfAny<TEntity>(this DbContext context, ref TEntity entity)
            where TEntity : class
        {
            ObjectStateEntry entry;
            IObjectContextAdapter adapter = context as IObjectContextAdapter;
            bool shouldAttach = false;

            string entitySetName = context.GetEntitySetName<TEntity>();

            EntityKey key = adapter.ObjectContext.CreateEntityKey(entitySetName, entity);
            if (adapter.ObjectContext.ObjectStateManager.
                TryGetObjectStateEntry(key, out entry))
            {
                if (entry.State == EntityState.Detached)
                {
                    //if the entity is already loaded but has been detatched
                    shouldAttach = true;
                }
                //if the object originally exist on the context return it
                entity = (TEntity)entry.Entity;
            }
            else
            {
                //object does not exist on the context
                shouldAttach = true;
            }
            return !shouldAttach;
        }

        public static DbEntityValidationResult RemoveEFFalseAlarms(this DbContext context, DbEntityValidationResult result,
            DbEntityEntry entityEntry)
        {
            //This function doesn't do anything unless AllowAll is the mode

            IDirectUpdateContext directContext = context as IDirectUpdateContext;

            if (directContext != null)
            {
                UpdateMode? mode = directContext.CurrentSaveOperationMode;

                if (mode.HasValue && mode.Value == UpdateMode.Allow) 
                {

                    List<DbValidationError> errorsToIgnore = new List<DbValidationError>();
                    foreach (DbValidationError error in result.ValidationErrors)
                    {
                        if (entityEntry.State == EntityState.Modified)
                        {
                            DbMemberEntry member = entityEntry.Member(error.PropertyName);
                            DbPropertyEntry property = member as DbPropertyEntry;
                            if (property != null)
                            {
                                if (!property.IsModified)
                                {
                                    //Add errors that resulted from not changing all the attributes
                                    errorsToIgnore.Add(error);
                                }
                            }
                        }
                    }
                    //Then the errors are ignored
                    errorsToIgnore.ForEach(e => result.ValidationErrors.Remove(e));
                }
            }

            return result;
        }

        private static string GetEntitySetName<T>(this DbContext context)
         where T : class
        {
            string className = typeof(T).Name;
            IObjectContextAdapter adapter = context as IObjectContextAdapter;

            //Get context default meta container
            EntityContainer container = adapter.ObjectContext.MetadataWorkspace.
                GetEntityContainer(adapter.ObjectContext.DefaultContainerName,
                DataSpace.CSpace);

            string entitySetName = container.BaseEntitySets.
                First(setMetaData => setMetaData.ElementType.Name == className).Name;
            return entitySetName;
        }
    }
}
