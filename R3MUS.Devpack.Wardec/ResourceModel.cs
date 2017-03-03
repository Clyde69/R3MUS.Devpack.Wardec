using System.Collections.Generic;

namespace Legend.Shared.SportsCourseServices.Models.Resource
{
    public class ResourceModel
    {
        public int Id { get; set; }
        public int? ParentResourceId { get; set; }

        public string Name { get; set; }
        private ICollection<ResourceModel> _childResources { get; set; }
        public ICollection<ResourceModel> ChildResources
        {
            get
            {
                if (_childResources == null)
                    _childResources = new List<ResourceModel>();

                return _childResources;
            }
            set
            {
                _childResources = value;
            }
        }
    }
}
