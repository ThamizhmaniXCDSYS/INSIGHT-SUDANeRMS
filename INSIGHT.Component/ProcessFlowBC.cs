using System;
using System.Collections.Generic;
using System.Linq;
using PersistenceFactory;
using INSIGHT.Entities;
using INSIGHT.Entities.TicketingSystem;

namespace INSIGHT.Component
{
    public class ProcessFlowBC : IDisposable
    {
        PersistenceServiceFactory PSF;
        public ProcessFlowBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
        }
        protected virtual void Dispose(bool disposing)
        {
            //this.Dispose();  
        }

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

        #region CreateMethods
        public bool BulkCompleteInformationActivity(string Template, string userId, long[] ActivityId)
        {
            bool retValue = false;
            IList<Activity> CurrentActivity = PSF.GetListById<Activity>("Id", ActivityId);
            if (CurrentActivity != null && CurrentActivity.Count > 0 && CurrentActivity.First() != null)
            {
                foreach (Activity ac in CurrentActivity)
                {
                    ac.Completed = true;
                    ac.Available = false;
                    ac.Assigned = false;
                    ac.Performer = userId;
                    PSF.SaveOrUpdate(ac);
                    ProcessInstance pi = PSF.Get<ProcessInstance>(ac.InstanceId);
                    pi.Status = "Completed";
                    PSF.SaveOrUpdate(pi);
                    retValue = true;
                }
            }
            return retValue;
        }

