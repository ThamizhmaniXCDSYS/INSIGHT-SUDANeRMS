﻿using INSIGHT.Entities;
using INSIGHT.Entities.DeletedRecordEntities;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
namespace PersistenceFactory
{
    public class ProjectSpecificPSF 
    {
        #region Properties
        public static ISessionFactory sessionFactory;
        const string sType = "Asc";
        const string sPercentage = "%";
        /// <summary>
        /// gets and sets the session property
        /// </summary>
        public ISession Session { get; private set; }
        /// <summary>
        /// gets and sets the session property Transaction Property
        /// </summary>
        public ITransaction Transaction { get; private set; }
        //private bool isDisposed = false;
        #endregion

        #region Constructor / Destructor
        /// <summary>
        /// Configuring and building service factory will be done by this default constructor
        /// </summary>
        /// 
        private static object _syncObject = new object();
        /// <summary>
        /// 
        /// </summary>
        public ProjectSpecificPSF()
        {
            try
            {
                lock (_syncObject)
                {
                    if (sessionFactory == null)
                    {
                        Configuration config = new Configuration().Configure();
                        config.AddAssembly(Assembly.GetCallingAssembly());
                        sessionFactory = config.BuildSessionFactory();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Configuring and building service factory will be done by this default constructor
        /// </summary>
        public ProjectSpecificPSF(string assemblyName)
        {
            try
            {
                lock (_syncObject)
                {
                    if (sessionFactory == null)
                    {
                        Configuration config = new Configuration().Configure();
                        config.AddAssembly(Assembly.Load(assemblyName));
                        sessionFactory = config.BuildSessionFactory();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Configuring and building service factory will be done by this default constructor
        /// The list parameter will contain the possible assemblies to be loaded
        /// </summary>
        /// <param name="assemblyName"></param>
        public ProjectSpecificPSF(List<String> assemblyName)
        {
            try
            {
                //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
                lock (_syncObject)
                {
                    if (sessionFactory == null)
                    {
                        Configuration config = new Configuration().Configure();
                        foreach (String listitem in assemblyName)
                        {
                            config.AddAssembly(Assembly.Load(listitem));
                        }
                        sessionFactory = config.BuildSessionFactory();
                        //to log the sql query which is generated by nhibernate
                        //log4net.Config.XmlConfigurator.Configure();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Destructor implementation
        /// </summary>
        ~ProjectSpecificPSF()
        {
            Dispose(false);
        }

        #endregion
        

        #region IDisposable Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            CloseSession();
            DisposeTransaction();
        }

        /// <summary>
        /// Method implementation for Dispose
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        
        #endregion
        #region Helper Methods

        /// <summary>
        /// Closes the session
        /// </summary>
        private void CloseSession()
        {
            if (Session == null) return;

            else if (Session.IsOpen)
            {
                Session.Close();
                Session.Dispose();
            }
        }

        private void DisposeTransaction()
        {
            if (Transaction != null)
            {
                if (Transaction.IsActive)
                    Transaction.Dispose();
            }
        }

        #endregion
        //public bool Save(IList<Invoice> objList)
        //{
        //    bool Result = false;
        //    try
        //    {
        //        using (ISession session = sessionFactory.OpenSession())
        //        {
        //            using (session.BeginTransaction())
        //            {
        //                try
        //                {
        //                    foreach (Invoice obj in objList)
        //                    {
        //                        session.Save(obj);
        //                    }
        //                    session.Transaction.Commit();
        //                    Result = true;
        //                }
        //                catch (Exception) {
        //                    session.Transaction.Rollback();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return Result;
        //}
        public int ExecuteUpdate(string Query)
        {
            int Result = 0;
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                {
                    using (session.BeginTransaction())
                    {
                        IQuery iQuery = session.CreateQuery(Query);
                        Result = iQuery.ExecuteUpdate();
                        session.Transaction.Commit();
                    }
                }
            }
            catch (Exception) { throw; }
            return Result;
        }
        public bool Save(IList<OrdersDel> OrdersDelList, IList<OrderItemsDel> OrderItemsDelList, IList<PODDel> PodIList)
        {
            bool Result = false;
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                {
                    using (session.BeginTransaction())
                    {
                        try
                        {
                            foreach (OrdersDel obj in OrdersDelList)
                            {
                                session.Save(obj);
                            }
                            foreach (OrderItemsDel obj1 in OrderItemsDelList)
                            {
                                session.Save(obj1);
                            }
                            foreach (PODDel obj2 in PodIList)
                            {
                                //obj2.DeletedDate=Convert.ToDateTime("dkdfjdfjdkj////");
                                session.Save(obj2);
                               
                            }
                            session.Transaction.Commit();
                            Result = true;
                        }
                        catch (Exception)
                        {
                            session.Transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result;
        }
        //Save or update the orders in sessions
        public bool SaveOrUpdateOrdersUsingSession(Orders ord, IList<OrderItems> Orderitemslist) {
            bool Result = false;
            try {
                using (ISession session = sessionFactory.OpenSession()) {

                    using (session.BeginTransaction()) {
                        try {
                          

                                session.SaveOrUpdate(ord);
                                foreach (OrderItems obj in Orderitemslist)
                                {
                                    obj.OrderId = ord.OrderId;
                                    session.Save(obj);
                                }

                                session.Transaction.Commit();
                                Result = true;
                           
                        }
                        catch(Exception) {
                            session.Transaction.Rollback();
                           
                        }
                    
                    }
                
                }
            
            
            
            
            
            
            
            }
            catch (Exception ex) {
                throw;
            }
            return Result;
        }


        //save or update importeddeliverynote and importeddeliverynoteitems in  a single session

        public bool SaveorUpdateDeliveryNoteInSingleSession(ImportedDeliveryNote impdel, IList<ImportedDeliveryNoteItems> importeddeliverynoteitems)
        {
            bool Result = false;
            try {
                using (ISession session = sessionFactory.OpenSession()) {

                    using (session.BeginTransaction()) {
                        try {

                            session.SaveOrUpdate(impdel);
                            foreach (ImportedDeliveryNoteItems obj in importeddeliverynoteitems)
                            {
                               obj.ImpDeliveryNoteId = impdel.ImpDeliveryNoteId;
                                session.Save(obj);
                            }

                            session.Transaction.Commit();
                            Result = true;
                        }
                        catch(Exception) {
                            session.Transaction.Rollback();
                           
                        }
                    
                    }
                
                }
            
            }
            catch (Exception ex) {
                throw;
            }
            return Result;
        }




        public bool SaveOrUpdateImportedExpDelDateListInSingleSession(IList<ImportedExpDelDate> expdeldtlist)
        {
            bool Result = false;
            try {
                using (ISession session = sessionFactory.OpenSession()) {

                    using (session.BeginTransaction()) {
                        try {

                            foreach (ImportedExpDelDate obj in expdeldtlist)
                            {
                                session.Save(obj);
                            }

                            session.Transaction.Commit();
                            Result = true;
                        }
                        catch(Exception) {
                            session.Transaction.Rollback();
                           
                        }
                    }
                }
            
            }
            catch (Exception ex) {
                throw;
            }
            return Result;
        }

        public bool SaveOrUpdateGCCRevisedList(IList<GCCRevised> revisedlist)
        {
            bool Result = false;
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                {

                    using (session.BeginTransaction())
                    {
                        try
                        {

                            foreach (GCCRevised obj in revisedlist)
                            {
                                session.Save(obj);
                            }

                            session.Transaction.Commit();
                            Result = true;
                        }
                        catch (Exception)
                        {
                            session.Transaction.Rollback();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return Result;
        }

        public void SaveOrUpdateGCCRevisedUsingSession(string Period, string PeriodYear)
        {
            int Result = 0;
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                {

                    using (session.BeginTransaction())
                    {
                        try
                        {
                            bool step1 = true; bool step2 = true; bool step3 = true; bool step4 = true; bool step5 = true;
                            string Query = "";

                            for (int i = 0; i < 5; i++)
                            {
                                if (step1 == true)
                                {
                                    Query = "UPDATE GCCRevised SET OrderId=null,PODId=null,LineId=null WHERE Period='" + Period + "' AND PeriodYear='" + PeriodYear + "'";
                                    step1 = false;
                                }
                                else if (step2 == true)
                                {
                                    Query = "";
                                    Query = "UPDATE GCCRevised Set OrderId=a.OrderId from Orders a JOIN GCCRevised b on a.ControlId=b.ControlId Where a.period='" + Period + "' and a.PeriodYear='" + PeriodYear + "'";
                                    step2 = false;
                                    
                                }
                                ISQLQuery iQuery = session.CreateSQLQuery(Query);
                                Result = iQuery.ExecuteUpdate();
                                //session.Update();
                                //iQuery.CancelQuery();
                                //session.Clear();
                            }
                            session.Transaction.Commit();
                        }
                        catch (Exception)
                        {
                            session.Transaction.Rollback();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
             //Save or update the orders in sessions
        public bool SaveOrUpdateInitialOrdersUsingSession(InitialOrders ord, IList<InitialOrderItems> Orderitemslist)
        {
            bool Result = false;
            try {
                using (ISession session = sessionFactory.OpenSession()) {

                    using (session.BeginTransaction()) {
                        try {
                          

                                session.SaveOrUpdate(ord);
                                foreach (InitialOrderItems obj in Orderitemslist)
                                {
                                    obj.OrderId = ord.OrderId;
                                    session.Save(obj);
                                }

                                session.Transaction.Commit();
                                Result = true;
                           
                        }
                        catch(Exception) {
                            session.Transaction.Rollback();
                           
                        }
                    
                    }
                
                }
            }
            catch (Exception ex) {
                throw;
            }
            return Result;
        }


    }
    
}

