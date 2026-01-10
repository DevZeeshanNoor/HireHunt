namespace HireHuntBackend.Common
{
    public class Enums
    {
        public enum ExternalJobs
        {
            WeWorkRemotely=1,
            RemoteOk=2,
                ArbeitNow=3,
            Remotive=4
        }
        public enum Roles
        {
            JobSeeker = 1,
            Recruiter = 2
        }
        public enum HearAboutAs
        {
            GoogleSearch=1,
            SocialMedia = 2,
            LinkedIn = 3,
            Advertisement=4,
            EmailNewsletter = 5,
            Other=99
        }

        public enum JobTypes
        {
            FullTime=1,
            PartTime=2,
            Temporary=3,
            Contract=4,
            Internship=5,
            Fresher=6
        }
    }
}
