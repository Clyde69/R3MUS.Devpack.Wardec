using Legend.Shared.Core.Enums;
using Legend.Shared.SportsCourseServices.Models.Resource;
using R3MUS.Devpack.Wardec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Legend.Shared.SportsCourseServices.Models.CourseDetailsSummary
{
    public class CourseDetailsViewResponseModel
    {
        public int CourseId { get; set; }
        public int TotalSessions { get; set; }
        public int AvailableCapacity { get; set; }
        public int CourseDisciplineId { get; set; }
        public string CourseDisciplineName { get; set; }
        public int? TotalCapacity { get; set; }
        public int? SpecificCourseWaitingList { get; set; }

        public string CourseName { get; set; }
        public string CourseShortCode { get; set; }
        public string CourseLevelName { get; set; }
        public string CourseInstructor { get; set; }
        public string CourseDescription { get; set; }

        public bool IsContinuous { get; set; }
        public bool AreSessionsAvailable { get; set; }
        public bool WaitingListAvailable { get; set; }
        public bool CoursePlacesAvailable { get; set; }
        public bool SessionDropinsAvailable { get; set; }

        public decimal? CourseSessionPrice { get; set; }
        public decimal? CourseInventoryPrice { get; set; }

        public DateTime? SessionStart { get; set; }

        public CourseStatus CourseStatus { get; set; }
        public PerSessionBookingType SessionBookingType { get; set; }

        public IEnumerable<ResourceModel> LocationResources { get; set; }

        public decimal? CourseDirectDebitPrice { get; set; }
        public decimal? CourseBlockBookingPrice { get; set; }

        #region Custom Properties

        public string LocationName
        {
            get
            {
                var locationBuilder = string.Empty;
                if (LocationResources.Any())
                {
                    var parents = LocationResources.Where(c => c.ParentResourceId == 0);

                    foreach (var parent in parents)
                    {
                        if (parent.ChildResources.Any())
                        {
                            locationBuilder = parent.Name + ": ";
                            foreach (var child in parent.ChildResources)
                            {
                                locationBuilder += child.Name + ", ";
                            }
                        }
                        else
                        {
                            locationBuilder = parent.Name;
                        }
                    }
                }

                return locationBuilder;
            }
        }

        public string StatusLabelColour
        {
            get
            {
                switch (CourseStatus)
                {
                    case CourseStatus.Published: return "badge-success";
                    case CourseStatus.Scheduled: return "badge-info";
                    default: return string.Empty;
                }
            }
        }

        public string FormattedCourseSessionPrice
        {
            get
            {
                return CourseSessionPrice.HasValue ? "£{0:0.00}".FormatWith(CourseSessionPrice) : "£ N/A";
            }
        }

        public string FormattedCourseInventoryPrice
        {
            get
            {
                return CourseInventoryPrice.HasValue ? "£{0:0.00}".FormatWith(CourseInventoryPrice) : "£ N/A";
            }
        }

        public bool CourseStatusPublishedAndAvailability
        {
            get
            {
                return CourseStatus == CourseStatus.Published && AreSessionsAvailable;
            }
        }

        public bool CourseStatusPublishedAndHasNewTerm
        {
            get
            {
                return CourseStatus == CourseStatus.Published && HasNewTerm;
            }
        }

        public string JavaScriptIsContinuousFlag
        {
            get
            {
                return IsContinuous.ToString().ToLower();
            }
        }

        public string WaitingListOnly
        {
            get
            {
                return (AvailableCapacity <= (SpecificCourseWaitingList ?? 0)).ToString().ToLower();
            }
        }
        public string FormattedCourseDirectDebitPrice
        {
            get
            {
                return CourseDirectDebitPrice.HasValue ? "£{0:0.00}".FormatWith(CourseDirectDebitPrice) : "£ N/A";
            }
        }


        #endregion Custom Properties

        #region Needs Removing - Blame the JS View Models

        public int? FieldsetId { get; set; }
        public int? BundleId { get; set; }
        public int CourseLevelId { get; set; }
        public int CourseFacilityId { get; set; }
        public int SessionsRemaining { get; set; }

        public bool HasNewTerm { get; set; }
        public bool ShowEmailsInReEnrollment { get; set; }
        public bool ShowWaitingListOption { get; set; }

        public DateTime CourseStartDate { get; set; }

        #endregion Needs Removing - Blame the JS View Models
    }
}