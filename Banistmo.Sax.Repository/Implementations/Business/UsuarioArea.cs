﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;
using System.Linq.Expressions;
using EntityFramework.Utilities;
using System.Transactions;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class UsuarioArea : RepositoryBase<SAX_USUARIO_AREA>, IUsuarioArea
    {
        public UsuarioArea()
            : this(new SaxRepositoryContext())
        {
        }
        public UsuarioArea(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public void CreateAndRemove(List<SAX_USUARIO_AREA> create, List<int> remove)
        {
            using (var trx = new TransactionScope())
            {
                using (var db = new DBModelEntities())
                {
                    if (remove .Count() != 0)
                    {
                        var countdelete = EFBatchOperation.For(db, db.SAX_USUARIO_AREA).Where(b => remove.Any(c => c == b.UA_ID_USUARIO_AREA)).Delete();
                        EFBatchOperation.For(db, db.SAX_USUARIO_AREA).InsertAll(create);
                    }
                    else
                    //var countdelete = EFBatchOperation.For(db, db.SAX_USUARIO_AREA).Where(b => remove.Any(c => c == b.UA_ID_USUARIO_AREA)).Delete();
                    EFBatchOperation.For(db, db.SAX_USUARIO_AREA).InsertAll(create);
                }
                trx.Complete();
            }
        }
        /*
        public void CreateAndRemove(List<SAX_USUARIO_AREA> create, String remove)
        {
            using (var trx = new TransactionScope())
            {
                using (var db = new DBModelEntities())
                {
                    var countdelete = EFBatchOperation.For(db, db.SAX_USUARIO_AREA).Where(b => remove.Any(c => c == b.UA_ID_USUARIO_AREA)).Delete();
                    EFBatchOperation.For(db, db.SAX_USUARIO_AREA).InsertAll(create);
                }
                trx.Complete();
            }
        }
      
        public void Remove(SAX_USUARIO_AREA remove)
        {
            using (var trx = new TransactionScope())
            {
                using (var db = new DBModelEntities())
                {
                    var countdelete = EFBatchOperation.For(db, db.SAX_USUARIO_AREA).Where(b => b.UA_ID_USUARIO_AREA == remove.UA_ID_USUARIO_AREA).Delete();
                }
                trx.Complete();
            }
        }
        */

        public override Expression<Func<SAX_USUARIO_AREA, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_USUARIO_AREA, bool>> SearchFilters(SAX_USUARIO_AREA obj)
        {
            return x => x.CA_ID_AREA == obj.CA_ID_AREA;
        }
    }
}