        public long CreateProcessInstance(ProcessInstance ProcessInstance)
        {
            try
            {
                if (ProcessInstance != null)
                { PSF.SaveOrUpdate<ProcessInstance>(ProcessInstance); return ProcessInstance.Id; }
                else throw new Exception("ProcessInstance Object is required and it cannot be empty.");
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }
        public long CreateWorkFlowStatus(WorkFlowStatus WorkFlowStatus)
        {
            try
            {
                if (WorkFlowStatus != null)
                { PSF.SaveOrUpdate<WorkFlowStatus>(WorkFlowStatus); return WorkFlowStatus.Id; }
                else throw new Exception("WorkFlowStatus Object is required and it cannot be empty.");
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }
        public long CreateActivity(Activity Activity)
        {
            try
            {
                if (Activity != null)
                { PSF.SaveOrUpdate<Activity>(Activity); return Activity.Id; }
                else throw new Exception("Activity Object is required and it cannot be empty.");
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }

        public bool AssignActivity(long activityId, string userId)
        {
            if (activityId > 0 && !string.IsNullOrEmpty(userId))
            {
                Activity AssignActivity = PSF.Get<Activity>(activityId);
                if (AssignActivity.Completed != true)
                {
                    AssignActivity.Assigned = true;
                }
                AssignActivity.Available = false;
                AssignActivity.Performer = userId;
                PSF.SaveOrUpdate<Activity>(AssignActivity);
                return true;
            }
            else return false;
        }
        public bool AssignActivityCheckBeforeAssigning(long activityId, string userId)
        {
            try
            {
                if (activityId > 0 && !string.IsNullOrEmpty(userId))
                {
                    Activity AssignActivity = PSF.Get<Activity>(activityId);
                    if (AssignActivity.Assigned == true && !string.IsNullOrEmpty(AssignActivity.Performer) && AssignActivity.Performer.ToUpper().Trim() != userId.ToUpper().Trim())
                    {
                        throw new Exception("This activity already assigned to " + AssignActivity.Performer + "");
                    }
                    if (AssignActivity.Completed != true)
                    {
                        AssignActivity.Assigned = true;
                    }
                    AssignActivity.Available = false;
                    AssignActivity.Performer = userId;
                    PSF.SaveOrUpdate<Activity>(AssignActivity);
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetMethods
        public ProcessInstance GetProcessInstanceById(long Id)
        {
            try
            {
                if (Id > 0)
                    return PSF.Get<ProcessInstance>(Id);
                else throw new Exception("Id is required and it cannot be zero.");
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }

        public WorkFlowStatus GetWorkFlowStatusById(long Id)
        {
            try
            {
                if (Id > 0)
                    return PSF.Get<WorkFlowStatus>(Id);
                else throw new Exception("Id is required and it cannot be zero.");
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }

        public Activity GetActivityById(long Id)
        {
            try
            {
                if (Id > 0)
                    return PSF.Get<Activity>(Id);
                else throw new Exception("Id is required and it cannot be zero.");
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }

        public Dictionary<long, IList<ProcessInstance>> GetProcessInstanceListWithsearchCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithSearchCriteriaCount<ProcessInstance>(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }


        public Dictionary<long, IList<WorkFlowStatus>> GetWorkFlowStatusListWithsearchCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithSearchCriteriaCount<WorkFlowStatus>(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }


        public Dictionary<long, IList<TicketSystemActivity>> GetTicketSystemActivityListWithsearchCriteria(int? page, int? pageSize, string sortBy, string sortType, Dictionary<string, object> criteria, string ColumnName, string[] values, string[] alias)
        {
            try
            {
                return PSF.GetListWithSearchCriteriaCountArray<TicketSystemActivity>(page, pageSize, sortBy, sortType, ColumnName, values, criteria, alias);
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }
        #endregion

        public string StartETicketingSystem(TicketSystem TicketSystem, string Template, string userId)
        {
            try
            {
                WorkFlowTemplate WorkFlowTemplate = PSF.Get<WorkFlowTemplate>("TemplateName", Template);
                //creating process instance
                ProcessInstance pi = new ProcessInstance();
                pi.CreatedBy = userId;
                pi.DateCreated = DateTime.Now;
                pi.TemplateId = WorkFlowTemplate.Id;
                pi.Status = "Activated";
                PSF.SaveOrUpdate<ProcessInstance>(pi);
                long pid = pi.Id;
                //create object with the pid for FFExport
                TicketSystem.InstanceId = pid;
                PSF.SaveOrUpdate<TicketSystem>(TicketSystem);
                string ticketNo = "ETicket-" + TicketSystem.Id;
                TicketSystem.TicketNo = ticketNo;
                PSF.SaveOrUpdate<TicketSystem>(TicketSystem);
                IList<WorkFlowStatus> WorkFlowStatusList = PSF.GetList<WorkFlowStatus>("TemplateId", WorkFlowTemplate.Id, "ActivityOrder", 1);
                foreach (WorkFlowStatus wfs in WorkFlowStatusList)
                {
                    Activity NextActivity = new Activity();
                    NextActivity.ActivityName = wfs.WFStatus;
                    NextActivity.ActivityFullName = wfs.Description;
                    NextActivity.AppRole = wfs.Performer;
                    NextActivity.Completed = false;
                    NextActivity.Available = true;
                    NextActivity.Assigned = false;
                    NextActivity.Performer = userId;
                    NextActivity.TemplateId = WorkFlowTemplate.Id;
                    NextActivity.InstanceId = pid;
                    NextActivity.NextActOrder = wfs.NextActOrder;
                    NextActivity.ActivityOrder = wfs.ActivityOrder;
                    NextActivity.PreviousActOrder = 1;
                    NextActivity.ProcessRefId = TicketSystem.Id;
                    NextActivity.CreatedDate = DateTime.Now;
                    NextActivity.BranchCode = TicketSystem.BranchCode;
                    NextActivity.DeptCode = TicketSystem.DeptCode;
                    if (wfs.IsRejectionRequired == true)
                    { NextActivity.IsRejApplicable = true; }
                    PSF.SaveOrUpdate<Activity>(NextActivity);
                }
                return TicketSystem.TicketNo;
            }
            catch (Exception) { throw; }
        }

        public bool CompleteActivityTicketSystem(TicketSystem TicketSystem, string Template, string userId, string ActivityName, bool isRejection)
        {
            bool retValue = false;
            try
            {
                PSF.SaveOrUpdate<TicketSystem>(TicketSystem);
                WorkFlowTemplate WorkFlowTemplate = PSF.Get<WorkFlowTemplate>("TemplateName", Template);
                //close the current activity
                Activity CurrentActivity = PSF.Get<Activity>("InstanceId", TicketSystem.InstanceId, "ActivityName", ActivityName, "Completed", false);
                if (CurrentActivity != null)
                {
                    //if current activity doesnt need any rejection and there is no rejection then it will be completed
                    //otherwise once the rejection resolved then it will be completed
                    //this need to be obdated as waiting
                    if (CurrentActivity.IsRejApplicable == true && isRejection == true)
                    {
                        CurrentActivity.BranchCode = TicketSystem.BranchCode;
                        CurrentActivity.DeptCode = TicketSystem.DeptCode;
                        CurrentActivity.Completed = true;
                        CurrentActivity.Available = false;
                        CurrentActivity.Assigned = false;
                        CurrentActivity.Performer = userId;
                        PSF.SaveOrUpdate(CurrentActivity);
                    }
                    else
                    {
                        Activity WaitingAct = PSF.Get<Activity>("WaitingFor", CurrentActivity.Id);
                        if (WaitingAct != null)
                        {
                            CurrentActivity.BranchCode = TicketSystem.BranchCode;
                            CurrentActivity.DeptCode = TicketSystem.DeptCode;
                            CurrentActivity.Waiting = false;
                            PSF.SaveOrUpdate(CurrentActivity);
                            WaitingAct.Completed = true;
                            WaitingAct.Assigned = false;
                            WaitingAct.Available = false;
                        }
                        CurrentActivity.BranchCode = TicketSystem.BranchCode;
                        CurrentActivity.DeptCode = TicketSystem.DeptCode;
                        CurrentActivity.Completed = true;
                        CurrentActivity.Available = false;
                        CurrentActivity.Assigned = false;
                        CurrentActivity.Performer = userId;
                        if (CurrentActivity.ActivityName == "CompleteETicket")
                        {
                            TicketSystem.IsTicketCompleted = true;
                            PSF.Update<TicketSystem>(TicketSystem);
                            ProcessInstance pi = PSF.Get<ProcessInstance>(TicketSystem.InstanceId);
                            pi.Status = "Completed"; PSF.SaveOrUpdate<ProcessInstance>(pi);
                            TicketSystem.Status = "Completed";
                            PSF.SaveOrUpdate<TicketSystem>(TicketSystem);
                        }
                        PSF.SaveOrUpdate(CurrentActivity);
                    }
                }
                //trigger the next activity //we need to check whether parallel activities are there to complete
                //activities that are coming in the next order
                IList<WorkFlowStatus> WorkFlowStatusList = PSF.GetList<WorkFlowStatus>("TemplateId", WorkFlowTemplate.Id, "ActivityOrder", CurrentActivity.NextActOrder);
                //checking parallel activities get the current order of activities
                Dictionary<string, object> WaitCriteria = new Dictionary<string, object>();
                WaitCriteria.Add("TemplateId", WorkFlowTemplate.Id);
                WaitCriteria.Add("ActivityOrder", CurrentActivity.ActivityOrder);
                WaitCriteria.Add("InstanceId", CurrentActivity.InstanceId);
                Dictionary<long, IList<Activity>> conditionList = PSF.GetListWithSearchCriteriaCount<Activity>(0, 100, string.Empty, string.Empty, WaitCriteria);
                if (conditionList != null && conditionList.Count > 0)
                {
                    IList<Activity> conditionWaitList = conditionList.First().Value;
                    bool? waiting = false;
                    foreach (Activity a in conditionWaitList)
                    {
                        if (a.Completed == false && waiting == false)
                        {
                            waiting = true;
                        }
                    } retValue = true;
                    if (waiting == true)
                    {
                        if (CurrentActivity.IsRejApplicable == true && isRejection == true)
                        { }
                        else
                            return retValue;
                    }
                }
                if (WorkFlowStatusList != null && WorkFlowStatusList.Count > 0)
                {
                    //if it is rejection flow then build the logic here
                    //{logic goes here }


                    foreach (WorkFlowStatus wfs in WorkFlowStatusList)
                    {
                        //Rejection Activity
                        if (CurrentActivity.IsRejApplicable == true && isRejection == true)
                        {
                            WorkFlowStatus wfsRej = PSF.Get<WorkFlowStatus>("TemplateId", WorkFlowTemplate.Id, "RejectionFor", CurrentActivity.ActivityOrder);
                            if (wfsRej != null)
                            {
                                Activity NextActivityRej = new Activity();
                                NextActivityRej.CreatedDate = DateTime.Now;
                                NextActivityRej.ActivityName = wfsRej.WFStatus;
                                if (NextActivityRej.ActivityName == "CompleteETicket")
                                {
                                    NextActivityRej.Completed = true;
                                    //ProcessInstance pi = PSF.Get<ProcessInstance>(CallManagement.InstanceId);
                                    //pi.Status = "Completed"; PSF.SaveOrUpdate<ProcessInstance>(pi);
                                    TicketSystem.Status = NextActivityRej.ActivityName;
                                    PSF.SaveOrUpdate<TicketSystem>(TicketSystem);
                                }
                                else NextActivityRej.Completed = false;
                                NextActivityRej.ActivityFullName = wfsRej.Description;
                                NextActivityRej.AppRole = wfsRej.Performer;

                                NextActivityRej.Performer = userId;
                                NextActivityRej.TemplateId = WorkFlowTemplate.Id;
                                NextActivityRej.InstanceId = TicketSystem.InstanceId;
                                NextActivityRej.NextActOrder = wfsRej.NextActOrder;
                                NextActivityRej.ActivityOrder = wfsRej.ActivityOrder;
                                NextActivityRej.PreviousActOrder = wfsRej.PreviousActOrder;
                                NextActivityRej.ProcessRefId = TicketSystem.Id;
                                NextActivityRej.RejectionFor = CurrentActivity.Id;
                                NextActivityRej.Completed = false;
                                NextActivityRej.Available = true;
                                NextActivityRej.Assigned = false;
                                NextActivityRej.BranchCode = TicketSystem.BranchCode;
                                NextActivityRej.DeptCode = TicketSystem.DeptCode;
                                PSF.SaveOrUpdate<Activity>(NextActivityRej);
                                TicketSystem.Status = NextActivityRej.ActivityName;
                                PSF.SaveOrUpdate<TicketSystem>(TicketSystem);
                                //CurrentActivity.WaitingFor = NextActivityRej.Id;
                                //PSF.SaveOrUpdate(CurrentActivity);
                            }
                        }
                        else
                        {
                            Activity NextActivity = new Activity();
                            NextActivity.CreatedDate = DateTime.Now;
                            if (wfs.IsRejectionRequired == true)
                            { NextActivity.IsRejApplicable = true; }

                            NextActivity.ActivityName = wfs.WFStatus;
                            NextActivity.ActivityFullName = wfs.Description;
                            NextActivity.AppRole = wfs.Performer;
                            NextActivity.Completed = wfs.WFStatus == "Complete" ? true : false;
                            NextActivity.Available = wfs.WFStatus != "Complete" ? true : false;
                            //NextActivity.Performer = userId;
                            NextActivity.TemplateId = WorkFlowTemplate.Id;
                            NextActivity.InstanceId = TicketSystem.InstanceId;
                            NextActivity.NextActOrder = wfs.NextActOrder;
                            NextActivity.ActivityOrder = wfs.ActivityOrder;
                            NextActivity.PreviousActOrder = wfs.PreviousActOrder;
                            NextActivity.ProcessRefId = TicketSystem.Id;
                            NextActivity.Available = true;
                            NextActivity.Assigned = false;
                            NextActivity.Completed = false;
                            NextActivity.BranchCode = TicketSystem.BranchCode;
                            NextActivity.DeptCode = TicketSystem.DeptCode;
                            PSF.SaveOrUpdate<Activity>(NextActivity);
                            TicketSystem.Status = NextActivity.ActivityName;
                            PSF.SaveOrUpdate<TicketSystem>(TicketSystem);
                        }
                    } retValue = true;
                }
                return retValue;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool MoveBackToAvailable(long activityId)
        {
            if (activityId > 0)
            {
                Activity AssignActivity = PSF.Get<Activity>(activityId);
                AssignActivity.Available = true;
                AssignActivity.Assigned = false;
                AssignActivity.Performer = "";
                PSF.SaveOrUpdate<Activity>(AssignActivity);
                return true;
            }
            else return false;
        }
    }
}
