using BusinessEntity;

namespace MotorClaimSurveyApp.Models
{
    public class StatisticsModel
    {
        public int ApprPolicyCount { get; set; }
        public int UnApprPolicyCount { get; set; }
        public int ApprClaimCount { get; set; }
        public int PendingClaimCount { get; set; }
        public int RejClaimCount { get; set; }
        public int PendingSurveyCount { get; set; }
        public int SubmittedSurveyCount { get; set; }
        public int PolicyCount { get; set; }
        public List<PolicyCountData> DailyPolicyCounts { get; set; }
        public List<string> PolicyDates { get; set; }
        public List<int> PolicyCounts { get; set; }

    }
    public class PolicyCountData
    {
        public DateTime PolicyDate { get; set; }
        public int PolicyCount { get; set; }
    }


}
