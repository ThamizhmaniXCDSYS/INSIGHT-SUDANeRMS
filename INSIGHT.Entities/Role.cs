using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    public class Role
    {
                [DataMember]
        public virtual long Id { get; set; }
                [DataMember]
        public virtual string RoleCode { get; set; }
                [DataMember]
        public virtual string RoleName { get; set; }
                [DataMember]
        public virtual string Description { get; set; }
                [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual string CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual string ModifiedDate { get; set; }
        //public virtual IList<AppRoleConfig> ListOfAppRoles { get; set; }

        //ROLECODE	VARCHAR2(20 BYTE)
        //NAME	VARCHAR2(100 BYTE)
        //DESCRIPTION	VARCHAR2(200 BYTE)
        //CREATEDDATE	DATE
        //MODIFIEDDATE	DATE
        //CREATEDBY	VARCHAR2(100 BYTE)
        //MODIFIEDBY	VARCHAR2(100 BYTE)



        //RQC	CF Req Branch
        //RQQ	RFQ ReqBranch
        //RVA	Report Viewer for ALL
        //RVB	Report Viewer for Branch
        //RVG	Report Viewer GOC
        //SBO	Submit BOLINS
        //SBP	Sorting Batch PIV
        //SCSR	Super CSR
        //SSA	Security Super Admin
        //TEST	TEST
        //TFG	Feedback GOC
        //TSS	TOP Support Request Suspension
        //TSU	TOP Support View
        //UMB	Upload MBL
        //UPO	Update POD


    }
}
