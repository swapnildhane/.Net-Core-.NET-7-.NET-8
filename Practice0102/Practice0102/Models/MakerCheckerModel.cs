namespace Practice0102.Models
{
    public class MakerCheckerModel
    {
        public int iPk_MakerId { get; set; }
        public string? sEmpName { get; set; }
        public string? sPanNo { get; set; }
        public decimal dMnthlyIncm { get; set; }
        public string? sMbleNo { get; set; }
        public string? MarriageStatus { get; set; }
        public int iMrgSts { get; set; }
        public int iAprvRjctSts { get; set; }   
        public string AprvRjctSts { get; set; }
        public string sAdrs { get; set; }
    }
}

