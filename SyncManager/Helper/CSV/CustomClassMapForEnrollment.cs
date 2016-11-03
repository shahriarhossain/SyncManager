using CsvHelper.Configuration;
using SyncManager.Model;

namespace SyncManager.Helper.CSV
{
    public class CustomClassMapForEnrollment : CsvClassMap<HealthEnrollment>
    {
        public CustomClassMapForEnrollment()
        {
            Map(m => m.ContractNumber).Name("Contract Number");
            Map(m => m.PlanID).Name("Plan ID");
            Map(m => m.SSAStateCountyCode).Name("SSA State County Code");
            Map(m => m.FIPSStateCountyCode).Name("FIPS State County Code");
            Map(m => m.State).Name("State");
            Map(m => m.County).Name("County");
            Map(m => m.Enrollment).Name("Enrollment");
        }
    }
}
